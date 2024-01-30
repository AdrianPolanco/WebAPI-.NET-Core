using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOS.Accounts;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        //UserManager<AppUser> es una clase que se encarga de administrar los usuarios y los roles de la aplicacion, usando un almacen de datos configurado, el cual le especificamos usando .AddEntityFrameworkStores<ApplicationDbContext>() en Program.cs
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                //.ModelState.IsValid valida si los datos insertados en las propieades de RegisterDto son validos de acuerdo a las reglas de validación que definimos en ella, devolvemos 403 Bad Request si falla al validación
                if (!ModelState.IsValid) return BadRequest(ModelState);

                //Creamos un nuevo usuario si pasa la validación
                AppUser user = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                //UserManager.CreateAsync(User, password) crea el usuario proporcionado, el cual en este caso debera ser de tipo AppUser, ya que el UserManager tiene como generico el AppUser: UserManager<AppUser>, y tambien recibe una contraseña tipo string. Esta contraseña la hasheara automaticamente con un algoritmo de hasheo con sal (que consiste en añadir un valor aleatorio a la contraseña antes de aplicar el algoritmo de hasheo, generando una representacion segura y aleatoria de la contraseña), y despues de hashearlo pues guardara el usuario en la BD, finalmente devuelve si la operacion tuvo exito o no
                IdentityResult? createdUser = await _userManager.CreateAsync(user, registerDto.Password);

                //Chequeando si la creacion del usuario tuvo exito
                if (createdUser.Succeeded)
                {
                    //Añadiendole el rol al usuario creado
                    IdentityResult? roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                        return Ok(new NewUserDto
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user)
                        }
                    );
                    else
                        return StatusCode(500, roleResult.Errors);
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //This line of code will allow us to find some user without accessing to our DbContext, the UserManager will do it for us
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            //Compares the password stored in the found user and the password passed through the request object, the false parameter is passed in order to not lock the account after 5 failed attempts
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("User not found and/or invalid credentials");

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }
    }
}