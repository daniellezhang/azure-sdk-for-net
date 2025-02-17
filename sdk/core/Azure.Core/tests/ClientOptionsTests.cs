// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Core.Pipeline;
using Azure.Core.Samples;
using Azure.Core.TestFramework;
using NUnit.Framework;

namespace Azure.Core.Tests
{
    [NonParallelizable]
    public class ClientOptionsTests
    {
        [TearDown]
        public void TearDown()
        {
            Core.ClientOptions.ResetDefaultOptions();
        }

        [Theory]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("1")]
        public void CanDisableDistributedTracingWithEnvironmentVariable(string value)
        {
            try
            {
                Environment.SetEnvironmentVariable("AZURE_TRACING_DISABLED", value);

                Core.ClientOptions.ResetDefaultOptions();

                var testOptions = new TestClientOptions();
                Assert.False(testOptions.Diagnostics.IsDistributedTracingEnabled);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_TRACING_DISABLED", null);
            }
        }

        [Test]
        public void UsesDefaultApplicationId()
        {
            try
            {
                DiagnosticsOptions.DefaultApplicationId = "Global-application-id";

                var testOptions = new TestClientOptions();
                Assert.AreEqual("Global-application-id", testOptions.Diagnostics.ApplicationId);
            }
            finally
            {
                DiagnosticsOptions.DefaultApplicationId = null;
            }
        }

        [Theory]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("1")]
        public void CanDisableTelemetryWithEnvironmentVariable(string value)
        {
            try
            {
                Environment.SetEnvironmentVariable("AZURE_TELEMETRY_DISABLED", value);

                Core.ClientOptions.ResetDefaultOptions();

                var testOptions = new TestClientOptions();
                Assert.False(testOptions.Diagnostics.IsTelemetryEnabled);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_TELEMETRY_DISABLED", null);
            }
        }

#if NETCOREAPP
        [Test]
        public void DefaultTransportIsHttpClientTransport()
        {
            var options = new TestClientOptions();

            Assert.IsInstanceOf<HttpClientTransport>(options.Transport);
        }
#else
        [Test]
        public void DefaultTransportIsHttpWebRequestTransport()
        {
            var options = new TestClientOptions();

            Assert.IsInstanceOf<HttpWebRequestTransport>(options.Transport);
        }

        [Test]
        public void DefaultTransportIsHttpClientTransportIfEnvVarSet()
        {
            string oldValue = Environment.GetEnvironmentVariable("AZURE_CORE_DISABLE_HTTPWEBREQUESTTRANSPORT");

            try
            {
                Environment.SetEnvironmentVariable("AZURE_CORE_DISABLE_HTTPWEBREQUESTTRANSPORT", "true");

                Core.ClientOptions.ResetDefaultOptions();

                var options = new TestClientOptions();

                Assert.IsInstanceOf<HttpClientTransport>(options.Transport);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AZURE_CORE_DISABLE_HTTPWEBREQUESTTRANSPORT", oldValue);
            }
        }

        [Test]
        public void DefaultTransportIsHttpClientTransportIfSwitchIsSet()
        {
            AppContext.TryGetSwitch("Azure.Core.Pipeline.DisableHttpWebRequestTransport", out bool oldSwitch);

            try
            {
                AppContext.SetSwitch("Azure.Core.Pipeline.DisableHttpWebRequestTransport", true);

                Core.ClientOptions.ResetDefaultOptions();

                var options = new TestClientOptions();

                Assert.IsInstanceOf<HttpClientTransport>(options.Transport);
            }
            finally
            {
                AppContext.SetSwitch("Azure.Core.Pipeline.DisableHttpWebRequestTransport", oldSwitch);
            }
        }

#endif

        [TestCaseSource(nameof(ClientOptions))]
        public void GlobalConfigurationIsApplied(Func<ClientOptions, object> set, Func<ClientOptions, object> get)
        {
            var initial = get(Core.ClientOptions.Default);
            var expected = set(Core.ClientOptions.Default);
            var testOptions = new TestClientOptions();

            Assert.AreNotEqual(initial, expected);
            Assert.AreEqual(expected, get(testOptions));
        }

        public static IEnumerable<object[]> ClientOptions()
        {
            object[] M(Func<ClientOptions, object> set, Func<ClientOptions, object> get) => new[] { set, get };

            yield return M(o => o.Transport = new MockTransport(), o => o.Transport);
            yield return M(o =>
            {
                var policy = new PipelineSamples.StopwatchPolicy();
                o.AddPolicy(policy, HttpPipelinePosition.PerCall);
                return policy;
            }, o => o.Policies?.LastOrDefault(p=>p.Position == HttpPipelinePosition.PerCall).Policy);

            yield return M(o =>
            {
                var policy = new PipelineSamples.StopwatchPolicy();
                o.AddPolicy(policy, HttpPipelinePosition.PerRetry);
                return policy;
            }, o => o.Policies?.LastOrDefault(p=>p.Position == HttpPipelinePosition.PerRetry).Policy);

            yield return M(o =>
            {
                var policy = new PipelineSamples.StopwatchPolicy();
                o.AddPolicy(policy, HttpPipelinePosition.BeforeTransport);
                return policy;
            }, o => o.Policies?.LastOrDefault(p=>p.Position == HttpPipelinePosition.BeforeTransport).Policy);

            yield return M(o => o.Retry.Delay = TimeSpan.FromDays(5), o => o.Retry.Delay);
            yield return M(o => o.Retry.Mode = RetryMode.Fixed, o => o.Retry.Mode);
            yield return M(o => o.Retry.MaxDelay = TimeSpan.FromDays(5), o => o.Retry.MaxDelay);
            yield return M(o => o.Retry.NetworkTimeout = TimeSpan.FromDays(5), o => o.Retry.NetworkTimeout);
            yield return M(o => o.Retry.MaxRetries = 44, o => o.Retry.MaxRetries);

            yield return M(o => o.Diagnostics.ApplicationId = "a", o => o.Diagnostics.ApplicationId);
            yield return M(o => o.Diagnostics.IsLoggingEnabled = false, o => o.Diagnostics.IsLoggingEnabled);
            yield return M(o => o.Diagnostics.IsTelemetryEnabled = false, o => o.Diagnostics.IsTelemetryEnabled);
            yield return M(o => o.Diagnostics.IsLoggingContentEnabled = true, o => o.Diagnostics.IsLoggingContentEnabled);
            yield return M(o => o.Diagnostics.LoggedContentSizeLimit = 100, o => o.Diagnostics.LoggedContentSizeLimit);
            yield return M(o => o.Diagnostics.IsDistributedTracingEnabled = false, o => o.Diagnostics.IsDistributedTracingEnabled);

            yield return M(o =>
            {
                o.Diagnostics.LoggedHeaderNames.Add("abc");
                return "abc";
            }, o => o.Diagnostics.LoggedHeaderNames.LastOrDefault());

            yield return M(o =>
            {
                o.Diagnostics.LoggedQueryParameters.Add("abc");
                return "abc";
            }, o => o.Diagnostics.LoggedQueryParameters.LastOrDefault());
        }

        private class TestClientOptions : ClientOptions
        {
        }
    }
}