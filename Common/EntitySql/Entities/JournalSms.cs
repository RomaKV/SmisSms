using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EntitySql.Entities
{
   public class JournalSms
   {
       [Key]
       public int Id { get; set; }

       public int PhoneId { get; set; }

       public int Status { get; set; }

       public int EDDSMsgId { get; set; }

       public virtual Phone Phone { get; set; }

       public EDDSMsg EDDSMsg { get; set; }

       public string Text { get; set; }

       public virtual DateTime DateSent { get; set; }

   }
}
