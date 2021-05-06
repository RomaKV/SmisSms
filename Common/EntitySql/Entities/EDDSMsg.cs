using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntitySql.Entities
{
   public class EDDSMsg
   {
       public EDDSMsg()
       {
           JournalSmses = new HashSet<JournalSms>();
        }


       [Key]
      public int idMsg { get; set; }

      public DateTime? DateIn { get; set; }

      public DateTime? DateSent { get; set; }

      public  int? idMsgType { get; set; }

      public string MsgText { get; set; }

      public string ResponceText { get; set; }

      public  int? Confirmed { get; set; }

      public Guid? MsgGUID { get; set; }

      public int? RuleIdLine { get; set; }

      public int? RuleResultId { get; set; }

      public int? allowSend { get; set; }

      public virtual ICollection<JournalSms> JournalSmses { get; set; }
   }
}
