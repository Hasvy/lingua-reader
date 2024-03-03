using iText.Forms.Form.Element;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Objects.Entities.Translator;
using Objects.Entities.Words;
using Radzen;
using Radzen.Blazor;

namespace BlazorApp.Components.Learning
{
    public partial class CardWithAnswers : ComponentBase
    {
        [Parameter] public List<WordToLearn> WordsToLearn { get; set; } = null!;
        [Parameter] public int ActualCardNumber { get; set; }
        [Parameter] public EventCallback EndPractice { get; set; }
        private RadzenButton[] buttons = new RadzenButton[4];
        private bool _isDelaying;

        private async Task ChooseVariant(RadzenButton clickedButton, VariantToAnswer chosenVariant)
        {
            _isDelaying = true;
            var rightVariant = WordsToLearn[ActualCardNumber].WordWithTranslations.Translations.First();
            RadzenButton rightButton = null;
            if (chosenVariant.isRight is true)
            {
                clickedButton.Variant = Variant.Filled;        //Doesn't work
                clickedButton.ButtonStyle = ButtonStyle.Success;
            }
            else
            {
                foreach (var button in buttons)
                {
                    if (button.Text == rightVariant.DisplayTarget)
                    {
                        rightButton = button;
                        break;
                    }
                }
                rightButton.ButtonStyle = ButtonStyle.Success;
                clickedButton.ButtonStyle = ButtonStyle.Danger;
            }
            await Task.Delay(2000);
            if (rightButton is not null)
            {
                rightButton.ButtonStyle = ButtonStyle.Primary;
            }
            clickedButton.Variant = Variant.Outlined;
            clickedButton.ButtonStyle = ButtonStyle.Primary;
            WordsToLearn[ActualCardNumber].Answer = chosenVariant;
            ActualCardNumber++;
            _isDelaying = false;
            if (ActualCardNumber >= WordsToLearn.Count())
                await EndPractice.InvokeAsync();
        }
    }
}
