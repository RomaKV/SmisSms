using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EntitySql.Entities
{
   public class GlobalSettings
   {
       [Key]
       [MaxLength(50)]
        public string paramName { get; set; }
      
        [MaxLength(250)]
        public string paramValue { get; set; }

    }
}
