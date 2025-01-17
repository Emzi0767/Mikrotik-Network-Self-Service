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

using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a CAPsMAN wireless device association.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "registration-table")]
public sealed class MikrotikCapsmanRegistration : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id")]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of the CAP interface this device is associated with.
    /// </summary>
    [DataMember(Name = "interface")]
    public string InterfaceName { get; internal set; }

    /// <summary>
    /// Gets the SSID the device associated with.
    /// </summary>
    [DataMember(Name = "ssid")]
    public string Ssid { get; internal set; }

    /// <summary>
    /// Gets the MAC address of the device.
    /// </summary>
    [DataMember(Name = "mac-address")]
    public MacAddress MacAddress { get; internal set; }

    /// <summary>
    /// Gets the comment for this entry.
    /// </summary>
    [DataMember(Name = "comment")]
    public string Comment { get; internal set; }

    /// <summary>
    /// Gets the EAP identity of this device, if applicable.
    /// </summary>
    [DataMember(Name = "eap-identity")]
    public string EapIdentity { get; internal set; }

    internal MikrotikCapsmanRegistration(MikrotikClient client)
    {
        this.Client = client;
    }
}
