﻿using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs
{
    public class AskSalaryWhen : ComponentDialog
    {
        private const string Response = "Se cobra el tercer (3) día hábil antes de que termine el mes";
        private const string AskSalaryWhenDialog = "AskSalaryWhenDialog";

        public const string AskSalaryWhenId = "Ask-Salary-When";
        public const string Intent = "Ask-Salary-When";

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        public AskSalaryWhen(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(AskSalaryWhenId)
        {
            var steps = new List<WaterfallStep>()
            {
                ReturnResponse
            };

            _onTurnProperty = onTurnProperty;

            AddDialog(new WaterfallDialog(AskSalaryWhenDialog, steps));
        }

        private async Task<DialogTurnResult> ReturnResponse(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var onTurnProperty = await _onTurnProperty.GetAsync(stepContext.Context, () => new OnTurnProperty());

            if (onTurnProperty.Intent == AskSalaryWhenId)
            {
                await stepContext.Context.SendActivityAsync(Response);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
