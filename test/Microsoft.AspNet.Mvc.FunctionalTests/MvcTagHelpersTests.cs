// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using MvcTagHelpersWebSite;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class MvcTagHelpersTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("MvcTagHelpersWebSite");
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;
        private readonly Assembly _resourcesAssembly = typeof(TagHelpersTests).GetTypeInfo().Assembly;

        [Theory]
        [InlineData("Index", null)]
        [InlineData("InvalidAnchor", null)]
        [InlineData("Order", "/Order/Submit")]
        [InlineData("Product", null)]
        [InlineData("Customer", "/Customer/Customer")]
        public async Task MvcTagHelpers_GeneratesExpectedResults(string action, string antiForgeryAction)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");

            // The K runtime compiles every file under compiler/resources as a resource at runtime with the same name
            // as the file name, in order to update a baseline you just need to change the file in that folder.
            var expectedContent =
                    await _resourcesAssembly.ReadResourceAsStringAsync
                                     ("compiler/resources/MvcTagHelpersWebSite.Home." + action + ".html");

            // Act
            // The host is not important as everything runs in memory and tests are isolated from each other.
            var response = await client.GetAsync("http://localhost/Home/" + action);
            var resposneContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);

            if (antiForgeryAction != null)
            {
                var forgeryToken = AntiForgeryTestHelper.RetrieveAntiForgeryToken(resposneContent, antiForgeryAction);
                expectedContent = string.Format(expectedContent, forgeryToken);
            }
            Assert.Equal(expectedContent.Trim(), resposneContent.Trim());
        }
    }
}