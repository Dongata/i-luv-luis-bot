using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ILuvLuis.Web.Entities
{
    public class OnTurnProperty
    {
        #region Constants

        public const string Luis = "LUIS";
        public const string Card = "CARD";

        private static readonly string[] luisEntities =
        {
            EntityProperty.DateTime,
            EntityProperty.Department,
            EntityProperty.EmergencyType,
            EntityProperty.EmployeeName

        };

        #endregion

        #region Constructors

        public OnTurnProperty()
        {
            Intent = null;
            Entities = new List<EntityProperty>();
        }

        public OnTurnProperty(string intent, List<EntityProperty> entities)
        {
            Intent = intent ?? throw new ArgumentNullException(nameof(intent));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        #endregion

        #region Properties

        public string Intent { get; set; }

        public double Score { get; set; }

        public string Type { get; set; }

        public List<EntityProperty> Entities { get; set; }

        #endregion

        public static OnTurnProperty FromLuisResults(RecognizerResult luisResults)
        {
            var (intent, score) = luisResults.GetTopScoringIntent();

            var onTurnProperties = new OnTurnProperty
            {
                Intent = intent,
                Score = score,
                Type = Luis
            };

            // Gather entity values if available. Uses a const list of LUIS entity names.
            foreach (var entity in luisEntities)
            {
                var value = luisResults.Entities.SelectTokens(entity).FirstOrDefault();
                if (value == null)
                {
                    continue;
                }

                onTurnProperties.Entities.Add(new EntityProperty(entity, value));
            }

            return onTurnProperties;
        }

        /// <summary>
        /// Static method to create an on turn property object from card input.
        /// </summary>
        /// <param name="cardValues">context.activity.value from a card interaction</param>
        /// <returns>OnTurnProperty.</returns>
        public static OnTurnProperty FromCardInput(Dictionary<string, string> cardValues)
        {
            // All cards used by this bot are adaptive cards with the card's 'data' property set to useful information.
            var onTurnProperties = new OnTurnProperty()
            {
                Type = Card
            };
            foreach (var entry in cardValues)
            {
                if (!string.IsNullOrWhiteSpace(entry.Key) && string.Compare(entry.Key.ToLower().Trim(), "intent") == 0)
                {
                    onTurnProperties.Intent = cardValues[entry.Key];
                }
                else
                {
                    onTurnProperties.Entities.Add(new EntityProperty(entry.Key, cardValues[entry.Key]));
                }
            }

            return onTurnProperties;
        }
    }
}
