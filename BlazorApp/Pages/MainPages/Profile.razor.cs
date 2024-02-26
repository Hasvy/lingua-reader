using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Objects.Entities;
using Radzen;
using Services;
using System.Runtime.CompilerServices;

namespace BlazorApp.Pages.MainPages
{
    public partial class Profile : ComponentBase, IDisposable
    {
        [Inject] UserService UserService { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        [Inject] HttpInterceptorService HttpInterceptorService { get; set; } = null!;
        private UserProfileSettingsDto user = new UserProfileSettingsDto();
        protected override async Task OnInitializedAsync()
        {
            HttpInterceptorService.RegisterEvent();
            user.NativeLanguage = await UserService.GetNativeLanguage();
            user.DesiredLanguage = await UserService.GetDesiredLanguage();
            await base.OnInitializedAsync();
        }

        private async Task ConfirmChanges()     //TODO check that languages are not same
        {
            var result = await UserService.ChangeUserSettings(user);
            if (result.IsSuccessfulChange is true)
                NotificationService.Notify(NotificationSeverity.Success, "Changes have been saved");
            else
                NotificationService.Notify(NotificationSeverity.Error, "An error occurred");
        }

        public void Dispose() => HttpInterceptorService.DisposeEvent();
    }
}
