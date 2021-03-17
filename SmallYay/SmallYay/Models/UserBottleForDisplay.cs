using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SmallYay.Models
{
    public class UserBottleForDisplay : BindableObject
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
        public string user_notes { get; set; }
        public string location_display { get; set; }
        public string winename_display { get; set; }
        public string vintner_winename_display { get; set; }
        public string rack_display { get; set; }
        public string position_display { get; set; }
        public string rack_position_display { get; set; }
        public string user_rating_display { get; set; }
        public string user_rating_display_nolabel { get; set; }
        public string bottle_color { get; set; }
        public string star1 { get; set; }
        public string star2 { get; set; }
        public string star3 { get; set; }
        public string star4 { get; set; }
        public string star5 { get; set; }
        public string starOpacity { get; set; }

        public UserBottleForDisplay(UserBottle bottle)
        {
            guid = bottle.guid;
            owner_guid = bottle.owner_guid;
            rack_guid = bottle.rack_guid;
            rack_name = bottle.rack_name;
            row = bottle.row;
            col = bottle.col;
            bottle_guid = bottle.bottle_guid;
            Year = bottle.Year == "0" ? "Non-Vintage" : bottle.Year;
            Vintner = bottle.Vintner;
            WineName = bottle.WineName;
            Category = bottle.Category;
            Varietal = bottle.Varietal;
            City_Town = bottle.City_Town;
            Region = bottle.Region;
            State_Province = bottle.State_Province;
            Country = bottle.Country;
            ExpertRatings = bottle.ExpertRatings;
            Size = bottle.Size;
            ABV = bottle.ABV;
            WinemakerNotes = bottle.WinemakerNotes;
            where_bought = bottle.where_bought;
            price_paid = bottle.price_paid;
            user_rating = bottle.user_rating;
            drink_date = bottle.drink_date;
            user_notes = bottle.user_notes;
            bottle_color = "#FFFFFF";
            if (String.IsNullOrEmpty(WineName)) winename_display = Varietal ?? "";
            else if (String.IsNullOrEmpty(Varietal)) winename_display = WineName ?? "";
            else
            {
                if (WineName.Contains(Varietal)) winename_display = WineName ?? "";
                else winename_display = WineName + ", " + Varietal;
            }
            if(String.IsNullOrEmpty(City_Town))
            {
                if (String.IsNullOrEmpty(State_Province)) location_display = Region + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else if (String.IsNullOrEmpty(Region)) location_display = State_Province + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else location_display = Region + ", " + State_Province;
            }
            else if(String.IsNullOrEmpty(Region))
            {
                if (String.IsNullOrEmpty(State_Province)) location_display = City_Town + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else location_display = City_Town + ", " + State_Province;
            }
            if (String.IsNullOrEmpty(Region) && String.IsNullOrEmpty(City_Town)) {
                if (String.IsNullOrEmpty(State_Province)) location_display = Country ?? "";
                else
                {
                    location_display = State_Province + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                }
            }
            if(!String.IsNullOrEmpty(Region) && !String.IsNullOrEmpty(City_Town))
            {
                location_display = Region + (String.IsNullOrEmpty(State_Province) ? "" : ", " + State_Province) + (String.IsNullOrEmpty(Country) || Country == "USA" || Country == "US" || Country == "United States of America" ? "" : ", " + Country);
            }
            position_display = row == 0 && col == 0 ? "" : "R: " + row.ToString() + ", C: " + col.ToString();
            rack_display = String.IsNullOrEmpty(rack_guid) ? "Unassigned" : rack_name;
            rack_position_display = rack_display == "Unassigned" ? rack_display : rack_display + " (" + position_display + ")";
            user_rating_display = user_rating == null ? "" : "Score: " + user_rating + "/10";
            user_rating_display_nolabel = user_rating == null ? "" : user_rating + "/10";
            vintner_winename_display = Vintner + " " + winename_display;
        }

        public UserBottleForDisplay(UserBottle bottle, string bottle_color) : this(bottle)
        {
            this.bottle_color = bottle_color;
            if (bottle.user_rating == null) starOpacity = "0.1";
            else starOpacity = "1";
            for (int i = 1; i <= 5; i++)
            {
                string thisSource = "baseline_star_border_black_48dp_empty.png";
                if (bottle.user_rating >= 2 * i) thisSource = "baseline_star_border_black_48dp_full.png";
                else if (bottle.user_rating + 1 == 2 * i) thisSource = "baseline_star_border_black_48dp_half.png";
                else thisSource = "baseline_star_border_black_48dp_empty.png";

                switch (i) {
                    case 1:
                        star1 = thisSource; break;
                    case 2:
                        star2 = thisSource; break;
                    case 3:
                        star3 = thisSource; break;
                    case 4:
                        star4 = thisSource; break;
                    case 5:
                        star5 = thisSource; break;
                    default:
                        break;
                }
            }
        }

        private void BuildReviewStars(int review)
        {

        }

        public UserBottleForDisplay(UserBottleForDisplay bottle)
        {
            guid = bottle.guid;
            owner_guid = bottle.owner_guid;
            rack_guid = bottle.rack_guid;
            rack_name = bottle.rack_name;
            row = bottle.row;
            col = bottle.col;
            bottle_guid = bottle.bottle_guid;
            Year = bottle.Year == "0" ? "Non-Vintage" : bottle.Year;
            Vintner = bottle.Vintner;
            WineName = bottle.WineName;
            Category = bottle.Category;
            Varietal = bottle.Varietal;
            City_Town = bottle.City_Town;
            Region = bottle.Region;
            State_Province = bottle.State_Province;
            Country = bottle.Country;
            ExpertRatings = bottle.ExpertRatings;
            Size = bottle.Size;
            ABV = bottle.ABV;
            WinemakerNotes = bottle.WinemakerNotes;
            where_bought = bottle.where_bought;
            price_paid = bottle.price_paid;
            user_rating = bottle.user_rating;
            drink_date = bottle.drink_date;
            user_notes = bottle.user_notes;
            if (String.IsNullOrEmpty(WineName)) winename_display = Varietal ?? "";
            else if (String.IsNullOrEmpty(Varietal)) winename_display = WineName ?? "";
            else
            {
                if (WineName.Contains(Varietal)) winename_display = WineName ?? "";
                else winename_display = WineName + ", " + Varietal;
            }
            if (String.IsNullOrEmpty(City_Town))
            {
                if (String.IsNullOrEmpty(State_Province)) location_display = Region + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else if (String.IsNullOrEmpty(Region)) location_display = State_Province + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else location_display = Region + ", " + State_Province;
            }
            else if (String.IsNullOrEmpty(Region))
            {
                if (String.IsNullOrEmpty(State_Province)) location_display = City_Town + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                else location_display = City_Town + ", " + State_Province;
            }
            if (String.IsNullOrEmpty(Region) && String.IsNullOrEmpty(City_Town))
            {
                if (String.IsNullOrEmpty(State_Province)) location_display = Country ?? "";
                else
                {
                    location_display = State_Province + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
                }
            }
            if (!String.IsNullOrEmpty(Region) && !String.IsNullOrEmpty(City_Town))
            {
                location_display = Region + (String.IsNullOrEmpty(State_Province) ? "" : ", " + State_Province) + (String.IsNullOrEmpty(Country) ? "" : ", " + Country);
            }
            position_display = row == 0 && col == 0 ? "" : "R: " + row.ToString() + ", C: " + col.ToString();
            rack_display = String.IsNullOrEmpty(rack_name) ? "Unassigned" : rack_name;
            user_rating_display = user_rating == null ? "" : "Score: " + user_rating + "/10";
            user_rating_display_nolabel = user_rating == null ? "" : user_rating + "/10";
            vintner_winename_display = Vintner + " " + winename_display;
        }
    }
}
