// Copyright (c) Tobias Oskarsson. All Rights Reserved
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Oskarsson.AspNetCore.JsonPropertyInputFormatter
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     A <see cref="TextInputFormatter" /> that uses <see cref="FromJsonPropertyAttribute" /> to map the JSON content.
    /// </summary>
    public class JsonPropertyInputFormatter : SystemTextJsonInputFormatter
    {
        private readonly ILogger<JsonPropertyInputFormatter> _logger;

        /// <summary>
        ///     Initializes a new instance of <see cref="JsonPropertyInputFormatter" />.
        /// </summary>
        /// <param name="options">The <see cref="JsonOptions" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public JsonPropertyInputFormatter(JsonOptions options, ILogger<JsonPropertyInputFormatter> logger) : base(options, logger) => _logger = logger;

        /// <inheritdoc />
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            try
            {
                if (!context.HttpContext.Items.TryGetValue(nameof(JsonPropertyInputFormatter), out var obj) ||
                    !(obj is IConfiguration body))
                {
                    if (context.HttpContext.Request == null)
                    {
                        Log.HttpRequestMissingException(_logger);
                        return InputFormatterResult.FailureAsync();
                    }

                    // Enable buffering since we might have multiple FromJsonPropertyAttributes.
                    context.HttpContext.Request.EnableBuffering();

                    var configurationBuilder = new ConfigurationBuilder();
                    configurationBuilder.AddJsonStream(context.HttpContext.Request.Body);

                    // Reset the stream position.
                    context.HttpContext.Request.Body.Position = 0;

                    context.HttpContext.Items.Add(nameof(JsonPropertyInputFormatter),
                                                  body = configurationBuilder.Build());
                }

                if (context.Metadata is DefaultModelMetadata metadata)
                {
                    var attribute = metadata.Attributes.ParameterAttributes.OfType<FromJsonPropertyAttribute>().SingleOrDefault();
                    if (attribute == null)
                    {
                        Log.AttributeMissingException(_logger, nameof(FromJsonPropertyAttribute), context.Metadata.ParameterName);
                        return InputFormatterResult.FailureAsync();
                    }

                    var propertyName = attribute.PropertyName ?? context.Metadata.ParameterName;

                    var section = body.GetSection(propertyName);

                    var value = section.Get(metadata.ModelType);

                    if (value == null && !context.TreatEmptyInputAsDefaultValue)
                        return InputFormatterResult.NoValueAsync();

                    Log.JsonInputSuccess(_logger, context.ModelType);
                    return InputFormatterResult.SuccessAsync(value);
                }

                return InputFormatterResult.FailureAsync();
            }
            catch (Exception ex)
            {
                if (ex is JsonException jsonException)
                    context.ModelState.TryAddModelError(jsonException.Path, new InputFormatterException(ex.Message, ex), context.Metadata);
                else
                    context.ModelState.TryAddModelError(string.Empty, ex, context.Metadata);

                Log.JsonInputException(_logger, ex);
                return InputFormatterResult.FailureAsync();
            }
        }

        /// <inheritdoc />
        public override bool CanRead(InputFormatterContext context)
        {
            if (!base.CanRead(context))
                return false;

            if (!(context.Metadata is DefaultModelMetadata metadata))
                return false;

            var attribute = metadata.Attributes.ParameterAttributes.OfType<FromJsonPropertyAttribute>().SingleOrDefault();
            return attribute != null;
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception?> _jsonInputFormatterException;
            private static readonly Action<ILogger, string, Exception?> _jsonInputSuccess;
            private static readonly Action<ILogger, string, string, Exception?> _attributeMissingException;
            private static readonly Action<ILogger, Exception?> _httpRequestMissingException;

            static Log()
            {
                _jsonInputSuccess = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, "FromJsonPropertyInputSuccess"), "JSON input formatter succeeded, deserializing to type '{TypeName}'");
                _jsonInputFormatterException = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2, "NewtonsoftJsonInputException"), "JSON input formatter throw an exception: {Message}");
                _attributeMissingException = LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(3, "FromJsonPropertyAttributeMissing"), "Could not find attribute {Attribute} on parameter {ParameterName}.");
                _httpRequestMissingException = LoggerMessage.Define(LogLevel.Error, new EventId(4, "HttpRequestMissing"), "Could not find HTTP request.");
            }

            public static void JsonInputException(ILogger logger, Exception exception) => _jsonInputFormatterException(logger, exception.Message, exception);

            public static void JsonInputSuccess(ILogger logger, Type modelType) => _jsonInputSuccess(logger, modelType.Name, null);

            public static void AttributeMissingException(ILogger logger, string attributeName, string parameterName) => _attributeMissingException(logger, attributeName, parameterName, null);

            public static void HttpRequestMissingException(ILogger logger) => _httpRequestMissingException(logger, null);
        }
    }
}