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
using System.Reflection;

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

internal ref struct MikrotikExpressionParserState
{
    public Type RootType { get; }
    public IList<IMikrotikWord> Words { get; }
    public Type ResultType { get; set; }
    public IList<string> IncludedMemberNames { get; }
    public Type QueryableType { get; set; }
    public ConstructorInfo AnonymousConstructor { get; set; }
    public IDictionary<string, string> AnonymousPropertyMap { get; set; }
    public bool IsSelectMany { get; set; }

    public MikrotikExpressionParserState(Type rootType)
    {
        this.RootType = rootType;
        this.Words = new List<IMikrotikWord>();
        this.ResultType = rootType;
        this.IncludedMemberNames = new List<string>();
        this.QueryableType = typeof(IAsyncQueryable<>).MakeGenericType(rootType);
        this.AnonymousConstructor = null;
        this.IsSelectMany = false;
    }
}
