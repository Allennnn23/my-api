using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_api.Data;
using my_api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// START: ADD EF CORE AND POSTGRESQL CONFIGURATION
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // 1. Get the connection string named "DefaultConnection"
    // 2. Tell the DbContext to use the Npgsql (PostgreSQL) provider
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// END: ADD EF CORE AND POSTGRESQL CONFIGURATION

// Add CORS service: Define a policy to allow Angular's development origin
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAngularDev",
                      policy =>
                      {
                          // **Allow requests from the Angular development server (http://localhost:4200)**
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()    // Allow all headers (e.g., Content-Type)
                                .AllowAnyMethod();   // Allow all HTTP methods (GET, POST, etc.)
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// **Crucial: Add the CORS middleware BEFORE any endpoint definitions**
app.UseCors("AllowAngularDev");

app.UseHttpsRedirection();

// 1. Endpoint to GET all users
app.MapGet("/users", async ([FromServices] AppDbContext db) =>
{
    // Use the AppDbContext to query the Users table
    return await db.Users.ToListAsync();
})
.WithName("GetUsers");

// 2. Endpoint to POST a new user
app.MapPost("/users", async ([FromBody] User user, [FromServices] AppDbContext db) =>
{
    // Check if a user with the same email already exists (simple validation)
    if (await db.Users.AnyAsync(u => u.Email == user.Email))
    {
        return Results.Conflict("A user with this email already exists.");
    }

    // Add the new user object to the DbContext
    db.Users.Add(user);

    // Save changes to the PostgreSQL database
    await db.SaveChangesAsync();

    // Return the newly created user with a 201 Created status
    return Results.Created($"/users/{user.Id}", user);
})
.WithName("CreateUser");

// END: ADD NEW API ENDPOINTS

app.Run();
