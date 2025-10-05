namespace my_api.Models
{
    public class User
    {
        // Primary Key 
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; } = string.Empty;

    }
}