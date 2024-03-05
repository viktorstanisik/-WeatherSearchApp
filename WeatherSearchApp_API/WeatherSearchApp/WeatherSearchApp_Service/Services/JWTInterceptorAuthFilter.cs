using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Shared.Enums;

namespace WeatherSearchApp_Service.Services
{
    public class JWTInterceptorAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IUserService _userService;

        public JWTInterceptorAuthFilter(IUserService userService)
        {
            _userService = userService;
        }

        // The OnAuthorizationAsync method is an asynchronous method that handles the authorization logic for a specific action or resource.
        // It is part of an Authorization Filter in ASP.NET Core, typically used to enforce access control rules before executing the associated action.

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // The method begins by retrieving the user ID of the currently logged-in user from the HttpContext using a UserService.

            // The method then fetches additional user information for the logged-in user asynchronously.
            var userId = _userService.GetLoggedUserIdFromHttpContext();
            var loggedOutUser = await _userService.GetUserByUserIdFromLoggedUserInfo(userId);

            // The following conditions check if the user is not authenticated (not logged in) or if the user is logged out based on certain criteria.
            // If the user is not authenticated or is flagged as logged out, the method sets the result of the AuthorizationFilterContext to an UnauthorizedResult,
            // indicating that the user does not have the necessary authorization to access the associated resource or perform the action.
            // The UnauthorizedResult will result in a 401 Unauthorized HTTP status response.
            if (!context.HttpContext.User.Identity.IsAuthenticated
                ||
                loggedOutUser != null && loggedOutUser.LastLogin.HasValue
                &&
                loggedOutUser.LoginStatusId == (int)LoginStatus.LoggedOut)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            // If the conditions are not met, the method allows the authorization process to continue, indicating that the user is authorized.
            return;
        }

    }
}
