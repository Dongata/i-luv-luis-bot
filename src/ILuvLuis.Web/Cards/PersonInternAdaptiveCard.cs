using AdaptiveCards;
using ILuvLuis.Web.Dialogs;

namespace ILuvLuis.Web.Cards
{
    public class PersonInternAdaptiveCard : AdaptiveCard
    {
        public PersonInternAdaptiveCard(PersonInternState personInternState) : base()
        {
            Speak = $"El número interno de {personInternState.FirstName} {personInternState.LastName}, es {personInternState.Intern}";
            Version = new AdaptiveSchemaVersion(1, 0);
            Body.Add(new AdaptiveTextBlock($"{personInternState.FirstName} {personInternState.LastName}")
            {
                Weight = AdaptiveTextWeight.Bolder,
                IsSubtle = false
            });

            Body.Add(new AdaptiveTextBlock($"{personInternState.Department}")
            {
                Separator = true
            });

            Body.Add(new AdaptiveTextBlock($"{personInternState.Intern}")
            {
                Color = AdaptiveTextColor.Accent
            });
        }
    }
}
