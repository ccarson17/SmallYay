using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SmallYay.Config;
using SmallYay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SmallYay.Services
{
    class WineAPIService
    {
        public string GetApiGuid()
        {
            var task = GetApiGuidTask();
            if (task.Result != null) return task.Result.ToString();
            return "";
        }

        private async Task<string> GetApiGuidTask()
        {
            return (await SecureStorage.GetAsync("user_guid") ?? "");
        }

        public string RootApiCall()
        {
            var client = new RestClient($"{Constants.WineApiUrl}/");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        #region Bottles
        public List<Bottle> GetAllBottles()
        {
            try
            {
                var client = new RestClient($"{Constants.WineApiUrl}/bottles");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Accept", "application/json");
                IRestResponse response = client.Execute(request);
                var allBottles = JsonConvert.DeserializeObject<List<Bottle>>(response.Content);
                return allBottles;
            }
            catch(Exception ex)
            {
                return new List<Bottle>();
            }
        }

        public string GetMyBottleMatchGuid(UserBottle bottle)
        {
            try
            {
                var client = new RestClient($"{Constants.WineApiUrl}/bottles?owner_guid={bottle.owner_guid}&year={bottle.Year}&vintner={bottle.Vintner}&wineName={bottle.WineName}&category={bottle.Category}&Varietal={bottle.Varietal}&SizeInML={bottle.Size}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                var allBottles = JsonConvert.DeserializeObject<List<BottleReturnFromApi>>(response.Content);
                if (allBottles.Count > 0) return allBottles.FirstOrDefault().guid;
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string CreateNewBottle(UserBottle userBottle)
        {
            string returnGuid = "";
            try
            {
                var client = new RestClient($"{Constants.WineApiUrl}/bottles");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                int Year = 0;
                int.TryParse(userBottle.Year, out Year);
                int SizeInML = 750;
                if((userBottle.Size ?? "").Contains("ml"))
                {
                    int.TryParse((userBottle.Size ?? "").Replace("ml", ""), out SizeInML);
                }
                else if((userBottle.Size ?? "").Contains("L"))
                {
                    double doubleSize = 0.750;
                    double.TryParse((userBottle.Size ?? "").Replace("L", ""), out doubleSize);
                    int.TryParse((doubleSize * 1000).ToString(), out SizeInML);
                }
                else
                {
                    int.TryParse(userBottle.Size ?? "", out SizeInML);
                }
                //decimal ABV = 0;
                //decimal.TryParse(userBottle.ABV, out ABV);
                string ABV = (userBottle.ABV ?? "").Replace("%", "");
                var inputBottle = new Bottle()
                {
                    Year = Year,
                    Vintner = userBottle.Vintner,
                    WineName = userBottle.WineName,
                    Category = userBottle.Category,
                    Varietal = userBottle.Varietal,
                    City_Town = userBottle.City_Town,
                    Region = userBottle.Region,
                    State_Province = userBottle.State_Province,
                    Country = userBottle.Country,
                    ExpertRatings = userBottle.ExpertRatings,
                    SizeInML = SizeInML,
                    ABV = ABV,
                    WinemakerNotes = userBottle.WinemakerNotes,
                    owner_guid = userBottle.owner_guid
                };
                string inputJSON = JsonConvert.SerializeObject(inputBottle);
                request.AddParameter("application/json", inputJSON, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    var parsedObject = JObject.Parse(response.Content);
                    var errorsJson = parsedObject["errors"].ToString();
                    var errorsJsonParsed = errorsJson.Replace("[", "").Replace("]", "");
                    var bottleErrors = JsonConvert.DeserializeObject<Bottle>(errorsJsonParsed);                  
                    return "API Error: " + response.StatusCode;
                }
                else
                {
                    var newBottle = JsonConvert.DeserializeObject<BottleReturnFromApi>(response.Content);
                    return newBottle.guid;
                }
            }
            catch (Exception ex)
            {
                return returnGuid;
            }
        }
        #endregion

        #region UserBottles
        public List<UserBottle> GetMyUserBottles(string owner_guid, string getCurrentOrHistory, bool cacheAllow, UserBottleFilter filters)
        {
            try
            {
                string clientURL = $"{Constants.WineApiUrl}/userbottles?owner_guid={owner_guid}&getCurrentOrHistory={getCurrentOrHistory}";
                if(filters != null)
                {
                    // client = filtered URL
                    if (!String.IsNullOrEmpty(filters.minYear)) clientURL += "&minYear=" + filters.minYear;
                    if (!String.IsNullOrEmpty(filters.maxYear)) clientURL += "&maxYear=" + filters.maxYear;
                    if (filters.minRating != 0 && filters.minRating != null) clientURL += "&minRating=" + filters.minRating;
                    if (filters.maxRating != 0 && filters.maxRating != null) clientURL += "&maxRating=" + filters.maxRating;
                    if (!String.IsNullOrEmpty(filters.orderBy)) clientURL += "&orderBy=" + filters.orderBy;
                    if (!String.IsNullOrEmpty(filters.Category)) clientURL += "&Category=" + filters.Category;
                    if (!String.IsNullOrEmpty(filters.Varietal)) clientURL += "&Varietal=" + filters.Varietal;
                    if (!String.IsNullOrEmpty(filters.Vintner)) clientURL += "&Vintner=" + filters.Vintner;
                    // Price Filters
                    if (!String.IsNullOrEmpty(filters.minPrice)) clientURL += "&minPrice=" + filters.minPrice;
                    if (!String.IsNullOrEmpty(filters.maxPrice)) clientURL += "&maxPrice=" + filters.maxPrice;
                    // Location Filters
                    if (!String.IsNullOrEmpty(filters.Region)) clientURL += "&Region=" + filters.Region;
                    if (!String.IsNullOrEmpty(filters.Country)) clientURL += "&Country=" + filters.Country;
                    if (!String.IsNullOrEmpty(filters.State_Province)) clientURL += "&State_Province=" + filters.State_Province;
                    if (!String.IsNullOrEmpty(filters.City_Town)) clientURL += "&City_Town=" + filters.City_Town;
                    // Search Query
                    if (!String.IsNullOrEmpty(filters.searchQuery)) clientURL += "&searchQuery=" + filters.searchQuery;
                }
                var client = new RestClient(clientURL);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                if(!cacheAllow)
                    request.AddHeader("Cache-Control", "no-cache");
                IRestResponse response = client.Execute(request);
                var allBottles = JsonConvert.DeserializeObject<List<UserBottle>>(response.Content);
                if (filters != null)
                {
                    switch (filters.orderBy)
                    {
                        case "year asc, vintner, winename":
                            allBottles = allBottles.OrderBy(x => x.Year == "0" ? "50000" : x.Year).ThenBy(x => x.Vintner).ThenBy(x => x.WineName).ToList();
                            break;
                        case "price_paid asc, vintner, winename":
                            allBottles = allBottles.OrderBy(x => String.IsNullOrEmpty(x.price_paid) ? Decimal.MaxValue : Decimal.Parse(x.price_paid.Replace("$", ""))).ThenBy(x => x.Vintner).ThenBy(x => x.WineName).ToList();
                            break;
                        case "price_paid desc, vintner, winename":
                            allBottles = allBottles.OrderByDescending(x => String.IsNullOrEmpty(x.price_paid) ? Decimal.MinValue : Decimal.Parse(x.price_paid.Replace("$", ""))).ThenBy(x => x.Vintner).ThenBy(x => x.WineName).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return allBottles;
            }
            catch (Exception ex)
            {
                return new List<UserBottle>();
            }
        }

        public UserBottle GetUserBottle(string owner_guid, string bottle_guid, bool cacheAllow)
        {
            try
            {
                var client = new RestClient($"{Constants.WineApiUrl}/userbottles/" + bottle_guid + "?owner_guid=" + owner_guid);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                if (!cacheAllow)
                    request.AddHeader("Cache-Control", "no-cache");
                IRestResponse response = client.Execute(request);
                var allBottles = JsonConvert.DeserializeObject<UserBottle>(response.Content);
                return allBottles;
            }
            catch (Exception ex)
            {
                return new UserBottle();
            }
        }

        public UserBottle CreateNewUserBottle(UserBottle userBottle)
        {
            try
            {
                UserBottleForCreation ubc = new UserBottleForCreation()
                {
                    owner_guid = userBottle.owner_guid,
                    rack_guid = userBottle.rack_guid,
                    rack_name = userBottle.rack_name,
                    bottle_guid = userBottle.bottle_guid,
                    rack_col = userBottle.col,
                    rack_row = userBottle.row,
                    where_bought = userBottle.where_bought,
                    price_paid = userBottle.price_paid,
                    user_rating = userBottle.user_rating,
                    drink_date = userBottle.drink_date,
                    created_date = DateTime.UtcNow,
                    user_notes = userBottle.user_notes
                };

                var client = new RestClient($"{Constants.WineApiUrl}/userbottles/");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                string inputJSON = JsonConvert.SerializeObject(ubc);
                request.AddParameter("application/json", inputJSON, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    var parsedObject = JObject.Parse(response.Content);
                    var errorsJson = parsedObject["errors"].ToString();
                    var errorsJsonParsed = errorsJson.Replace("[", "").Replace("]", "");
                    var bottleErrors = JsonConvert.DeserializeObject<Bottle>(errorsJsonParsed);
                    return new UserBottle(){
                        guid = "API Error: " + response.StatusCode
                    };
                }
                else
                {
                    var newBottle = JsonConvert.DeserializeObject<UserBottle>(response.Content);
                    return newBottle;
                }
            }
            catch (Exception ex)
            {
                return new UserBottle();
            }
        }

        public string UpdateUserBottle(UserBottle userBottle)
        {
            var oldBottle = GetUserBottle(userBottle.owner_guid, userBottle.guid, false);
            var bottleMatch = GetMyBottleMatchGuid(userBottle);
            if(bottleMatch != userBottle.bottle_guid) // bottle attributes have changed
            {
                if(String.IsNullOrEmpty(bottleMatch)) // no bottle matches at all, create new bottle
                {
                    bottleMatch = CreateNewBottle(userBottle);
                    if (bottleMatch.StartsWith("API Error") || String.IsNullOrEmpty(bottleMatch)) return "CreateError";
                }
                userBottle.bottle_guid = bottleMatch;
            }
            if(oldBottle.rack_guid != userBottle.rack_guid || oldBottle.row != userBottle.row || oldBottle.col != userBottle.col) // rack info has changed
            {
                List<UserBottle> myBottles = GetMyUserBottles(userBottle.owner_guid, "current", false, null);
                UserBottle userBottleMatch = myBottles.Where(x => x.rack_name == userBottle.rack_guid && x.col == userBottle.col && x.row == userBottle.row).FirstOrDefault();
                if (userBottleMatch != null) return "Conflict with existing bottle";
            }
            string bottleGuid = userBottle.guid;
            string ownerGuid = userBottle.owner_guid;
            var client = new RestClient($"{Constants.WineApiUrl}/userbottles/{bottleGuid}?ownerId={ownerGuid}");
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json-patch+json");

            if (userBottle.rack_name == "Unassigned") userBottle.rack_name = null;

            string patch = "[";
            patch += "{\"op\":\"replace\",\"path\":\"/bottle_guid\",\"value\": \"" + userBottle.bottle_guid + "\"},";
            patch += "{\"op\":\"replace\",\"path\":\"/where_bought\",\"value\": \"" + userBottle.where_bought + "\"},";
            patch += "{\"op\":\"replace\",\"path\":\"/price_paid\",\"value\": \"" + userBottle.price_paid + "\"},";
            patch += "{\"op\":\"replace\",\"path\":\"/rack_guid\",\"value\": \"" + userBottle.rack_guid + "\"},";
            patch += "{\"op\":\"replace\",\"path\":\"/rack_name\",\"value\": \"" + userBottle.rack_name + "\"},";
            patch += "{\"op\":\"replace\",\"path\":\"/rack_row\",\"value\": " + userBottle.row + "},";
            patch += "{\"op\":\"replace\",\"path\":\"/rack_col\",\"value\": " + userBottle.col + "}";
            patch += "]";

            //request.AddParameter("application/json-patch+json", "[{\"op\":\"replace\",\"path\":\"/drink_date\",\"value\": " + drinkDate +
            //    "},{\"op\":\"replace\",\"path\":\"/rack_guid\",\"value\": null},{\"op\":\"replace\",\"path\":\"/rack_row\",\"value\": 0}" +
            //    ",{\"op\":\"replace\",\"path\":\"/rack_col\",\"value\": 0}]", ParameterType.RequestBody);

            request.AddParameter("application/json-patch+json", patch, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }

        public string DrinkUserBottle(UserBottleForDisplay userBottle)
        {
            string bottleGuid = userBottle.guid;
            string ownerGuid = userBottle.owner_guid;
            var client = new RestClient($"{Constants.WineApiUrl}/userbottles/{bottleGuid}?ownerId={ownerGuid}");
            string drinkDate = JsonConvert.SerializeObject(DateTime.UtcNow);
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json-patch+json");
            //request.AddParameter("application/json-patch+json", "[{\"op\":\"replace\",\"path\":\"/drink_date\",\"value\": " + drinkDate + "}]", ParameterType.RequestBody);
            request.AddParameter("application/json-patch+json", "[{\"op\":\"replace\",\"path\":\"/drink_date\",\"value\": " + drinkDate + 
                "},{\"op\":\"replace\",\"path\":\"/rack_guid\",\"value\": null},{\"op\":\"replace\",\"path\":\"/rack_row\",\"value\": 0}" + 
                ",{\"op\":\"replace\",\"path\":\"/rack_col\",\"value\": 0}]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }

        public string AddUserBottleToRack(UserBottleForDisplay userBottle)
        {
            string bottleGuid = userBottle.guid;
            string ownerGuid = userBottle.owner_guid;
            var client = new RestClient($"{Constants.WineApiUrl}/userbottles/{bottleGuid}?ownerId={ownerGuid}");
            string drinkDate = JsonConvert.SerializeObject(DateTime.UtcNow);
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json-patch+json");
            string patchDocument = "[{\"op\":\"replace\",\"path\":\"/rack_name\",\"value\": \"" + userBottle.rack_name + "\"},{\"op\":\"replace\",\"path\":\"/rack_guid\",\"value\": \"" +
                userBottle.rack_guid + "\"},{\"op\":\"replace\",\"path\":\"/rack_row\",\"value\": " + userBottle.row + "},{\"op\":\"replace\",\"path\":\"/rack_col\",\"value\": " +
                userBottle.col + "}]";
            request.AddParameter("application/json-patch+json", patchDocument, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }

        public string ReviewUserBottle(UserBottleForDisplay userBottle)
        {
            string bottleGuid = userBottle.guid;
            string ownerGuid = userBottle.owner_guid;
            var client = new RestClient($"{Constants.WineApiUrl}/userbottles/{bottleGuid}?ownerId={ownerGuid}");
            int user_rating = userBottle.user_rating ?? 0;
            string user_notes = userBottle.user_notes;
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json-patch+json");
            request.AddParameter("application/json-patch+json", "[{\"op\":\"replace\",\"path\":\"/user_rating\",\"value\": " + user_rating +
                "},{\"op\":\"replace\",\"path\":\"/user_notes\",\"value\": \"" + user_notes + "\"}]", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }

        public string DeleteUserBottle(string owner_guid, string bottle_guid)
        {
            var client = new RestClient($"{Constants.WineApiUrl}/userbottles/{bottle_guid}?ownerId={owner_guid}");
            client.Timeout = -1;
            var request = new RestRequest(Method.DELETE);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }
        #endregion

        #region Racks
        public List<Rack> GetMyRacks(string user_guid, bool cacheAllow)
        {
            if (String.IsNullOrEmpty(user_guid)) return new List<Rack>();
            var client = new RestClient($"{Constants.WineApiUrl}/racks?owner_guid=" + user_guid);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            if (!cacheAllow)
                request.AddHeader("Cache-Control", "no-cache");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return new List<Rack>();
            var myRacks = JsonConvert.DeserializeObject<List<Rack>>(response.Content);
            return myRacks;
        }

        public Rack CreateNewRack(Rack newRack)
        {
            var client = new RestClient($"{Constants.WineApiUrl}/racks");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            string inputJSON = JsonConvert.SerializeObject(newRack);
            request.AddParameter("application/json", inputJSON, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                if (response.Content.Contains("errors") && response.Content.Contains("rackForCreationDto"))
                {
                    var parsedObject = JObject.Parse(response.Content);
                    var errorsJson = parsedObject["errors"]["rackForCreationDto"];
                    var bottleErrors = JsonConvert.DeserializeObject<List<string>>(errorsJson.ToString());
                    newRack.owner_guid = "API Error: " + response.StatusCode;
                    foreach (var item in bottleErrors)
                    {
                        newRack.owner_guid += "\n" + item;
                    }
                    return newRack;
                }
                else
                {
                    newRack.owner_guid = "API Error: " + response.StatusCode;
                    return newRack;
                }
            }
            else
            {
                var newRackOutput = JsonConvert.DeserializeObject<Rack>(response.Content);
                return newRackOutput;
            }
        }

        public string UpdateRack(Rack inputRack)
        {
            var client = new RestClient($"{Constants.WineApiUrl}/racks/"+ inputRack.guid + "?ownerId=" + inputRack.owner_guid);
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json-patch+json");
            string patchDoc = "[{\"op\":\"replace\",\"path\":\"/rack_name\",\"value\": \"" + inputRack.rack_name +
                "\"},{\"op\":\"replace\",\"path\":\"/rows\",\"value\": " + inputRack.rows + "},{\"op\":\"replace\",\"path\":\"/cols\",\"value\": " +
                inputRack.cols + "}]";
            request.AddParameter("application/json-patch+json", patchDoc, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }
        #endregion
    }
}
