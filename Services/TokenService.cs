using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Services
{
    public class TokenService : ITokenService
    {

        //We use IConfiguration because we wanna pull data from the appsettings.json
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            //Usamos Encoding.UTF8.GetBytes(string Key) debido a que el constructor de SymetricSecurityKey no acepta string, sino un array de bytes: bytes[]
            //Usaremos el SymmetricSecurityKey para encriptar nuestros token de forma unica e íntegra
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //Creamos una lista de Claims, cada claim sera un dato dentro del JWT Token
            List<Claim> claims = new List<Claim>
            {
                //Claim con la clave email y el valor de user.Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //Claim con la clave given_name(en el token) y el valor de user.UserName
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            //Creamos SigningCredentials, que es un objeto que representa las credenciales con la que firmaremos los tokens
            //Este objeto recibe como parametro nuestra llave simetrica (SymmetricSecurityKey), la cual sera el string que se encuentra en el appsettings.json dentro de JWT y SigningKey, el cual estara encodeado a UTF8
            //Y el segundo parametro es el algoritmo que usaremos para generar la firma
            SigningCredentials creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //El SecurityTokenDescriptor describe, finalmente, todos los datos que tendra el JWT en su payload(es decir, su cuerpo principal con la informacion)
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                //ClaimsIdentity es un objeto que representa la identidad del usuario basada en los Claims que creamos anteriormente, esto añadira al token los claims que especificamos nosotros anteriormente
                Subject = new ClaimsIdentity(claims),
                //Fecha de expiracion del token sera de 7 dias a partir del momento actual
                Expires = DateTime.Now.AddDays(7),
                //Usamos aqui el SigningCredentials creado anteriormente para generar la firma, esto agregara la fecha de expiracion en el token, en formato, supongo, de milisegundo en la clave "exp" en el token, que representa "Expires"
                SigningCredentials = creds,
                //Añadira la entidad emisora del token a partir del string que obtengamos en el appsettings.json en esa clave especifica (JWT:Issure), esto añadira la clave "iss" en el token, que representa el Issuer
                Issuer = _config["JWT:Issuer"],
                //Añadira la audiencia del token a aprtir del string que obtengamos de appsettings.json en la clave (JWT:Audience), esto añadira la clave "aud" en el token, que representa Audience
                Audience = _config["JWT:Audience"]
            };

            //Usamos JwtSecurityTokenHandler, una clase usada para crear, validar y escribir tokens JWT
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            //Creamos el token usando tokenHandler.CreateToken(tokenDescriptor), osea, creamos el token a partir de la descripcion que nos brinda la clase SecurityTokenDescriptor, y este metodo devuelve un SecurityToken, que representa el token que generamos para el usuario, con sus respectivas partes que tiene un JWT: Headers, Payload y Signatura. Es nulleable porque asi puede contener un null en caso de que el metodo falle
            SecurityToken? createdToken = tokenHandler.CreateToken(tokenDescriptor);

            //Usamos token.Handler.WriteToken(createdToken) para escribir un string del token que acabamos de generar en formato Base64URL para poder retornarlo
            string token = tokenHandler.WriteToken(createdToken);

            //Retornamos el token al cliente para que este pueda usarlo mas adelante, en solicitudes que requieran autenticacion
            return token;

        }
    }
}