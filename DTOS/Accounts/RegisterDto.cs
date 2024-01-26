
using System.ComponentModel.DataAnnotations;


namespace WebApi.DTOS.Accounts
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = null!;

        //El atributo EmailAddress automaticamente validará el formato de correo
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}