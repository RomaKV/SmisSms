using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JsonModels;
using Models;

namespace HttpClients.Interfaces
{
   public interface ISmsDbService
   {
       Task<IEnumerable<SmsJsonModel>> GetAllSmsAsync(string date);
       Task<IEnumerable<SmsJsonModel>> GetAllSmsAsync();
       Task<bool> UpdateStatusAsync(IEnumerable<SmsResultJsonModel> statusSms);
    }
}
