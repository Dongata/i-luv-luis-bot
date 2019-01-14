namespace ILuvLuis.Web.Dialogs
{
    public class PersonInternState
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Intern { get; set; }
        public string Department { get; set; }

        public bool IsEverithyngSetted()
        {
            return !string.IsNullOrEmpty(FirstName)
                && !string.IsNullOrEmpty(LastName)
                && !string.IsNullOrEmpty(Intern)
                && !string.IsNullOrEmpty(Department);
        }
    }
}
