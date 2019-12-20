// Copyright (c) Tobias Oskarsson. All Rights Reserved
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Oskarsson.AspNetCore.JsonPropertyInputFormatter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///     Sets up MVC options for <see cref="JsonPropertyInputFormatter" />.
    /// </summary>
    public class JsonPropertyInputFormatterConfigureSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IOptions<JsonOptions> _jsonOptions;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonPropertyInputFormatterConfigureSetup" /> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="jsonOptions">The <see cref="JsonOptions" />.</param>
        public JsonPropertyInputFormatterConfigureSetup(ILoggerFactory loggerFactory, IOptions<JsonOptions> jsonOptions)
        {
            _loggerFactory = loggerFactory;
            _jsonOptions = jsonOptions;
        }

        /// <summary>
        ///     Invoked to configure a <see cref="MvcOptions" /> instance.
        /// </summary>
        /// <param name="options">The options instance to configure.</param>
        public void Configure(MvcOptions options) => options.InputFormatters.Insert(0, new JsonPropertyInputFormatter(_jsonOptions.Value, _loggerFactory.CreateLogger<JsonPropertyInputFormatter>()));
    }
}