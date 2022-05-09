using FarmCraft.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace FarmCraft.Users.Api.Controllers
{
    [ApiController]
    [Route("api/graph")]
    public class GraphController : ControllerBase
    {
        private static object? DeltaLink = null;
        private static IUserDeltaCollectionPage? LastPage = null;

        [HttpPost("users")]
        //[Produces("text/plain")]
        public async Task<IActionResult> HandleUserChanges([FromQuery]string? validationToken = null)
        {
            // Handle validation send from AzureAD
            if (!string.IsNullOrEmpty(validationToken))
                return Ok(validationToken);

            // Handle the actual notification
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string content = await reader.ReadToEndAsync();
                ChangeNotificationCollection notifications = JsonConvert
                    .DeserializeObject<ChangeNotificationCollection>(content);

                if(notifications != null)
                {
                    foreach(ChangeNotification notification in notifications.Value)
                    {
                        var pause = true;
                        // Do the logic
                    }
                }
            }

            return Ok();
        }
    }
}
