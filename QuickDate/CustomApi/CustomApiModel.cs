using Android.App;
using Android.Widget;
using Newtonsoft.Json.Linq;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace QuickDate.CustomApi
{
    /// <summary>
    /// you just need these three params.
    /// WebsiteUrl, ServerKey, UserId, AccessToken. and then write your own definition of apis.
    /// If you are a developer, you will know how to use these params.
    /// And if you are not, please hire a dev for customization.
    /// </summary>
    public class CustomApiModel
    {
        public static string WebsiteUrl { get; private set; }
        public static string ServerKey { get; private set; }
        public static string UserId { get; private set; }
        public static string AccessToken { get; private set; }

        public CustomApiModel()
        {
            try
            {
                WebsiteUrl = InitializeQuickDate.WebsiteUrl;
                ServerKey = InitializeQuickDate.ServerKey;
                AccessToken = UserDetails.AccessToken;
                UserId = UserDetails.UserId.ToString();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private static readonly string UrlFunPost = WebsiteUrl + "/api/UrlFunPost" + "?access_token=";
        public static async void FunPost()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    var client = new HttpClient();
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("server_key", ServerKey),
                        new KeyValuePair<string, string>("user_id", UserId),
                    });

                    var response = await client.PostAsync(UrlFunPost + AccessToken, formContent); // changed the urls 
                    string json = await response.Content.ReadAsStringAsync();
                    string code = JObject.Parse(json)["api_status"]?.ToString() ?? "400";
                    Console.WriteLine(code);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        private static readonly string UrlFunGet = WebsiteUrl + "/api/UrlFunGet" + "?access_token=";
        public static async void FunGet()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
                else
                {
                    var client = new HttpClient();
                    var response = await client.GetAsync(UrlFunGet + AccessToken + "&server_key=" + ServerKey); // changed the urls 
                    string json = await response.Content.ReadAsStringAsync();
                    string code = JObject.Parse(json)["api_status"]?.ToString() ?? "400";
                    Console.WriteLine(code);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}