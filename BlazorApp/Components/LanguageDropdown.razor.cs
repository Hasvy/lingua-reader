using Microsoft.AspNetCore.Components;
using Objects.Constants;
using Objects.Dto;
using Objects.Entities;

namespace BlazorApp.Components
{
    public partial class LanguageDropdown : ComponentBase
    {
        [Parameter] public string? UserMainLang
        { 
        get => _userMainLang;
        set
            {
                if (_userMainLang == value)
                    return;

                _userMainLang = value;
                UserMainLangChanged.InvokeAsync(value);
            }
        }
        [Parameter] public EventCallback<string> UserMainLangChanged { get; set; }
        private string _userMainLang;
        private async Task ValueChanged()
        {
            await UserMainLangChanged.InvokeAsync(UserMainLang);
        }
    }
}
