using AdaptiveCards;
using ILuvLuis.Web.Cards;
using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs
{
    public class AskPersonIntern : ComponentDialog
    {
        private const string SearchDialog = "SearchDialog";
        private const string AskNameDialog = "AskNameDialog";
        private const string AskDepartmentDialog = "AskDepartmentDialog";

        public const string AskPersonInternId = "AskPersonIntern";

        private readonly IStatePropertyAccessor<PersonInternState> _personInternStateAccessor;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnPropertyAccessor;
        private readonly IStatePropertyAccessor<DialogStateProperties> _dialogStateProperties;

        private readonly List<PersonInternState> _personInternStates;

        public AskPersonIntern(
            IStatePropertyAccessor<OnTurnProperty> onTurnPropertyAccessor,
            IStatePropertyAccessor<PersonInternState> personInternStateAccessor,
            IStatePropertyAccessor<DialogStateProperties> dialogStateProperties) : base(AskPersonInternId)
        {
            _personInternStates = new List<PersonInternState>()
            {
                new PersonInternState(){FirstName = "Gaston", LastName = "Cerioni", Department = "Desarrollo", Intern = "1234"},
                new PersonInternState(){FirstName = "Ariel", LastName = "Cabrejos", Department = "Administracíon", Intern = "1234"},
                new PersonInternState(){FirstName = "Ariel", LastName = "Test", Department = "Administracíon", Intern = "4321"},
                new PersonInternState(){FirstName = "Ariel", LastName = "Test", Department = "desarrollo", Intern = "1234"},
                new PersonInternState(){FirstName = "Ariel", LastName = "Human", Department = "Recursos Humanos", Intern = "4321"}
            };

            _onTurnPropertyAccessor = onTurnPropertyAccessor;
            _personInternStateAccessor = personInternStateAccessor;
            _dialogStateProperties = dialogStateProperties;

            var steps = new WaterfallStep[]
            {
                //SearchUserStep,
                Initialize,
                CheckFirstState,
                CheckName,
                CheckDepartment
            };

            AddDialog(new WaterfallDialog(SearchDialog, steps));
            AddDialog(new TextPrompt(AskNameDialog));
            AddDialog(new TextPrompt(AskDepartmentDialog));
        }

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var state = new DialogStateProperties();

            var turnEntities = (await _onTurnPropertyAccessor.GetAsync(stepContext.Context, () => new OnTurnProperty()))
                .Entities;

            var name = turnEntities.FirstOrDefault(
                a => a.EntityName == "employeeName"
                && a.Value != null
                && a.Value.ToString() != string.Empty);

            if (name != null)
            {
                var nameValue = ((JArray)name.Value).First().ToString();
                state.Add("employeeName", nameValue);
            }

            var department = turnEntities.FirstOrDefault(
                a => a.EntityName == "department"
                && a.Value != null
                && a.ToString() != string.Empty);

            if (department != null)
            {
                var departmentValue = ((JArray)department.Value).First().ToString();
                state.Add("department", departmentValue);
            }

            await _dialogStateProperties.SetAsync(stepContext.Context, state);
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> CheckFirstState(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var state = await _dialogStateProperties.GetAsync(stepContext.Context);

            if (state.ContainsKey("employeeName"))
            {
                var name = (string)state["employeeName"];

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

            if (!state.ContainsKey("employeeName"))
            {
                state.Add("employeeName", name);
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
                // Check if lastName is setted
                if (state.ContainsKey("lastName"))
                {
                    return await stepContext.NextAsync(state["employeeName"]);
                }
                else
                {
                    return await stepContext.PromptAsync(
                        AskDepartmentDialog,
                        new PromptOptions() { Prompt = MessageFactory.Text("Dame solamente el departamento de la persona") },
                        cancellationToken);
                }
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
                await stepContext.Context.SendActivityAsync($"se encontraron {userCount} personas con ese criterio");
                return await stepContext.EndDialogAsync();
            }
        }

        private async Task<int> TryShowUser(ITurnContext turnContext, DialogStateProperties state)
        {
            var users = new List<PersonInternState>();

            if (state.ContainsKey("employeeName") && state["employeeName"].ToString().Split(" ").Count() > 1)
            {
                users = _personInternStates.FindAll(
                    u => (!state.ContainsKey("employeeName") || state["employeeName"].ToString().Trim().ToLowerInvariant().Contains(u.FirstName.Trim().ToLowerInvariant()))
                    && (!state.ContainsKey("employeeName") || state["employeeName"].ToString().Trim().ToLowerInvariant().Contains(u.LastName.Trim().ToLowerInvariant()))
                    && (!state.ContainsKey("department") || state["department"].ToString().Trim().ToLowerInvariant() == u.Department.Trim().ToLowerInvariant()));
            }
            else
            {
                users = _personInternStates.FindAll(
                    u => ((!state.ContainsKey("employeeName") || state["employeeName"].ToString().Trim().ToLowerInvariant().Contains(u.FirstName.Trim().ToLowerInvariant()))
                    || (!state.ContainsKey("employeeName") || state["employeeName"].ToString().Trim().ToLowerInvariant().Contains(u.LastName.Trim().ToLowerInvariant())))
                    && (!state.ContainsKey("department") || state["department"].ToString().Trim().ToLowerInvariant() == u.Department.Trim().ToLowerInvariant()));
            }

            if (users.Count == 1)
            {
                var user = users.First();
                var adaptiveCardAttch = new PersonInternAdaptiveCard(user).ToAttachment();

                var reply = turnContext.Activity.CreateReply();
                reply.Attachments = new List<Attachment>() { adaptiveCardAttch };
                await turnContext.SendActivityAsync(reply);

                await _personInternStateAccessor.DeleteAsync(turnContext);
                await _dialogStateProperties.DeleteAsync(turnContext);
            }

            return users.Count;
        }
    }
}
