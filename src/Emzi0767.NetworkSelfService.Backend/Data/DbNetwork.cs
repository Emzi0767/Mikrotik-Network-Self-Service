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
/// Represents the configuration of a network.
/// </summary>
public sealed class DbNetwork
{
    /// <summary>
    /// Gets or sets the name of the network.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the DHCP server, which serves requests for this network.
    /// </summary>
    public string DhcpServer { get; set; }

    /// <summary>
    /// Gets or sets the name of the interface list, to which all CAP interfaces get attached.
    /// </summary>
    public string WirelessInterfaceList { get; set; }

    /// <summary>
    /// Gets the user associated with the network.
    /// </summary>
    public DbUser User { get; set; }
}
