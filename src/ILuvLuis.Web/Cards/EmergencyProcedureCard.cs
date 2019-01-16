using AdaptiveCards;
using System.Collections.Generic;

namespace ILuvLuis.Web.Cards
{
    public class EmergencyProcedureCard : AdaptiveCard
    {
        public EmergencyProcedureCard(string procedureName, List<string> procedures)
        {
            Version = new AdaptiveSchemaVersion(1, 0);
            Body.Add(new AdaptiveTextBlock(procedureName)
            {
                Color = AdaptiveTextColor.Accent,
                Height = new AdaptiveHeight(24),
                Separator = true
            });

            foreach (var procedure in procedures)
            {
                Body.Add(new AdaptiveTextBlock(procedure)
                {
                    Wrap = true,
                    Color = AdaptiveTextColor.Default,
                    Separator = true
                });
            }
        }
    }
}
