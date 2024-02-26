using Objects.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetNativeLanguage()
        {
            return await GetLanguage("Native");
        }

        public async Task<string> GetDesiredLanguage()
        {
            return await GetLanguage("Desired");
        }

        public async Task<UserProfileResponseDto> ChangeUserSettings(UserProfileSettingsDto userProfileSettingsDto)
        {
            var content = JsonSerializer.Serialize(userProfileSettingsDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/user/ChangeUserSettings", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                return new UserProfileResponseDto { IsSuccessfulChange = true };
            }
            else
            {
                return new UserProfileResponseDto { IsSuccessfulChange = false };
            }
        }

        private async Task<string> GetLanguage(string languageType)     //TODO exceptions if languages are not set
        {
            string language = string.Empty;
            var response = await _httpClient.GetAsync($"api/user/Get{languageType}Language");
            if (response != null)
                language = await response.Content.ReadAsStringAsync();
            return language;
        }
    }
}
