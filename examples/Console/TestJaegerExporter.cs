// <copyright file="TestJaegerExporter.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Examples.Console
{
    internal class TestJaegerExporter
    {
        internal static object Run(string host, int port)
        {
            // Prerequisite for running this example.
            // Setup Jaegar inside local docker using following command (Source: https://www.jaegertracing.io/docs/1.21/getting-started/#all-in-one):
            /*
            $ docker run -d --name jaeger \
            -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 \
            -p 5775:5775/udp \
            -p 6831:6831/udp \
            -p 6832:6832/udp \
            -p 5778:5778 \
            -p 16686:16686 \
            -p 14268:14268 \
            -p 14250:14250 \
            -p 9411:9411 \
            jaegertracing/all-in-one:1.21
            */

            // To run this example, run the following command from
            // the reporoot\examples\Console\.
            // (eg: C:\repos\opentelemetry-dotnet\examples\Console\)
            //
            // dotnet run jaeger -h localhost -p 6831
            // For non-Windows (e.g., MacOS)
            // dotnet run jaeger -- -h localhost -p 6831
            return RunWithActivity(host, port);
        }

        internal static object RunWithActivity(string host, int port)
        {
            var attributes = new List<KeyValuePair<string, object>>();
            attributes.Add(new KeyValuePair<string, object>("testAttribute", "2wenty three"));
            attributes.Add(new KeyValuePair<string, object>("array", new long[] { 0, 1, 2, 3 }));

            // Enable OpenTelemetry for the sources "Samples.SampleServer" and "Samples.SampleClient"
            // and use the Jaeger exporter.
            using var openTelemetry = Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddAttributes(attributes).AddService("jaeger-test"))
                    .AddSource("Samples.SampleClient", "Samples.SampleServer")
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = host;
                        o.AgentPort = port;

                        // Examples for the rest of the options, defaults unless otherwise specified
                        // Omitting Process Tags example as Resource API is recommended for additional tags
                        o.MaxPayloadSizeInBytes = 4096;

                        // Using Batch Exporter (which is default)
                        // The other option is ExportProcessorType.Simple
                        o.ExportProcessorType = ExportProcessorType.Batch;
                        o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>()
                        {
                            MaxQueueSize = 2048,
                            ScheduledDelayMilliseconds = 5000,
                            ExporterTimeoutMilliseconds = 30000,
                            MaxExportBatchSize = 512,
                        };
                    })
                    .Build();

            var source = new ActivitySource("Samples.SampleClient");
            using (var span = source.StartActivity("autan-span"))
            {
                span.SetTag("testTag", 3);
                span.SetTag("SpanArrLong", new long[] { 0, 1, 2, 3 });
                span.SetTag("SpanArrString", new string[] { "0", "a", string.Empty, "test" });
                span.SetTag("SpanArrInt", new int[] { 0, 1, 2 });
            }

            System.Console.WriteLine("Finished austin's test section");
            System.Console.ReadLine();

            return null;
        }
    }
}
