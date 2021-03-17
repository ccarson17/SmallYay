using System;
using System.Collections.Generic;
using System.Text;

namespace SmallYay.Config
{
    public class Constants
    {
        public const string AuthStateKey = "authState";
        public const string AuthServiceDiscoveryKey = "authServiceDiscovery";

        public const string ClientId = "0oa138hfqseIm0EpE4x7";
        public const string RedirectUri = "com.okta.dev-364313:/callback";
        public const string OrgUrl = "https://dev-364313.okta.com";
        public const string AuthorizationServerId = "default";
        public const string CallbackScheme = "com.okta.dev-364313";

        public static readonly string DiscoveryEndpoint =
            $"{OrgUrl}/oauth2/{AuthorizationServerId}/.well-known/openid-configuration";


        public static readonly string[] Scopes = new string[] {
        "openid", "profile", "email", "offline_access" };

        public const string LogoutRedirectUri = "com.okta.dev-364313:/";

        public static readonly string LogoutEndpoint = $"{OrgUrl}/oauth2/{AuthorizationServerId}/v1/logout";

        public static readonly string WineApiUrl = "http://wineapi-dev.us-east-1.elasticbeanstalk.com/api";
        //public static readonly string WineApiUrl = "http://localhost:51044/api";

        // Default Values for fields
        public const string Vintner_Default = "Vintner (required)";
        public const string Varietal_Default = "Varietal (required)";
        public const string Wine_Name_Default = "Wine Name (required)";
        public const string Category_Default = "Category (required)";
        public const string Year_Default = "Year (required)";
        public const string Region_Default = "Region";
        public const string City_Town_Default = "City/Town";
        public const string State_Province_Default = "State/Province";
        public const string Country_Default = "Country";
        public const string Where_Bought_Default = "Where Bought";
        public const string Price_Paid_Default = "Price Paid";

    }
}
