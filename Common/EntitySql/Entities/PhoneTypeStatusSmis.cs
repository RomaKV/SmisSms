using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text;

namespace EntitySql.Entities
{
    public class PhoneTypeStatusSmis
    {
     
        public int PhoneId { get; set; }

        public Phone Phone { get; set; }

        public int TypeStatusSmisId { get; set; }

        public TypeStatusSmis TypeStatusSmis { get; set; }

    }
}
