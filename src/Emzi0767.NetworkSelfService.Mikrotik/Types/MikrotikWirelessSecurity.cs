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

namespace Emzi0767.NetworkSelfService.Mikrotik.Types;

/// <summary>
/// Specifies the type of wireless security.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikWirelessSecurity
{
    /// <summary>
    /// Specifies WPA-PSK security, also known as WPA Personal.
    /// </summary>
    [DataMember(Name = "wpa-psk")]
    WpaPsk,

    /// <summary>
    /// Specifies WPA2-PSK security, also known as WPA2 Personal.
    /// </summary>
    [DataMember(Name = "wpa2-psk")]
    Wpa2Psk,

    /// <summary>
    /// Specifies WPA-EAP security, also known as WPA Enterprise or 802.1x.
    /// </summary>
    [DataMember(Name = "wpa-eap")]
    WpaEap,

    /// <summary>
    /// Specifies WPA2-EAP security, also known as WPA2 Enterprise or 802.1x.
    /// </summary>
    [DataMember(Name = "wpa2-eap")]
    Wpa2Eap,
}
