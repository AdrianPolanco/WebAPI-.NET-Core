using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    //Al AppUser heredar de IdentityUser, heredara una serie de propiedades que tiene la clase IdentityUser, como:

    /*
    Id: Obtiene o establece el identificador único del usuario.
    UserName: Obtiene o establece el nombre de usuario para este usuario.
    NormalizedUserName: Obtiene o establece el nombre de usuario normalizado para este usuario.
    Email: Obtiene o establece la dirección de correo electrónico para este usuario.
    NormalizedEmail: Obtiene o establece la dirección de correo electrónico normalizada para este usuario.
    EmailConfirmed: Obtiene o establece un indicador que señala si la dirección de correo electrónico de este usuario ha sido confirmada.
    PasswordHash: Obtiene o establece una representación con hash y salada de la contraseña para este usuario.
    SecurityStamp: Obtiene o establece un sello de seguridad que se usa para verificar que el usuario es quien dice ser.
    ConcurrencyStamp: Obtiene o establece un sello que se usa para manejar la concurrencia optimista al actualizar el usuario.
    PhoneNumber: Obtiene o establece un número de teléfono para el usuario.
    PhoneNumberConfirmed: Obtiene o establece un indicador que señala si el número de teléfono de este usuario ha sido confirmado.
    TwoFactorEnabled: Obtiene o establece un indicador que señala si el usuario tiene habilitada la autenticación de dos factores.
    LockoutEnd: Obtiene o establece la fecha y hora en que finaliza el bloqueo del usuario.
    LockoutEnabled: Obtiene o establece un indicador que señala si el usuario puede ser bloqueado.
    AccessFailedCount: Obtiene o establece el número de intentos fallidos de acceso al usuario.
    */
    public class AppUser : IdentityUser
    {

    }
}