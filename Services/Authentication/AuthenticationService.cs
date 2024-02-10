using Blazored.LocalStorage;
using iText.Layout.Tagging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Objects.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            if (!registrationResult.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RegistrationResponseDto>(registrationContent, _options);
                return result;
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

        public async Task SendEmail(ForgotPasswordDto forgotPasswordDto)
        {
            var content = JsonSerializer.Serialize(forgotPasswordDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("api/accounts/ForgotPassword", bodyContent);
            if (!result.IsSuccessStatusCode)
            {
                //TODO error message
            }
            else
            {
                _navigationManager.NavigateTo("/ForgotPasswordConfirmation");
            }
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var content = JsonSerializer.Serialize(resetPasswordDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("api/accounts/ResetPassword", bodyContent);
            if (!result.IsSuccessStatusCode)
            {
                //TODO error message
            }
            else
            {
                _navigationManager.NavigateTo("/ResetPasswordConfirmation");
            }
        }
    }
}
