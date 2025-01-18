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
/// Represents a Mikrotik CAPsMAN configuration.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "configuration")]
public sealed class MikrotikCapsmanConfiguration : IMikrotikEntity
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of this configuration.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the SSID of this configuration.
    /// </summary>
    [DataMember(Name = "ssid")]
    public string Ssid { get; internal set; }

    /// <summary>
    /// Gets whether to not broadcast the SSID.
    /// </summary>
    [DataMember(Name = "hide-ssid")]
    public bool HideSsid { get; internal set; }

    /// <summary>
    /// Gets the country of installation for the APs.
    /// </summary>
    [DataMember(Name = "country")]
    public string Country { get; internal set; }

    /// <summary>
    /// Gets the type of installation of the APs.
    /// </summary>
    [DataMember(Name = "installation")]
    public MikrotikWirelessInstallation InstallationType { get; internal set; }

    /// <summary>
    /// Gets the name of the associated security profile.
    /// </summary>
    [DataMember(Name = "security")]
    public string SecurityProfileName { get; internal set; }

    /// <summary>
    /// Gets the name of the associated datapath.
    /// </summary>
    [DataMember(Name = "datapath")]
    public string DatapathName { get; internal set; }

    /// <summary>
    /// Gets the name of the associated wireless channel configuration.
    /// </summary>
    [DataMember(Name = "channel")]
    public string ChannelConfigurationName { get; internal set; }

    /// <summary>
    /// Gets the name of the associated wireless radio configuration.
    /// </summary>
    [DataMember(Name = "rates")]
    public string RateConfigurationName { get; internal set; }

    internal MikrotikCapsmanConfiguration(MikrotikClient client)
    {
        this.Client = client;
    }
}
