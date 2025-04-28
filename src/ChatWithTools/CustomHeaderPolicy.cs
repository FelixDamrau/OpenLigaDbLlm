using System.ClientModel.Primitives;

namespace OpenLigaDb.Client;

/// <summary>
/// A pipeline policy that adds a custom HTTP header to the request.
/// </summary>
/// <param name="name">The name of the header.</param>
/// <param name="value">The value of the header.</param>
public class CustomHeaderPolicy(string name, string value) : PipelinePolicy
{
    private readonly string name = name;
    private readonly string value = value;

    /// <inheritdoc/>
    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int index)
    {
        message.Request.Headers.Add(name, value);
        ProcessNext(message, pipeline, index);
    }

    /// <inheritdoc/>
    public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int index)
    {
        message.Request.Headers.Add(name, value);
        return ProcessNextAsync(message, pipeline, index);
    }
}
