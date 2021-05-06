using System.Collections.Generic;
using System.Threading.Tasks;
using HttpClients.Interfaces;
using JsonModels;
using Microsoft.AspNetCore.Mvc;

namespace HostingServiceDB.Controllers
{
    [Produces("application/json")]
    [Route("api/sms")]
    [ApiController]
    public class SmsApiController : Controller, ISmsService
    {
        private readonly ISmsService smsService;

        public SmsApiController(ISmsService smsService)
        {
            this.smsService = smsService;
        }

        [HttpGet("getall/{date}")]
        [ActionName("Get")]
        public async Task<IActionResult> GetAllSmsAsync(string date)
        {
            return await smsService.GetAllSmsAsync(date);
        }

        [HttpGet("getall")]
        [ActionName("Get")]
        public async Task<IActionResult> GetAllSmsAsync()
        {
            
            
            return await smsService.GetAllSmsAsync();
        }

        [HttpPost("update/status")]
        [ActionName("Post")]
        public async Task<IActionResult> UpdateStatusAsync(IEnumerable<SmsResultJsonModel> statusSms)
        {
            return await smsService.UpdateStatusAsync(statusSms);
        }


        [HttpGet()]
        [ActionName("Get")]
        public string Get(string date)
        {
            return GetType().Assembly.GetName().Version?.ToString();
        }
    }
}