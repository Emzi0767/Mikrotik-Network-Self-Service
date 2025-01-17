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

namespace Emzi0767.NetworkSelfService.Backend.Data;

/// <summary>
/// Represents a mapping of AP to location or description.
/// </summary>
public sealed class DbApMap
{
    /// <summary>
    /// Gets or sets the identity of the access point.
    /// </summary>
    public string Identity { get; set; }

    /// <summary>
    /// Gets or sets the comment for the access point.
    /// </summary>
    public string Comment { get; set; }
}
