using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API sentence attribute word.
/// </summary>
public sealed class MikrotikSentenceAttributeWord : IMikrotikWord
{
    /// <summary>
    /// Gets the name of the tag attribute.
    /// </summary>
    public const string Tag = "tag";

    /// <summary>
    /// Gets the prefix for all attributes.
    /// </summary>
    public const string Prefix = ".";

    /// <summary>
    /// Gets the separator for name and value.
    /// </summary>
    public const string Separator = "=";

    /// <inheritdoc />
    public int Length { get; }

    /// <summary>
    /// Gets the name of this attribute.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new sentence attribute word, with specified name and value.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">Value of the attribute.</param>
    public MikrotikSentenceAttributeWord(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Name must be non-null and cannot be all-whitespace.");

        if (value is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(value), "Value must be non-null.");

        this.Name = name;
        this.Value = value;
        this.Length = Prefix.ComputeEncodedLength()
            + this.Name.ComputeEncodedLength()
            + Separator.ComputeEncodedLength()
            + this.Value.ComputeEncodedLength();
    }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
    {
        if ((int)destination.Length < this.Length)
            return false;

        if (!Prefix.TryEncodeTo(destination))
            return false;

        if (!this.Name.TryEncodeTo(destination))
            return false;

        if (!Separator.TryEncodeTo(destination))
            return false;

        if (!this.Value.TryEncodeTo(destination))
            return false;

        return true;
    }
}
