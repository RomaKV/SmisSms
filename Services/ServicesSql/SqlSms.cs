using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommonHelper;
using EntitySql;
using EntitySql.Entities;
using HttpClients.Interfaces;
using JsonModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ServicesSql
{
    public class SqlSms : ControllerBase, ISmsService
    {
        private readonly MsgSmisContext _context;

        public SqlSms(MsgSmisContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> GetAllSmsAsync(string date)
        {
            return await GetSmsAsync(date);
        }

        public async Task<IActionResult> GetAllSmsAsync()
        {
            return await GetSmsAsync();
        }


        public async Task<IActionResult> UpdateStatusAsync(IEnumerable<SmsResultJsonModel> statusSms)
        {
            List<int> notFindSms = new List<int>();
            try
            {
                bool flgUpdate = false;

                if (statusSms != null)
                {
                    foreach (var sms in statusSms)
                    {
                        var smsFromDb = await _context.JournalSms.SingleOrDefaultAsync(i => i.Id == sms.Id);

                        if (smsFromDb != null)
                        {
                            smsFromDb.Status = sms.State;

                            smsFromDb.DateSent = sms.Date;

                            flgUpdate = true;
                        }
                        else
                        {
                            notFindSms.Add(sms.Id);
                        }
                    }
                }
                else
                {
                    return new ObjectResult("Request contains no data.")
                        {StatusCode = 400};
                }

                if (flgUpdate)
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(
                        $"{HelperMsg.GetAllMessages(ex, $"Error has occurred in {GetType().FullName} UpdateStatusAsync(statusSms)")}")
                    {StatusCode = 500};
            }

            if (notFindSms.Any())
            {
                StringBuilder notFind = new StringBuilder("SMS not found in the Database Id: ");
                foreach (var id in notFindSms)
                {
                    notFind.Append($"{id}, ");
                }

                return new ObjectResult(notFind.ToString().TrimEnd().TrimEnd(','))
                    {StatusCode = 400};
            }


            return new ObjectResult("Statuses have updated.")
                {StatusCode = 200};
        }


        private async Task<IActionResult> GetSmsAsync(string date = null)
        {
            List<SmsJsonModel> result = new List<SmsJsonModel>();

            try
            {
                var smsResendOnFailures =
                    await _context.GlobalSettings.AsNoTracking().SingleOrDefaultAsync(s => s.paramName == "SmsResendOnFailures");

                if (smsResendOnFailures != null &&
                    smsResendOnFailures.paramValue == "true")
                {
                    result.AddRange(await GetSmsForResendAsync());
                }

                var maxId = await _context.JournalSms.MaxAsync(x => (int?) x.EDDSMsgId) ?? 0;

                List<EDDSMsg> eddMsgNew;

                if (string.IsNullOrEmpty(date))
                {
                    eddMsgNew = await _context.EDDSMsg
                        .AsNoTracking()
                        .Where(i => i.idMsg > maxId && (i.idMsgType == 1 || i.idMsgType == 2))
                        .ToListAsync();
                }
                else
                {
                    if (DateTime.TryParse(date, out DateTime dt))
                    {
                        eddMsgNew = await _context.EDDSMsg.AsNoTracking().Where(i =>
                            i.idMsg > maxId && i.DateIn >= dt && (i.idMsgType == 1 || i.idMsgType == 2)).ToListAsync();
                    }
                    else
                    {
                        return new ObjectResult($"The date in request is not in a valid format: {date}")
                            {StatusCode = 400}; // throw new ArgumentException(nameof(date));
                    }
                }


                var phones = await _context.Phone
                    .AsNoTracking()
                    .Include(t => t.PhoneTypeStatusSmis)
                    .ThenInclude(c => c.TypeStatusSmis)
                    .Select(p => new {p.Id, p.PhoneTypeStatusSmis})
                    .ToListAsync();

                List<JournalSms> listSmsNew = new List<JournalSms>();

                foreach (var smsNew in eddMsgNew)
                {
                    var smsXml = GetSmsXml(smsNew.MsgText);

                    if (smsXml == null)
                    {
                        continue;
                    }

                    foreach (var phone in phones)
                    {
                        var status = phone.PhoneTypeStatusSmis.SingleOrDefault(s =>
                            s.TypeStatusSmis.Code == smsXml.IncidentStatus);


                        if (status == null)
                        {
                            continue;
                        }

                        listSmsNew.Add(new JournalSms
                        {
                            Text = GetTextSms(status, smsXml),
                            PhoneId = phone.Id,
                            DateSent = DateTime.Now,
                            EDDSMsgId = smsNew.idMsg,
                            Status = -1
                        });
                    }
                }

                // Save to DB
                if (listSmsNew.Any())
                {
                    await _context.JournalSms.AddRangeAsync(listSmsNew);

                    await _context.SaveChangesAsync();


                    foreach (var sms in listSmsNew)
                    {
                        result.Add(new SmsJsonModel
                        {
                            Id = sms.Id,
                            Message = sms.Text,
                            Phone = sms.Phone.PhoneNumber
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(
                        $"{HelperMsg.GetAllMessages(ex, $"Error has occurred in {GetType().FullName} GetSmsAsync(date)")}")
                    {StatusCode = 500};
            }

            return new ObjectResult(result) {StatusCode = 200};
        }


        private string GetSystemName(string name)
        {
            int start = name.IndexOf('(');
            int end = name.IndexOf(')');

            if (start >= 0 && end > 0 && start + 1 < name.Length)
            {
                int length = end - start - 1;
                if (length > 0)
                {
                    string value = name.Substring(start + 1, length);

                    if (value.Length > 0)
                    {
                        return value;
                    }
                }
            }

            return name;
        }

        private string GetTextSms(PhoneTypeStatusSmis status, SmsXml sms)
        {
            StringBuilder result = new StringBuilder();

            result.Append($"{status.TypeStatusSmis.DisplayName}");
            result.Append(".");
            result.Append($"{GetSystemName(sms.IncidentTypeName)}");
            result.Append(".");
            result.Append($"{sms.Date}");

            return result.ToString();
        }

        private SmsXml GetSmsXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            XDocument xDoc = XDocument.Parse(xml);
            // var mesXml = xdoc.Element("DispatchMessageRequest")?.Elements("Message").FirstOrDefault();
            var mesXml = xDoc.Descendants().FirstOrDefault(el => el.Name == "Message");

            if (mesXml != null)
            {
                SmsXml sms = new SmsXml
                {
                    IncidentTypeName = mesXml.Element("IncidentTypeName")?.Value,
                    IncidentStatus = int.Parse(mesXml.Element("IncidentStatus")?.Value ?? string.Empty),
                    Text = mesXml.Element("Text")?.Value,
                    Date = DateTime.Parse(mesXml.Element("Time")?.Value ?? string.Empty)
                };

                return sms;
            }

            return null;
        }


        private async Task<IEnumerable<SmsJsonModel>> GetSmsForResendAsync()
        {
            List<SmsJsonModel> result = new List<SmsJsonModel>();

            var smsFailed = await _context.JournalSms.Include(p => p.Phone)
                .Where(sms => sms.Status > 0 && sms.DateSent > DateTime.Now.AddHours(-24)).ToListAsync();

            if (smsFailed?.Any() == true)
            {
                foreach (var sms in smsFailed)
                {
                    result.Add(new SmsJsonModel
                    {
                        Id = sms.Id,
                        Phone = sms.Phone.PhoneNumber,
                        Message = sms.Text
                    });
                }
            }

            return result;
        }

        private class SmsXml
        {
            public string IncidentTypeName { get; set; }
            public int IncidentStatus { get; set; }
            public string Text { get; set; }
            public DateTime Date { get; set; }
        }
    }
}