﻿// <copyright file="DefaultHistogramManager.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Core.Options;
using App.Metrics.Histogram.Abstractions;
using App.Metrics.Registry.Abstractions;
using App.Metrics.Tagging;

namespace App.Metrics.Histogram
{
    internal sealed class DefaultHistogramManager : IMeasureHistogramMetrics
    {
        private readonly IBuildHistogramMetrics _histogramBuilder;
        private readonly IMetricsRegistry _registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultHistogramManager" /> class.
        /// </summary>
        /// <param name="registry">The registry storing all metric data.</param>
        /// <param name="histogramBuilder">The histogram builder.</param>
        public DefaultHistogramManager(IBuildHistogramMetrics histogramBuilder, IMetricsRegistry registry)
        {
            _registry = registry;
            _histogramBuilder = histogramBuilder;
        }

        /// <inheritdoc />
        public void Update(HistogramOptions options, long value)
        {
            _registry.Histogram(options, () => _histogramBuilder.Build(options.Reservoir)).Update(value);
        }

        /// <inheritdoc />
        public void Update(HistogramOptions options, MetricTags tags, long value)
        {
            _registry.Histogram(options, tags, () => _histogramBuilder.Build(options.Reservoir)).Update(value);
        }

        /// <inheritdoc />
        public void Update(HistogramOptions options, long value, string userValue)
        {
            _registry.Histogram(options, () => _histogramBuilder.Build(options.Reservoir)).Update(value, userValue);
        }

        /// <inheritdoc />
        public void Update(HistogramOptions options, MetricTags tags, long value, string userValue)
        {
            _registry.Histogram(options, tags, () => _histogramBuilder.Build(options.Reservoir)).Update(value, userValue);
        }
    }
}