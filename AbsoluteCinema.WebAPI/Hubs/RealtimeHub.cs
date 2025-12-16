using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.Application.DTO.TicketsDTO;

namespace AbsoluteCinema.WebAPI.Hubs
{
    public class RealtimeHub : Hub
    {
        private readonly ITicketService _ticketService;
        private static int _activeConnections = 0;
        private static readonly object _lockObject = new object();

        public RealtimeHub(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ConnectionStatus", true, "Підключено до сервера");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            lock (_lockObject)
            {
                _activeConnections--;
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, DateTime.UtcNow);
        }

        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        public async Task JoinSessionGroup(int sessionId)
        {
            var groupName = $"Session_{sessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            await LoadAndSendBookedSeats(sessionId);
        }

        public async Task LeaveSessionGroup(int sessionId)
        {
            var groupName = $"Session_{sessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
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

