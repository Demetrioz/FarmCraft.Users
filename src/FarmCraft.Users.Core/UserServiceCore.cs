using Akka.Configuration;
using FarmCraft.Core.Actors;

namespace FarmCraft.Users.Core
{
    public class UserServiceCore : FarmCraftCore<UserServiceManager>
    {
        public UserServiceCore(IServiceProvider provider) : base(provider)
        {
        }

        protected override Akka.Configuration.Config BuildActorConfig()
        {
            // https://medium.com/@jotarios/ngrok-secure-tunnels-local-dead8685bd71
            //ngrok authtoken TOKEN
            //ngrok tcp port => ngrok tcp 1234

            return ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                            port = 8082
                            hostname = 0.0.0.0
                            public-hostname = 4.tcp.ngrok.io
                        }
                    }
                }
            ");
            /*
            return ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                            port = 8082
                            hostname = 0.0.0.0
                            public-hostname = localhost
                        }
                    }
                }
            ");
            */
        }
    }
}