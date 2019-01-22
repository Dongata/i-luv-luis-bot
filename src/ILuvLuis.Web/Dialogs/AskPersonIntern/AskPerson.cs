using AdaptiveCards;
using ILuvLuis.Models;
using ILuvLuis.Models.Stores;
using ILuvLuis.Web.Cards;
using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs
{
    public class AskPerson : ComponentDialog
    {
        #region Constants

        private const string SearchDialog = "SearchDialog";
        private const string AskNameDialog = "AskNameDialog";
        private const string AskDepartmentDialog = "AskDepartmentDialog";
        private const string EmployeeNameKey = "employeeName";
        private const string DepartmentKey = "department";
        private const string IntentKey = "intent";

        public const string AskPersonInternId = "AskPersonIntern";
        public const string Intent = "Ask-Person";

        #endregion

        #region State accessors

        private readonly IStatePropertyAccessor<PersonInternState> _personInternStateAccessor;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnPropertyAccessor;
        private readonly IStatePropertyAccessor<DialogStateProperties> _dialogStateProperties;

        #endregion

        #region Fields

        private readonly IPersonStore _personStore;

        #endregion

        #region Consturctor

        public AskPerson(
            IPersonStore personStore,
            IStatePropertyAccessor<OnTurnProperty> onTurnPropertyAccessor,
            IStatePropertyAccessor<PersonInternState> personInternStateAccessor,
            IStatePropertyAccessor<DialogStateProperties> dialogStateProperties) : base(AskPersonInternId)
        {
            _personStore = personStore;
            _onTurnPropertyAccessor = onTurnPropertyAccessor;
            _personInternStateAccessor = personInternStateAccessor;
            _dialogStateProperties = dialogStateProperties;

            var steps = new WaterfallStep[]
            {
                Initialize,
                CheckFirstState,
                CheckName,
                CheckDepartment
            };

            AddDialog(new WaterfallDialog(SearchDialog, steps));
            AddDialog(new TextPrompt(AskNameDialog));
            AddDialog(new TextPrompt(AskDepartmentDialog));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var state = new DialogStateProperties();

            var turnProperties = await _onTurnPropertyAccessor.GetAsync(stepContext.Context, () => new OnTurnProperty());

            var decomposedIntent = turnProperties.Intent.Split('-');

            state.Add(IntentKey, decomposedIntent[2]);

            var turnEntities = turnProperties.Entities;
            
            var name = turnEntities.FirstOrDefault(
                a => a.EntityName == EmployeeNameKey
                && a.Value != null
                && a.Value.ToString() != string.Empty);

            if (name != null)
            {
                var nameValue = ((JArray)name.Value).First().ToString();
                state.Add(EmployeeNameKey, nameValue);
            }

            var department = turnEntities.FirstOrDefault(
                a => a.EntityName == DepartmentKey
                && a.Value != null
                && a.ToString() != string.Empty);

            if (department != null)
            {
                var departmentValue = ((JArray)department.Value).First().ToString();
                state.Add(DepartmentKey, departmentValue);
            }

            await _dialogStateProperties.SetAsync(stepContext.Context, state);
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> CheckFirstState(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var state = await _dialogStateProperties.GetAsync(stepContext.Context);

            if (state.ContainsKey(EmployeeNameKey))
            {
                var name = (string)state[EmployeeNameKey];

                var userCount = await TryShowUser(stepContext.Context, state);

                if (userCount == 0)
                {
                    await stepContext.Context.SendActivityAsync($"No se encontro ninguna persona con los datos proporcionados");
                    return await stepContext.EndDialogAsync();
                }
                else if (userCount == 1)
                {
                    return await stepContext.EndDialogAsync();
                }
                else
                {
                    return await stepContext.NextAsync(name);
                }
            }
            else
            {
                return await stepContext.PromptAsync(
                    AskNameDialog,
                    new PromptOptions() { Prompt = MessageFactory.Text("Dame el nombre de la persona") },
                    cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CheckName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var name = stepContext.Result as string;

            var state = await _dialogStateProperties.GetAsync(stepContext.Context);

            if (!state.ContainsKey(EmployeeNameKey))
            {
                state.Add(EmployeeNameKey, name);
                await _dialogStateProperties.SetAsync(stepContext.Context, state);
            }

            var userCount = await TryShowUser(stepContext.Context, state);

            if (userCount == 0)
            {
                await stepContext.Context.SendActivityAsync($"No se encontro ninguna persona con los datos ingresados");
                return await stepContext.EndDialogAsync();
            }
            else if (userCount == 1)
            {
                return await stepContext.EndDialogAsync();
            }
            else
            {
                return await stepContext.PromptAsync(
                    AskDepartmentDialog,
                    new PromptOptions() { Prompt = MessageFactory.Text("Dame solamente el departamento de la persona") },
                    cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CheckDepartment(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var department = stepContext.Result as string;

            var state = await _dialogStateProperties.GetAsync(stepContext.Context);

            if (!state.ContainsKey("department"))
            {
                state.Add("department", department);
                await _dialogStateProperties.SetAsync(stepContext.Context, state);
            }

            var userCount = await TryShowUser(stepContext.Context, state, true);

            if (userCount == 0)
            {
                await stepContext.Context.SendActivityAsync($"No se encontro ninguna persona con los datos ingresados");
            }

            return await stepContext.EndDialogAsync();
        }

        #endregion

        #region private members

        private async Task<int> TryShowUser(ITurnContext turnContext, DialogStateProperties state, bool showMultiple = false)
        {
            var users = await _personStore.Search(state[EmployeeNameKey].ToString());

            if (users.Count() > 1 && state.ContainsKey(DepartmentKey))
            {
                users = users.Where(a => state[DepartmentKey].ToString().Normalize() == a.Area.Normalize());
            }

            if (!showMultiple)
            {
                if (users.Count() == 1)
                {
                    await SendCard(turnContext, users.First(), state[IntentKey].ToString());

                    await _personInternStateAccessor.DeleteAsync(turnContext);
                    await _dialogStateProperties.DeleteAsync(turnContext);
                }
            }
            else
            {
                foreach (var user in users)
                {
                    await SendCard(turnContext, user, state[IntentKey].ToString());
                }
            }

            return users.Count();
        }

        private static async Task SendCard(ITurnContext turnContext, Person user, string accentField = "")
        {
            var adaptiveCardAttch = new PersonAdaptiveCard(user, accentField).ToAttachment();

            var reply = turnContext.Activity.CreateReply();
            reply.Attachments = new List<Attachment>() { adaptiveCardAttch };
            await turnContext.SendActivityAsync(reply);
        }

        #endregion
    }
}
