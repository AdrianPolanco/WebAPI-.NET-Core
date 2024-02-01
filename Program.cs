using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

//You create a new ASP.NET Core Web API project with the command: dotnet new webapi -o PROJECT_NAME
//Command in order to run and see the Swagger UI with the endpoints: dotnet watch run
/*We were able to add through UI the EF Core and other packages thanks to the extension Nuget Gallery, what it does is to make able in VS Code the Nuget Package Manager integrated en Visual Studio*/
var builder = WebApplication.CreateBuilder(args);

//Adding the controllers we have code before
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Adding Swagger Auth UI
builder.Services.AddSwaggerGen(option =>
{
    //Specifying the version
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    //Adding the security definitions with the name of "Bearer"
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        //Will be ubicated in the Header of the HTTP Request
        In = ParameterLocation.Header,
        //Description
        Description = "Please enter a valid token",
        //nAME
        Name = "Authorization",
        //Type of security Scheme
        Type = SecuritySchemeType.Http,
        //Format of the bearer toekn
        BearerFormat = "JWT",
        //Scheme
        Scheme = "Bearer"
    });
    //
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            //New Security Scheme
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    //Reference set to a SecurityScheme reference type
                    Type=ReferenceType.SecurityScheme,
                    //Reference set to "Bearer"
                    Id="Bearer"
                }
            },
            //This empty array indicates that the security scheme does not require any additional permission, else, we should specify them inside the array
            new string[]{}
        }
    });
});

//Adding NewtonJsonSoft in order to have our comments shown in the stock response
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //Usamos esta linea de codigo para evitar un bucle de referencia de objetos (osea, una referencia de un objeto a si mismo o a uno que ya ha sido serializado), para ignorarlo y evitar errores
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

//Registrating our ApplicationDbContext service, we must declare the services before the builder.Build in order to make work it

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    options.UseSqlServer(connectionString);
});

//Usamos .AddIdentity<TUser, TRole>, siendo TUser la representacion de la clase que usaremos para representar a nuestros usuarios y TRol la representacion para representar los roles dentro de nuestra aplicacion, y a continuacion se usan los options para establecer las distintas politicas de contraseñas, etc.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //Configurando la politica de contraseñas, es decir, el formato admitido
    //La contraseña debera de tener digitos
    options.Password.RequireDigit = true;
    //La contraseña deberá tener letras mayusculas, minisculas y caracteres no alfanumeticos
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    //La contraseña debera tener al menos 12 digitos
    options.Password.RequiredLength = 12;

    //AddEntityFrameworkStores<ApplicationDbContext>() especifica que todo lo configurado anteriormente se guarda en la base de datos asociada a ese DbContext
}).AddEntityFrameworkStores<ApplicationDbContext>();

//Añadiendo los servicios necesarios para habilitar la autenticacion
builder.Services.AddAuthentication(options =>
{
    /*DefaultAuthenticateScheme: Indica el esquema que se usará para autenticar al usuario, es decir, para crear su identidad y ponerla a disposición del marco
    DefaultChallengeScheme: Indica el esquema que se usará para desafiar al usuario, es decir, para solicitarle que se autentique cuando no lo está o cuando no tiene los permisos necesarios
    DefaultForbidScheme: Indica el esquema que se usará para prohibir al usuario, es decir, para rechazar su acceso cuando no tiene los permisos necesarios
    DefaultScheme: Indica el esquema predeterminado que se usará cuando no se especifique otro esquema
    DefaultSignInScheme: Indica el esquema que se usará para iniciar sesión al usuario, es decir, para establecer su identidad y guardarla para futuras solicitudes.
    DefaultSignOutScheme: Indica el esquema que se usará para cerrar sesión al usuario, es decir, para eliminar su identidad y limpiar cualquier dato asociado.*/
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    //Al asignar todas estas opciones al valor de JwtBearerDefaults.AuthenticationScheme, se está indicando que se usará el mismo esquema para todas las acciones de autenticación. Este esquema usa el token JWT que se proporciona como parte del encabezado Authorization en la solicitud para crear, verificar y eliminar la identidad del usuario.

    //Usa .AddJwtBearer para agregar la autenticacion basada en JWT, este metodo requiere haber instalado el paquete Microsoft.AspNetCore.Authentication.JwtBearer
}).AddJwtBearer(options =>
{
    //Accede a TokenValidationParameters, una clase que contiene las opciones para validar los tokens JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //Valida la entidad que genero el token
        ValidateIssuer = true,
        //Indica el valor esperado que debe tener la entidad emisora del token, obtenida en el appsettings.json
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        //Activa la validacion del publico del token
        ValidateAudience = true,
        //Indica el valor esperado que debe tener el publico del token, obtenido en el appsettings.json
        ValidAudience = builder.Configuration["JWT:Audience"],
        //Activa la validacion de la firma del emisor del token, es decir la clave que se uso para encodear el token y garantizar su integridad, es decir, que no haya sido manipulado
        ValidateIssuerSigningKey = true,
        /* Indica la clave de firma del emisor del token,
        que se crea a partir de un valor de bytes que se obtiene de la configuración de la aplicación con la clave “JWT:SigningKey”.*/
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
    };
});

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//Mapping the controllers so the Https Redirection error dont show up
app.MapControllers();

app.Run();


