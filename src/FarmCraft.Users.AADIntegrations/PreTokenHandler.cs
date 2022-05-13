using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FarmCraft.Users.AADIntegrations.Requests;
using FarmCraft.Users.AADIntegrations.Responses;
using Akka.Actor;
using FarmCraft.Users.Data.Messages.User;
using FarmCraft.Core.Messages;
using FarmCraft.Core.Data.DTOs;

namespace FarmCraft.Users.AADIntegrations
{
    public class PreTokenHandler
    {
        private readonly IActorRef _rootActor;

        public PreTokenHandler(IActorRef rootActor)
        {
            _rootActor = rootActor;
        }

        [FunctionName("CreateUserDbEntity")]
        public async Task<IActionResult> CreateUserDbEntity(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PreTokenEntity details = JsonConvert.DeserializeObject<PreTokenEntity>(requestBody);

            var searchResponse = await _rootActor.Ask(
                new AskForUser(details.ObjectId),
                TimeSpan.FromSeconds(10)
            ) as FarmCraftActorResponse;

            if(searchResponse != null && searchResponse.Data == null)
            {
                FarmCraftActorResponse createResponse = await _rootActor.Ask(
                    new AskToRegisterUser(details.ObjectId, ""),
                    TimeSpan.FromSeconds(10)
                ) as FarmCraftActorResponse;

                if (createResponse != null && createResponse.Status == ResponseStatus.Success)
                    log.Log(LogLevel.Information, $"Created entity for user {details.ObjectId}");
                else
                    log.Log(LogLevel.Error, $"Unable to create entity for user {details.ObjectId}");
            }

            return new OkObjectResult(new ContinueCreationResponse 
            { 
                Version = "1.0.0",
                Action = "Continue"
            });
        }
    }
}
