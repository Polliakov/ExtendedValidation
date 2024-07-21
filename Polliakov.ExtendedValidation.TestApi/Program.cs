
using Polliakov.ExtendedValidation.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Polliakov.ExtendedValidation.TestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services
                .AddControllers()
                .AddJsonOptions(json =>
                {
                    json.JsonSerializerOptions.TypeInfoResolver =
                        JsonTypeInfoResolvers.DissalowMissingFields();
                    json.AllowInputFormatterExceptionMessages = false;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
