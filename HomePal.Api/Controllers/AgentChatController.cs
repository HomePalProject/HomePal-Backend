using HomePal.Api.Attributes;
using HomePal.Application.Features.AgentChat.DTOs;
using HomePal.Application.Features.AgentChat.Interfaces;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[RequireActiveSubscription]
[Route("api/agent-chat")]
public class AgentChatController : BaseApiController
{
    private readonly IAgentChatService _agentChatService;

    public AgentChatController(IAgentChatService agentChatService)
    {
        _agentChatService = agentChatService;
    }

    /// <summary>
    /// Gets the current authenticated user's single agent chat session and messages.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<AgentChatSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserSession(CancellationToken cancellationToken)
    {
        var result = await _agentChatService.GetUserSessionAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Clears the current authenticated user's single agent chat session history.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearUserSession(CancellationToken cancellationToken)
    {
        var result = await _agentChatService.ClearUserSessionAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Sends a message or tool approval to the agent and streams the response via Server-Sent Events (SSE).
    /// </summary>
    [HttpPost("run")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> RunAgent([FromBody] RunAgentRequest req, CancellationToken cancellationToken)
    {
        var stream = _agentChatService.RunAgentStreamAsync(CurrentUserId, req, cancellationToken);
        return TypedResults.ServerSentEvents(stream);
    }
}
