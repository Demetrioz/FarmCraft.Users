using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Core;
using FarmCraft.Users.Core.Config;
using FarmCraft.Users.Data.Context;
using FarmCraft.Users.Data.Repositories.Invitation;
using FarmCraft.Users.Data.Repositories.Organization;
using FarmCraft.Users.Data.Repositories.User;
using Microsoft.EntityFrameworkCore;

public class Program
{
    private static IConfiguration Configuration { get; set; }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                Configuration = config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json")
                    .Build();
            })
            .ConfigureServices((context, services) =>
            {
                //AzureSettings settings = new AzureSettings();
                //Configuration.Bind("AzureSettings", settings);

                AppSettings settings = new AppSettings();
                Configuration.Bind("AppSettings", settings);

                //services.Configure<AzureSettings>(Configuration.GetSection("AzureSettings"));

                services.AddDbContext<IFarmCraftContext, UserContext>(options =>
                    options.UseCosmos(settings.CosmosConnection, settings.CosmosDb));

                services.AddTransient<IUserRepository, UserRepository>();
                services.AddTransient<IOrganizationRepository, OrganizationRepository>();
                services.AddTransient<IInvitationRepository, InvitationRepository>();
                services.AddTransient<ILogService, FarmCraftLogService>();

                services.AddHostedService<UserServiceCore>();
            });
}
