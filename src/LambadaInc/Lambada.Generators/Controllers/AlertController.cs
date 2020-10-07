using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Lambada.Generators.Hubs;
using Lambada.Generators.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SubscriptionValidationEventData = Lambada.Generators.ViewModels.SubscriptionValidationEventData;

namespace Lambada.Generators.Controllers
{
    //[Produces("application/json")]
    [Route("notification")]
    public class AlertController : Controller
    {
        private readonly IHubContext<AlertHub> hubContext;

        public AlertController(IHubContext<AlertHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost]
        [Route("alert")]
        public async Task<IActionResult> Add([FromBody]string receivedEvent)
        {
            var eventGridEvents = JsonConvert.DeserializeObject<EventGridEvent[]>(receivedEvent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                var dataObject = eventGridEvent.Data as JObject;

                if (string.Equals(eventGridEvent.EventType, "Microsoft.EventGrid.SubscriptionValidationEvent",
                                  StringComparison.OrdinalIgnoreCase))
                {
                    var eventData = dataObject?.ToObject<SubscriptionValidationEventData>();

                    var responseData =
                        new SubscriptionValidationResponseData { ValidationResponse = eventData?.ValidationCode };
                    return Ok(responseData);
                }

                //TODO: store URL of the pic and show it later on - display only message
                //LIMIT this to user
                await hubContext.Clients.All.SendAsync("alertMessage", "1 message");
            }
            
            return Ok();
        }
    }
}