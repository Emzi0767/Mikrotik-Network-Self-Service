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
/// Specifies the type of wireless band and standard.
/// </summary>
[GenerateEnumMetadata]
public enum MikrotikWirelessBand
{
    /// <summary>
    /// Specifies 802.11b radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-b")]
    B_2_4GHz,

    /// <summary>
    /// Specifies 802.11b/g radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-b/g")]
    BG_2_4GHz,

    /// <summary>
    /// Specifies 802.11b/g/n radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-b/g/n")]
    BGN_2_4GHz,

    /// <summary>
    /// Specifies 802.11g/n radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-g/n")]
    GN_2_4GHz,

    /// <summary>
    /// Specifies 802.11g radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-onlyg")]
    G_2_4GHz,

    /// <summary>
    /// Specifies 802.11n radio, operating in 2.4GHz band.
    /// </summary>
    [DataMember(Name = "2ghz-onlyn")]
    N_2_4GHz,

    /// <summary>
    /// Specifies 802.11a radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-a")]
    A_5GHz,

    /// <summary>
    /// Specifies 802.11a/n radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-a/n")]
    AN_5GHz,

    /// <summary>
    /// Specifies 802.11a/n/ac radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-a/n/ac")]
    ANAC_5GHz,

    /// <summary>
    /// Specifies 802.11n/ac radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-n/ac")]
    NAC_5GHz,

    /// <summary>
    /// Specifies 802.11ac radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-onlyac")]
    AC_5GHz,

    /// <summary>
    /// Specifies 802.11n radio, operating in 5GHz band.
    /// </summary>
    [DataMember(Name = "5ghz-onlyn")]
    N_5GHz,
}
