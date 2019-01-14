using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.AskEmergencyProc
{
    public class EmergencyProcedureDialog : ComponentDialog
    {
        #region Constants

        public const string EmergencyProcedureId = "AskEmergencyProcedure";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

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


            AddDialog(new WaterfallDialog("", steps));
        }

        #endregion

        #region Steps

        private async Task<DialogTurnResult> Initialize(WaterfallStepContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        private Task<DialogTurnResult> ShowEmergencyProcedure(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
