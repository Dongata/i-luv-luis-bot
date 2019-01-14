using ILuvLuis.Web.Dialogs.Base;

namespace ILuvLuis.Web.Dialogs.DefaultFallbacks
{
    public class DefaultFallback : RandomTextResponse
    {
        public DefaultFallback() : base(new string[]
        {
            "Disculpame, no te entendí",
            "Todavía no puedo procesar eso",
            "Mi cerebro no está tan completo, como para entender lo que me dijiste"
        })
        {
        }
    }
}
