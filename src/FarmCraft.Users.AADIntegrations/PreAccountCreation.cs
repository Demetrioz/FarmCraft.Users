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

namespace FarmCraft.Users.AADIntegrations
{
    public static class PreAccountCreation
    {
        [FunctionName("CreateUserDbEntity")]
        public static async Task<IActionResult> CreateUserDbEntity(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PreCreationEntity details = JsonConvert.DeserializeObject<PreCreationEntity>(requestBody);


            log.LogInformation($"Received request for {details.ObjectId}");
            // TODO: Create the user in the DB


            return new OkObjectResult(new ContinueCreationResponse 
            { 
                Version = "1.0.0",
                Action = "Continue"
            });
        }
    }
}
