using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using Com.Flutterwave.Raveandroid;
using Com.Flutterwave.Raveandroid.Rave_core.Models;
using Com.Flutterwave.Raveandroid.Rave_java_commons;
using Com.Flutterwave.Raveandroid.Rave_presentation;
using Com.Flutterwave.Raveandroid.Rave_presentation.Card;
using Com.Flutterwave.Raveandroid.Rave_presentation.Data;
using Com.Flutterwave.Raveutils.Verification;
using Com.Stripe.Android.View;
using QuickDate.Activities.Base;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;
using Math = System.Math;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.PaymentUtil
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class FlutterWaveActivity : BaseActivity, ICardPaymentCallback, ISavedCardsListener
    {
        #region Variables Basic

        private TextView CardNumber, CardExpire, CardCvv, CardName;
        private EditText EtName;
        private AppCompatButton BtnApply;
        private CardMultilineWidget MultilineWidget;

        private string Price, PayType;
        private CardPaymentManager CardPayManager;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Create your application here
                SetContentView(Resource.Layout.PaymentCardDetailsLayout);

                Price = Intent?.GetStringExtra("Price") ?? "";
                PayType = Intent?.GetStringExtra("PayType") ?? "";

                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                CardNumber = (TextView)FindViewById(Resource.Id.card_number);
                CardExpire = (TextView)FindViewById(Resource.Id.card_expire);
                CardCvv = (TextView)FindViewById(Resource.Id.card_cvv);
                CardName = (TextView)FindViewById(Resource.Id.card_name);

                MultilineWidget = (CardMultilineWidget)FindViewById(Resource.Id.card_multiline_widget);

                EtName = (EditText)FindViewById(Resource.Id.et_name);
                BtnApply = (AppCompatButton)FindViewById(Resource.Id.ApplyButton);

                Methods.SetColorEditText(EtName, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetString(Resource.String.Lbl_FlutterWave);
                    toolBar.SetTitleTextColor(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    var icon = AppCompatResources.GetDrawable(this, AppSettings.FlowDirectionRightToLeft ? Resource.Drawable.icon_back_arrow_right : Resource.Drawable.icon_back_arrow_left);
                    icon?.SetTint(QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                    SupportActionBar.SetHomeAsUpIndicator(icon);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    MultilineWidget.CvcEditText.AfterTextChanged += CvcEditTextOnAfterTextChanged;
                    EtName.TextChanged += EtNameOnTextChanged;
                    BtnApply.Click += BtnApplyOnClick;
                }
                else
                {
                    MultilineWidget.CvcEditText.AfterTextChanged -= CvcEditTextOnAfterTextChanged;
                    EtName.TextChanged -= EtNameOnTextChanged;
                    BtnApply.Click -= BtnApplyOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void CvcEditTextOnAfterTextChanged(object sender, StripeEditText.AfterTextChangedEventArgs e)
        {
            try
            {
                if (MultilineWidget.CardParams != null && MultilineWidget.ValidateAllFields())
                {
                    var cardNumber = MultilineWidget.CardNumberEditText.Text;
                    var cardExpire = MultilineWidget.ExpiryDateEditText.Text;
                    var cardCvv = MultilineWidget.CvcEditText.Text;

                    CardNumber.Text = cardNumber.Trim().Length == 0 ? "**** **** **** ****" : InsertPeriodically(cardNumber.Trim(), " ", 4);
                    CardExpire.Text = cardExpire.Trim().Length == 0 ? "MM/YY" : cardExpire;
                    CardCvv.Text = cardCvv.Trim().Length == 0 ? "***" : cardCvv.Trim();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        private void EtNameOnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CardName.Text = e?.Text?.ToString()?.Trim().Length == 0 ? GetString(Resource.String.Lbl_YourName) : e?.Text?.ToString()?.Trim();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //AuthorizeNet
        private void BtnApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MultilineWidget.ValidateAllFields() && !string.IsNullOrEmpty(EtName.Text))
                {
                    var cardNumber = MultilineWidget.CardNumberEditText.Text.Replace(" ", "");
                    var cardExpire = MultilineWidget.ExpiryDateEditText.Text;

                    var ExpMonth = cardExpire.Split("/").First();
                    var ExpYear = cardExpire.Split("/").Last();

                    var cardCvv = MultilineWidget.CvcEditText.Text;

                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var local = ListUtils.MyUserInfo?.FirstOrDefault();
                    if (local != null)
                    {
                        //.AcceptMpesaPayments(isMpesaSwitch.isChecked())
                        //.acceptAccountPayments(accountSwitch.isChecked())
                        //.acceptCardPayments(cardSwitch.isChecked())
                        //.allowSaveCardFeature(allowSavedCardsSwitch.isChecked(), true)
                        //.acceptAchPayments(accountAchSwitch.isChecked())
                        //.acceptGHMobileMoneyPayments(ghMobileMoneySwitch.isChecked())
                        //.acceptUgMobileMoneyPayments(ugMobileMoneySwitch.isChecked())
                        //.acceptZmMobileMoneyPayments(zmMobileMoneySwitch.isChecked())
                        //.acceptRwfMobileMoneyPayments(rwfMobileMoneySwitch.isChecked())
                        //.acceptUkPayments(ukbankSwitch.isChecked())
                        //.acceptSaBankPayments(saBankSwitch.isChecked())
                        //.acceptFrancMobileMoneyPayments(francMobileMoneySwitch.isChecked(), countryEt.getText().toString())
                        //.acceptBankTransferPayments(bankTransferSwitch.isChecked())
                        //.acceptUssdPayments(ussdSwitch.isChecked())
                        //.acceptBarterPayments(barterSwitch.isChecked())
                        //.withTheme(R.style.TestNewTheme)
                        //.ShowStagingLabel(shouldShowStagingLabelSwitch.isChecked())

                        var raveManager = new RaveNonUIManager()
                            .SetAmount(double.Parse(Price))
                            ?.SetCurrency(AppSettings.FlutterWaveCurrency)
                            ?.SetEmail(local.Email)
                            ?.SetfName(local.FirstName)
                            ?.SetlName(local.LastName)
                            ?.SetPhoneNumber(local.PhoneNumber)
                            ?.SetNarration(AppSettings.ApplicationName)
                            ?.SetPublicKey(AppSettings.FlutterWavePublicKey)
                            ?.SetEncryptionKey(AppSettings.FlutterWaveEncryptionKey)
                            ?.SetTxRef(UserDetails.UserId + "_" + Methods.Time.CurrentTimeMillis())
                            ?.OnStagingEnv(false)
                            ?.SetSubAccounts(new List<SubAccount>())
                            ?.SetMeta(new List<Meta>()
                            {
                                new Meta("price", Price)
                            })
                            //?.SetUniqueDeviceId("1")
                            //?.AcceptBankTransferPayments(true, true)
                            ?.InvokeIsPreAuth(true);

                        raveManager?.Initialize();

                        CardPayManager = new CardPaymentManager(raveManager, this, this);
                        var card = new Card(cardNumber, ExpMonth, ExpYear, cardCvv);
                        //cardPayManager.fetchSavedCards();
                        //cardPayManager.fetchTransactionFee(card,this);
                        CardPayManager.ChargeCard(card);
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_PleaseVerifyDataCard), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private string InsertPeriodically(string text, string insert, int period)
        {
            try
            {
                var parts = SplitInParts(text, period);
                string formatted = string.Join(insert, parts);
                return formatted;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return text;
            }
        }

        public static IEnumerable<string> SplitInParts(string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        #endregion

        #region Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                switch (requestCode)
                {
                    case RaveConstants.PinRequestCode:
                        string pin = data.GetStringExtra(PinFragment.ExtraPin);
                        // Use the collected PIN
                        CardPayManager.SubmitPin(pin);
                        break;
                    case RaveConstants.AddressDetailsRequestCode:
                        string streetAddress = data.GetStringExtra(AVSVBVFragment.ExtraAddress);
                        string state = data.GetStringExtra(AVSVBVFragment.ExtraState);
                        string city = data.GetStringExtra(AVSVBVFragment.ExtraCity);
                        string zipCode = data.GetStringExtra(AVSVBVFragment.ExtraZipcode);
                        string country = data.GetStringExtra(AVSVBVFragment.ExtraCountry);
                        AddressDetails address = new AddressDetails(streetAddress, city, state, zipCode, country);

                        // Use the address details
                        CardPayManager.SubmitAddress(address);
                        break;
                    case RaveConstants.OtpRequestCode:
                        String otp = data.GetStringExtra(OTPFragment.ExtraOtp);
                        // Use OTP
                        CardPayManager.SubmitOtp(otp);
                        break;
                    case RaveConstants.RaveRequestCode when data != null:
                        {
                            string message = data.GetStringExtra("response");

                            if (message != null)
                            {
                                Console.WriteLine("rave response", message);
                            }

                            if ((int)resultCode == RavePayActivity.ResultSuccess)
                            {
                                Toast.MakeText(this, "SUCCESS " + message, ToastLength.Long)?.Show();
                            }
                            else if ((int)resultCode == RavePayActivity.ResultError)
                            {
                                Toast.MakeText(this, "ERROR " + message, ToastLength.Long)?.Show();
                            }
                            else if ((int)resultCode == RavePayActivity.ResultCancelled)
                            {
                                Toast.MakeText(this, "CANCELLED " + message, ToastLength.Long)?.Show();
                            }
                        }
                        break;
                    case RaveConstants.WebVerificationRequestCode:
                        {
                            CardPayManager.OnWebpageAuthenticationComplete();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        #endregion

        #region Flutterwave

        public void CollectAddress()
        {
            try
            {
                Console.WriteLine("Submitting address details");
                new RaveVerificationUtils(this, false, AppSettings.FlutterWavePublicKey).ShowAddressScreen();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void CollectCardPin()
        {
            try
            {
                Console.WriteLine("CollectCardPin");
                new RaveVerificationUtils(this, false, AppSettings.FlutterWavePublicKey).ShowPinScreen();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void CollectOtp(string message)
        {
            try
            {
                Console.WriteLine("Otp for saved card");
                new RaveVerificationUtils(this, false, AppSettings.FlutterWavePublicKey).ShowOtpScreen(message);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnError(string errorMessage, string flwRef)
        {
            try
            {
                AndHUD.Shared.Dismiss();
                Toast.MakeText(this, errorMessage, ToastLength.Short)?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnSuccessful(string flwRef)
        {
            try
            {
                Console.WriteLine("Transaction Successful");

                //CardPayManager.SaveCard(); // Save card if needed

                //wael change to api after fix error 
                if (PayType == "membership")
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => HomeActivity.GetInstance()?.SetPro("FlutterWave") });
                }
                else
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => HomeActivity.GetInstance()?.SetCredit("FlutterWave") });
                }

                AndHUD.Shared.Dismiss();
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowAuthenticationWebPage(string authenticationUrl)
        {
            try
            {
                Console.WriteLine("Loading auth web page");

                // Load webpage
                new RaveVerificationUtils(this, false, AppSettings.FlutterWavePublicKey).ShowWebpageVerificationScreen(authenticationUrl);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ShowProgressIndicator(bool active)
        {
            try
            {
                if (active)
                {
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));
                }
                else
                {
                    AndHUD.Shared.Dismiss();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }



        public void CollectOtpForSaveCardCharge()
        {
            try
            {
                CollectOtp("Otp for saved card");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnCardSaveFailed(string message)
        {

        }

        public void OnCardSaveSuccessful(string phoneNumber)
        {

        }

        public void OnDeleteSavedCardRequestFailed(string message)
        {

        }

        public void OnDeleteSavedCardRequestSuccessful()
        {

        }

        public void OnSavedCardsLookupFailed(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short)?.Show();
        }

        public void OnSavedCardsLookupSuccessful(IList<SavedCard> cards, string phoneNumber)
        {
            try
            {
                // Check that the list is not empty, show the user to select which they'd like to charge, then proceed to chargeSavedCard()
                if (cards.Count != 0) CardPayManager.ChargeSavedCard(cards.First());
                else
                    Console.WriteLine("No saved cards found for " + phoneNumber);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion


    }
}