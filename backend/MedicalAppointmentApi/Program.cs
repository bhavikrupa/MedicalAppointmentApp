using Supabase;
using MedicalAppointmentApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Medical Appointment API",
        Version = "v1",
        Description = "API for managing medical appointments, patients, doctors, and billing"
    });
});

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL not configured");
var supabaseKey = builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key not configured");

var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};

builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, options));

// Register application services
builder.Services.AddScoped<ISupabaseService, SupabaseService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Appointment API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Use CORS before HTTPS redirection to handle preflight requests
app.UseCors("AllowAngularApp");

// Only use HTTPS redirection in production to avoid CORS preflight issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();
