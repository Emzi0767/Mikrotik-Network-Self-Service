﻿// This file is part of Network Self-Service Project.
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

namespace Emzi0767.NetworkSelfService.Mikrotik.Entities;

internal sealed class MikrotikEntityModifyBuilder<T> : MikrotikEntityBuilder<T>
    where T : class, IMikrotikEntity
{
    private readonly T _entity;

    public MikrotikEntityModifyBuilder(MikrotikClient client, T entity)
        : base(client)
    {
        if (entity is null)
            MikrotikThrowHelper.Throw_ArgumentNull(nameof(entity), "Entity cannot be null.");

        this._entity = entity;
        this.AddExtraWord(MikrotikQueryWord.PropertyEquals(EntityProxies.MapToSerialized<T>(nameof(this._entity.Id)), this._entity.Id));
    }
}
