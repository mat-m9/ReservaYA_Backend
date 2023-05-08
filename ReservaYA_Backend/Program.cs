using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReservaYA_Backend;
using ReservaYA_Backend.Models;
using ReservaYA_Backend.Options;
using ReservaYA_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Text.Json.Serialization;
using ReservaYA_Backend.ResponseModels;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(key: nameof(jwtSettings), jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddTransient<PasswordGeneratorService>();
builder.Services.AddSingleton<DefaultRolesService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.ASCII.GetBytes(jwtSettings.Secret)),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = true,
    ValidateLifetime = true
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(configureOptions: options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
        //options.TokenValidationParameters.NameClaimType = "sub";
        //options.TokenValidationParameters.RoleClaimType = "role";
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.Headers.TryAdd("Token-Expired", "True");
                    IdentityService identityService;
                    ResponseModel response = new()
                    {
                        Tipo = "Unauthorized",
                        Message = "Token caducado",

                    };
                    string respuestaSerializada = EncodingHelper.JsonSpanishSerializer(response, response.GetType());
                    return context.Response.WriteAsync(respuestaSerializada);
                }
                else if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenUnableToValidateException))
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.Headers.TryAdd("Token-Invalid", "True");
                    ResponseModel response = new()
                    {
                        Tipo = "Unauthorized",
                        Message = "Token Inválido"
                    };
                    string respuestaSerializada = EncodingHelper.JsonSpanishSerializer(response, response.GetType());
                    return context.Response.WriteAsync(respuestaSerializada);
                }
                return Task.CompletedTask;
            }
        };
    });




var security = new Dictionary<string, IEnumerable<string>>
        {
            {"Bearer", new string[0] }
        };


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ReservaYa!!!", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



PopulateDB.Initialize(app.Services);

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
