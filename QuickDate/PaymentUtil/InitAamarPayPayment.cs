using AamarPay;
using Android.App;
using Android.Widget;
using Newtonsoft.Json;
using Org.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDate.PaymentUtil
{
    public class InitAamarPayPayment : InitAamarPay.IOnInitListener, InitAamarPay.ITransactionInfoListener
    {
        private readonly Activity ActivityContext;
        private readonly HomeActivity GlobalContext;
        private DialogBuilder DialogBuilder;
        private AlertDialog AlertDialog;
        private string TransactionId;
        private InitAamarPay AamarPay;
        private string Price, PayType, Credits, Id;

        public InitAamarPayPayment(Activity activity)
        {
            try
            {
                ActivityContext = activity;
                GlobalContext = HomeActivity.GetInstance();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //AamarPay
        public void BtnAamarPayOnClick(string price, string payType, string credits, string id)
        {
            try
            {
                Price = price; PayType = payType; Credits = credits; Id = id;
                var option = ListUtils.MyUserInfo.FirstOrDefault();
                var currency = ListUtils.SettingsSiteList?.Currency ?? "USD";

                DialogBuilder = new DialogBuilder(ActivityContext, AlertDialog);

                // Initiate payment
                AamarPay = new InitAamarPay(ActivityContext, ListUtils.SettingsSiteList?.AamarpayStoreId, ListUtils.SettingsSiteList?.AamarpaySignatureKey);

                switch (ListUtils.SettingsSiteList?.AamarpayMode)
                {
                    case "live":
                        // Set Live Mode
                        AamarPay.TestMode(false);
                        break;
                    case "sandbox":
                        // Set Test Mode
                        AamarPay.TestMode(true);
                        break;
                    default:
                        // Set Test Mode
                        AamarPay.TestMode(true);
                        break;
                }

                // Auto generate Trx
                AamarPay.AutoGenerateTransactionId(true);

                // Generate unique transaction id
                TransactionId = AamarPay.generate_trx_id();

                DialogBuilder.ShowLoading();
                AamarPay.SetTransactionParameter(price, currency, "Pay the card");
                AamarPay.SetCustomerDetails(option.FullName, option.Email, option.PhoneNumber, "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1", "Istanbul", "Turkey");
                AamarPay.InitPgw(this);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void OnInitFailure(bool? error, string message)
        {
            DialogBuilder.DismissDialog();
            DialogBuilder.ErrorPopUp(message);
        }

        public void OnPaymentSuccess(JSONObject jsonObject)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<SuccessAamarPayObject>(jsonObject.ToString());
                if (data != null)
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SuccessAamarPay(data.MerTxnid, data.PayStatus) });
                    DialogBuilder.DismissDialog();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnPaymentFailure(JSONObject jsonObject)
        {
            DialogBuilder.DismissDialog();
        }

        public void OnPaymentProcessingFailed(JSONObject jsonObject)
        {
            DialogBuilder.DismissDialog();
        }

        public void OnPaymentCancel(JSONObject jsonObject)
        {
            try
            {
                AamarPay.GetTransactionInfo(jsonObject.GetString("trx_id"), this);
            }
            catch (JSONException e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSuccess(JSONObject jsonObject)
        {
            DialogBuilder.DismissDialog();
        }

        public void OnFailure(bool? error, string message)
        {
            DialogBuilder.DismissDialog();
            DialogBuilder.ErrorPopUp(message);
        }

        private async Task SuccessAamarPay(string merTxnid, string payStatus)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (PayType == "membership")
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetPro("AamarPay") });
                    }
                    else
                    {
                        //wael update to new aamarpay/success 
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetCredit("AamarPay") });
                        //var (apiStatus, respond) = await RequestsAsync.Payments.SuccessAamarPayAsync(PayType, Price, merTxnid, payStatus);
                        //if (apiStatus == 200)
                        //    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_PaymentSuccessfully), ToastLength.Long)?.Show();
                        //else
                        //    Methods.DisplayReportResult(ActivityContext, respond);
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}