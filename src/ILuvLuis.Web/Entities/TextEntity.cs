namespace ILuvLuis.Web.Entities
{
    public class TextEntity
    {
        public TextEntity(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; }

        public string Value { get; }
    }
}
