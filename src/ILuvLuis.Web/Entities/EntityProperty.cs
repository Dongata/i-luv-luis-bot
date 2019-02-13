namespace ILuvLuis.Web.Entities
{
    public class EntityProperty
    {
        #region Constants

        public const string EmployeeName = "employeeName";
        public const string Department = "department";
        public const string EmergencyType = "emergency_type";
        public const string LegalTreatyType = "legal_treaty_type";
        public const string DateTime = "datetime";

        #endregion

        #region Constructors

        public EntityProperty(string name, object value)
        {
            EntityName = name;
            Value = value;
        }

        #endregion

        #region Properties

        public string EntityName { get; }

        public object Value { get; } 

        #endregion
    }
}
