using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string jwt1secret = "aV3ryS3cur3K3yTh4tIsAtLe4st32Byt3sLng";
const string Jwt2Secret = "ab3ryS3cur3K3yTh4tIsAtLe4st32Byt3sLng";
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(op =>
    {
        op.DefaultScheme = "Jwt1";
        op.DefaultAuthenticateScheme = "Jwt2";
        // op.DefaultChallengeScheme = "oidc";
        // op.DefaultSignInScheme = "cookiessss";
    })
    // .AddCookie("cookiessss")
    // .AddOpenIdConnect("oidc", options =>
    //     {
    //         options.Authority = "https://login.microsoftonline.com/06e2775e-9d3d-49de-ad36-da82e295fa67/v2.0";
    //         options.ClientId = "2c166b98-5c04-4ac9-b7b6-4301f508ee4c";
    //         options.ClientSecret = "Dbu8Q~-QC3ZP9evheCwgXx~-XK2YbNqtCINJXbu8";
    //         options.ResponseType = "code";
    //         
    //         
    //         options.SaveTokens = true;
    //         
    //         options.CallbackPath="/callback";
    //     })
    .AddJwtBearer("Jwt1", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "myapi.com",
            ValidAudience = "myapi",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt1secret))
        };
    })
    .AddJwtBearer("Jwt2",options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "myapi.com",
            ValidAudience = "myap2",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt2Secret))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("Login", (HttpContext httpContext) =>
{
    return Results.Challenge(new AuthenticationProperties()
    {
       RedirectUri = "callback",
       
    }, new List<string>(){"oidc"});

});

app.Map("/callback", (HttpContext httpContext) =>
{
    var result = httpContext.AuthenticateAsync("oidc").Result;
    var token = result.Properties.GetTokenValue("access_token");
    return Results.Ok(token);
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("Onlyjwt1", TokenHandler).RequireAuthorization(new AuthorizeAttribute(){AuthenticationSchemes="Jwt1"});

app.MapGet("Onlyjwt2", TokenHandler).RequireAuthorization(new AuthorizeAttribute(){AuthenticationSchemes="Jwt2"});
app.MapGet("LoginWithJwt1", (HttpContext context) =>
{
    var token= GetBearerToken(jwtSecretKey: jwt1secret, schemeName: "Jwt1",  "myapi");
    return token;
}).AllowAnonymous();

app.MapGet("LoginWithJwt2", (HttpContext context) =>
{
    var token= GetBearerToken(jwtSecretKey: Jwt2Secret, schemeName: "Jwt2",  "myap2");
    return token;
}).AllowAnonymous();

app.Run();

static Results<Ok<string>, NotFound> TokenHandler(HttpContext context)
{
    var scheme = context.User.Claims.FirstOrDefault(i => i.Type == "scheme");

    if (scheme is null || string.IsNullOrWhiteSpace(scheme.Value))
        return TypedResults.NotFound();

    return TypedResults.Ok(scheme.Value);
}

string GetBearerToken(string jwtSecretKey,  string schemeName,string audience)
{
    // Convert the secret key to a byte array
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);

    // Ensure the secret key is of a valid length for HMACSHA256 (at least 256 bits/32 bytes)
    if (keyBytes.Length < 32)
    {
        throw new ArgumentException("Secret key must be at least 256 bits (32 bytes) in length.");
    }

    // Create the security key
    SymmetricSecurityKey securityKey = new SymmetricSecurityKey(keyBytes);

    // Create signing credentials using the security key and HMACSHA256 algorithm
    SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    

    // Define token descriptor
    JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
        issuer: "myapi.com",
        audience: audience,
        claims: new[]
        {
            new Claim("scheme", schemeName),
            new Claim("username", "Anish"),
            new Claim("roles", "Admin"),
            new Claim("roles", "reader"),
            new Claim("roles", "writer"),
        },
        expires: DateTime.Now.AddMinutes(5),
        signingCredentials: credentials);

    // Generate the token
    string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    return token;
}
