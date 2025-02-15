using Android.App;
using Com.Razorpay;
using Newtonsoft.Json.Linq;
using Org.Json;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Encoding = System.Text.Encoding;

namespace QuickDate.PaymentUtil
{
    public class InitRazorPayPayment
    {
        private readonly Activity ActivityContext;
        private Checkout CheckOut;

        public InitRazorPayPayment(Activity activity)
        {
            try
            {
                ActivityContext = activity;

                // To ensure faster loading of the Checkout form,call this method as early as possible in your checkout flow.
                Checkout.Preload(activity);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //RazorPay
        public async void BtnRazorPayOnClick(string price)
        {
            try
            {
                (bool init, string orderId) = await InitRazorPay(price);
                switch (init)
                {
                    case false:
                        return;
                }

                Activity activity = ActivityContext;
                JSONObject options = new JSONObject();

                options.Put("name", "Add to balance");

                options.Put("description", "");

                //options.Put("image", "https://demo.QuickDate.com/themes/default/img/logo.png");
                options.Put("order_id", orderId);

                options.Put("send_sms_hash", true);
                options.Put("allow_rotation", true);

                options.Put("theme.color", AppSettings.MainColor);
                options.Put("currency", AppSettings.RazorPayCurrency);

                var priceInt = Convert.ToInt32(price) * 100;
                options.Put("amount", priceInt.ToString());//pass amount in currency subunits

                var option = ListUtils.MyUserInfo.FirstOrDefault();
                if (option != null)
                {
                    JSONObject preFill = new JSONObject();
                    preFill.Put("email", option.Email);
                    preFill.Put("contact", option.PhoneNumber);

                    options.Put("prefill", preFill);
                }

                JSONObject retryObj = new JSONObject();
                retryObj.Put("enabled", true);
                retryObj.Put("max_count", 4);
                options.Put("retry", retryObj);

                CheckOut?.Open(activity, options);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async Task<(bool, string)> InitRazorPay(string price)
        {
            try
            {
                //PayerID 
                string keyId = ActivityContext.GetText(Resource.String.razorpay_api_Key);
                string keySecret = "";

                var option = ListUtils.SettingsSiteList;
                if (option != null)
                {
                    //keyId = option.RazorpayKeyId;
                    keySecret = option.RazorpayKeySecret;
                }

                if (string.IsNullOrEmpty(keyId))
                    return (false, "");

                string orderId = await CreateOrders(keyId, keySecret, price);

                CheckOut = new Checkout();
                CheckOut.SetKeyID(keyId);
                CheckOut.SetImage(Resource.Mipmap.icon);
                CheckOut.SetFullScreenDisable(false);

                return (true, orderId);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return (false, "");
            }
        }

        public void StopRazorPay()
        {
            try
            {
                if (CheckOut != null)
                {
                    CheckOut = null;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public async Task<string> CreateOrders(string username, string password, string amount)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.razorpay.com/v1/orders");
                var plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
                var basicAuthKey = Convert.ToBase64String(plainTextBytes);

                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {basicAuthKey}");

                //{"amount": 500,"currency": "INR","receipt": "qwsaq1","partial_payment": true,"first_payment_min_amount": 230}
                var priceInt = Convert.ToInt32(amount) * 100;
                JSONObject payload = new JSONObject();
                payload.Put("amount", priceInt);
                payload.Put("currency", AppSettings.RazorPayCurrency);
                payload.Put("partial_payment", false);

                string jsonData = payload.ToString();
                request.Content = new StringContent(jsonData);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);
                string json = await response.Content.ReadAsStringAsync();
                string orderId = JObject.Parse(json)["id"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(orderId))
                {
                    return orderId;
                }

                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }
    }
}