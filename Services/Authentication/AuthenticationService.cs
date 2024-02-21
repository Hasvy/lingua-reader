using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Objects.Dto;
using Objects.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var content = JsonSerializer.Serialize(userForRegistration);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var registrationResult = await _httpClient.PostAsync("api/accounts/Registration", bodyContent);

            if (!registrationResult.IsSuccessStatusCode)
            {
                return await DeserializeError<RegistrationResponseDto>(registrationResult);
            }

            return new RegistrationResponseDto { IsSuccessfulRegistration = true };
        }

        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication)
        {
            var content = JsonSerializer.Serialize(userForAuthentication);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var authResult = await _httpClient.PostAsync("api/accounts/Login", bodyContent);
            var authContent = await authResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponseDto>(authContent, _options);
            if (!authResult.IsSuccessStatusCode)
                return result;
            await _localStorage.SetItemAsync("authToken", result.Token);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(userForAuthentication.Email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
            return new AuthResponseDto { IsAuthSuccessful = true };
        }
        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        //public async Task<string> RefreshToken()
        //{
        //    var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
        //    var refreshResult = await _httpClient.PostAsync("token/refresh", bodyContent);
        //    var refreshContent = await refreshResult.Content.ReadAsStringAsync();
        //    var result = JsonSerializer.Deserialize<AuthResponseDto>(refreshContent, _options);

        //    return result.Token;
        //}

        public async Task SendEmail(ForgotPasswordDto forgotPasswordDto)
        {
            var content = JsonSerializer.Serialize(forgotPasswordDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("api/accounts/ForgotPassword", bodyContent);
            if (!result.IsSuccessStatusCode)
            {
                _navigationManager.NavigateTo("/ForgotPasswordConfirmation/Failed");
            }
            else
            {
                _navigationManager.NavigateTo("/ForgotPasswordConfirmation/Success");
            }
        }

        public async Task<ResetPasswordResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var content = JsonSerializer.Serialize(resetPasswordDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("api/accounts/ResetPassword", bodyContent);

            if (!result.IsSuccessStatusCode)
            {
                return await DeserializeError<ResetPasswordResponseDto>(result);
                //_navigationManager.NavigateTo("/ResetPasswordConfirmation/Failed");
            }
            else
            {
                return new ResetPasswordResponseDto { IsSuccessfulRegistration = true };
                //_navigationManager.NavigateTo("/ResetPasswordConfirmation/Success");
            }
        }

        public async Task<ConfirmEmailResponseDto> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            var content = JsonSerializer.Serialize(confirmEmailDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var confirmResult = await _httpClient.PostAsync("api/accounts/ConfirmEmail", bodyContent);

            if (!confirmResult.IsSuccessStatusCode)
            {
                return await DeserializeError<ConfirmEmailResponseDto>(confirmResult);
            }

            return new ConfirmEmailResponseDto { IsSuccessfulConfirmation = true };
        }

        public async Task<ConfirmEmailResponseDto> ResendConfirmationEmail(string email)
        {
            var resendResult = await _httpClient.PostAsJsonAsync("api/accounts/ResendAddressConfirmationEmail", email);

            if (!resendResult.IsSuccessStatusCode)
            {
                return await DeserializeError<ConfirmEmailResponseDto>(resendResult);
            }

            return new ConfirmEmailResponseDto { IsSuccessfulConfirmation = true };
        }

        //Helpers
        private async Task<T> DeserializeError<T>(HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _options);
            return result!;
        }
    }
}
