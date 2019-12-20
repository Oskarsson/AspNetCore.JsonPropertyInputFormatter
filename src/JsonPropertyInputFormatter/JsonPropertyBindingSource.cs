// Copyright (c) Tobias Oskarsson. All Rights Reserved
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Oskarsson.AspNetCore.JsonPropertyInputFormatter
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    ///     A metadata object representing a source of data for model binding.
    /// </summary>
    internal class JsonPropertyBindingSource : BindingSource
    {
        /// <summary>
        ///     Creates a new <see cref="JsonPropertyBindingSource" />.
        /// </summary>
        public JsonPropertyBindingSource() : base(nameof(JsonPropertyBindingSource), "JsonProperty", true, true) { }

        /// <summary>
        ///     Gets a value indicating whether or not the <see cref="JsonPropertyBindingSource" /> can accept data from
        ///     <paramref name="bindingSource" />.
        /// </summary>
        /// <param name="bindingSource">The <see cref="BindingSource" /> to consider as input.</param>
        /// <returns><c>True</c> if the source is compatible, otherwise <c>false</c>.</returns>
        /// <remarks>
        ///     When using this method, it is expected that the left-hand-side is metadata specified
        ///     on a property or parameter for model binding, and the right hand side is a source of
        ///     data used by a model binder or value provider.
        ///     This distinction is important as the left-hand-side may be a composite, but the right
        ///     may not.
        /// </remarks>
        public override bool CanAcceptDataFrom(BindingSource bindingSource) => bindingSource == Body;
    }
}