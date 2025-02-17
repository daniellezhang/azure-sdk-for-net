﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.WebPubSub.Common;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Microsoft.Azure.WebPubSub.AspNetCore.Tests
{
    [TestFixture]
    public class WebPubSubEventRequestTests
    {
        private static readonly Uri TestUri = new Uri("https://my-host.com");

        [Test]
        public void TestUpdateConnectionState()
        {
            var exist = new Dictionary<string, object>
            {
                { "aaa", "aaa" },
                { "bbb", "bbb" }
            };
            var connectionContext = new WebPubSubConnectionContext();

            connectionContext.InitStates(exist);

            var response = new ConnectEventResponse
            {
                UserId = "aaa"
            };
            response.SetState("test", "ddd");
            response.SetState("bbb", "bbb1");
            var updated = connectionContext.UpdateStates(response.States);

            // new
            Assert.AreEqual("ddd", updated["test"]);
            // no change
            Assert.AreEqual("aaa", updated["aaa"]);
            // update
            Assert.AreEqual("bbb1", updated["bbb"]);

            response.ClearStates();
            updated = connectionContext.UpdateStates(response.States);

            // After clear is null.
            Assert.IsNull(updated);
        }

        [Test]
        public void TestEncodeAndDecodeState()
        {
            var state = new Dictionary<string, object>
            {
                { "aaa", "aaa" },
                { "bbb", "bbb" }
            };

            var encoded = state.EncodeConnectionStates();

            var decoded = encoded.DecodeConnectionStates();

            Assert.AreEqual(state, decoded);
        }

        [Test]
        public void TestConnectEventDeserialize()
        {
            var request = "{\"claims\":{\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\":[\"ddd\"],\"nbf\":[\"1629183374\"],\"exp\":[\"1629186974\"],\"iat\":[\"1629183374\"],\"aud\":[\"http://localhost:8080/client/hubs/chat\"],\"sub\":[\"ddd\"]},\"query\":{\"access_token\":[\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZGQiLCJuYmYiOjE2MjkxODMzNzQsImV4cCI6MTYyOTE4Njk3NCwiaWF0IjoxNjI5MTgzMzc0LCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvY2xpZW50L2h1YnMvY2hhdCJ9.tqD8ykjv5NmYw6gzLKglUAv-c-AVWu-KNZOptRKkgMM\"]},\"subprotocols\":[\"protocol1\",\"protocol2\"],\"clientCertificates\":[]}";

            var converted = JsonSerializer.Deserialize<ConnectEventRequest>(request);

            Assert.AreEqual(6, converted.Claims.Count);
            Assert.AreEqual(1, converted.Query.Count);
            Assert.AreEqual(2, converted.Subprotocols.Count);
            Assert.AreEqual(new string[] { "protocol1", "protocol2" }, converted.Subprotocols);
            Assert.NotNull(converted.ClientCertificates);
            Assert.AreEqual(0, converted.ClientCertificates.Count);
        }

        [Test]
        public void TestConnectEventRequestSerialize()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };
            var claims = new Dictionary<string, string[]>
            {
                {"aaa", new string[]{"a1", "a2"} },
                {"bbb", new string[]{"b1", "b2"} },
                {"ccc", new string[]{"c1", "c2"} },
            };
            var query = new Dictionary<string, string[]>
            {
                {"query1", new string[]{"a1", "a2"} },
                {"query2", new string[]{"b1"} },
            };
            var certs = new List<WebPubSubClientCertificate>
            { 
                new WebPubSubClientCertificate("111"), 
                new WebPubSubClientCertificate("222")
            };
            var request = new ConnectEventRequest(connectionContext, claims, query, new string[] { "protocol1", "protocol2" }, certs);

            var serilized = JsonSerializer.Serialize(request);

            var converted = JsonSerializer.Deserialize<ConnectEventRequest>(serilized);

            Assert.AreEqual(3, converted.Claims.Count);
            Assert.AreEqual(2, converted.Query.Count);
            Assert.AreEqual(2, converted.Subprotocols.Count);
            Assert.AreEqual(new string[] { "protocol1", "protocol2" }, converted.Subprotocols);
            Assert.NotNull(converted.ClientCertificates);
            Assert.AreEqual(2, converted.ClientCertificates.Count);
        }

        [Test]
        public void TestUserEventRequestSerialize()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };

            var request = new UserEventRequest(connectionContext, BinaryData.FromString("Hello World"), WebPubSubDataType.Text);

            var serialized = JsonSerializer.Serialize(request);
        }

        [Test]
        public void TestDisconnectedEventDeserialize()
        {
            var request = "{\"reason\":\"invalid\"}";

            var converted = JsonSerializer.Deserialize<DisconnectedEventRequest>(request);

            Assert.AreEqual("invalid", converted.Reason);
        }

        [Test]
        public async Task TestParseConnectRequest()
        {
            var body = "{\"claims\":{\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\":[\"ddd\"],\"nbf\":[\"1629183374\"],\"exp\":[\"1629186974\"],\"iat\":[\"1629183374\"],\"aud\":[\"http://localhost:8080/client/hubs/chat\"],\"sub\":[\"ddd\"]},\"query\":{\"access_token\":[\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkZGQiLCJuYmYiOjE2MjkxODMzNzQsImV4cCI6MTYyOTE4Njk3NCwiaWF0IjoxNjI5MTgzMzc0LCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvY2xpZW50L2h1YnMvY2hhdCJ9.tqD8ykjv5NmYw6gzLKglUAv-c-AVWu-KNZOptRKkgMM\"]},\"subprotocols\":[\"protocol1\", \"protocol2\"],\"clientCertificates\":[]}";
            var context = PrepareHttpContext(TestUri, WebPubSubEventType.System, Constants.Events.ConnectEvent, body: body);

            var request = await context.Request.ReadWebPubSubEventAsync(null);

            Assert.AreEqual(typeof(ConnectEventRequest), request.GetType());

            var connectRequest = request as ConnectEventRequest;

            Assert.NotNull(connectRequest.ConnectionContext);
            Assert.AreEqual(TestUri.Host, connectRequest.ConnectionContext.Origin);
        }

        [Test]
        public async Task TestParseConnectedRequest()
        {
            var context = PrepareHttpContext(TestUri, WebPubSubEventType.System, Constants.Events.ConnectedEvent);

            var request = await context.Request.ReadWebPubSubEventAsync(null);

            Assert.AreEqual(typeof(ConnectedEventRequest), request.GetType());

            var connectedRequest = request as ConnectedEventRequest;

            Assert.NotNull(connectedRequest.ConnectionContext);
            Assert.AreEqual(TestUri.Host, connectedRequest.ConnectionContext.Origin);
        }

        [Test]
        public async Task TestParseUserRequest()
        {
            var text = "hello world";
            var context = PrepareHttpContext(TestUri, WebPubSubEventType.User, "message", body: text);

            var request = await context.Request.ReadWebPubSubEventAsync(null);

            Assert.AreEqual(typeof(UserEventRequest), request.GetType());

            var userRequest = request as UserEventRequest;

            Assert.NotNull(userRequest.ConnectionContext);
            Assert.AreEqual(TestUri.Host, userRequest.ConnectionContext.Origin);
            Assert.AreEqual(text, userRequest.Data.ToString());
        }

        [TestCase("7aab239577fd4f24bc919802fb629f5f", true)]
        [TestCase("ccc", false)]
        public void TestSignatureCheck(string accessKey, bool valid)
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };
            var options = new WebPubSubValidationOptions($"Endpoint={TestUri};AccessKey={accessKey};Version=1.0;");
            var result = connectionContext.IsValidSignature(options);
            Assert.AreEqual(valid, result);
        }

        [Test]
        public void TestSignatureCheck_OptionsNullSuccess()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };
            var result = connectionContext.IsValidSignature(null);
            Assert.True(result);
        }

        [Test]
        public void TestSignatureCheck_OptionsEmptySuccess()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };
            var result = connectionContext.IsValidSignature(null);
            Assert.True(result);
        }

        [Test]
        public void TestSignatureCheck_AccessKeyEmptySuccess()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
                Origin = TestUri.Host
            };
            var options = new WebPubSubValidationOptions($"Endpoint={TestUri};Version=1.0;");
            var result = connectionContext.IsValidSignature(options);
            Assert.True(result);
        }

        [Test]
        public void TestSignatureCheck_SignatureNullFail()
        {
            var connectionContext = new WebPubSubConnectionContext()
            {
                ConnectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
                Signature = null,
                Origin = TestUri.Host
            };
            var options = new WebPubSubValidationOptions($"Endpoint={TestUri};AccessKey=7aab239577fd4f24bc919802fb629f5f;Version=1.0;");
            var result = connectionContext.IsValidSignature(options);
            Assert.False(result);
        }

        [TestCase("OPTIONS", true)]
        [TestCase("DELETE", false)]
        public void TestAbuseProtection(string httpMethod, bool valid)
        {
            var context = PrepareHttpContext(TestUri, WebPubSubEventType.System, Constants.Events.ConnectEvent, httpMethod: httpMethod);

            var result = context.Request.IsPreflightRequest(out var requestHosts);

            Assert.AreEqual(valid, result);

            if (valid)
            {
                Assert.NotNull(requestHosts);
                Assert.AreEqual(TestUri.Host, requestHosts[0]);
            }
        }

        [TestCase("my-host.com", true)]
        [TestCase("my-host1.com", false)]
        [TestCase("localhost", false)]
        [TestCase("http://localhost", false)]
        public void TestAbuseProtectionCompare(string requestHost, bool expected)
        {
            var options = new WebPubSubValidationOptions($"Endpoint=https://my-host.com;AccessKey=7aab239577fd4f24bc919802fb629f5f;Version=1.0;");

            var result = false;
            if (options.ContainsHost(requestHost))
            {
                result = true;
            }
            Assert.AreEqual(expected, result);
        }

        private static HttpContext PrepareHttpContext(
            Uri uri,
            WebPubSubEventType type,
            string eventName,
            string connectionId = "0f9c97a2f0bf4706afe87a14e0797b11",
            string signatures = "sha256=7767effcb3946f3e1de039df4b986ef02c110b1469d02c0a06f41b3b727ab561",
            string hub = "testhub",
            string httpMethod = "POST",
            string userId = "testuser",
            string body = null,
            string contentType = Constants.ContentTypes.PlainTextContentType)
        {
            var context = new DefaultHttpContext();
            var services = new ServiceCollection();
            var sp = services.BuildServiceProvider();
            context.RequestServices = sp;

            var request = context.Request;
            var requestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>();
            requestFeature.Method = httpMethod;
            requestFeature.Scheme = uri.Scheme;
            requestFeature.PathBase = uri.Host;
            requestFeature.Path = uri.GetComponents(UriComponents.KeepDelimiter | UriComponents.Path, UriFormat.Unescaped);
            requestFeature.PathBase = "/";
            requestFeature.QueryString = uri.GetComponents(UriComponents.KeepDelimiter | UriComponents.Query, UriFormat.Unescaped);

            var headers = new HeaderDictionary
            {
                { Constants.Headers.CloudEvents.Hub, hub },
                { Constants.Headers.CloudEvents.Type, GetFormedType(type, eventName) },
                { Constants.Headers.CloudEvents.EventName, eventName },
                { Constants.Headers.CloudEvents.ConnectionId, connectionId },
                { Constants.Headers.CloudEvents.Signature, signatures }
            };

            if (!string.IsNullOrEmpty(uri.Host))
            {
                headers.Add("Host", uri.Host);
                headers.Add(Constants.Headers.WebHookRequestOrigin, uri.Host);
            }

            if (userId != null)
            {
                headers.Add(Constants.Headers.CloudEvents.UserId, userId);
            }

            if (body != null)
            {
                requestFeature.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                request.ContentLength = request.Body.Length;
                headers.Add("Content-Length", request.Body.Length.ToString());
                request.ContentType = contentType;
                headers.Add("Content-Type", contentType);
            }

            requestFeature.Headers = headers;

            return context;
        }

        private static string GetFormedType(WebPubSubEventType type, string eventName)
        {
            return type == WebPubSubEventType.User ?
                $"{Constants.Headers.CloudEvents.TypeUserPrefix}{eventName}" :
                $"{Constants.Headers.CloudEvents.TypeSystemPrefix}{eventName}";
        }
    }
}
#endif