using Microsoft.Bot.Schema;
using System;

namespace ILuvLuis.Web.Dialogs.Base
{
    public class RandomTextResponse : Activity
    {
        private readonly string[] _randomResponses;

        public RandomTextResponse(string[] randomResponses)
        {
            _randomResponses = randomResponses ?? throw new ArgumentNullException(nameof(randomResponses));

            var r = new Random();
            var response = _randomResponses[r.Next(0, _randomResponses.Length - 1)];

            Type = ActivityTypes.Message;
            Text = response;
        }
    }
}
