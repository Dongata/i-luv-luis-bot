using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.AskHolidayDay
{
    public class AskHolidayDayDialog : ComponentDialog
    {
        #region Consts

        public const string AskHolidayDayId = "AskHolidayDay";
        public const string Intent = "Ask-Holiday-Day";

        private const string AskHolidayDay = "AskHolidayDayDialog";
        private const string PromtDateTime = "PromtDateTime";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        private readonly Dictionary<DateTime, string> _holidays = new Dictionary<DateTime, string>()
        {
            { new DateTime(2019, 03, 04), "Carnaval" },
            { new DateTime(2019, 03, 05), "Carnaval" },
            { new DateTime(2019, 03, 25), "Día Nacional de la Memoria por la Verdad y la Justicia" },
            { new DateTime(2019, 04, 02), "Día del Veterano y de los Caídos en la Guerra de Malvina" },
            { new DateTime(2019, 04, 19), "Viernes Santo" },
            { new DateTime(2019, 05, 01), "Día del Trabajador" },
            { new DateTime(2019, 05, 25), "Día de la Revolución de Mayo" },
            { new DateTime(2019, 06, 17), "Día Paso a la Inmortalidad del General Martín Miguel de Güemes" },
            { new DateTime(2019, 06, 20), "Día Paso a la Inmortalidad del General Manuel Belgrano" },
            { new DateTime(2019, 07, 09), "Día de la Independencia" },
            { new DateTime(2019, 08, 17), "Paso a la Inmortalidad del General José de San Martín" },
            { new DateTime(2019, 10, 12), "Día del Respeto a la Diversidad Cultural" },
            { new DateTime(2019, 11, 18), "Día de la Soberanía Nacional" },
            { new DateTime(2019, 12, 08), "Inmaculada Concepción de María" },
            { new DateTime(2019, 12, 24), "Navidad" }
        };

        #endregion

        #region Constructors

        public AskHolidayDayDialog(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(AskHolidayDayId)
        {
            var steps = new WaterfallStep[]
            {
                Initialize,
                CheckAndShow
            };

            _onTurnProperty = onTurnProperty;

            AddDialog(new WaterfallDialog(AskHolidayDay, steps));
            AddDialog(new DateTimePrompt(PromtDateTime, DatetimeValidator));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var onTurnProperty = await _onTurnProperty.GetAsync(stepContext.Context);

            if(onTurnProperty != null)
            {
                var date = onTurnProperty.Entities.FirstOrDefault(a => a.EntityName == EntityProperty.DateTime);
                if(date != null)
                {
                    var serializeddate = (date.Value as JArray)?.FirstOrDefault()?.ToString();

                    if (!string.IsNullOrEmpty(serializeddate))
                    {
                        var dtRes = JsonConvert.DeserializeObject<DateTimeResolution>(
                            serializeddate.ToString().Replace("[", "").Replace("]", ""));

                        if (dtRes.Value == null && dtRes.Timex != null)
                        {
                            dtRes.Value = dtRes.Timex;
                        }

                        if (dtRes != null)
                        {
                            return await stepContext.NextAsync(dtRes);
                        }
                    }
                }
            }

            return await stepContext.PromptAsync(
                PromtDateTime,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("No entendí la fecha, ¿me la repetís?")
                });
        }

        private async Task<DialogTurnResult> CheckAndShow(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var res = stepContext.Result as DateTimeResolution;

            var response = string.Empty;

            if (res != null && !string.IsNullOrEmpty(res.Value))
            {
                if (res.Value.Contains("X"))
                {
                    res.Value = DecideVariableValues(res.Value);
                }

                if (DateTime.TryParse(res.Value, out var datetime))
                {
                    if (_holidays.ContainsKey(datetime.Date))
                    {
                        response = "si, es " + _holidays[datetime.Date];
                    }
                    else
                    {
                        response = "Lamentablemente, no :c";
                    }
                }
                else
                {
                    response = "sigo sin entender la fecha";
                }
            }
            else
            {
                response = "sigo sin entender la fecha";
            }


            await stepContext.Context.SendActivityAsync(response);

            return await stepContext.EndDialogAsync();
        }

        private string DecideVariableValues(string value)
        {
            var results = value.Split('-');

            if(results[2].Length > 2)
            {
                results[2] = results[2].Substring(1);
            }

            if (results[2].Contains("X"))
            {
                return string.Empty;
            }

            if (results[1].Contains("X"))
            {
                results[1] = DateTime.Now.Month.ToString();
            }

            if (results[0].Contains("X"))
            {
                if(DateTime.Now.Date.Month > int.Parse(results[1]))
                {
                    results[0] = (DateTime.Now.Date.Year + 1).ToString();
                }
                else if(DateTime.Now.Date.Month == int.Parse(results[1]) 
                    && DateTime.Now.Date.Day >= int.Parse(results[2]))
                {
                    results[0] = (DateTime.Now.Date.Year + 1).ToString();
                }
                else
                {
                    results[0] = DateTime.Now.Date.Year.ToString();
                }
            }

            return string.Join('-', results);
        }

        #endregion

        #region Validators

        private Task<bool> DatetimeValidator(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            var dt = promptContext.Recognized.Value;

            return Task.FromResult(true);
        }

        #endregion
    }
}
