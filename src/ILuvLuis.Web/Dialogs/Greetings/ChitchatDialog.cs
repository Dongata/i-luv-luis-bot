using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.Greetings
{
    public class ChitchatDialog : ComponentDialog
    {
        #region Constants

        public const string ChitchatDialogId = "ChitChat";

        private const string DefaultDialog = "DefaultDialog";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        #endregion

        public ChitchatDialog(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(ChitchatDialogId)
        {
            _onTurnProperty = onTurnProperty;

            var steps = new List<WaterfallStep>()
            {
                ReturnResponse
            };

            AddDialog(new WaterfallDialog(DefaultDialog, steps));
        }

        private async Task<DialogTurnResult> ReturnResponse(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var turnProperty = await _onTurnProperty.GetAsync(stepContext.Context);

            if (turnProperty != null)
            {
                if (turnProperty.Intent.Contains("Insults"))
                {
                    await stepContext.Context.SendActivityAsync("Mi creador me prohíbe responder insultos :c");
                }
                else if (turnProperty.Intent.Contains("Greetings"))
                {
                    await stepContext.Context.SendActivityAsync("Hey. Hola, ¿en que puedo ayudarte?");
                }
                else if (turnProperty.Intent.Contains("HowAreYou"))
                {
                    await stepContext.Context.SendActivityAsync("Realmente soy un bot no tengo sentimientos, pero gracias por preguntar");
                }
                else if (turnProperty.Intent.Contains("WhoAreYou"))
                {
                    await stepContext.Context.SendActivityAsync("Me llamo lucho, soy tu asistente virtual!");
                }
                else if (turnProperty.Intent.Contains("WhatYoureDoing"))
                {
                    await stepContext.Context.SendActivityAsync("Lo que toda computadora hace, procesando datos c:");
                }
                else if (turnProperty.Intent.Contains("DoYouKnowSiri"))
                {
                    await stepContext.Context.SendActivityAsync("Siri, es una colega.");
                }
                else if (turnProperty.Intent.Contains("WhatDoYouNeed"))
                {
                    await stepContext.Context.SendActivityAsync("sí, ¿Como no?, preguntame");
                }
                else if (turnProperty.Intent.Contains("WhatsYourAge"))
                {
                    await stepContext.Context.SendActivityAsync($"me empezaron a desarrollar el 03/01/2019 así que tengo {GetMyAge()}");
                }
                else if (turnProperty.Intent.Contains("WhatsYourGender"))
                {
                    await stepContext.Context.SendActivityAsync($"Soy un bot, así que realmente no se en que genero entro");
                }
            }

            return await stepContext.EndDialogAsync();
        }

        private string GetMyAge()
        {
            var timeAlive = DateTime.Now - new DateTime(03, 01, 2019);

            if (timeAlive.Days > 365)
            {
                return $"{timeAlive.Days / 365} años";
            }
            else if (timeAlive.Days > 30)
            {
                return $"{timeAlive.Days / 30} meses";
            }
            else
            {
                return $"{timeAlive.Days} días";
            }
        }
    }
}