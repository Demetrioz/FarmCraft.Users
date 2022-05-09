using Akka.Actor;
using Akka.TestKit.NUnit;
using FarmCraft.Core.Data.Context;
using FarmCraft.Core.Data.DTOs;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Services.Logging;
using FarmCraft.Users.Core.Actors;
using FarmCraft.Users.Data.Context;
using FarmCraft.Users.Data.Entities;
using FarmCraft.Users.Data.Messages.User;
using FarmCraft.Users.Data.Repositories.Organization;
using FarmCraft.Users.Data.Repositories.User;
using FarmCraft.Users.Tests.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmCraft.Users.Tests.ActorTests
{
    [TestFixture]
    public class UserTests : TestKit
    {
        private IServiceProvider? _serviceProvider { get; set; }
        private string? _orgOwnerId { get; set; }
        private string? _invitedUserId { get; set; }
        private string? _organizationName { get; set; }
        private Organization? _organization { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            TestSettings settings = new TestSettings();
            config.Bind("TestSettings", settings);

            _serviceProvider = new ServiceCollection()
                .AddDbContext<IFarmCraftContext, UserContext>(options =>
                    options.UseCosmos(settings.CosmosConnection, settings.CosmosDb))
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ILogService, FarmCraftLogService>()
                .BuildServiceProvider();

            _orgOwnerId = Guid.NewGuid().ToString();
            _invitedUserId = Guid.NewGuid().ToString();
            _organizationName = "Test Organization";
        }

        [SetUp]
        public void Setup()
        {
            if (_serviceProvider == null)
                Assert.Fail("Null Service Provider");
        }


        [Test]
        public async Task CanCreateContext()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                UserContext dbContext = scope.ServiceProvider
                    .GetRequiredService<UserContext>();

                await dbContext.Database.EnsureCreatedAsync();
            }

            Assert.Pass();
        }

        [Test]
        public void UserCanRegister()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IUserRepository userRepo = scope.ServiceProvider
                    .GetRequiredService<IUserRepository>();
                IOrganizationRepository orgRepo = scope.ServiceProvider
                    .GetRequiredService<IOrganizationRepository>();
                ILogService logger = scope.ServiceProvider
                    .GetRequiredService<ILogService>();

                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor(userRepo, orgRepo, logger)));

                if (string.IsNullOrEmpty(_orgOwnerId) || string.IsNullOrEmpty(_organizationName))
                    Assert.Fail("Owner Id or Orginization Name not set");

                userActor.Tell(new AskToRegisterUser(_orgOwnerId, _organizationName));

                FarmCraftActorResponse response = ExpectMsg<FarmCraftActorResponse>(
                    TimeSpan.FromSeconds(1));

                Assert.True(response != null);
                Assert.AreEqual(response?.Status, ResponseStatus.Success);
                
                User? newUser = response?.Data as User;
                Assert.IsNotNull(newUser);

                PartialOrganization? newUserOrg = newUser?.Organizations?.FirstOrDefault();
                Assert.IsNotNull(newUserOrg);
                Assert.IsNotNull(newUserOrg?.Id);

                Assert.AreEqual(newUser?.AzureId, _orgOwnerId);
                Assert.AreEqual(newUser?.Preferences.AlertPreference, AlertPreference.Email);
                Assert.AreEqual(newUser?.Organizations.Count, 1);
                Assert.AreEqual(newUserOrg?.Name, _organizationName);
                Assert.AreEqual(newUserOrg?.Role, Role.Owner);

                UserContext dbContext = scope.ServiceProvider
                    .GetRequiredService<UserContext>();

                Organization? org = dbContext.Organizations
                    .AsNoTracking()
                    .Where(o => 
                        o.OrganizationId == newUserOrg.Id
                        && o.Name == _organizationName
                    )
                    .FirstOrDefault();

                Assert.IsNotNull(org);
                Assert.AreEqual(org?.OwnerId, newUser?.AzureId);

                _organization = org;
            }
        }

        [Test]
        public async Task WillNotCreateDuplicateUsers()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor()));

                userActor.Tell(new AskToCreateUser());
            }
        }

        [Test]
        public async Task UserCanBeInvited()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor()));

                userActor.Tell(new AskToInviteUser());
            }
        }

        [Test]
        public async Task InvitedUserCanRegister()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor()));

                userActor.Tell(new AskToCreateUser());
            }
        }
    }
}
