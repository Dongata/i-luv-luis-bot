using ILuvLuis.Models.Stores;
using ILuvLuis.Web.Dialogs;
using ILuvLuis.Web.Dialogs.AskEmergencyProc;
using ILuvLuis.Web.Dialogs.AskHolidayDay;
using ILuvLuis.Web.Dialogs.DefaultFallbacks;
using ILuvLuis.Web.Dialogs.Greetings;
using ILuvLuis.Web.Dialogs.LegalTreatyTransport;
using ILuvLuis.Web.Entities;
using ILuvLuis.Web.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Bots
{
    /// <summary>
    /// Registered bot for this particular application
    /// </summary>
    public class MainBot : IBot
    {
        #region Constants

        public const string OnTurnPropertyName = "onTurnStateProperty";
        public const string PersonInternStateName = "personInternState";
        public const string DialogStateProperty = "dialogStateProperty";
        public const string DialogStateProperties = "dialogStateProperties";

        private const string BotKey = "MainBot";

        #endregion

        #region Services

        private readonly BotServices _botServices;
        private readonly DialogSet _dialogSet;
        private readonly IPersonStore _personStore;
        private readonly ILogger<MainBot> _logger;

        #endregion

        #region Accessors

        private readonly IStatePropertyAccessor<DialogState> _dialogAccessor;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        private readonly IStatePropertyAccessor<PersonInternState> _personInternState;
        private readonly IStatePropertyAccessor<DialogStateProperties> _dialogStateProperties;

        #endregion

        #region Constructor

        public MainBot(BotServices botServices, ConversationState conversationState, IPersonStore personStore, ILogger<MainBot> logger)
        {
            _botServices = botServices ?? throw new ArgumentNullException(nameof(BotServices));

            ConversationState = conversationState;

            _logger = logger;

            _personStore = personStore;
            _onTurnAccessor = conversationState.CreateProperty<OnTurnProperty>(OnTurnPropertyName);
            _personInternState = conversationState.CreateProperty<PersonInternState>(PersonInternStateName);
            _dialogAccessor = conversationState.CreateProperty<DialogState>(DialogStateProperty);
            _dialogStateProperties = conversationState.CreateProperty<DialogStateProperties>(DialogStateProperties);

            if (!_botServices.LuisServices.ContainsKey(BotKey))
            {
                throw new ArgumentException($"Invalid configuration. Please check your '.bot' file for a LUIS service named '{BotKey}'.");
            }

            _dialogSet = new DialogSet(_dialogAccessor);
            _dialogSet.Add(new AskSalaryWhen(_onTurnAccessor));
            _dialogSet.Add(new ChitchatDialog(_onTurnAccessor));
            _dialogSet.Add(new AskPerson(_personStore, _onTurnAccessor, _personInternState, _dialogStateProperties));
            _dialogSet.Add(new EmergencyProcedureDialog(_onTurnAccessor));
            _dialogSet.Add(new AskHolidayDayDialog(_onTurnAccessor));
            _dialogSet.Add(new LegalTreatyBetweenCountriesDialog(_onTurnAccessor));
        }

        #endregion

        #region Properties

        public ConversationState ConversationState { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Every time a user interacts directly or inderectly this method is called
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dc = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                /*
                var jobject = turnContext.Activity.ChannelData as JObject;
                _personStore.Token = jobject.ToObject<TokenChannelData>().QavantToken;
                */

                _personStore.Token = "J6AF2O8xpIuiCD2RHLWCBEPcQCeWaC7jJzJ2J8cD8CfHTBXnQGDaLWu54eWCOOSEGJGA/p8V7VB0vUiq67cNCQxGx+8u/U1RhNXmd04EQKS+NAOQrWFipx0BwWTyMZl4nhYTjfhAuu4R7mqqcNGrVG7MCXQSYrC4JdtaJxYQ9WpgBPa3zgxsGR5tt9V1f4xFlPc+YzTHgxIxPljxbs+SMA==";

                var turnProperty = await DetectIntentAndEntitiesAsync(turnContext);
                await _onTurnAccessor.SetAsync(turnContext, turnProperty);

                if (turnProperty?.Type == OnTurnProperty.Luis && turnProperty?.Intent == "Cancel" && turnProperty?.Score > 0.5f)
                {
                    await turnContext.SendActivityAsync("Ok, no hay drama");
                    await dc.EndDialogAsync();
                    return;
                }

                var results = await dc.ContinueDialogAsync(cancellationToken);

                if (results.Status == DialogTurnStatus.Cancelled || results.Status == DialogTurnStatus.Waiting || results.Status == DialogTurnStatus.Complete)
                {
                    return;
                }
                else
                {
                    if (turnProperty.Type == OnTurnProperty.Luis)
                    {
                        if (turnProperty.Score < 0.4f)
                        {
                            await turnContext.SendActivityAsync(new DefaultFallback().Text);
                        }
                        else
                        {
                            await dc.ContinueDialogAsync();

                            if (!dc.Context.Responded)
                            {
                                if (turnProperty.Intent == AskSalaryWhen.Intent)
                                {
                                    await dc.BeginDialogAsync(AskSalaryWhen.AskSalaryWhenId);
                                }
                                else if (turnProperty.Intent.Contains(AskPerson.Intent))
                                {
                                    await dc.BeginDialogAsync(AskPerson.AskPersonInternId);
                                }
                                else if (turnProperty.Intent == EmergencyProcedureDialog.Intent)
                                {
                                    await dc.BeginDialogAsync(EmergencyProcedureDialog.EmergencyProcedureId);
                                }
                                else if (turnProperty.Intent == AskHolidayDayDialog.Intent)
                                {
                                    await dc.BeginDialogAsync(AskHolidayDayDialog.AskHolidayDayId);
                                }
                                else if (turnProperty.Intent.Contains(ChitchatDialog.Intent))
                                {
                                    await dc.BeginDialogAsync(ChitchatDialog.ChitchatDialogId);
                                }
                                else if (turnProperty.Intent.Contains(LegalTreatyBetweenCountriesDialog.Intent))
                                {
                                    await dc.BeginDialogAsync(LegalTreatyBetweenCountriesDialog.DialogId);
                                }
                                else
                                {
                                    await turnContext.SendActivityAsync(new DefaultFallback(), cancellationToken);
                                }
                            }
                        }
                    }
                }
            }
            else if (turnContext.Activity.Type == "requestWelcomeDialog")
            {

                if (string.IsNullOrEmpty(turnContext.Activity.From.Name))
                {
                    await turnContext.SendActivityAsync($"Hola, me llamo lucho, tu asistente virtual, ¿en que puedo ayudarte?");
                }
                else
                {
                    await turnContext.SendActivityAsync($"Hola {turnContext.Activity.From.Name}, me llamo lucho, tu asistente virtual, ¿en que puedo ayudarte?");
                }
            }
        }

        #endregion

        #region Private methds

        private async Task<OnTurnProperty> DetectIntentAndEntitiesAsync(ITurnContext turnContext)
        {
            // Handle card input (if any), update state and return.
            if (turnContext.Activity.Value != null)
            {
                var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(turnContext.Activity.Value as string);
                return OnTurnProperty.FromCardInput(response);
            }

            // Acknowledge attachments from user.
            if (turnContext.Activity.Attachments != null && turnContext.Activity.Attachments.Count > 0)
            {
                await turnContext.SendActivityAsync("Todavía no entiendo archivos c:");
                return null;
            }

            // Nothing to do for this turn if there is no text specified.
            if (string.IsNullOrWhiteSpace(turnContext.Activity.Text) || string.IsNullOrWhiteSpace(turnContext.Activity.Text.Trim()))
            {
                return null;
            }

            // Call to LUIS recognizer to get intent + entities.
            var luisResults = await _botServices.LuisServices[BotKey].RecognizeAsync(turnContext, default(CancellationToken));

            // Return new instance of on turn property from LUIS results.
            // Leverages static fromLUISResults method.
            return OnTurnProperty.FromLuisResults(luisResults);
        }

        #endregion
    }
}
