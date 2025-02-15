using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Utils;
using QuickDate.PaymentGoogle;
using QuickDateClient;
using System;

namespace QuickDate.PaymentUtil.Utils
{
    public class PaymentXBottomSheetDialog : BottomSheetDialogFragment
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private ImageView IconClose;
        private LinearLayout GooglePayLayout, PaypalLayout, CreditCardLayout, BankTransferLayout, RazorPayLayout, CashFreeLayout, PayStackLayout, PaySeraLayout;
        private LinearLayout SecurionPayLayout, AuthorizeNetLayout, IyziPayLayout, AamarPayLayout, FlutterWaveLayout;

        private string Price, Credits, PayType, IdPay;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your fragment here
                GlobalContext = HomeActivity.GetInstance();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                var contextThemeWrapper = QuickDateTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);

                // clone the inflater using the ContextThemeWrapper

                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);
                View view = localInflater?.Inflate(Resource.Layout.PaymentXBottomSheetLayout, container, false);

                IdPay = Arguments?.GetString("Id") ?? "";
                Credits = Arguments?.GetString("Credits") ?? "";
                Price = Arguments?.GetString("Price") ?? "";
                PayType = Arguments?.GetString("payType") ?? "";

                InitComponent(view);

                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                IconClose = view.FindViewById<ImageView>(Resource.Id.iconClose);
                IconClose.Click += IconCloseOnClick;

                GooglePayLayout = view.FindViewById<LinearLayout>(Resource.Id.GooglePayLayout);
                GooglePayLayout.Click += GooglePayLayoutOnClick;

                PaypalLayout = view.FindViewById<LinearLayout>(Resource.Id.PaypalLayout);
                PaypalLayout.Click += PaypalLayoutOnClick;

                CreditCardLayout = view.FindViewById<LinearLayout>(Resource.Id.CreditCardLayout);
                CreditCardLayout.Click += CreditCardLayoutOnClick;

                BankTransferLayout = view.FindViewById<LinearLayout>(Resource.Id.BankTransferLayout);
                BankTransferLayout.Click += BankTransferLayoutOnClick;

                RazorPayLayout = view.FindViewById<LinearLayout>(Resource.Id.RazorPayLayout);
                RazorPayLayout.Click += RazorPayLayoutOnClick;

                CashFreeLayout = view.FindViewById<LinearLayout>(Resource.Id.CashFreeLayout);
                CashFreeLayout.Click += CashFreeLayoutOnClick;

                PayStackLayout = view.FindViewById<LinearLayout>(Resource.Id.PayStackLayout);
                PayStackLayout.Click += PayStackLayoutOnClick;

                PaySeraLayout = view.FindViewById<LinearLayout>(Resource.Id.PaySeraLayout);
                PaySeraLayout.Click += PaySeraLayoutOnClick;

                SecurionPayLayout = view.FindViewById<LinearLayout>(Resource.Id.SecurionPayLayout);
                SecurionPayLayout.Click += SecurionPayLayoutOnClick;

                AuthorizeNetLayout = view.FindViewById<LinearLayout>(Resource.Id.AuthorizeNetLayout);
                AuthorizeNetLayout.Click += AuthorizeNetLayoutOnClick;

                IyziPayLayout = view.FindViewById<LinearLayout>(Resource.Id.IyziPayLayout);
                IyziPayLayout.Click += IyziPayLayoutOnClick;

                AamarPayLayout = view.FindViewById<LinearLayout>(Resource.Id.AamarPayLayout);
                AamarPayLayout.Click += AamarPayLayoutOnClick;

                FlutterWaveLayout = view.FindViewById<LinearLayout>(Resource.Id.FlutterWaveLayout);
                FlutterWaveLayout.Click += FlutterWaveLayoutOnClick;

                if (AppSettings.ShowInAppBilling && InitializeQuickDate.IsExtended)
                    GooglePayLayout.Visibility = ViewStates.Visible;
                else
                    GooglePayLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowPaypal)
                    PaypalLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowCreditCard)
                    CreditCardLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowBankTransfer)
                    BankTransferLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowRazorPay)
                    RazorPayLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowCashFree)
                    CashFreeLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowSecurionPay)
                    SecurionPayLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowAuthorizeNet)
                    AuthorizeNetLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowIyziPay)
                    IyziPayLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowPayStack)
                    PayStackLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowAamarPay)
                    AamarPayLayout.Visibility = ViewStates.Gone;

                if (!AppSettings.ShowFlutterWave)
                    FlutterWaveLayout.Visibility = ViewStates.Gone;

                //wael add after update
                PaySeraLayout.Visibility = ViewStates.Gone;

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        private void GooglePayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                string type = "";
                if (PayType == "membership")
                {
                    switch (IdPay)
                    {
                        case "1":
                            type = InAppBillingGoogle.MembershipWeekly;
                            break;
                        case "2":
                            type = InAppBillingGoogle.MembershipMonthly;
                            break;
                        case "3":
                            type = InAppBillingGoogle.MembershipYearly;
                            break;
                        case "4":
                            type = InAppBillingGoogle.MembershipLifetime;
                            break;
                    }
                }
                else
                {
                    var option = ListUtils.SettingsSiteList;
                    if (Credits == option?.BagOfCreditsAmount)
                    {
                        type = InAppBillingGoogle.BagOfCredits;
                    }
                    else if (Credits == option?.BoxOfCreditsAmount)
                    {
                        type = InAppBillingGoogle.BoxofCredits;
                    }
                    else if (Credits == option?.ChestOfCreditsAmount)
                    {
                        type = InAppBillingGoogle.ChestofCredits;
                    }
                }

                if (!string.IsNullOrEmpty(type))
                    GlobalContext.BillingSupport?.PurchaseNow(type);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void PaypalLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.InitPayPalPayment.BtnPaypalOnClick(Price, PayType, Credits, IdPay);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CreditCardLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(GlobalContext, typeof(PaymentCardDetailsActivity));
                intent.PutExtra("Id", IdPay);
                intent.PutExtra("credits", Credits);
                intent.PutExtra("Price", Price);
                intent.PutExtra("payType", PayType);
                GlobalContext.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BankTransferLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(GlobalContext, typeof(PaymentLocalActivity));
                intent.PutExtra("Id", IdPay);
                intent.PutExtra("credits", Credits);
                intent.PutExtra("Price", Price);
                intent.PutExtra("payType", PayType);
                GlobalContext.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void RazorPayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.InitRazorPay?.BtnRazorPayOnClick(Price);
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void CashFreeLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.OpenCashFreeDialog();
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void PayStackLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.OpenPayStackDialog();

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void PaySeraLayoutOnClick(object sender, EventArgs e)
        {
            try
            {

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void SecurionPayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Toast.MakeText(Activity, GetText(Resource.String.Lbl_Please_wait), ToastLength.Long)?.Show();

                await GlobalContext.OpenSecurionPay();

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void AuthorizeNetLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(GlobalContext, typeof(AuthorizeNetPaymentActivity));
                intent.PutExtra("Id", IdPay);
                intent.PutExtra("credits", Credits);
                intent.PutExtra("Price", Price);
                intent.PutExtra("payType", PayType);
                GlobalContext.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void IyziPayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.IyziPay();

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void AamarPayLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.AamarPayPayment?.BtnAamarPayOnClick(Price, PayType, Credits, IdPay);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void FlutterWaveLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(GlobalContext, typeof(FlutterWaveActivity));
                intent.PutExtra("Id", IdPay);
                intent.PutExtra("credits", Credits);
                intent.PutExtra("Price", Price);
                intent.PutExtra("payType", PayType);
                GlobalContext.StartActivity(intent);

                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void IconCloseOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

    }
}