using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EntitySql.Entities
{
    public class TypeStatusSmis
    {
        public TypeStatusSmis()
        {
            PhoneTypeStatusSmis = new HashSet<PhoneTypeStatusSmis>();
        }
        

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int Code { get; set; }

        public int EscalationCode { get; set; }

        public virtual ICollection<PhoneTypeStatusSmis> PhoneTypeStatusSmis { get; set; }

    }
}
