

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntitySql.Entities
{
    public class Phone
    {

        public Phone()
        {

            JournalSmses = new HashSet<JournalSms>();

            PhoneTypeStatusSmis = new HashSet<PhoneTypeStatusSmis>();

        }


        [Key]
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        
        public int UserMsgToId { get; set; }
        
        public virtual UserMsgTo UserMsgTo { get; set; }

        public virtual ICollection<JournalSms> JournalSmses { get; set; }

        public virtual ICollection<PhoneTypeStatusSmis> PhoneTypeStatusSmis { get; set; }

    }
}
