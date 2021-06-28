using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmallYay.Models
{
    public class UserBottle : BindableObject
    {
        public string guid { get; set; }
        public string owner_guid { get; set; }
        public string rack_guid { get; set; }
        public string rack_name { get; set; }
        public long row { get; set; }
        public long col { get; set; }
        public string bottle_guid { get; set; }
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
        public string where_bought { get; set; }
        public string price_paid { get; set; }
        public int? user_rating { get; set; }
        public DateTime? drink_date { get; set; }
        public DateTime? created_date { get; set; }
        public string user_notes { get; set; }

        public UserBottle()
        {               
        }

        public UserBottle(UserBottleForDisplay ub)
        {
            guid = ub.guid;
            owner_guid = ub.owner_guid;
            rack_guid = ub.rack_guid;
            rack_name = ub.rack_name;
            row = ub.row;
            col = ub.col;
            bottle_guid = ub.bottle_guid;
            Year = ub.Year;
            Vintner = ub.Vintner;
            WineName = ub.WineName;
            Category = ub.Category;
            Varietal = ub.Varietal;
            City_Town = ub.City_Town;
            Region = ub.Region;
            State_Province = ub.State_Province;
            Country = ub.Country;
            ExpertRatings = ub.ExpertRatings;
            Size = ub.Size;
            ABV = ub.ABV;
            WinemakerNotes = ub.WinemakerNotes;
            where_bought = ub.where_bought;
            price_paid = ub.price_paid;
            user_rating = ub.user_rating;
            drink_date = ub.drink_date;
            user_notes = ub.user_notes;
        }
    }
}
