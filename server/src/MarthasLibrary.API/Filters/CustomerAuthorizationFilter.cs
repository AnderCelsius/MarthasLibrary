using MarthasLibrary.Core.Entities;
using MarthasLibrary.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MarthasLibrary.API.Filters;

public class CustomerAuthorizationFilter(IGenericRepository<Customer> customerRepository,
        IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    : IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        var user = httpContext.User;
        var identityUserId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(identityUserId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var existingCustomer = await customerRepository.Table
            .SingleOrDefaultAsync(c => c.IdentityUserId == identityUserId);

        if (existingCustomer == null)
        {
            // Create a new customer record
            var firstName =
                user.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            var lastName =
                user.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            var email = user.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email)) return;
            var newCustomer = Customer.CreateInstance(firstName, lastName, email, identityUserId);
            await customerRepository.InsertAsync(newCustomer);
            await customerRepository.SaveAsync(cancellationToken);
        }
        else
        {
            // Update existing customer record if needed
            var firstName = user.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            var lastName = user.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) return;
            if (existingCustomer.FirstName == firstName && existingCustomer.LastName == lastName) return;
            existingCustomer.UpdateDetails(new Customer.UserUpdate(firstName, lastName));
            await customerRepository.SaveAsync(cancellationToken);
        }
    }
}