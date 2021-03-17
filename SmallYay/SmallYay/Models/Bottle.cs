using System;
using System.Collections.Generic;
using System.Text;

namespace SmallYay.Models
{
    class Bottle
    {
        public string guid { get; set; }
        public string owner_guid { get; set; }
        public int? Year { get; set; }
        public string Vintner { get; set; }
        public string WineName { get; set; }
        public string Category { get; set; }
        public string Varietal { get; set; }
        public string City_Town { get; set; }
        public string Region { get; set; }
        public string State_Province { get; set; }
        public string Country { get; set; }
        public string ExpertRatings { get; set; }
        public int SizeInML { get; set; }
        public string ABV { get; set; }
        public string WinemakerNotes { get; set; }
    }

    class BottleReturnFromApi
    {
        public string guid { get; set; }
        public int? Year { get; set; }
        public string Vintner { get; set; }
        public string WineName { get; set; }
        public string Category { get; set; }
        public string Varietal { get; set; }
        public string City_Town { get; set; }
        public string Region { get; set; }
        public string State_Province { get; set; }
        public string Country { get; set; }
        public string ExpertRatings { get; set; }
        public string size { get; set; }
        public string ABV { get; set; }
        public string WinemakerNotes { get; set; }
    }
}
