using AdaptiveCards;
using ILuvLuis.Web.Entities;
using System.Collections.Generic;

namespace ILuvLuis.Web.Cards
{
    public class LegalTreatyCard : AdaptiveCard
    {
        public LegalTreatyCard(string treatyName, List<TextEntity> legal)
        {
            Version = new AdaptiveSchemaVersion(1, 0);
            Body.Add(new AdaptiveTextBlock(treatyName)
            {
                Color = AdaptiveTextColor.Accent,
                Height = new AdaptiveHeight(24),
                Separator = true
            });

            foreach (var procedure in legal)
            {
                if (procedure.Type == "link")
                {
                    Body.Add(new AdaptiveMedia()
                    {
                        AltText = "Link",
                        Sources = new List<AdaptiveMediaSource>() { new AdaptiveMediaSource("url", procedure.Value) }
                    });
                }
                Body.Add(new AdaptiveTextBlock(procedure.Value)
                {
                    Wrap = true,
                    Color = procedure.Type == "title" ? AdaptiveTextColor.Accent : AdaptiveTextColor.Default,
                    Separator = procedure.Type == "title"
                });
            }
        }
    }
}
