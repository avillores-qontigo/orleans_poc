
using Orleans.Runtime;

namespace OrleansURLShortener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Silos
            builder.Host.UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();
                siloBuilder.AddMemoryGrainStorage("urls");
                siloBuilder.UseKubernetesHosting();
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
    

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
