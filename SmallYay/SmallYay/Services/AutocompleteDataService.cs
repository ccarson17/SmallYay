using Newtonsoft.Json;
using SmallYay.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                var vintner_list = myBottlesApi.OrderBy(x => x.Vintner).Where(x => !string.IsNullOrWhiteSpace(x.Vintner)).Select(x => textInfo.ToTitleCase(x.Vintner.Trim())).Distinct().ToList();
                string vintner_json = JsonConvert.SerializeObject(vintner_list.Distinct());
                await SecureStorage.SetAsync("vintner_autocomplete", vintner_json);

                var varietal_list = myBottlesApi.OrderBy(x => x.Varietal).Where(x => !string.IsNullOrWhiteSpace(x.Varietal)).Select(x => textInfo.ToTitleCase(x.Varietal.Trim())).Distinct().ToList();
                string varietal_json = JsonConvert.SerializeObject(varietal_list.Distinct());
                await SecureStorage.SetAsync("varietal_autocomplete", varietal_json);

                var winename_list = myBottlesApi.OrderBy(x => x.WineName).Where(x => !string.IsNullOrWhiteSpace(x.WineName)).Select(x => textInfo.ToTitleCase(x.WineName.Trim())).Distinct().ToList();
                winename_list.AddRange(varietal_list);
                string winename_json = JsonConvert.SerializeObject(winename_list.Distinct());
                await SecureStorage.SetAsync("winename_autocomplete", winename_json);

                var city_list = myBottlesApi.OrderBy(x => x.City_Town).Where(x => !string.IsNullOrWhiteSpace(x.City_Town)).Select(x => textInfo.ToTitleCase(x.City_Town.Trim())).Distinct().ToList();
                string city_json = JsonConvert.SerializeObject(city_list.Distinct());
                await SecureStorage.SetAsync("city_autocomplete", city_json);

                var region_list = myBottlesApi.OrderBy(x => x.Region).Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => textInfo.ToTitleCase(x.Region.Trim())).Distinct().ToList();
                string region_json = JsonConvert.SerializeObject(region_list.Distinct());
                await SecureStorage.SetAsync("region_autocomplete", region_json);

                var state_list = myBottlesApi.OrderBy(x => x.State_Province).Where(x => !string.IsNullOrWhiteSpace(x.State_Province)).Select(x => x.State_Province.Trim()).Distinct();
                state_list = state_list.Select(x => x.Length <= 3 ? textInfo.ToTitleCase(x.ToUpper()) : textInfo.ToTitleCase(x));
                var state_list_corrected = state_list.ToList().Distinct();
                string state_json = JsonConvert.SerializeObject(state_list_corrected);
                await SecureStorage.SetAsync("state_autocomplete", state_json);

                var country_list = myBottlesApi.OrderBy(x => x.Country).Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country.Trim()).Distinct();
                country_list = country_list.Select(x => x.Length <= 3 ? textInfo.ToTitleCase(x.ToUpper()) : textInfo.ToTitleCase(x));
                string country_json = JsonConvert.SerializeObject(country_list.ToList().Distinct());
                await SecureStorage.SetAsync("country_autocomplete", country_json);

                string where_bought_json = JsonConvert.SerializeObject(myBottlesApi.OrderBy(x => x.where_bought).Where(x => x.where_bought != null).Select(x => x.where_bought.Trim()).Distinct().ToList());
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
