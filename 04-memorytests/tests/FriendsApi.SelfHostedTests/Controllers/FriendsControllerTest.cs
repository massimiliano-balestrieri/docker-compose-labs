using FluentAssertions;
using FriendsApi.Host.Constants;
using FriendsApi.SelfHostedTests.Helpers;
using FriendsApi.Types.Models;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FriendsApi.SelfHostedTests.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class FriendsControllerTest
    {
        private static IHost Host;
        private static HttpClient Client;

        [SetUp]
        public static async Task SetUp()
        {
            Host = await TestHelper.CreateHost();
            Client = TestHelper.CreateClientAsync(Host);
        }

        [TearDown]
        public static void TearDown()
        {
            Client.Dispose();
            Host.Dispose();
        }


        [Test]
        public async Task FriendCrudTest()
        {
            //1) list friends
            var responseList = await Client.GetNotNullAsync<List<Friend>>(Routes.Friends);
            responseList.Should().NotBeNull();
            responseList.Should().HaveCountGreaterThan(0);

            //2) create a friend
            var response = await Client.PostAsync(Routes.Friends, new Friend { Name = "Test" + new Random(100).Next() });
            var content = await response.Content.ReadAsStreamAsync();
            var dto = await JsonSerializer.DeserializeAsync<Friend>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            //3) get by id 
            var route = GetRoute(dto.Id);
            var getById = await Client.GetAsync(route);
            getById.EnsureSuccessStatusCode();

            //4) update friend
            await Client.PutAsync(route, new Friend
            {
                Id = dto.Id,
                Name = "Edited" + new Random(100).Next()
            });

            //5) delete
            await Client.DeleteAsync(route);

            //6) not found
            var last = await Client.GetAsync(route);
            last.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        private static string GetRoute(int id)
        {
            var route = Routes.Friend.Replace("{id}", id.ToString());
            return route;
        }
    }
}
