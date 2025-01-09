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

using System;

namespace Emzi0767.NetworkSelfService.Mikrotik.Exceptions;

/// <summary>
/// Represents an error returned from the API.
/// </summary>
public sealed class MikrotikApiException : Exception
{
    /// <summary>
    /// Gets the category of the error.
    /// </summary>
    public MikrotikApiErrorCategory Category { get; }

    /// <summary>
    /// Creates a new Mikrotik API exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    public MikrotikApiException(string message)
        : this(message, MikrotikApiErrorCategory.Unknown)
    { }

    /// <summary>
    /// Creates a new Mikrotik API exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="category">Error category.</param>
    public MikrotikApiException(string message, MikrotikApiErrorCategory category)
        : base(message)
    {
        this.Category = category;
    }
}
