using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JsonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace HttpClients.Interfaces
{
   public interface ISmsService
   {
       Task<IActionResult> GetAllSmsAsync(string date);
       Task<IActionResult> GetAllSmsAsync();
       Task<IActionResult> UpdateStatusAsync(IEnumerable<SmsResultJsonModel> statusSms);
    }
}
