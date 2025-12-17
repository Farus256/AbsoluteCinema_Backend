using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.Application.DTO.TicketsDTO;
using AbsoluteCinema.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AbsoluteCinema.WebAPI.Hubs;
using Microsoft.Extensions.Logging;

namespace AbsoluteCinema.WebAPI.Controllers;

public class TicketController : BaseController
{
    private readonly ITicketService _ticketService;
    private readonly IHubContext<RealtimeHub> _hubContext;
    private readonly ILogger<TicketController> _logger;
        
    public TicketController(ITicketService ticketService, IHubContext<RealtimeHub> hubContext, ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _hubContext = hubContext;
        _logger = logger;
    }
        
    [HttpGet]
    public async Task<ActionResult> GetTicketById(int id)
    {
        var ticketDto = await _ticketService.GetTicketByIdAsync(id);
        if (ticketDto == null)
            return NotFound();
        return Ok(ticketDto);
    }
        
    [HttpGet]
    public async Task<ActionResult> GetAllTickets([FromQuery]GetAllTicketsDto getAllTicketsDto)
    {
        var tickets = await _ticketService.GetAllTicketsAsync(getAllTicketsDto);
        return Ok(tickets);
    }
        
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policy.UserPolicy)]
    public async Task<ActionResult> CreateTicket([FromBody] CreateTicketDto createTicketDto)
    {
        try
        {
            var id = await _ticketService.CreateTicketAsync(createTicketDto);
            
            try
            {
                await RealtimeHub.NotifySeatBooked(_hubContext, createTicketDto.SessionId, createTicketDto.Row, createTicketDto.Place);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to broadcast seat booking notification for session {SessionId}", createTicketDto.SessionId);
            }
            
            return Ok(id);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message, title = "Помилка бронювання" });
        }
    }
        
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policy.AdminPolicy)]
    public async Task<ActionResult> DeleteTicket(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket != null)
        {
            await _ticketService.DeleteTicketAsync(id);
            await RealtimeHub.NotifySeatReleased(_hubContext, ticket.SessionId, ticket.Row, ticket.Place);
        }
        return Ok();
    }
        
    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policy.AdminPolicy)]
    public async Task<ActionResult> UpdateTicket([FromBody] UpdateTicketDto updateTicketDto)
    {
        await _ticketService.UpdateTicketAsync(updateTicketDto);
        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetTicketWithStrategy([FromQuery] GetTicketWithStrategyDto getTicketWithStrategyDto)
    {
        var tickets = await _ticketService.GetTicketWithStrategyAsync(getTicketWithStrategyDto);
        return Ok(tickets);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateTicketStatus([FromQuery] int ticketId, [FromBody] TicketStatusIdDto ticketStatusIdDto)
    {
        await _ticketService.UpdateTicketStatusAsync(ticketId, ticketStatusIdDto);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTicketsForAdmin([FromBody] GetAllTicketsDto getAllTicketsDto)
    {
        var tickets = await _ticketService.GetAllTicketsWithIncludeAsync(getAllTicketsDto);
        return Ok(tickets);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllTicketsForUser(int userId)
    {
        var tickets = await _ticketService.GetTicketsForUserAsync(userId);
        return Ok(tickets);
    }
}