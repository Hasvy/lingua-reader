using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages
{
    public partial class Index : ComponentBase
    {
        string value = "Text";
        string dropDownValue = "Around the Horn";
        int intvalue;
        List<string> companyNames = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            companyNames.AddRange(new string[] { "sfjkewfwe", "dasfgehwf", "ohfweh" });
            base.OnInitializedAsync();
        }
    }
}
