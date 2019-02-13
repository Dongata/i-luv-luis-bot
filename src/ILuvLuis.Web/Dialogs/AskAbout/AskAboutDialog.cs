using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.AskAbout
{
    public class AskAboutDialog : ComponentDialog
    {
        #region public methods

        public const string Intent = "Ask-About";
        public const string AskAboutDialogId = "AskAboutDialogId";

        private const string MainDialogWaterfall = "MainDialogWaterfall";

        #endregion

        #region Constuctor

        public AskAboutDialog() : base(AskAboutDialogId)
        {
            var steps = new WaterfallStep[]
            {
                Show
            };

            AddDialog(new WaterfallDialog(MainDialogWaterfall, steps));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Show(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Todavía no está implementado");
            return await stepContext.EndDialogAsync();
        }

        #endregion
    }
}
