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

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

public sealed class MikrotikEntityInflater : IMikrotikInflater
{
    public Type ObjectType { get; init; }

    public object Inflate(MikrotikClient client, IReadOnlyDictionary<string, object> data)
    {
        var instance = EntityProxies.Instantiate(this.ObjectType, client);

        var t = this.ObjectType;
        var proxy = EntityProxies.GetProxy(t, instance);
        foreach (var (k, v) in data)
        {
            var property = EntityProxies.MapFromSerialized(t, k);
            proxy.Set(property, v);
        }

        return instance;
    }
}
