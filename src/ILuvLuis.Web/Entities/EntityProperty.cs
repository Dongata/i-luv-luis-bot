namespace ILuvLuis.Web.Entities
{
    public class EntityProperty
    {
        public EntityProperty(string name, object value)
        {
            EntityName = name;
            Value = value;
        }

        public string EntityName { get; }

        public object Value { get; }
    }
}
