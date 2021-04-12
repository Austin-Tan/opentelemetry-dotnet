// <copyright file="TestZipkinExporter.cs" company="OpenTelemetry Authors">
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
    internal class TestZipkinExporter
    {
        internal static object Run(string zipkinUri)
        {
            // Prerequisite for running this example.
            // Setup zipkin inside local docker using following command:
            // docker run -d -p 9411:9411 openzipkin/zipkin

            // To run this example, run the following command from
            // the reporoot\examples\Console\.
            // (eg: C:\repos\opentelemetry-dotnet\examples\Console\)
            //
            // dotnet run zipkin -u http://localhost:9411/api/v2/spans

            var resAttributes = new List<KeyValuePair<string, object>>();
            resAttributes.Add(new KeyValuePair<string, object>("service.name", "AutanTest"));
            resAttributes.Add(new KeyValuePair<string, object>("testLong", 2L));
            resAttributes.Add(new KeyValuePair<string, object>("testShort", (short)2));
            resAttributes.Add(new KeyValuePair<string, object>("testBool", true));
            resAttributes.Add(new KeyValuePair<string, object>("testInt", (int)2));
            resAttributes.Add(new KeyValuePair<string, object>("testDouble", 2D));
            resAttributes.Add(new KeyValuePair<string, object>("testFloat", 2F));
            resAttributes.Add(new KeyValuePair<string, object>("testAttribute", "2wenty three"));
            resAttributes.Add(new KeyValuePair<string, object>("longarray", new long[] { 0, 1, 2, 3 }));
            resAttributes.Add(new KeyValuePair<string, object>("stringarray", new string[] { null, "0", "two", string.Empty, "three" }));

            using var tracerProvider = Sdk.CreateTracerProviderBuilder().AddSource("Test").SetResourceBuilder(ResourceBuilder.CreateDefault().AddAttributes(resAttributes)).AddZipkinExporter(o =>
            {
                o.Endpoint = new Uri(zipkinUri);
            }).Build();

            var activitySource = new ActivitySource("Test");
            using (var activity = activitySource.StartActivity("TestActivity"))
            {
                activity.SetTag("stringTag", "test");
                activity.SetTag("boolTag", true);
                activity.SetTag("longTag", 2L);
                activity.SetTag("doubleTag", 2D);

                activity.SetTag("intTag", (int)2);
                activity.SetTag("shortTag", (short)2);
                activity.SetTag("floatTag", 2f);

                activity.SetTag("stringArrTag", new string[] { null, "0", "two", string.Empty, "three" });
                activity.SetTag("boolArrTag", new bool[] { true, false });
                activity.SetTag("longArrTag", new long[] { 0, 1, 2 });
                activity.SetTag("doubleArrTag", new double[] { 0, 1, 2 });
                activity.SetTag("intArrTag", new int[] { 0, 1, 2 });
                activity.SetTag("shortArrTag", new short[] { 0, 1, 2 });
                activity.SetTag("floatArrTag", new float[] { 0, 1, 2 });
                activity.Stop();
            }

            activitySource.Dispose();

            return null;
        }
    }
}
