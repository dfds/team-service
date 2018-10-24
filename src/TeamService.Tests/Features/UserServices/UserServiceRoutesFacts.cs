using System;
using System.Net;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using Moq;
using Xunit;

namespace DFDS.TeamService.Tests.Features.UserServices
{
    public class UserServiceRoutesFacts
    {
        [Fact]
        public async Task GIVEN_none_existing_userId_EXPECT_NotFound()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var userRepositoryBuilder = new Mock<IUserRepository>();
                userRepositoryBuilder
                    .Setup(u => u.GetById(It.IsAny<string>()))
                    .ReturnsAsync((User)null);
                
                
                var client = builder
                    .WithService(userRepositoryBuilder.Object)
                    .Build();


                // Act
                var userIdThatDoesNotExist = "userIdThatDoesNotExist";
                var response = await client.GetAsync($"api/users/{userIdThatDoesNotExist}/services");
                Console.WriteLine("code: " + response.StatusCode);
            
                
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    response.StatusCode
                );
            }
        }
    }
}