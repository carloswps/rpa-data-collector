using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using rpa_data_collector.Controllers;
using rpa_data_collector.DTOs;

namespace WebApi.Tests.Controllers;

public class AuthControllerTests
{
    private AuthController BuildController(string configUser, string configPass)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:UserName"] = configUser,
                ["Auth:Password"] = configPass,
                ["Jwt:Secret"] = "chave-secreta-para-testes-unitarios-longa",
                ["Jwt:Issuer"] = "test",
                ["Jwt:Audience"] = "test",
                ["Jwt:ExpirationInHours"] = "1"
            })
            .Build();

        var tokenService = new rpa_data_collector.Application.Services.TokenService(config);
        return new AuthController(config, tokenService);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsOkWithToken()
    {
        var controller = BuildController("admin", "senha123");
        var request = new LoginRequestDto { Username = "admin", Password = "senha123" };

        var result = controller.Login(request);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Login_WrongPassword_ReturnsUnauthorized()
    {
        var controller = BuildController("admin", "senha123");
        var request = new LoginRequestDto { Username = "admin", Password = "errada" };

        var result = controller.Login(request);

        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void Login_WrongUsername_ReturnsUnauthorized()
    {
        var controller = BuildController("admin", "senha123");
        var request = new LoginRequestDto { Username = "outro", Password = "senha123" };

        var result = controller.Login(request);

        result.Should().BeOfType<UnauthorizedResult>();
    }
}
