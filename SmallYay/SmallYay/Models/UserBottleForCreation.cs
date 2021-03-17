using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmallYay.Models
{
    class UserBottleForCreation
    {
        [MaxLength(128)]
        public string owner_guid { get; set; }
        [MaxLength(128)]
        public string rack_guid { get; set; }
        [MaxLength(128)]
        public string rack_name { get; set; }
        [MaxLength(128)]
        public string bottle_guid { get; set; }
        public long rack_col { get; set; }
        public long rack_row { get; set; }
        public string where_bought { get; set; }
        public string price_paid { get; set; }
        public int? user_rating { get; set; }
        public DateTime? drink_date { get; set; }
        public DateTime? created_date { get; set; }
        public string user_notes { get; set; }
    }
}
