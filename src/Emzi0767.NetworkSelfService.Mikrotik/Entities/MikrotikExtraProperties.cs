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

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikExtraProperties<T> : IMikrotikExtraProperties<T>
    where T : class, IMikrotikEntity
{
    [DataMember(Name = "place-before")]
    public T PlaceBefore { get => this._before; set { this._before = value; this._after = null; } }

    [DataMember(Name = "place-after")]
    public T PlaceAfter { get => this._after; set { this._after = value; this._before = null; } }

    private T _before, _after;

    public MikrotikExtraProperties()
    {
        this._before = null;
        this._after = null;
    }
}
