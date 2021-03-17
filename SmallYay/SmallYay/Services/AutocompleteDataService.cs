using Newtonsoft.Json;
using SmallYay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SmallYay.Services
{
    public class AutocompleteDataService
    {
        private List<UserBottle> myBottlesApi { get; set; }
        private readonly WineAPIService wineApiService = new WineAPIService();

        public async void PopulateAutocompleteLists(bool forceUpdate)
        {
            var ac = GetAutocomplete("vintner").Result;
            if (ac == null || forceUpdate)
            {
                myBottlesApi = wineApiService.GetMyUserBottles(wineApiService.GetApiGuid(), "all", false, new UserBottleFilter());
                string vintner_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.Vintner).Where(x => x.Vintner != null).Select(x => x.Vintner).Distinct().ToList());
                await SecureStorage.SetAsync("vintner_autocomplete", vintner_json);
                string varietal_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.Varietal).Where(x => x.Varietal != null).Select(x => x.Varietal).Distinct().ToList());
                await SecureStorage.SetAsync("varietal_autocomplete", varietal_json);
                string winename_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.WineName).Where(x => x.WineName != null).Select(x => x.WineName).Distinct().ToList());
                await SecureStorage.SetAsync("winename_autocomplete", winename_json);
                string city_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.City_Town).Where(x => x.City_Town != null).Select(x => x.City_Town).Distinct().ToList());
                await SecureStorage.SetAsync("city_autocomplete", city_json);
                string region_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.Region).Where(x => x.Region != null).Select(x => x.Region).Distinct().ToList());
                await SecureStorage.SetAsync("region_autocomplete", region_json);
                string state_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.State_Province).Where(x => x.State_Province != null).Select(x => x.State_Province).Distinct().ToList());
                await SecureStorage.SetAsync("state_autocomplete", state_json);
                string country_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.Country).Where(x => x.Country != null).Select(x => x.Country).Distinct().ToList());
                await SecureStorage.SetAsync("country_autocomplete", country_json);
                string where_bought_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.where_bought).Where(x => x.where_bought != null).Select(x => x.where_bought).Distinct().ToList());
                await SecureStorage.SetAsync("where_bought_autocomplete", where_bought_json);
            }
        }

        public async Task<string> GetAutocomplete(string keyword)
        {
            var output = await SecureStorage.GetAsync(keyword + "_autocomplete");
            if (output == "[null]" || output == null) return null;
            return output;
        }
    }
}
