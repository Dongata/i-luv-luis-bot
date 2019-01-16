using Microsoft.Bot.Schema;

namespace AdaptiveCards
{
    public static class AdaptiveCardsExtension
    {
        public static Attachment ToAttachment(this AdaptiveCard adaptiveCard)
        {
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = adaptiveCard,
            };

            return adaptiveCardAttachment;
        }
    }
}
