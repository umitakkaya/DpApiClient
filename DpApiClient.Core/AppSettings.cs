using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DpApiClient.Core
{
    public static class AppSettings
    {
        private static HospitalContext _db = new HospitalContext();

        private static string _clientId;
        private static string _clientSecret;
        private static string _locale;

        public static string ClientId
        {
            get
            {

                return _clientId ?? (_clientId = _db.AppSettings.Single(s => s.SettingName == "dpapi.clientId").SettingValue);
            }
        }

        public static string ClientSecret
        {
            get
            {
                return _clientSecret ?? (_clientSecret = _db.AppSettings.Single(s => s.SettingName == "dpapi.clientSecret").SettingValue);
            }
        }

        public static string Locale
        {
            get
            {
                return _locale ?? (_locale = _db.AppSettings.Single(s => s.SettingName == "dpapi.locale").SettingValue);
            }
        }
    }
}