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

namespace ILuvLuis.Web.Dialogs.LegalTreatyTransport
{
    public class LegalTreatyBetweenCountriesDialog : ComponentDialog
    {
        #region Constants

        public const string DialogId = "LegalTreatyBetweenCountriesDialog";
        public const string Intent = "Legal-Treaty-Between-Countries";

        private const string MainDialogId = "MainDialog";
        private const string TreatyPrompt = "TreatyPrompt";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        private readonly IDictionary<string, List<TextEntity>> _treaties = new Dictionary<string, List<TextEntity>>()
        {
            {
                "transporte terrestre",
                new List<TextEntity>()
                {
                    new TextEntity("title", "Acuerdo de Transporte Internacional Terrestre"),
                    new TextEntity("content", "inscripto como Acuerdo de Alcance Parcial en el marco de la Asociación Latinoamericana de Integración (ALADI)"),
                    new TextEntity("content", "Entrada en vigor: ARGENTINA: 16/11/1990, BRASIL: 21/11/1990"),
                    new TextEntity("content", "Norma de internalización:"),
                    new TextEntity("content", "ARGENTINA: Resolución 263-90 Subsecretaría de Transporte"),
                    new TextEntity("content", "BRASIL: Decreto N° 99.704 de 20/11/1990"),
                    new TextEntity("link", "http://servicios.infoleg.gob.ar/infolegInternet/anexos/20000-24999/20787/norma.htm"),

                    new TextEntity("title", "Protocolo Adicional sobre Infracciones y Sanciones"),
                    new TextEntity("content", "deja sin efecto el Primer Protocolo Adicional al Acuerdo sobre Transporte Internacional Terrestre."),
                    new TextEntity("content", "Entrada en vigor: 16/02/2005"),
                    new TextEntity("content", "Norma de internalización:"),
                    new TextEntity("content", "ARGENTINA: Nota C.R. N° 61/01"),
                    new TextEntity("content", "BRASIL: Nota N° 047 de 13/06/2005 - Decreto N° 5.462 de 09/06/05"),
                    new TextEntity("link", "http://www.aladi.org/biblioteca/publicaciones/aladi/acuerdos/Art_14/es/03/A14TM_003_002.pdf"),

                    new TextEntity("title", "Ley 22.111"),
                    new TextEntity("content", "Convenio de Transporte Internacional Terrestre "),
                    new TextEntity("link", "http://servicios.infoleg.gob.ar/infolegInternet/anexos/35000-39999/39693/norma.htm")
                }
            },
            {
                "transporte maritimo",
                new List<TextEntity>()
                {
                    new TextEntity("title", "LEY N° 23.557"),
                    new TextEntity("content", "Convenio de transporte marítimo argentina-brasil"),
                    new TextEntity("link", "http://servicios.infoleg.gob.ar/infolegInternet/anexos/20000-24999/20982/norma.htm")
                }
            },
            {
                "transporte aereo",
                new List<TextEntity>()
                {
                    new TextEntity("title", "Ley 25.806"),
                    new TextEntity("content", "Acuerdo sobre servicios aereos subregionales entre los gobiernos de la republica argentina, de la republica de bolivia, de la republica federativa del brasil, de la republica de chile, de la republica del paraguay y de la republica oriental del uruguay"),
                    new TextEntity("link", "http://servicios.infoleg.gob.ar/infolegInternet/anexos/90000-94999/90688/norma.htm")
                }
            },
            {
                "comercio exterior",
                new List<TextEntity>()
                {
                    new TextEntity("content", "todavía no implementado")
                }
            }
        };

        #endregion

        #region Constructor

        public LegalTreatyBetweenCountriesDialog(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(DialogId)
        {
            _onTurnProperty = onTurnProperty;
            var steps = new WaterfallStep[]
            {
                Initialize,
                ShowResult
            };

            AddDialog(new WaterfallDialog(MainDialogId, steps));
            AddDialog(new ChoicePrompt(TreatyPrompt));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var onTurnProp = await _onTurnProperty.GetAsync(stepContext.Context);

            if (onTurnProp.Entities != null)
            {
                var entity = onTurnProp.Entities.Find(a => a.EntityName == EntityProperty.LegalTreatyType);

                if (entity != null)
                {
                    var entityString = ((JArray)entity.Value).First().ToString().ToLowerInvariant().Trim();
                    if (!string.IsNullOrEmpty(entityString))
                    {
                        return await stepContext.NextAsync(entityString);
                    }
                }
            }

            return await stepContext.PromptAsync(TreatyPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text("¿Que tratado?"),
                Choices = GetChoices()
            });
        }

        private async Task<DialogTurnResult> ShowResult(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = stepContext.Result;
            var treaty = string.Empty;

            if (result is string stringResult)
            {
                treaty = stringResult;
            }
            else if (result is FoundChoice choiceResult)
            {
                treaty = choiceResult.Value;
            }

            if (_treaties.TryGetValue(treaty.ToLowerInvariant().Trim(), out var legal))
            {
                var reply = stepContext.Context.Activity.CreateReply();
                reply.Attachments = new List<Attachment>
                {
                    new LegalTreatyCard(treaty, legal).ToAttachment()
                };

                await stepContext.Context.SendActivityAsync(reply);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("perdón pero no conozco ese tipo de tratado");
            }

            return await stepContext.EndDialogAsync();
        }

        #endregion

        #region Prompts

        private IList<Choice> GetChoices()
        {
            var keys = _treaties.Keys;
            var choices = new List<Choice>();

            foreach (var key in keys)
            {
                choices.Add(new Choice(key));
            }

            return choices;
        }

        #endregion
    }
}
