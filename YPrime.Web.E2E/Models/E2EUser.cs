namespace YPrime.Web.E2E.Models
{
    public class E2EUser
    {
        public E2EUser(
            string mappingName,
            string username,
            string password)
        {
            MappingName = mappingName;
            Username = username;
            Password = password;
        }

        public string MappingName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
