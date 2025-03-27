namespace Makai_APIProject.Models
{
    public class UsersModel
    {

        public long user_id { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? email { get; set; }

        public string? phone { get; set; }

        public string? address { get; set; }
        public long role_id { get; set; }
        public string? Token { get; set; }
    }
}

