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

            TryAddField(
                "Cumpleaños: {0}",
                person.BirthDate.ToString("dd-MM-yyyy"),
                fieldToAccent.NormalizeString() == "birthdate");

            TryAddField("Area: {0}", person.Area, fieldToAccent.NormalizeString() == "area");
            TryAddField("Interno: {0}", person.Intern, fieldToAccent.NormalizeString() == "intern");
            TryAddField("Móvil: {0}", person.Mobile, fieldToAccent.NormalizeString() == "mobile");
            TryAddField("Telefono: {0}", person.Phone, fieldToAccent.NormalizeString() == "phone");
            TryAddField("Email: {0}", person.Email, fieldToAccent.NormalizeString() == "email");
            TryAddField("Direccion {0}", person.Address, fieldToAccent.NormalizeString() == "address");
        }

        private void TryAddField(string pattern, string value, bool isAccent = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Body.Add(new AdaptiveTextBlock(string.Format(pattern, value))
                {
                    Separator = true,
                    Color = isAccent ? AdaptiveTextColor.Accent : AdaptiveTextColor.Default
                });
            }
        }
    }
}
