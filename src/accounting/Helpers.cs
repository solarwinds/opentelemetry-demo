// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Collections;

namespace Accounting
{
    internal static class Helpers
    {
        private static List<string> RelevantPrefixes = ["DOTNET_", "CORECLR_", "OTEL_", "KAFKA_"];

        // The SWO agent injects OTEL_EXPORTER_OTLP_HEADERS with the ingestion token at runtime.
        private static readonly string[] SensitiveKeyParts = ["HEADERS", "TOKEN", "KEY", "SECRET", "PASSWORD"];

        public static IEnumerable<DictionaryEntry> FilterRelevant(this IDictionary envs)
        {
            foreach (DictionaryEntry env in envs)
            {
                foreach (var prefix in RelevantPrefixes)
                {
                    if (env.Key.ToString()?.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        yield return env;
                    }
                }
            }
        }

        public static void OutputInOrder(this IEnumerable<DictionaryEntry> envs)
        {
            foreach (var env in envs.OrderBy(x => x.Key))
            {
                var key = env.Key.ToString() ?? string.Empty;
                var sensitive = SensitiveKeyParts.Any(p => key.Contains(p, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(sensitive ? $"[{key}, <redacted>]" : env.ToString());
            }
        }
    }
}
