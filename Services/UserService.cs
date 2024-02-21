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

        public async Task<string> GetUserMainLanguage()
        {
            string userMainLang = string.Empty;
            var response = await _httpClient.GetAsync("api/user/GetUserMainLanguage");
            if (response != null)
                userMainLang = await response.Content.ReadAsStringAsync();
            return userMainLang;
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
    }
}
