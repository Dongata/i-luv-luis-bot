using AdaptiveCards;
using ILuvLuis.Web.Cards;
using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.AskEmergencyProc
{
    public class EmergencyProcedureDialog : ComponentDialog
    {
        #region Constants

        public const string EmergencyProcedureId = "AskEmergencyProcedure";
        public const string Intent = "Ask-Emergency-Procedure";

        private const string EmergencyDialog = "EmergencyDialog";
        private const string EmegencyPrompt = "EmegencyPrompt";
        private const string EmergencyTypeEntityName = "emergency_type";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        private readonly IEnumerable<string> emergencyTypes = new List<string>()
        {
            "incendio",
            "terremoto",
            "maremoto"
        };

        private readonly Dictionary<string, List<string>> emergencies = new Dictionary<string, List<string>>()
        {
            {
                "incendio",
                new List<string>()
                {
                    "Mantengase en calma",
                    "Si tiene un matafuego cerca, y sabe como usarlo, uselo sobre la base de la llama",
                    "Si nó encuentre una vía segura y trate de salir de la zona afectada",
                    "En una zona segura, llame a emergencias"
                }
            },
            {
                "terremoto",
                new List<string>()
                {
                    "Escondase bajo una mesa o escritorio",
                    "No salga de ahí hasta que pase el terremoto",
                    "O hasta que empieze a sonar la alarma de peligro de derrume",
                    "Si esta misma suena, dirijase inmediatamente hasta las escaleras de emergencia",
                    "Baje hasta planta baja siempre del lado más derecho de las escaleras",
                    "Una vez en la planta baja salga inmediatamente del edificio y alejesé una cuadra"
                }
            },
            {
                "maremoto",
                new List<string>()
                {
                    "Si se encuentra en el 3er piso quedesé ahí y encuentre una mesa para esconderse",
                    "Si nó proceda al tercer piso por las escaleras de emergencia y escondase bajo una mesa o escritorio",
                    "Espere hasta que las autoridades declaren zona segura"
                }
            }
        };

        #endregion

        #region Constructor

        public EmergencyProcedureDialog(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(EmergencyProcedureId)
        {
            _onTurnProperty = onTurnProperty;

            var steps = new WaterfallStep[]
            {
                Initialize,
                ShowEmergencyProcedure
            };

            AddDialog(new WaterfallDialog(EmergencyDialog, steps));
            AddDialog(new ChoicePrompt(EmegencyPrompt));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext context, CancellationToken cancellationToken)
        {
            var turnProperty = await _onTurnProperty.GetAsync(context.Context);

            var emergencyType = turnProperty?.Entities?.FirstOrDefault(a => a.EntityName == EmergencyTypeEntityName);

            if (emergencyType != null)
            {
                var emergencyTypeString = ((JArray)emergencyType.Value).First().ToString().ToLowerInvariant().Trim();
                if(emergencyTypeString.Trim().ToLowerInvariant() != "emergencia")
                {
                    return await context.NextAsync(emergencyTypeString);
                }
            }

            return await context.PromptAsync(
                EmegencyPrompt,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("¿Que tipo de emergencia?"),
                    Choices = GetChoices()
                });
        }

        private List<Choice> GetChoices()
        {
            var choices = new List<Choice>();
            foreach (var emergencyType in emergencyTypes)
            {
                var capitalizedType = emergencyType.Substring(0, 1).ToUpper() + emergencyType.Substring(1);
                choices.Add(new Choice(capitalizedType));
            }

            return choices;
        }

        private async Task<DialogTurnResult> ShowEmergencyProcedure(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = stepContext.Result;
            string proc = string.Empty;

            if (result is string stringResult)
            {
                proc = stringResult;
            }
            else if(result is FoundChoice choiceResult)
            {
                proc = choiceResult.Value;
            }

            if (emergencies.TryGetValue(proc.ToLowerInvariant().Trim(), out var procedures))
            {
                var reply = stepContext.Context.Activity.CreateReply();
                reply.Attachments = new List<Attachment>
                {
                    new EmergencyProcedureCard(proc, procedures).ToAttachment()
                };

                await stepContext.Context.SendActivityAsync(reply);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("perdón pero no conozco ese tipo de emergencia");
            }

            return await stepContext.EndDialogAsync();
        }

        #endregion
    }
}
