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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Emzi0767.NetworkSelfService.Mikrotik.Attributes;
using Emzi0767.NetworkSelfService.Mikrotik.Types;

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

/// <summary>
/// Represents a Mikrotik CAPsMAN security profile.
/// </summary>
[GenerateMikrotikEntityMetadata, MikrotikEntity("caps-man", "security")]
public sealed class MikrotikCapsmanSecurityProfile : IMikrotikEntity, IMikrotikModifiableEntity<MikrotikCapsmanSecurityProfile>
{
    /// <inheritdoc />
    public MikrotikClient Client { get; }

    /// <summary>
    /// Gets the unique identifier of this entity.
    /// </summary>
    [DataMember(Name = ".id"), MikrotikReadonlyProperty]
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of the profile.
    /// </summary>
    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the configured wireless authentication types.
    /// </summary>
    [DataMember(Name = "authentication-types")]
    public IEnumerable<MikrotikWirelessSecurity> AuthenticationTypes { get; internal set; }

    /// <summary>
    /// Gets whether to disable PMKID.
    /// </summary>
    [DataMember(Name = "disable-pmkid")]
    public bool? DisablePmkid { get; internal set; }

    /// <summary>
    /// Gets the wireless encryption type.
    /// </summary>
    [DataMember(Name = "encryption")]
    public MikrotikWirelessEncryption Encryption { get; internal set; }

    /// <summary>
    /// Gets the wireless group encryption type.
    /// </summary>
    [DataMember(Name = "group-encryption")]
    public MikrotikWirelessEncryption GroupEncryption { get; internal set; }

    /// <summary>
    /// Gets the amount of time after which group encryption keys are to be updated.
    /// </summary>
    [DataMember(Name = "group-key-update")]
    public TimeSpan GroupKeyUpdate { get; internal set; }

    /// <summary>
    /// Gets the configured passphrase.
    /// </summary>
    [DataMember(Name = "passphrase")]
    public string Password { get; internal set; }

    /// <summary>
    /// Gets whether to use RADIUS for accounting if the client is authenticated via RADIUS.
    /// </summary>
    [DataMember(Name = "eap-radius-accounting")]
    public bool? UseRadiusAccounting { get; internal set; }

    /// <summary>
    /// Gets the certificate to use for TLS-based EAP modes.
    /// </summary>
    [DataMember(Name = "tls-certificate")]
    public string TlsCertificate { get; internal set; }

    /// <summary>
    /// Gets the TLS verification mode.
    /// </summary>
    [DataMember(Name = "tls-mode")]
    public MikrotikTlsMode TlsMode { get; internal set; }

    internal MikrotikCapsmanSecurityProfile(MikrotikClient client)
    {
        this.Client = client;
    }

    /// <inheritdoc />
    public IMikrotikEntityModifier<MikrotikCapsmanSecurityProfile> Modify()
        => new MikrotikEntityModifyBuilder<MikrotikCapsmanSecurityProfile>(this);
}
