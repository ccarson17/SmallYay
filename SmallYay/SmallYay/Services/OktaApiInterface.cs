using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SmallYay.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallYay.Services
{
    public class OktaApiInterface
    {
        public OktaUserProfile GetUserProfile(string userId)
        {
            var userProfile = new OktaUserProfile();
            //var client = new RestClient($"{Constants.OrgUrl}/api/v1/apps/{Constants.ClientId}/users/{userId}");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.GET);
            ////request.AddHeader("Authorization", "SSWS 00CA9AO0WSHMqMguu2U_MkNIj_px03gtRPyxnjt_Qn");
            //request.AddHeader("Authorization", "SSWS 00N1sNpyFu0bXooEYaXEzNvkqQvozi0H-vANQ744Eb");
            //request.AddHeader("Cookie", "JSESSIONID=A3BE173E706E0FF7AE787C92ABD698B3");
            //request.AddParameter("text/plain", "", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //var parsedObject = JObject.Parse(response.Content);
            //var profileJson = (parsedObject["profile"] ?? "").ToString();
            //userProfile = JsonConvert.DeserializeObject<OktaUserProfile>(profileJson);
            //return userProfile;

            var client = new RestClient(Constants.WineApiUrl + "/users/" + userId);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddParameter("text/plain", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            userProfile = JsonConvert.DeserializeObject<OktaUserProfile>(response.Content);
            return userProfile;
        }

        public OktaUserProfile UpdateUserProfile(OktaUserProfile userProfile, string userId)
        {
            //var client = new RestClient($"{Constants.OrgUrl}/api/v1/apps/{Constants.ClientId}/users/{userId}");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Authorization", "SSWS 00CA9AO0WSHMqMguu2U_MkNIj_px03gtRPyxnjt_Qn");
            //request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Cookie", "JSESSIONID=A3BE173E706E0FF7AE787C92ABD698B3");
            //var requestBody = "{\"profile\": " + JsonConvert.SerializeObject(userProfile) + "}";
            //request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //var parsedObject = JObject.Parse(response.Content);
            //var profileJson = parsedObject["profile"].ToString();
            //userProfile = JsonConvert.DeserializeObject<OktaUserProfile>(profileJson);
            //return userProfile;

            var client = new RestClient(Constants.WineApiUrl + "/users/" + userId);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            var requestBody = JsonConvert.SerializeObject(userProfile);
            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            userProfile = JsonConvert.DeserializeObject<OktaUserProfile>(response.Content);
            return userProfile;
        }
    }
}
