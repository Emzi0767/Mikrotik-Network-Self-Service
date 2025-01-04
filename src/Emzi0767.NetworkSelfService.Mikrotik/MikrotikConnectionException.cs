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

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Indicates that a problem occured when connecting to a router.
/// </summary>
public sealed class MikrotikConnectionException : Exception
{
    /// <summary>
    /// Creates a new connection exception with specified message.
    /// </summary>
    /// <param name="message">Message describing the problem.</param>
    public MikrotikConnectionException(string message)
        : base(message)
    { }
}
