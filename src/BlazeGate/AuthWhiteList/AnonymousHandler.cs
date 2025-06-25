using BlazeGate.JwtBearer;
using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlazeGate.AuthWhiteList
{
    public class AnonymousHandler : AuthorizationHandler<AnonymousRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AnonymousRequirement requirement)
        {
            context.Succeed(requirement);
        }
    }
}