using Android.App;
using Android.Widget;
using Com.Cashfree.PG.Api;
using Com.Cashfree.PG.Core.Api;
using Com.Cashfree.PG.Core.Api.Callback;
using Com.Cashfree.PG.Core.Api.Utils;
using Com.Cashfree.PG.UI.Api;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace QuickDate.PaymentUtil
{
    public class InitCashFreePayment : Object, ICFCheckoutResponseCallback
    {
        private readonly Activity ActivityContext;
        private readonly HomeActivity GlobalContext;
        private string Price, PayType, Credits, Id;
        private CashFreeObject CashFreeObject;

        public InitCashFreePayment(Activity context)
        {
            try
            {
                ActivityContext = context;
                GlobalContext = HomeActivity.GetInstance();

                CFPaymentGatewayService.Initialize(context); // Application Context.
                AnalyticsUtil.SendPaymentEventsToBackend(); // required for error reporting.

                CFPaymentGatewayService.Instance?.SetCheckoutCallback(this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void DisplayCashFreePayment(CashFreeObject cashFreeObject, string price, string payType, string credits, string id)
        {
            ActivityContext.RunOnUiThread(() =>
            {
                try
                {
                    CashFreeObject = cashFreeObject;
                    Price = price; PayType = payType; Credits = credits; Id = id;

                    CFSession.Environment cfEnvironment = ListUtils.SettingsSiteList?.CashfreeMode switch
                    {
                        "SandBox" => CFSession.Environment.Sandbox,
                        "Live" => CFSession.Environment.Production,
                        _ => CFSession.Environment.Sandbox
                    };

                    CFSession cfSession = new CFSession.CFSessionBuilder()
                        ?.SetEnvironment(cfEnvironment)
                        ?.SetPaymentSessionID(CashFreeObject.OrderLinkObject.PaymentSessionId)
                        ?.SetOrderId(CashFreeObject.OrderId)
                        ?.Build();

                    //CFPaymentComponent cfPaymentComponent = new CFPaymentComponent.CFPaymentComponentBuilder()
                    //    ?.Add(CFPaymentComponent.CFPaymentModes.Card)
                    //    ?.Add(CFPaymentComponent.CFPaymentModes.Upi)
                    //    ?.Add(CFPaymentComponent.CFPaymentModes.Wallet)
                    //    ?.Build();

                    CFTheme cfTheme = new CFTheme.CFThemeBuilder()
                        ?.SetNavigationBarBackgroundColor(AppSettings.MainColor)
                        ?.SetNavigationBarTextColor("#ffffff")
                        ?.SetButtonBackgroundColor(AppSettings.MainColor)
                        ?.SetButtonTextColor("#ffffff")
                        ?.SetPrimaryTextColor("#000000")
                        ?.SetSecondaryTextColor("#000000")
                        ?.Build();

                    CFDropCheckoutPayment cfDropCheckoutPayment = new CFDropCheckoutPayment.CFDropCheckoutPaymentBuilder()
                        ?.SetSession(cfSession)
                        //By default all modes are enabled. If you want to restrict the payment modes uncomment the next line
                        //?.SetCFUIPaymentModes(cfPaymentComponent)
                        ?.SetCFNativeCheckoutUITheme(cfTheme)
                        ?.Build();

                    CFPaymentGatewayService gatewayService = CFPaymentGatewayService.Instance;
                    gatewayService?.DoPayment(ActivityContext, cfDropCheckoutPayment);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            });
        }

        public void OnPaymentFailure(CFErrorResponse cfErrorResponse, string orderId)
        {
            try
            {
                //Error  
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnPaymentVerify(string orderId)
        {
            try
            {
                //verifyPayment triggered
                if (Methods.CheckConnectivity())
                {
                    if (PayType == "membership")
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetPro("CashFree") });
                    }
                    else
                    {
                        //wael update to new cashfree/success 
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GlobalContext?.SetCredit("CashFree") });

                        //var keyValues = new Dictionary<string, string>
                        //{
                        //    {"txStatus", CashFreeObject.TxStatus},
                        //    {"orderId", CashFreeObject.OrderId},
                        //    {"orderAmount", statusObject.OrderAmount},
                        //    {"referenceId", CashFreeObject.ReferenceId},
                        //    {"paymentMode", CashFreeObject.PaymentMode},
                        //    {"txMsg", CashFreeObject.TxMsg},
                        //    {"txTime", CashFreeObject.TxTime},
                        //    {"signature", CashFreeObject.Signature},
                        //};

                        //keyValues.Add("price", Price);

                        //var (apiStatus, respond) = await RequestsAsync.Payments.CashFreeAsync(keyValues);
                        //switch (apiStatus)
                        //{
                        //    case 200:
                        //        AndHUD.Shared.Dismiss();

                        //        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_PaymentSuccessfully), ToastLength.Short)?.Show();

                        //        StopCashFree();
                        //        break;
                        //    default:
                        //        Methods.DisplayAndHudErrorResult(ActivityContext, respond);
                        //        break;
                        //}  }

                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}