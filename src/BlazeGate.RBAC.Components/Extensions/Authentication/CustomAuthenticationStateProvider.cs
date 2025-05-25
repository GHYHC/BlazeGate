using BlazeGate.Model.JwtBearer;
using BlazeGate.RBAC.Components.Extensions.Menu;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccountService accountService;
        private readonly IConfiguration configuration;
        private readonly IPageService pageService;
        private readonly IPageDataStorageServices pageDataStorageServices;
        private readonly string ServiceName;

        public CustomAuthenticationStateProvider(IAccountService accountService, IConfiguration configuration, IPageService pageService, IPageDataStorageServices pageDataStorageServices)
        {
            this.accountService = accountService;
            this.configuration = configuration;
            this.pageService = pageService;
            this.pageDataStorageServices = pageDataStorageServices;
            ServiceName = configuration["BlazeGate:ServiceName"];
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await accountService.GetUser(ServiceName);
                if (result.Success)
                {
                    List<Claim> claims = result.Data.CreateClaimByUser().ToList();

                    //加载页面数据
                    var pageResult = await pageService.GetUserPageByServiceName(ServiceName, result.Data.Id);
                    if (pageResult.Success)
                    {
                        await pageDataStorageServices.SetPageData(pageResult.Data);
                    }

                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom Authentication"));
                    return await Task.FromResult(new AuthenticationState(claimsPrincipal));
                }
                else
                {
                    return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
            }
        }

        public void NotifyAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            base.NotifyAuthenticationStateChanged(task);
        }
    }
}