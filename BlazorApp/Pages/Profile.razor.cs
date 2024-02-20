using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Objects.Entities;
using Radzen;
using Services;
using System.Runtime.CompilerServices;

namespace BlazorApp.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject] UserService UserService { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        private UserProfileSettingsDto user = new UserProfileSettingsDto();
        protected override async Task OnInitializedAsync()
        {
            user.UserMainLang = await UserService.GetUserMainLanguage();
            await base.OnInitializedAsync();
        }

        private async Task ConfirmChanges()
        {
            var result = await UserService.ChangeUserSettings(user);
            if (result.IsSuccessfulChange is true)
                NotificationService.Notify(NotificationSeverity.Success);
            else
                NotificationService.Notify(NotificationSeverity.Error);
        }

        private async Task OnChangeHandler(string newMainLang)
        {
            user.UserMainLang = newMainLang;
        }
    }
}
