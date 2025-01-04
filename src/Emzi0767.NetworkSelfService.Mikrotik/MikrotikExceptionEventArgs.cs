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
using Emzi0767.Utilities;

namespace Emzi0767.NetworkSelfService.Mikrotik;

/// <summary>
/// Represents arguments for <see cref="MikrotikApiClient.Exception"/> event.
/// </summary>
public sealed class MikrotikExceptionEventArgs : EventArgs
{
    /// <summary>
    /// Gets the exception that occured.
    /// </summary>
    public Exception Exception { get; init; }
    
    /// <summary>
    /// Gets the event in which the problem occured.
    /// </summary>
    public AsyncEvent Event { get; init; }
    
    /// <summary>
    /// Gets the arguments passed to the event.
    /// </summary>
    public AsyncEventArgs EventArgs { get; init; }
    
    /// <summary>
    /// Gets the handler in which the exception occured.
    /// </summary>
    public AsyncEventHandler<MikrotikApiClient, AsyncEventArgs> Handler { get; init; }
}
