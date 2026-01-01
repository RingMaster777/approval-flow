using ApprovalFlow.Application.Commands;
using ApprovalFlow.Application.DTOs;
using ApprovalFlow.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApprovalFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprovalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApprovalRequestResponseDto>> CreateRequest(CreateApprovalRequestDto dto)
    {
        var command = new CreateApprovalRequestCommand(
            dto.Title,
            dto.Description,
            dto.RequesterId,
            dto.RequesterName
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRequest), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> GetRequest(Guid id)
    {
        var query = new GetApprovalRequestQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ApprovalRequestResponseDto>>> GetAllRequests([FromQuery] string? status = null)
    {
        var query = new GetAllApprovalRequestsQuery(status);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id}/review")]
    public async Task<ActionResult<ApprovalRequestResponseDto>> ReviewRequest(Guid id, ReviewApprovalRequestDto dto)
    {
        var command = new ReviewApprovalRequestCommand(
            id,
            dto.IsApproved,
            dto.ReviewerId,
            dto.ReviewerName,
            dto.Comments
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}/history")]
    public async Task<ActionResult<List<ApprovalHistoryDto>>> GetHistory(Guid id)
    {
        var query = new GetApprovalHistoryQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
