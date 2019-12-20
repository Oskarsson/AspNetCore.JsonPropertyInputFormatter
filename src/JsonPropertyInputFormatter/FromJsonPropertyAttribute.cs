// Copyright (c) Tobias Oskarsson. All Rights Reserved
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Oskarsson.AspNetCore.JsonPropertyInputFormatter
{
    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    ///     Specifies that a parameter should be bound to a JSON property using the request body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromJsonPropertyAttribute : Attribute, IBindingSourceMetadata
    {
        private static readonly BindingSource _bindingSourceInstance = new JsonPropertyBindingSource();

        /// <summary>
        ///     The name of the property.
        /// </summary>
        /// <remarks>If null the name of the parameter is used instead.</remarks>
        public string? PropertyName { get; }

        /// <summary>
        ///     Gets the <see cref="BindingSource" />.
        /// </summary>
        /// <remarks>
        ///     The <see cref="IBindingSourceMetadata.BindingSource" /> is metadata which can be used to determine which data
        ///     sources are valid for model binding of a property or parameter.
        /// </remarks>
        public BindingSource BindingSource => _bindingSourceInstance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FromJsonPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">The name of the JSON property.</param>
        public FromJsonPropertyAttribute(string? propertyName = null) => PropertyName = propertyName;
    }
}