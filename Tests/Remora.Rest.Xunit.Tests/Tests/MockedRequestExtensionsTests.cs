//
//  MockedRequestExtensionsTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="Extensions.MockedRequestExtensions"/> class.
/// </summary>
public class MockedRequestExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithNoContent"/> method.
    /// </summary>
    public class WithNoContent
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request contains any content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestContainsContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithNoContent()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("wooga");

            await Assert.ThrowsAsync<NullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request does not contain any content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfRequestDoesNotContainContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithNoContent()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithAuthentication"/> method.
    /// </summary>
    public class WithAuthentication
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestHasNoAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has an authorization header that does not match
        /// the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestHasAuthorizationHeaderThatDoesNotMatch()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication(a => a.Scheme is "Bearer" && a.Parameter is "wooga")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "booga");

            await Assert.ThrowsAsync<TrueException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfRequestHasAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfRequestHasMatchingAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication(a => a.Scheme is "Bearer" && a.Parameter is "wooga")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithJson"/> method.
    /// </summary>
    public class WithJson
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestHasNoContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestHasNonJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("not json");

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a JSON body that does not match the
        /// predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfRequestHasJsonContentThatDoesNotMatch()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson(j => j.IsObject(o => o.WithProperty("value", p => p.Is(0))))
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 1 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await Assert.ThrowsAnyAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfRequestHasJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfRequestHasMatchingJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson(j => j.IsObject(o => o.WithProperty("value", p => p.Is(0))))
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.MockedRequestExtensions.WithMultipartFormData(MockedRequest,string,string)"/> method
    /// and its overloads.
    /// </summary>
    public class WithMultipartFormData
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request has no content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestHasNoContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request content is not multipart content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestIsNotMultipartContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("fail");

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no form data field with the correct name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestHasNoFormDataWithName()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("something else"), "not value");

            request.Content = multipart;

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestHasFormDataWithCorrectNameButWrongType()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "value");

            request.Content = multipart;

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestHasFormDataWithCorrectNameButWrongValue()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("not value"), "value");

            request.Content = multipart;

            await Assert.ThrowsAsync<EqualException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method returns true if the request has a form data field where the name and value match.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFieldRequestHasFormDataWithCorrectNameAndCorrectValue()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("0"), "value");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestHasNoContent()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request content is not multipart content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestIsNotMultipartContent()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("fail");

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no form data field with the correct name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestHasNoFormDataWithName()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("something else"), "not value");

            request.Content = multipart;

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongType()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "file", "filename.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndValueButWrongFilename()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", "wrong.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<NotNullException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongValue()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(new MemoryStream()), "file", "filename.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<EqualException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method returns true if the request has a form data field where the name and value match.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task PassesIfFileRequestHasFormDataWithCorrectNameAndFilenameAndValue()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", "filename.txt");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
