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

namespace Emzi0767.NetworkSelfService.Mikrotik.Expressions;

internal interface IMikrotikQuery
{ }

internal enum MikrotikBinaryQueryOperator
{
    Invalid,
    Or,
    And,
}

internal enum MikrotikComparisonQueryOperator
{
    Invalid,
    Equals,
    GreaterThan,
    LessThan,
}

internal readonly record struct MikrotikBinaryQuery(
    IMikrotikQuery Left,
    IMikrotikQuery Right,
    MikrotikBinaryQueryOperator Operator
) : IMikrotikQuery;

internal readonly record struct MikrotikHasPropertyQuery(string Property) : IMikrotikQuery;

internal readonly record struct MikrotikLacksPropertyQuery(string Property) : IMikrotikQuery;

internal readonly record struct MikrotikComparisonQuery(
    string Property,
    string Value,
    MikrotikComparisonQueryOperator Operator
) : IMikrotikQuery;

internal readonly record struct MikrotikNegationQuery(
    IMikrotikQuery Inner
) : IMikrotikQuery;
