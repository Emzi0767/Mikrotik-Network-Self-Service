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

namespace Emzi0767.NetworkSelfService.Mikrotik.Exceptions;

/// <summary>
/// Specifies the category of a Mikrotik error.
/// </summary>
public enum MikrotikApiErrorCategory
{
    /// <summary>
    /// Specifies that a category is unknown.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Specifies that the command is missing something.
    /// </summary>
    MissingItemOrCommand = 0,

    /// <summary>
    /// Specifies that parsing a value failed.
    /// </summary>
    ArgumentValueFailure = 1,

    /// <summary>
    /// Specifies that execution of the command was interrupted.
    /// </summary>
    ExecutionInterrupted = 2,

    /// <summary>
    /// Specifies that a failure occured in scripting engine.
    /// </summary>
    ScriptingFailure = 3,

    /// <summary>
    /// Specifies a general failure.
    /// </summary>
    GeneralFailure = 4,

    /// <summary>
    /// Specifies an API failure.
    /// </summary>
    ApiFailure = 5,

    /// <summary>
    /// Specifies a TTY failure.
    /// </summary>
    TtyFailure = 6,

    /// <summary>
    /// Specifies a failure related to a :return command.
    /// </summary>
    ReturnValue = 7,
}
