
using Orleans.Runtime;

namespace OrleansURLShortener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Build configuration
            builder.Configuration.AddEnvironmentVariables();

            // Configure Silos
            builder.Host.UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();
                siloBuilder.UseKubernetesHosting();
                siloBuilder.AddCosmosGrainStorage(
                    name: "cosmosStore",
                    configureOptions: static options =>
                    {
                        options.IsResourceCreationEnabled = true;
                        options.DatabaseName = "state";
                        options.ContainerName = "urls";
                        options.PartitionKeyPath = "/PartitionKey";
                        options.ConfigureCosmosClient("<azure-cosmos-db-nosql-connection-string>");
                    });
            });

            // Add services to the container.

            builder.Services.AddControllers();
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
