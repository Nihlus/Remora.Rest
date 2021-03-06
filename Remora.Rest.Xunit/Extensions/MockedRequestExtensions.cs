//
//  MockedRequestExtensions.cs
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using JetBrains.Annotations;
using Remora.Rest.Xunit.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Rest.Xunit.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="MockHttpMessageHandler"/> class.
/// </summary>
[PublicAPI]
public static class MockedRequestExtensions
{
    /// <summary>
    /// Adds a requirement that the request has no content.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <returns>The request; with the new requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public static MockedRequest WithNoContent(this MockedRequest request)
    {
        return request.With(m =>
        {
            Assert.Null(m.Content);
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has an authorization header.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="headerPredicate">The predicate check.</param>
    /// <returns>The request; with the new requirement.</returns>
    public static MockedRequest WithAuthentication
    (
        this MockedRequest request,
        Func<AuthenticationHeaderValue, bool>? headerPredicate = null
    )
    {
        return request.With(m =>
        {
            Assert.NotNull(m.Headers.Authorization);
            if (headerPredicate is null)
            {
                return true;
            }

            var predicateMatches = headerPredicate(m.Headers.Authorization!);
            Assert.True(predicateMatches, "The authentication predicate did not match.");

            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has a Json body.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the Json body.</param>
    /// <returns>The request; with the new requirements.</returns>
    public static MockedRequest WithJson
    (
        this MockedRequest request,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        return request.With(new JsonRequestMatcher(elementMatcher.Build()));
    }

    /// <summary>
    /// Adds a requirement that the multipart request has a JSON payload.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the JSON payload.</param>
    /// <returns>The request; with the new requirements.</returns>
    public static MockedRequest WithMultipartJsonPayload
    (
        this MockedRequest request,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        return request.With(new MultipartJsonPayloadRequestMatcher(elementMatcher.Build()));
    }

    /// <summary>
    /// Adds a requirement that the request has multipart form data with the given string field.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="name">The name of the form field.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithMultipartFormData
    (
        this MockedRequest request,
        string name,
        string value
    )
    {
        return request.With
        (
            m =>
            {
                Assert.NotNull(m.Content);
                Assert.IsType<MultipartFormDataContent>(m.Content);

                var formContent = (MultipartFormDataContent)m.Content!;
                var contentWithName = formContent.FirstOrDefault
                (
                    c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}")))
                );

                Assert.NotNull(contentWithName);
                Assert.IsType<StringContent>(contentWithName);
                var stringContent = (StringContent)contentWithName!;

                var actualValue = stringContent.ReadAsStringAsync().GetAwaiter().GetResult();

                Assert.Equal(value, actualValue);
                return true;
            }
        );
    }

    /// <summary>
    /// Adds a requirement that the request has multipart form data with the given file-type stream field.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="name">The name of the form field.</param>
    /// <param name="fileName">The filename of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithMultipartFormData
    (
        this MockedRequest request,
        string name,
        string fileName,
        Stream value
    )
    {
        return request.With
        (
            m =>
            {
                Assert.NotNull(m.Content);
                Assert.IsType<MultipartFormDataContent>(m.Content);

                var formContent = (MultipartFormDataContent)m.Content!;

                var contentWithName = formContent.FirstOrDefault
                (
                    c =>
                        c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}"))) &&
                        c.Headers.Any(h => h.Value.Any(v => v.Contains($"filename={fileName}")))
                );

                Assert.NotNull(contentWithName);
                Assert.IsType<StreamContent>(contentWithName);
                var streamContent = (StreamContent)contentWithName!;

                // Reflection hackery
                var innerStream = (Stream)typeof(StreamContent)
                    .GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .GetValue(streamContent)!;

                Assert.Equal(value, innerStream);
                return true;
            }
        );
    }
}
