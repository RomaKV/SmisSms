using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntitySql.Entities
{
   public class UserMsgTo
   {
       public UserMsgTo()
        {
            Phones = new HashSet<Phone>();
        }


       [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<Phone> Phones { get; set; }
   }
}
