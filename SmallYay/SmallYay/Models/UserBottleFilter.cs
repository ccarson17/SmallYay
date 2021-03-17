using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmallYay.Models
{
    public class UserBottleFilter
    {
        public string guid { get; set; }
        public string owner_guid { get; set; }
        public string rack_guid { get; set; }
        public string bottle_guid { get; set; }
        public int rack_col { get; set; }
        public int rack_row { get; set; }
        public string rack_name { get; set; }
        public string where_bought { get; set; }
        public string price_paid { get; set; }
        public int? user_rating { get; set; }
        public DateTime? drink_date { get; set; }
        public DateTime? created_date { get; set; }
        public string user_notes { get; set; }
        public string Year { get; set; }
        public string Vintner { get; set; }
        public string WineName { get; set; }
        public string Category { get; set; }
        public string Varietal { get; set; }
        public string City_Town { get; set; }
        public string Region { get; set; }
        public string State_Province { get; set; }
        public string Country { get; set; }
        public string ExpertRatings { get; set; }
        public string Size { get; set; }
        public string ABV { get; set; }
        public string WinemakerNotes { get; set; }
        public string searchQuery { get; set; }
        public string orderBy { get; set; } = "rack_row, rack_col";
        public string getCurrentOrHistory { get; set; }
        public string minYear { get; set; }
        public string maxYear { get; set; }
        public string minPrice { get; set; }
        public string maxPrice { get; set; }
        public int? minRating { get; set; }
        public int? maxRating { get; set; }
    }
}
