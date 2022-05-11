using Akka.Actor;
using FarmCraft.Users.Core.Config;
using FarmCraft.Users.Core.Messages.Graph;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace FarmCraft.Users.Core.Actors
{
    public class GraphManager : ReceiveActor
    {
        private readonly GraphServiceClient _graphClient;
        private AzureSettings _settings;
        private ConcurrentDictionary<string, Subscription> _subscriptions;

        // Use EventHub instead of an API endpoint
        // https://devblogs.microsoft.com/microsoft365dev/get-microsoft-graph-change-notifications-delivered-through-azure-event-hubs/
        // https://docs.microsoft.com/en-us/graph/change-notifications-delivery

        public GraphManager(IOptions<AzureSettings> options)
        {
            _graphClient = CreateGraphClient();
            _settings = options.Value;
            _subscriptions = new ConcurrentDictionary<string, Subscription>();

            Receive<CheckForGraphSubscriptionExpiration>(message => HandleSubscriptionCheck(message));
        }

        protected override void PreStart()
        {
            RequestSubscription("created");
            RequestSubscription("updated");

            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.FromMinutes(30),
                    TimeSpan.FromMinutes(30),
                    Self,
                    new CheckForGraphSubscriptionExpiration(),
                    Self
                );
        }

        private void HandleSubscriptionCheck(CheckForGraphSubscriptionExpiration message)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset renewalCutoff = now.AddMinutes(90);
            List<KeyValuePair<string, Subscription>> expiringSubscriptions = _subscriptions.Where(kvp =>
                kvp.Value.ExpirationDateTime >= now
                && kvp.Value.ExpirationDateTime <= renewalCutoff
            ).ToList();

            foreach (KeyValuePair<string, Subscription> kvp in expiringSubscriptions)
                RequestSubscription(kvp.Value.ChangeType, kvp.Key);
        }

        private GraphServiceClient CreateGraphClient()
        {
            GraphServiceClient client = new GraphServiceClient(
                new DelegateAuthenticationProvider(requestMessage =>
                {
                    var accessToken = GetAccessToken().Result;
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.FromResult(0);
                }));

            return client;
        }

        private async Task<string> GetAccessToken()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(_settings.AppId)
                .WithClientSecret(_settings.GraphSubscriptionSecret)
                .WithAuthority($"https://login.microsoft.com/{_settings.TenantId}")
                .WithRedirectUri("https://daemon")
                .Build();

            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        private void RequestSubscription(string changeType, string? id = null)
        {
            Subscription sub = new Subscription();
            Subscription newSub = new Subscription();

            if(id != null)
            {
                sub.ExpirationDateTime = DateTime.UtcNow.AddHours(24);

                newSub = _graphClient.Subscriptions[id]
                    .Request()
                    .UpdateAsync(sub)
                    .Result;
            }
            else
            {
                sub.ChangeType = changeType;
                sub.NotificationUrl = "https://5b94-2604-2d80-de86-400-e8a8-1fb7-d4b3-424.ngrok.io/api/graph/users";
                sub.Resource = "/users";
                sub.ExpirationDateTime = DateTime.UtcNow.AddHours(24);
                sub.ClientState = "SecretClientState";

                newSub = _graphClient.Subscriptions
                    .Request()
                    .AddAsync(sub)
                    .Result;
            }

            _subscriptions.AddOrUpdate(
                id ?? newSub.Id,
                newSub,
                (id, updatedSub) => updatedSub
            );
        }
    }
}
