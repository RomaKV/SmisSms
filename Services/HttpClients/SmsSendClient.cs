using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClients.Interfaces;
using JsonModels;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace HttpClients
{
    public class SmsSendClient : BaseClient, ISmsService
    {
        public SmsSendClient(string baseAddress) : base(baseAddress)
        {
            ServiceAddress = "api/sms";
        }

        protected override string ServiceAddress { get; }


        public async Task<IActionResult> GetAllSmsAsync(string date)
        {
           return await GetAsync<IEnumerable<SmsJsonModel>>($"{ServiceAddress}/getall/{date}");

        }


        public async Task<IActionResult> GetAllSmsAsync()
        {
            return await GetAsync<IEnumerable<SmsJsonModel>>($"{ServiceAddress}/getall");
        }

        public async Task<IActionResult> UpdateStatusAsync(IEnumerable<SmsResultJsonModel> statusSms)
        {
            return await PostAsync($"{ServiceAddress}/update/status", statusSms);
        }
    }
}
