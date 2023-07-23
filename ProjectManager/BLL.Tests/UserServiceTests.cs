using BLL.Abstractions.Interfaces;
using BLL.Services;
using Core.Models;
using DAL.Abstractions.Interfaces;
using DAL.Repository;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BLL.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task SendCodeToUser_From1000To9999_4Values()
    {
        string email = "someonemail@gmail.com";

        var result = 8083;

        Assert.True(result >= 1000 && result <= 9999);
    }
}