using Microsoft.AspNetCore.Components;
using Services.Authentication;
using Toolbelt.Blazor;
using Radzen;

namespace Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly AuthStateProvider _authProvider;
        private readonly IAuthenticationService _authService;
        private readonly NavigationManager _navigationManager;
        private readonly NotificationService _notificationService;

        public HttpInterceptorService(HttpClientInterceptor interceptor, AuthStateProvider authProvider, IAuthenticationService authService, NavigationManager navigationManager, NotificationService notificationService)
        {
            _interceptor = interceptor;
            _authProvider = authProvider;
            _authService = authService;
            _navigationManager = navigationManager;
            _notificationService = notificationService;
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath;

            if (!absPath.Contains("token") && !absPath.Contains("accounts"))
            {
                await CheckToken();

                //var token = await _refreshTokenService.TryRefreshToken();
                //if (!string.IsNullOrEmpty(token))
                //{
                //    e.Request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                //}
            }
        }

        private async Task CheckToken()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))!.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 0)
            {
                await LogoutAndNotify();
            }
        }

        private async Task LogoutAndNotify()
        {
            await _authService.Logout();
            _navigationManager.NavigateTo("/Login");
            _notificationService.Notify(NotificationSeverity.Info, "Your token is expired", "Please log in again");
        }

        public void DisposeEvent()
        {
            _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
        }
    }
}
