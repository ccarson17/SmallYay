using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmallYay.Models
{
    class UserBottleForEzCreation
    {
        [MaxLength(128)]
        public string owner_guid { get; set; }
        [MaxLength(128)]
        public string rack_guid { get; set; }
        [MaxLength(128)]
        public string bottle_guid { get; set; }
        public string rack_name { get; set; }
        public long rack_col { get; set; }
        public long rack_row { get; set; }
        public string where_bought { get; set; }
        public string price_paid { get; set; }
        public int? user_rating { get; set; }
        public string user_notes { get; set; }
        public int? Year { get; set; }
        [MaxLength(128)]
        public string Vintner { get; set; }
        [MaxLength(128)]
        public string WineName { get; set; }
        [MaxLength(128)]
        public string Category { get; set; }
        [MaxLength(128)]
        public string Varietal { get; set; }
        public string City_Town { get; set; }
        public string Region { get; set; }
        public string State_Province { get; set; }
        public string Country { get; set; }
        public string ExpertRatings { get; set; }
        public int SizeInML { get; set; }
        public Decimal? ABV { get; set; }
        public string WinemakerNotes { get; set; }
    }
}
