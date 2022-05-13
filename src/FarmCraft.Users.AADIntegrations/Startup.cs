using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using FarmCraft.Users.AADIntegrations.Actors;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(FarmCraft.Users.AADIntegrations.Startup))]

namespace FarmCraft.Users.AADIntegrations
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(provider =>
            {
                var config = ConfigurationFactory.ParseString(@"
                    akka {  
                        actor {
                            provider = remote
                        }
                        remote {
                            dot-netty.tcp {
		                        port = 80
		                        hostname = func-centus-farmcraftaad-dev.azurewebsites.net
                            }
                        }
                    }
                ");

                /*
                var config = ConfigurationFactory.ParseString(@"
                    akka {  
                        actor {
                            provider = remote
                        }
                        remote {
                            dot-netty.tcp {
		                        port = 8080
		                        hostname = localhost
                            }
                        }
                    }
                ");
                */

                var bootstrap = BootstrapSetup.Create().WithConfig(config);
                var di = DependencyResolverSetup.Create(provider);
                var actorSystemSetup = bootstrap.And(di);
                return ActorSystem.Create("AADIntegrations", actorSystemSetup);
            });

            builder.Services.AddSingleton(provider =>
            {
                var actorSystem = provider.GetRequiredService<ActorSystem>();
                return actorSystem.ActorOf(Props.Create(() => new IntegrationActor()), "RootActor");
            });
        }
    }
}
