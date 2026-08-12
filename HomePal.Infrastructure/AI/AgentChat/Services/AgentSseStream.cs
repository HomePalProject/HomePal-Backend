using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace HomePal.Infrastructure.AI.AgentChat.Services;

/// <summary>
/// Converts a streaming agent run into a sequence of AG-UI SSE events.
/// Keeps content-type mapping logic out of the controller.
/// </summary>
public sealed class AgentSseStream
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Wraps updates in AG-UI SSE events and yields them.
    /// </summary>
    public async IAsyncEnumerable<SseItem<string>> RunAsync(
        IAsyncEnumerable<AgentResponseUpdate> updates,
        Guid chatSessionId,
        string runId,
        string threadId,
        string messageId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var sb = new StringBuilder();

        yield return Evt(new { type = "RUN_STARTED", runId, threadId });

        await foreach (var update in updates.WithCancellation(ct))
        {
            foreach (var item in update.Contents)
            {
                await foreach (var sseItem in MapContentAsync(item, messageId, sb))
                    yield return sseItem;
            }
        }

        if (sb.Length > 0)
            yield return Evt(new { type = "TEXT_MESSAGE_END", messageId });

        yield return Evt(new { type = "RUN_FINISHED", runId, threadId });
    }

    private async IAsyncEnumerable<SseItem<string>> MapContentAsync(
        AIContent item,
        string messageId,
        StringBuilder sb)
    {
        switch (item)
        {
            case TextContent tc when !string.IsNullOrEmpty(tc.Text):
            {
                if (sb.Length == 0)
                    yield return Evt(new
                    {
                        type = "TEXT_MESSAGE_START",
                        messageId,
                        role = "assistant"
                    });

                sb.Append(tc.Text);

                yield return Evt(new
                {
                    type = "TEXT_MESSAGE_CONTENT",
                    messageId,
                    delta = tc.Text
                });

                break;
            }

            case FunctionCallContent fcc:
            {
                yield return Evt(new
                {
                    type       = "TOOL_CALL_START",
                    toolCallId = fcc.CallId,
                    toolName   = fcc.Name,
                    args       = fcc.Arguments
                });

                break;
            }

            case ToolApprovalRequestContent approval:
            {
                var toolCall = approval.ToolCall as FunctionCallContent;

                yield return Evt(new
                {
                    type       = "TOOL_CALL_REQUEST",
                    toolCallId = approval.RequestId ?? toolCall?.CallId,
                    toolName   = toolCall?.Name ?? "tool",
                    args       = toolCall?.Arguments
                });

                break;
            }

            case FunctionResultContent result:
            {
                yield return Evt(new
                {
                    type       = "TOOL_CALL_RESULT",
                    toolCallId = result.CallId,
                    result     = result.Result?.ToString()
                });

                break;
            }
        }

        await Task.CompletedTask;
    }

    private static SseItem<string> Evt(object payload) =>
        new(JsonSerializer.Serialize(payload, _json));
}
