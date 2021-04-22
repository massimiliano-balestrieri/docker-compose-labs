using FluentAssertions;
using FriendsApi.Host.Controllers;
using FriendsApi.Repositories;
using FriendsApi.Types.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FriendsApi.UnitTests.Controllers
{
    [TestFixture]
    public class FriendControllerTest
    {

        [Test]
        public async Task PutNotExistsFriendShouldReturnNotFound()
        {
            // arrange
            var controller = new FriendsController(Mock.Of<IFriendsRepository>(), Mock.Of<ILogger<FriendsController>>());

            // act
            var actionResult = await controller.PutAsync(0, new Friend());

            // assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }
    }
}