using MarthasLibrary.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MarthasLibrary.Tests.Common;

public static class UserManagerHelper
{
    public static Mock<UserManager<Customer>> MockUserManager()
    {
        var userStoreMock = new Mock<IUserStore<Customer>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<Customer>>();
        var userValidators = new List<IUserValidator<Customer>>();
        var passwordValidators = new List<IPasswordValidator<Customer>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var logger = new Mock<ILogger<UserManager<Customer>>>();

        return new Mock<UserManager<Customer>>(
          userStoreMock.Object,
          options.Object,
          passwordHasher.Object,
          userValidators,
          passwordValidators,
          keyNormalizer.Object,
          errors.Object,
          services.Object,
          logger.Object);
    }

}