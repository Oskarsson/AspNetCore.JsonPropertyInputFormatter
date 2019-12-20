// Copyright (c) Tobias Oskarsson. All Rights Reserved
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Oskarsson.AspNetCore.JsonPropertyInputFormatter
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///     Extension methods for adding <see cref="JsonPropertyInputFormatter" />.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        ///     Configures <see cref="JsonPropertyInputFormatter" /> for the specified <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" />.</param>
        /// <returns>The <see cref="IMvcBuilder" />.</returns>
        public static IMvcBuilder AddJsonPropertyInputFormatter(this IMvcBuilder builder)
        {
            builder.Services.AddTransient<IConfigureOptions<MvcOptions>, JsonPropertyInputFormatterConfigureSetup>();

            // Required to be able to use ConfigurationBuilder since there is not asynchronous version.
            builder.Services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
            builder.Services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);

            return builder;
        }
    }
}