using Application;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Persistence;
using Persistence.Context;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using WebAPI.Infrastructure.Extensions;
using WebAPI.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidationFilter));
}).AddFluentValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
});
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructreServices(builder.Configuration);

IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
{
    {"Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    {"Message_Template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    {"Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    {"TimeStamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
    {"Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    {"Properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
};



var logConnectionString = builder.Configuration.GetConnectionString("SeriLogConnection");
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.PostgreSQL(connectionString: logConnectionString, tableName: "Logs", needAutoCreateTable: true, columnOptions: columnWriters)
    .CreateLogger();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<CAContext>();
    dataContext.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCustomExceptionMiddleware();
app.UseAuthentication();
app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});
app.UseAuthorization();

app.MapControllers();

app.Run();
