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
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Emzi0767.NetworkSelfService.Backend;

/// <summary>
/// Specifies that a given property requires a value, but only if other properties have one.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public sealed class RequiredIfAttribute : ValidationAttribute
{
    /// <summary>
    /// Gets the property names which this property's optionality is conditional on.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the value which this property's optionality is conditional on.
    /// </summary>
    public object PropertyValue { get; }

    /// <summary>
    /// Designates this property as conditionally optional, if other properties have the value.
    /// </summary>
    /// <param name="propertyName">Property the optionality of this property is dependent on.</param>
    /// <param name="propertyValue">Value the optionality of this property is dependent on.</param>
    public RequiredIfAttribute(string propertyName, object propertyValue)
    {
        this.PropertyName = propertyName;
        this.PropertyValue = propertyValue;
    }

    /// <summary>
    /// Performs validation of the property value.
    /// </summary>
    /// <param name="value">Value to validate.</param>
    /// <param name="validationContext">Context to validate in.</param>
    /// <returns>Validation result.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var obj = validationContext.ObjectInstance;
        if (obj is null)
            return new("Object instance is null.", [ this.PropertyName ]);

        var prop = validationContext.ObjectType
            .GetProperty(this.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var propVal = prop.GetValue(obj);
        return (value is not null || !object.Equals(value, propVal))
            ? ValidationResult.Success
            : new("A value is required.", [ validationContext.MemberName ]);
    }

}
