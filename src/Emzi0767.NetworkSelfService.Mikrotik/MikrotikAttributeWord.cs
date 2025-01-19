// This file is part of Network Self-Service Project.
// Copyright © 2024-2025 Mateusz Brawański <Emzi0767>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics;
using Emzi0767.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents a Mikrotik API attribute word.
/// </summary>
[DebuggerDisplay("ATTR: ={Name}={Value}")]
public sealed class MikrotikAttributeWord : IMikrotikWord
{
    /// <summary>
    /// Gets the prefix for all attributes.
    /// </summary>
    public const string Prefix = "=";

    /// <summary>
    /// Gets the separator for name and value.
    /// </summary>
    public const string Separator = "=";

    /// <summary>
    /// Gets the prefix for unsets.
    /// </summary>
    public const string UnsetPrefix = "!";

    /// <summary>
    /// Gets the key for property list.
    /// </summary>
    public const string KeyPropertyList = ".proplist";

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
    /// Creates a new attribute word, with specified name and value.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">Value of the attribute.</param>
    public MikrotikAttributeWord(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(name), "Name must be non-null and cannot be all-whitespace.");

        this.Name = name;
        this.Value = value;
        this.Length = Prefix.ComputeEncodedLength()
            + this.Name.ComputeEncodedLength()
            + Separator.ComputeEncodedLength()
            + (this.Value?.ComputeEncodedLength() ?? 0)
            + (this.Value is null ? UnsetPrefix.Length : 0);
    }

    /// <inheritdoc />
    public bool TryEncode(IMemoryBuffer<byte> destination)
    {
        if (!Prefix.TryEncodeTo(destination))
            return false;

        if (this.Value is null && !UnsetPrefix.TryEncodeTo(destination))
            return false;

        if (!this.Name.TryEncodeTo(destination))
            return false;

        if (!Separator.TryEncodeTo(destination))
            return false;

        if (this.Value is not null && !this.Value.TryEncodeTo(destination))
            return false;

        return true;
    }
}
