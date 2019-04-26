using ILuvLuis.Web.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ILuvLuis.Web.Dialogs.MomQuestions
{
    public class MomDialog : ComponentDialog
    {
        #region Constants

        public const string ChitchatDialogId = "MomQuestion";
        public const string Intent = "MOM";

        private const string DefaultDialog = "DefaultDialog";

        #endregion

        #region Fields

        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnProperty;

        #endregion

        #region Constructor

        public MomDialog(IStatePropertyAccessor<OnTurnProperty> onTurnProperty) : base(ChitchatDialogId)
        {
            _onTurnProperty = onTurnProperty;

            var steps = new List<WaterfallStep>()
            {
                ReturnResponse
            };

            AddDialog(new WaterfallDialog(DefaultDialog, steps));
        }

        #endregion

        private async Task<DialogTurnResult> ReturnResponse(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var turnProperty = await _onTurnProperty.GetAsync(stepContext.Context);

            if (turnProperty != null)
            {
                if (turnProperty.Intent.Contains("Wifi"))
                {
                    await stepContext.Context.SendActivityAsync("Nombre: MOMWIFI");
                    await stepContext.Context.SendActivityAsync("Contraseña: abcde12345");
                }
                else if (turnProperty.Intent.Contains("Floor"))
                {
                    await stepContext.Context.SendActivityAsync("Primer (1) piso");
                }
                else if (turnProperty.Intent.Contains("Print-Card"))
                {
                    await stepContext.Context.SendActivityAsync("Solicitar una nueva en sistemas y por única vez ingresar login y contraseña en la impresora");
                }
                else if (turnProperty.Intent.Contains("Unlock-Web"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que mandar la página a mesa de ayuda para que la verifiquen y luego la desbloquean");
                }
                else if (turnProperty.Intent.Contains("Unlock-User"))
                {
                    await stepContext.Context.SendActivityAsync("Si sos socio, debes mandar un mail a mesa de ayuda con copia al comité ejecutivo.");
                    await stepContext.Context.SendActivityAsync("En el caso de los abogados, deben mandar un mail a mesa de ayuda con copia a tu socio.");
                }
                else if (turnProperty.Intent.Contains("Update-File-Room"))
                {
                    await stepContext.Context.SendActivityAsync("Podes ingresar a tu mail desde la sala a https:\\mail.marval.com");
                    await stepContext.Context.SendActivityAsync("O enviarlo por mail a mesa de ayudar solicitando la copia del mismo");
                }
                else if (turnProperty.Intent.Contains("Vpn-Access"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que enviar un mail a mesa de ayuda con copia a tu socio solicitando los respectivos permisos");
                }
                else if (turnProperty.Intent.Contains("New-Access-Card"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que pedirla a RRHH");
                }
                else if (turnProperty.Intent.Contains("Upload-Media-Hard"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que enviar por mail a mesa de ayuda los documentos y luego bajar con el CD/Pendrive a sistemas");
                }
                else if (turnProperty.Intent.Contains("Change-Password"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que presionar Ctrl Alt Supr y elegir la opción cambiar una contraseña");
                }
                else if (turnProperty.Intent.Contains("Update-File-Subject"))
                {
                    await stepContext.Context.SendActivityAsync("Primero debes chequear que el asunto tenga cliente designado");
                    await stepContext.Context.SendActivityAsync("es la única condición para que NetDocuments te permita trabajar con dicho asunto");
                }
                else if (turnProperty.Intent.Contains("Full-Mail-Space"))
                {
                    await stepContext.Context.SendActivityAsync("Debes comunicarte con mesa de ayuda para que te ayuden en la creación de una carpeta personal y asi poder mover mails ahí");
                }
                else if (turnProperty.Intent.Contains("Pdf-Word"))
                {
                    await stepContext.Context.SendActivityAsync("En tu escritorio tenes una carpeta que se llama “Pdf a Word”, dentro de la misma vas a encontrar varias carpetas con la descripción del proceso que realiza cada una, elegí la que se adecue a tu necesidad");
                }
                else if (turnProperty.Intent.Contains("Link-To-Send"))
                {
                    await stepContext.Context.SendActivityAsync("Tenes que crear una carpeta en tu escritorio con la información a subir a dicho link y luego contactar a la mesa de ayuda para solicitarlo");
                }
                else if (turnProperty.Intent.Contains("Failed-Password"))
                {
                    await stepContext.Context.SendActivityAsync("Se va a bloquear tu usuario, debes comunicarte con sistemas para realizar el desbloqueo");
                }
            }

            return await stepContext.EndDialogAsync();
        }

    }
}
