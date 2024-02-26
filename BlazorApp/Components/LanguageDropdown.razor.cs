using Microsoft.AspNetCore.Components;
using Objects.Constants;
using Objects.Dto;
using Objects.Entities;

namespace BlazorApp.Components
{
    public partial class LanguageDropdown : ComponentBase
    {
        [Parameter] public string? Language
        { 
        get => _language;
        set
            {
                if (_language == value)
                    return;

                _language = value;
                LanguageChanged.InvokeAsync(value);
            }
        }
        [Parameter] public string[] Data { get; set; } = null!;
        [Parameter] public EventCallback<string> LanguageChanged { get; set; }
        private string _language = string.Empty;
        private async Task ValueChanged()
        {
            await LanguageChanged.InvokeAsync(Language);
        }
    }
}
