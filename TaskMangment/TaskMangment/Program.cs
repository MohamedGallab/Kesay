using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskMangment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

var tasksGroup = app.MapGroup("/api/tasks");

tasksGroup.MapGet("/", async (TaskContext context) =>
    await context.Tasks.ToListAsync());

tasksGroup.MapGet("/{id:int}", async (int id, TaskContext context) =>
    await context.Tasks.FindAsync(id) is MyTask task
    ? Results.Ok(task)
    : Results.NotFound());

tasksGroup.MapPost("/", async (MyTask task, TaskContext context) =>
{
    context.Tasks.Add(task);
    await context.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
}).RequireAuthorization();

tasksGroup.MapPut("/{id:int}", async (int id, MyTask inputTask, TaskContext context) =>
{
    var task = await context.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Title = inputTask.Title;
    task.Description = inputTask.Description;
    task.IsCompleted = inputTask.IsCompleted;

    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

tasksGroup.MapDelete("/{id:int}", async (int id, TaskContext context) =>
{
    if (await context.Tasks.FindAsync(id) is MyTask task)
    {
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return Results.Ok(task);
    }

    return Results.NotFound();
}).RequireAuthorization();

app.MapPost("/api/auth/register", async (RegisterModel model, UserManager<IdentityUser> userManager) =>
{
    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await userManager.CreateAsync(user, model.Password);

    return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
});

app.MapPost("/api/auth/login", async (LoginModel model, SignInManager<IdentityUser> signInManager, IConfiguration configuration) =>
{
    var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

    if (result.Succeeded)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(model.Email);
        var tokenString = GenerateJSONWebToken(user, configuration);
        return Results.Ok(new { token = tokenString });
    }

    return Results.Unauthorized();
});

app.Run();

string GenerateJSONWebToken(IdentityUser user, IConfiguration configuration)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
      configuration["Jwt:Issuer"],
      null,
      expires: DateTime.Now.AddMinutes(120),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}