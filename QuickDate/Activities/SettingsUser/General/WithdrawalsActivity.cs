using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Content.Res;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Dialog;
using QuickDate.Activities.Base;
using QuickDate.Activities.SettingsUser.Adapters;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class WithdrawalsActivity : BaseActivity, IDialogListCallBack
    {
        #region Variables Basic

        private TextView CountBalanceText, SendText;
        private EditText AmountEditText, PayPalEmailEditText;
        private EditText TxtWithdrawMethod, TxtAccountNumber, TxtCountry, TxtAccountName, TxtSwiftCode, TxtAddress;
        private LinearLayout LayoutPayPalEmail, LayoutBank;
        private double CountBalance;
        private LinearLayout PaymentHistoryLinear;
        private TextView IconPaymentHistory;
        private RecyclerView MRecycler;
        private PaymentHistoryAdapter MAdapter;
        private string TypeDialog, TypeWithdrawMethod = "paypal";

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
                SetContentView(Resource.Layout.WithdrawalsLayout);
                //Get Value And Set Toolbar
                InitComponent();
                InitBackground();
                InitToolbar();
                SetRecyclerViewAdapters();

                Get_Data_User();
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

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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
                SendText = FindViewById<TextView>(Resource.Id.toolbar_title);
                SendText.SetTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);

                CountBalanceText = FindViewById<TextView>(Resource.Id.countBalanceText);

                AmountEditText = FindViewById<EditText>(Resource.Id.AmountEditText);
                TxtWithdrawMethod = FindViewById<EditText>(Resource.Id.WithdrawMethodEditText);

                LayoutPayPalEmail = FindViewById<LinearLayout>(Resource.Id.LayoutPayPalEmail);
                PayPalEmailEditText = FindViewById<EditText>(Resource.Id.PayPalEmailEditText);

                LayoutBank = FindViewById<LinearLayout>(Resource.Id.LayoutBank);
                TxtAccountNumber = FindViewById<EditText>(Resource.Id.AccountNumberEditText);

                TxtCountry = FindViewById<EditText>(Resource.Id.CountryEditText);

                TxtAccountName = FindViewById<EditText>(Resource.Id.AccountNameEditText);

                TxtSwiftCode = FindViewById<EditText>(Resource.Id.SwiftCodeEditText);

                TxtAddress = FindViewById<EditText>(Resource.Id.AddressEditText);

                PaymentHistoryLinear = (LinearLayout)FindViewById(Resource.Id.PaymentHistoryLinear);
                IconPaymentHistory = (TextView)FindViewById(Resource.Id.iconPaymentHistory);
                MRecycler = (RecyclerView)FindViewById(Resource.Id.recyler);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconPaymentHistory, FontAwesomeIcon.ListUl);

                Methods.SetColorEditText(TxtWithdrawMethod, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(AmountEditText, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(PayPalEmailEditText, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtAccountNumber, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtCountry, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtAccountName, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtSwiftCode, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtAddress, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                Methods.SetFocusable(TxtWithdrawMethod);
                Methods.SetFocusable(TxtCountry);

                TypeWithdrawMethod = "paypal";
                TxtWithdrawMethod.Text = GetText(Resource.String.Btn_Paypal);

                AdsGoogle.Ad_AdMobNative(this);
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
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_Withdrawals);
                    toolbar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
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

        private void SetRecyclerViewAdapters()
        {
            try
            {
                //Pro Recycler View 
                MAdapter = new PaymentHistoryAdapter(this)
                {
                    AffPaymentList = new ObservableCollection<AffPayment>()
                };
                MRecycler.SetLayoutManager(new LinearLayoutManager(this));
                MRecycler.SetItemViewCacheSize(20);
                MRecycler.HasFixedSize = true;
                MRecycler.NestedScrollingEnabled = false;
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.SetAdapter(MAdapter);
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
                    TxtWithdrawMethod.Touch += TxtWithdrawMethodOnTouch;
                    TxtCountry.Touch += TxtCountryOnTouch;

                    SendText.Click += SendTextOnClick;
                }
                else
                {
                    TxtWithdrawMethod.Touch -= TxtWithdrawMethodOnTouch;
                    TxtCountry.Touch -= TxtCountryOnTouch;

                    SendText.Click -= SendTextOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void TxtCountryOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                TypeDialog = "Country";

                var countriesArray = ListUtils.SettingsSiteList?.Countries;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                if (countriesArray != null) arrayAdapter.AddRange(countriesArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault()?.Name)));

                dialogList.SetTitle(GetText(Resource.String.Lbl_Country));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetPositiveButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void TxtWithdrawMethodOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action != MotionEventActions.Up) return;

                Methods.HideKeyboard(this);

                TypeDialog = "WithdrawMethod";

                var dialogList = new MaterialAlertDialogBuilder(this);

                var arrayAdapter = new List<string>
                {
                    GetText(Resource.String.Btn_Paypal), GetText(Resource.String.Lbl_Bank)
                };

                dialogList.SetTitle(GetText(Resource.String.Lbl_WithdrawMethod));
                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void SendTextOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CountBalance < Convert.ToDouble(AmountEditText.Text))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_ThereIsNoBalance), ToastLength.Long)?.Show();
                    return;
                }

                if (Convert.ToDouble(AmountEditText.Text) < Convert.ToDouble(ListUtils.SettingsSiteList?.MWithdrawal))
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CantRequestWithdrawals), ToastLength.Long)?.Show();
                    return;
                }

                switch (TypeWithdrawMethod)
                {
                    case "paypal" when string.IsNullOrEmpty(PayPalEmailEditText.Text.Replace(" ", "")) || string.IsNullOrEmpty(AmountEditText.Text.Replace(" ", "")):
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long)?.Show();
                        return;
                    case "bank" when string.IsNullOrEmpty(AmountEditText.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtAccountNumber.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtCountry.Text.Replace(" ", ""))
                                     || string.IsNullOrEmpty(TxtAccountName.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtSwiftCode.Text.Replace(" ", "")) || string.IsNullOrEmpty(TxtAddress.Text.Replace(" ", "")):
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long)?.Show();
                        return;
                }

                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"withdraw_method", TypeWithdrawMethod},
                        {"amount", AmountEditText.Text},
                    };

                    switch (TypeWithdrawMethod)
                    {
                        case "paypal":
                            dictionary.Add("paypal_email", PayPalEmailEditText.Text);
                            break;
                        case "bank":
                            dictionary.Add("iban", TxtAccountNumber.Text);
                            dictionary.Add("country", TxtCountry.Text);
                            dictionary.Add("full_name", TxtAccountName.Text);
                            dictionary.Add("swift_code", TxtSwiftCode.Text);
                            dictionary.Add("address", TxtAddress.Text);
                            break;
                    }

                    var (apiStatus, respond) = await RequestsAsync.Users.RequestWithdrawAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is InfoObject result)
                        {
                            Console.WriteLine(result.Message);
                            Toast.MakeText(this, GetText(Resource.String.Lbl_RequestSentWithdrawals), ToastLength.Long)?.Show();
                            AndHUD.Shared.Dismiss();
                        }
                    }
                    else
                    {
                        if (respond is ErrorObject errorMessage)
                        {
                            var errorText = errorMessage.ErrorData.ErrorText;
                            //Show a Error image with a message
                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));
                        }

                        //Methods.DisplayReportResult(this, respond);
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long)?.Show();
                }
            }
            catch (Exception exception)
            {
                AndHUD.Shared.Dismiss();
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                switch (TypeDialog)
                {
                    case "WithdrawMethod":
                        {
                            if (itemString == GetText(Resource.String.Btn_Paypal))
                            {
                                TypeWithdrawMethod = "paypal";
                                LayoutPayPalEmail.Visibility = ViewStates.Visible;
                                LayoutBank.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                TypeWithdrawMethod = "bank";

                                LayoutPayPalEmail.Visibility = ViewStates.Gone;
                                LayoutBank.Visibility = ViewStates.Visible;
                            }

                            TxtWithdrawMethod.Text = itemString;
                            break;
                        }
                    case "Country":
                        TxtCountry.Text = itemString;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        private async void Get_Data_User()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                    await ApiRequest.GetInfoData(this, UserDetails.UserId.ToString());

                var local = ListUtils.MyUserInfo?.FirstOrDefault();
                if (local != null)
                {
                    CountBalance = Convert.ToDouble(local.AffBalance);
                    CountBalanceText.Text = "$" + CountBalance.ToString(CultureInfo.InvariantCulture);

                    if (string.IsNullOrEmpty(local.PaypalEmail))
                        PayPalEmailEditText.Text = local.PaypalEmail;

                    if (local.AffPayments?.Count > 0)
                    {
                        MAdapter.AffPaymentList = new ObservableCollection<AffPayment>(local.AffPayments);

                        MAdapter.AffPaymentList.Insert(0, new AffPayment
                        {
                            Id = "000",
                            Amount = GetString(Resource.String.Lbl_Amount),
                            Time = GetString(Resource.String.Lbl_Requested),
                            Status = GetString(Resource.String.Lbl_Status)
                        });

                        MAdapter.NotifyDataSetChanged();
                    }
                    else
                    {
                        PaymentHistoryLinear.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}