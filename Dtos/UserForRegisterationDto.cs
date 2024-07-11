namespace DotnetAPI.Dtos
{
    partial class UserForRegisteration
    {
        string Email { get; set; }
        string Password { get; set; }
        string PasswordConfirm { get; set; }

        public UserForRegisteration()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Password == null)
            {
                Password = "";
            }
            if (PasswordConfirm == null)
            {
                PasswordConfirm = "";
            }
        }
    }
}