﻿// <copyright file="MetricsEndpointMiddleware.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using App.Metrics.Extensions.Middleware.Abstractions;
using App.Metrics.Extensions.Middleware.DependencyInjection.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Middleware
{
    // ReSharper disable ClassNeverInstantiated.Global
    public class MetricsEndpointMiddleware : AppMetricsMiddleware<AspNetMetricsOptions>
        // ReSharper restore ClassNeverInstantiated.Global
    {
        private readonly IMetricsResponseWriter _metricsResponseWriter;
        private readonly RequestDelegate _next;

        public MetricsEndpointMiddleware(
            RequestDelegate next,
            AspNetMetricsOptions aspNetOptions,
            ILoggerFactory loggerFactory,
            IMetrics metrics,
            IMetricsResponseWriter metricsResponseWriter)
            : base(next, aspNetOptions, loggerFactory, metrics)
        {
            _metricsResponseWriter = metricsResponseWriter ?? throw new ArgumentNullException(nameof(metricsResponseWriter));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        // ReSharper disable UnusedMember.Global
        public async Task Invoke(HttpContext context)
            // ReSharper restore UnusedMember.Global
        {
            if (Options.MetricsEndpointEnabled && Options.MetricsEndpoint.IsPresent() && Options.MetricsEndpoint == context.Request.Path)
            {
                Logger.MiddlewareExecuting(GetType());

                context.Response.Headers["Content-Type"] = new[] { _metricsResponseWriter.ContentType };
                context.SetNoCacheHeaders();
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                await _metricsResponseWriter.WriteAsync(context, Metrics.Snapshot.Get(), context.RequestAborted).ConfigureAwait(false);

                Logger.MiddlewareExecuted(GetType());

                return;
            }

            await _next(context);
        }
    }
}