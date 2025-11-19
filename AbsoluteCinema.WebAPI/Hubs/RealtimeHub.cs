using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.Application.DTO.TicketsDTO;

namespace AbsoluteCinema.WebAPI.Hubs
{
    public class RealtimeHub : Hub
    {
        private readonly ILogger<RealtimeHub> _logger;
        private readonly ITicketService _ticketService;
        private static int _activeConnections = 0;
        private static readonly object _lockObject = new object();

        public RealtimeHub(ILogger<RealtimeHub> logger, ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }

        public override async Task OnConnectedAsync()
        {
            lock (_lockObject)
            {
                _activeConnections++;
            }
            _logger.LogInformation($"Клієнт підключився: {Context.ConnectionId}. Активних з'єднань: {_activeConnections}");
            await Clients.Caller.SendAsync("ConnectionStatus", true, "Підключено до сервера");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_lockObject)
            {
                _activeConnections--;
            }
            _logger.LogInformation($"Клієнт відключився: {Context.ConnectionId}. Активних з'єднань: {_activeConnections}");
            if (exception != null)
            {
                _logger.LogError(exception, "Помилка при відключенні клієнта");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            _logger.LogInformation($"Отримано повідомлення від {Context.ConnectionId}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", message, DateTime.UtcNow);
        }

        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        public async Task GetServerStatistics()
        {
            var process = Process.GetCurrentProcess();
            var memoryUsed = process.WorkingSet64 / (1024 * 1024); // MB
            var memoryTotal = GC.GetTotalMemory(false) / (1024 * 1024); // MB
            var memoryPrivate = process.PrivateMemorySize64 / (1024 * 1024); // MB
            var memoryVirtual = process.VirtualMemorySize64 / (1024 * 1024); // MB
            var cpuTime = process.TotalProcessorTime.TotalMilliseconds;
            var userCpuTime = process.UserProcessorTime.TotalMilliseconds;
            var threadCount = process.Threads.Count;
            var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();
            var gcCollections = new
            {
                Gen0 = GC.CollectionCount(0),
                Gen1 = GC.CollectionCount(1),
                Gen2 = GC.CollectionCount(2)
            };

            var statistics = new
            {
                IsOnline = true,
                ServerTime = DateTime.UtcNow,
                Uptime = new
                {
                    Days = uptime.Days,
                    Hours = uptime.Hours,
                    Minutes = uptime.Minutes,
                    Seconds = uptime.Seconds,
                    TotalSeconds = (long)uptime.TotalSeconds,
                    TotalHours = (long)uptime.TotalHours,
                    TotalDays = uptime.TotalDays
                },
                Memory = new
                {
                    UsedMB = memoryUsed,
                    GCMemoryMB = memoryTotal,
                    PrivateMB = memoryPrivate,
                    VirtualMB = memoryVirtual,
                    AvailableMB = Environment.WorkingSet / (1024 * 1024),
                    MaxWorkingSetMB = process.MaxWorkingSet.ToInt64() / (1024 * 1024),
                    MinWorkingSetMB = process.MinWorkingSet.ToInt64() / (1024 * 1024)
                },
                Process = new
                {
                    Id = process.Id,
                    ProcessName = process.ProcessName,
                    ThreadCount = threadCount,
                    HandleCount = process.HandleCount,
                    CpuTimeMs = cpuTime,
                    UserCpuTimeMs = userCpuTime,
                    StartTime = process.StartTime.ToUniversalTime(),
                    Responding = process.Responding
                },
                Connections = new
                {
                    Active = _activeConnections
                },
                GC = new
                {
                    Collections = gcCollections,
                    TotalMemory = GC.GetTotalMemory(false),
                    MaxGeneration = GC.MaxGeneration
                },
                Environment = new
                {
                    MachineName = Environment.MachineName,
                    ProcessorCount = Environment.ProcessorCount,
                    OSVersion = Environment.OSVersion.ToString(),
                    DotNetVersion = Environment.Version.ToString(),
                    UserName = Environment.UserName,
                    UserDomainName = Environment.UserDomainName,
                    TickCount = Environment.TickCount,
                    Is64BitProcess = Environment.Is64BitProcess,
                    Is64BitOperatingSystem = Environment.Is64BitOperatingSystem
                }
            };

            await Clients.Caller.SendAsync("ServerStatistics", statistics);
        }

        public async Task JoinSessionGroup(int sessionId)
        {
            var groupName = $"Session_{sessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Клієнт {Context.ConnectionId} приєднався до групи {groupName}");
            
            await LoadAndSendBookedSeats(sessionId);
        }

        public async Task LeaveSessionGroup(int sessionId)
        {
            var groupName = $"Session_{sessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Клієнт {Context.ConnectionId} покинув групу {groupName}");
        }

        public async Task CheckSeatAvailability(int sessionId, int row, int place)
        {
            try
            {
                var strategyDto = new GetTicketWithStrategyDto
                {
                    SessionId = sessionId,
                    Row = row,
                    Place = place,
                    Page = 1,
                    PageSize = 1
                };

                var tickets = await _ticketService.GetTicketWithStrategyAsync(strategyDto);
                var isAvailable = !tickets.Any();

                await Clients.Caller.SendAsync("SeatAvailabilityChecked", sessionId, row, place, isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Помилка перевірки місця SessionId={sessionId}, Row={row}, Place={place}");
                await Clients.Caller.SendAsync("SeatAvailabilityError", sessionId, row, place, ex.Message);
            }
        }

        public async Task GetBookedSeats(int sessionId)
        {
            await LoadAndSendBookedSeats(sessionId);
        }

        private async Task LoadAndSendBookedSeats(int sessionId)
        {
            try
            {
                var strategyDto = new GetTicketWithStrategyDto
                {
                    SessionId = sessionId,
                    Page = 1,
                    PageSize = 1000
                };

                var tickets = await _ticketService.GetTicketWithStrategyAsync(strategyDto);
                var bookedSeats = tickets.Select(t => new { t.Row, t.Place }).ToList();

                await Clients.Caller.SendAsync("BookedSeatsLoaded", sessionId, bookedSeats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Помилка завантаження зайнятих місць для SessionId={sessionId}");
                await Clients.Caller.SendAsync("BookedSeatsError", sessionId, ex.Message);
            }
        }

        public static async Task NotifySeatBooked(IHubContext<RealtimeHub> hubContext, int sessionId, int row, int place)
        {
            var groupName = $"Session_{sessionId}";
            await hubContext.Clients.Group(groupName).SendAsync("SeatBooked", sessionId, row, place);
        }

        public static async Task NotifySeatReleased(IHubContext<RealtimeHub> hubContext, int sessionId, int row, int place)
        {
            var groupName = $"Session_{sessionId}";
            await hubContext.Clients.Group(groupName).SendAsync("SeatReleased", sessionId, row, place);
        }
    }
}

