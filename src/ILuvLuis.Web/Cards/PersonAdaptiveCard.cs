using AdaptiveCards;
using ILuvLuis.Models;
using ILuvLuis.Web.Extensions;

namespace ILuvLuis.Web.Cards
{
    public class PersonAdaptiveCard : AdaptiveCard
    {
        public PersonAdaptiveCard(Person person, string fieldToAccent = "") : base()
        {
            Version = new AdaptiveSchemaVersion(1, 0);
            Body.Add(new AdaptiveTextBlock($"{person.FullName}")
            {
                Weight = AdaptiveTextWeight.Bolder,
                IsSubtle = false
            });

            if (!string.IsNullOrEmpty(person.Image))
            {
                Body.Add(new AdaptiveImage(person.Image)
                {
                    PixelWidth = 32,
                    PixelHeight = 32
                });
            }

            TryAddField($"Cumpleañpos: {person.BirthDate.ToString("dd-MM-yyyy")}",
                fieldToAccent.NormalizeString() == "birthdate");

            TryAddField($"Area: {person.Area}", fieldToAccent.NormalizeString() == "area");
            TryAddField($"Interno: {person.Intern}", fieldToAccent.NormalizeString() == "intern");
            TryAddField($"Móvil: { person.Mobile}", fieldToAccent.NormalizeString() == "mobile");
            TryAddField($"Telefono: {person.Phone}", fieldToAccent.NormalizeString() == "phone");
            TryAddField($"Email: {person.Email}", fieldToAccent.NormalizeString() == "email");
            TryAddField($"Direccion {person.Address}", fieldToAccent.NormalizeString() == "address");
        }

        private void TryAddField(string text, bool isAccent = false)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Body.Add(new AdaptiveTextBlock(text)
                {
                    Separator = true,
                    Color = isAccent ? AdaptiveTextColor.Accent : AdaptiveTextColor.Default
                });
            }
        }
    }
}
