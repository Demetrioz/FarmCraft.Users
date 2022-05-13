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
using FarmCraft.Users.Data.Repositories.Invitation;
using FarmCraft.Users.Data.Repositories.Organization;
using FarmCraft.Users.Data.Repositories.User;
using FarmCraft.Users.Tests.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FarmCraft.Users.Tests.ActorTests
{
    [TestFixture]
    public class UserTests : TestKit
    {
        private readonly string _orgOwnerId = Guid.NewGuid().ToString();
        private readonly string _invitedUserId = Guid.NewGuid().ToString();
        private readonly string _organizationName = "Test Organization";
        private readonly string _inviteEmail = "test@test.com";
        private readonly Role _inviteRole = Role.Member;

        private IServiceProvider? _serviceProvider { get; set; }
        private Organization? _organization { get; set; }
        private string? _inviteId { get; set; }

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
                .AddTransient<IOrganizationRepository, OrganizationRepository>()
                .AddTransient<IInvitationRepository, InvitationRepository>()
                .AddTransient<ILogService, FarmCraftLogService>()
                .BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                UserContext dbContext = scope.ServiceProvider
                    .GetRequiredService<UserContext>();

                List<User> users = dbContext.Users.ToList();
                List<Invitation> invitations = dbContext.Invitations.ToList();
                List<Organization> organizations = dbContext.Organizations.ToList();

                dbContext.RemoveRange(users);
                dbContext.RemoveRange(invitations);
                dbContext.RemoveRange(organizations);

                dbContext.SaveChanges();
            }
        }

        [SetUp]
        public void Setup()
        {
            if (_serviceProvider == null)
                Assert.Fail("Null Service Provider");
        }


        [Test]
        [Order(100)]
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
        [Order(101)]
        public void UserCanRegister()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor(_serviceProvider)));

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
        [Order(102)]
        public void WillNotRegisterDuplicateUsers()
        {
            IActorRef userActor = Sys.ActorOf(
                Props.Create(() => new UserActor(_serviceProvider)));

            userActor.Tell(new AskToRegisterUser(_orgOwnerId, _organizationName));

            FarmCraftActorResponse response = ExpectMsg<FarmCraftActorResponse>(
                TimeSpan.FromSeconds(1));

            Assert.True(response != null);
            Assert.AreEqual(response?.Status, ResponseStatus.Failure);
            Assert.IsNotNull(response?.Error);
        }

        [Test]
        [Order(103)]
        public void UserCanBeInvited()
        {
            IActorRef userActor = Sys.ActorOf(
                Props.Create(() => new UserActor(_serviceProvider)));

            userActor.Tell(new AskToInviteUser(
                _orgOwnerId, 
                _organization.OrganizationId, 
                _inviteEmail, 
                _inviteRole
            ));

            FarmCraftActorResponse response = ExpectMsg<FarmCraftActorResponse>(
                TimeSpan.FromSeconds(1));

            Assert.True(response != null);
            Assert.AreEqual(response?.Status, ResponseStatus.Success);

            Invitation invite = response?.Data as Invitation;
            Assert.IsNotNull(invite);
            Assert.AreEqual(invite?.InvitedBy, _orgOwnerId);
            Assert.AreEqual(invite?.Email, _inviteEmail);
            Assert.AreEqual(invite?.RegistrationStatus, RegistrationStatus.Invited);
            Assert.AreEqual(invite?.Role, _inviteRole);

            _inviteId = invite?.InvitationId;
        }

        [Test]
        [Order(104)]
        public void InvitedUserCanRegister()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IActorRef userActor = Sys.ActorOf(
                    Props.Create(() => new UserActor(_serviceProvider)));

                userActor.Tell(
                    new AskToRegisterUser(
                        _invitedUserId, 
                        _organizationName, 
                        _inviteId
                    ));

                FarmCraftActorResponse response = ExpectMsg<FarmCraftActorResponse>(
                    TimeSpan.FromSeconds(1));

                Assert.True(response != null);
                Assert.AreEqual(response?.Status, ResponseStatus.Success);

                User? newUser = response?.Data as User;
                Assert.IsNotNull(newUser);

                PartialOrganization? newUserOrg = newUser?.Organizations?.FirstOrDefault();
                Assert.IsNotNull(newUserOrg);
                Assert.IsNotNull(newUserOrg?.Id);

                Assert.AreEqual(newUser?.AzureId, _invitedUserId);
                Assert.AreEqual(newUser?.Preferences.AlertPreference, AlertPreference.Email);
                Assert.AreEqual(newUser?.Organizations.Count, 1);
                Assert.AreEqual(newUserOrg?.Name, _organizationName);
                Assert.AreEqual(newUserOrg?.Role, _inviteRole);

                UserContext dbContext = scope.ServiceProvider
                    .GetRequiredService<UserContext>();

                Invitation? invitation = dbContext.Invitations
                    .FirstOrDefault();

                Assert.IsNotNull(invitation);
                Assert.AreEqual(invitation?.RegistrationStatus, RegistrationStatus.Registered);
            }
        }
    }
}
