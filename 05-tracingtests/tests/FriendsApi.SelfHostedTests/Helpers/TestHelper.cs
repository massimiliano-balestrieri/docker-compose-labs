using FluentAssertions;
using FriendsApi.Context;
using FriendsApi.Host;
using FriendsApi.Repositories;
using FriendsApi.Types.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FriendsApi.SelfHostedTests.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class TestHelper
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpResponseMessage response) where T : class
        {
            var content = await response.Content.ReadAsStreamAsync();
            Console.WriteLine(content);
            var dto = await JsonSerializer.DeserializeAsync<T>(content);
            return dto;
        }
        public static async Task<T> GetNotNullAsync<T>(this HttpClient httpClient, string uri) where T : class
        {
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var ret = await response.ReadAsJsonAsync<T>();
            ret.Should().NotBeNull();
            return ret;
        }
        public static async Task<HttpResponseMessage> PostAsync<TRequest>(this HttpClient httpClient, string uri, TRequest request)
        {
            var response = await httpClient.PostAsJsonAsync(uri, request);
            response.EnsureSuccessStatusCode();
            return response;
        }
        public static async Task<HttpResponseMessage> PutAsync<TRequest>(this HttpClient httpClient, string uri, TRequest request)
        {
            var response = await httpClient.PutAsJsonAsync(uri, request);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public static HttpClient CreateClientAsync(IHost host)
        {
            var client = host.GetTestClient();
            // https://stackoverflow.com/questions/59577491/asp-net-core-3-mock-authorization-during-integration-testing
            // https://github.com/DOMZE/fake-authentication-jwtbearer
            //client.SetFakeBearerToken((object)new ExpandoObject());
            return client;
        }

        public static async Task<IHost> CreateHost()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{environment.ToLowerInvariant()}.json")
                .Build();

            Console.WriteLine("Selfhosted env" + environment);
            //Log.Logger.Information("Selfhosted env" + environment);

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    //webHost.UseSerilog();
                    webHost.UseStartup<Startup>();
                    webHost.UseConfiguration(configuration);
                    webHost.UseTestServer();
                    webHost.ConfigureTestServices(services =>
                    {
                        services.AddLogging(x => x.AddConsole());
                        //services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                        if (environment.ToLowerInvariant() != "compose")
                        {
                            SetupInMemoryDatabase(services);
                        }

                    });
                });
            var host = await hostBuilder.StartAsync();
            return host;
        }

        private static void SetupInMemoryDatabase(IServiceCollection services)
        {

            var repository = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(FriendContext));
            services.Remove(repository);

            services.AddSingleton<FriendContext>(sp =>
            {
                var options = new DbContextOptionsBuilder<FriendContext>()
                    .UseInMemoryDatabase(databaseName: "test")
                    .Options;

                var testContext = new FriendContext(options);
                testContext.Database.EnsureDeleted();
                testContext.Friends.Add(new Friend()
                {
                    Name = "test"
                });

                testContext.SaveChanges();
                return testContext;
            });

        }
    }
}
