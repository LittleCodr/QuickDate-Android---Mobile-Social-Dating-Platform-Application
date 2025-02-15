using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Dialog;
using Me.Relex.CircleIndicatorLib;
using QuickDate.Activities;
using QuickDate.Activities.Premium;
using QuickDate.Activities.Premium.Adapters;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.PaymentGoogle;
using QuickDate.PaymentUtil.Utils;
using QuickDateClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = Java.Lang.Exception;

namespace QuickDate.Helpers.Controller
{
    public class PopupController
    {
        private readonly FragmentActivity ActivityContext;
        private readonly HomeActivity GlobalContext;
        private CreditAdapter CreditAdapter;
        private Dialog PremiumWindow, DialogAddCredits, AddPhoneNumberWindow;

        private EditText TxtNumber1, TxtNumber2;
        private string FullNumber;

        public PopupController(FragmentActivity context)
        {
            try
            {
                ActivityContext = context;
                GlobalContext = HomeActivity.GetInstance();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //============================ Phone Number ================================   

        #region Dialog >> Add Phone Number

        public void DisplayAddPhoneNumber()
        {
            try
            {
                var dataTwilio = ListUtils.SettingsSiteList;
                if (dataTwilio != null && string.IsNullOrEmpty(dataTwilio.SmsTwilioUsername) && string.IsNullOrEmpty(dataTwilio.SmsTwilioPassword) && string.IsNullOrEmpty(dataTwilio.SmsTPhoneNumber))
                    return;

                AddPhoneNumberWindow = new Dialog(ActivityContext, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AddPhoneNumberWindow?.SetContentView(Resource.Layout.DialogAddPhoneNumber);

                TxtNumber1 = AddPhoneNumberWindow?.FindViewById<EditText>(Resource.Id.numberEdit1); //Gone
                TxtNumber2 = AddPhoneNumberWindow?.FindViewById<EditText>(Resource.Id.numberEdit2);

                Methods.SetColorEditText(TxtNumber1, QuickDateTools.IsTabDark() ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtNumber2, QuickDateTools.IsTabDark() ? Color.White : Color.Black);

                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (!string.IsNullOrEmpty(dataUser?.PhoneNumber))
                {
                    var correctly = Methods.FunString.IsPhoneNumber(dataUser.PhoneNumber);
                    if (correctly)
                    {
                        TxtNumber2.Text = dataUser.PhoneNumber/*.TrimStart(new[] { '0' , '+' })*/;
                    }
                }

                FullNumber = TxtNumber2.Text/*.TrimStart(new[] { '0', '+' })*/;

                var btnAddPhoneNumber = AddPhoneNumberWindow?.FindViewById<AppCompatButton>(Resource.Id.sentButton);
                var btnSkipAddPhoneNumber = AddPhoneNumberWindow?.FindViewById<TextView>(Resource.Id.skipbutton);

                btnAddPhoneNumber.Click += BtnAddPhoneNumberOnClick;
                btnSkipAddPhoneNumber.Click += BtnSkipAddPhoneNumberOnClick;

                AddPhoneNumberWindow?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void BtnSkipAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                AddPhoneNumberWindow?.Hide();
                AddPhoneNumberWindow?.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                FullNumber = TxtNumber2.Text;

                if (Methods.FunString.IsPhoneNumber(FullNumber) && FullNumber.Length >= 10)
                {
                    if (!string.IsNullOrEmpty(FullNumber))
                    {
                        Intent intent = new Intent(ActivityContext, typeof(VerificationCodeActivity));
                        intent.PutExtra("Number", FullNumber);
                        ActivityContext.StartActivityForResult(intent, 125);

                        AddPhoneNumberWindow?.Hide();
                        AddPhoneNumberWindow?.Dismiss();
                    }
                }
                else
                {
                    var dialog = new MaterialAlertDialogBuilder(ActivityContext);
                    dialog.SetTitle(Resource.String.Lbl_Warning);
                    dialog.SetMessage(FullNumber + " - " + ActivityContext.GetText(Resource.String.Lbl_ISNotValidNumber));
                    dialog.SetNegativeButton(ActivityContext.GetText(Resource.String.Lbl_Ok), new MaterialDialogUtils());

                    dialog.Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //============================ Upgrade ================================   

        #region Dialog >> Upgrade

        public void DisplayPremiumWindow()
        {
            if (!AppSettings.PremiumSystemEnabled)
                return;
            try
            {
                PremiumWindow = new Dialog(ActivityContext, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                PremiumWindow?.SetContentView(Resource.Layout.UpgradePremiumLayout);

                var btnWeeklyLayout = PremiumWindow?.FindViewById<RelativeLayout>(Resource.Id.btnWeeklyLayout);
                var btnMonthlyLayout = PremiumWindow?.FindViewById<RelativeLayout>(Resource.Id.btnMonthlyLayout);
                var btnYearlyLayout = PremiumWindow?.FindViewById<RelativeLayout>(Resource.Id.btnYearlyLayout);
                var btnLifetimeLayout = PremiumWindow?.FindViewById<RelativeLayout>(Resource.Id.btnLifetimeLayout);

                var saveWeekly = PremiumWindow?.FindViewById<TextView>(Resource.Id.SaveWeekly);
                var saveMonthly = PremiumWindow?.FindViewById<TextView>(Resource.Id.SaveMonthly);
                var saveYearly = PremiumWindow?.FindViewById<TextView>(Resource.Id.SaveYearly);
                var saveLifetime = PremiumWindow?.FindViewById<TextView>(Resource.Id.SaveLifetime);

                var priceWeekly = PremiumWindow?.FindViewById<TextView>(Resource.Id.PriceWeekly);
                var priceMonthly = PremiumWindow?.FindViewById<TextView>(Resource.Id.PriceMonthly);
                var priceYearly = PremiumWindow?.FindViewById<TextView>(Resource.Id.PriceYearly);
                var priceLifetime = PremiumWindow?.FindViewById<TextView>(Resource.Id.PriceLifetime);

                var option = ListUtils.SettingsSiteList;
                if (option != null)
                {
                    var currencySymbol = option.CurrencySymbol ?? "$";

                    saveWeekly.Text = ActivityContext.GetText(Resource.String.Lbl_Normal);
                    saveMonthly.Text = ActivityContext.GetText(Resource.String.Lbl_Save) + " 51%";
                    saveYearly.Text = ActivityContext.GetText(Resource.String.Lbl_Save) + " 90%";
                    saveLifetime.Text = ActivityContext.GetText(Resource.String.Lbl_PayOnesAccessForEver);

                    priceWeekly.Text = option.WeeklyProPlan + currencySymbol;
                    priceMonthly.Text = option.MonthlyProPlan + currencySymbol;
                    priceYearly.Text = option.YearlyProPlan + currencySymbol;
                    priceLifetime.Text = option.LifetimeProPlan + currencySymbol;

                    btnWeeklyLayout.Click += (sender, args) => PremiumAdapterOnItemClick("1", ActivityContext.GetText(Resource.String.Lbl_Weekly), option.WeeklyProPlan);
                    btnMonthlyLayout.Click += (sender, args) => PremiumAdapterOnItemClick("2", ActivityContext.GetText(Resource.String.Lbl_Monthly), option.MonthlyProPlan);
                    btnYearlyLayout.Click += (sender, args) => PremiumAdapterOnItemClick("3", ActivityContext.GetText(Resource.String.Lbl_Yearly), option.YearlyProPlan);
                    btnLifetimeLayout.Click += (sender, args) => PremiumAdapterOnItemClick("4", ActivityContext.GetText(Resource.String.Lbl_Lifetime), option.LifetimeProPlan);
                }

                var btnSkipAddCredits = PremiumWindow?.FindViewById<TextView>(Resource.Id.skippButton);
                btnSkipAddCredits.Click += BtnSkipAddCreditsOnClick;

                PremiumWindow?.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void BtnSkipAddCreditsOnClick(object sender, EventArgs e)
        {
            try
            {
                PremiumWindow?.Hide();
                PremiumWindow?.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        //Open walletFragment with Google 
        private void PremiumAdapterOnItemClick(string id, string typePro, string price)
        {
            try
            {
                if (!AppSettings.ShowPaypal && !AppSettings.ShowCreditCard && !AppSettings.ShowBankTransfer && !AppSettings.ShowInAppBilling && !InitializeQuickDate.IsExtended)
                    return;

                GlobalContext.Id = id;
                GlobalContext.Credits = typePro;
                GlobalContext.Price = price;
                GlobalContext.PayType = "membership";

                switch (AppSettings.PaymentsSystem)
                {
                    case PaymentsSystem.All:
                        {
                            Bundle bundle = new Bundle();

                            bundle.PutString("Id", GlobalContext.Id);
                            bundle.PutString("Credits", GlobalContext.Credits);
                            bundle.PutString("Price", GlobalContext.Price);
                            bundle.PutString("payType", "membership"); // credits|membership

                            PaymentXBottomSheetDialog bottomSheetDialog = new PaymentXBottomSheetDialog()
                            {
                                Arguments = bundle
                            };
                            bottomSheetDialog.Show(ActivityContext.SupportFragmentManager, bottomSheetDialog.Tag);

                            break;
                        }
                    case PaymentsSystem.JustInAppBillingGoogle when AppSettings.ShowInAppBilling && InitializeQuickDate.IsExtended:

                        string type = "";
                        switch (GlobalContext.Id)
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

                        if (!string.IsNullOrEmpty(type))
                            GlobalContext?.BillingSupport?.PurchaseNow(type);
                        break;
                }

                PremiumWindow?.Hide();
                PremiumWindow?.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //============================ Credit ================================   

        #region Dialog >> Credit

        public void DisplayCreditWindow()
        {
            try
            {
                DialogAddCredits = new Dialog(ActivityContext, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                DialogAddCredits.SetContentView(Resource.Layout.DialogAddCredits);

                var recyclerView = DialogAddCredits.FindViewById<RecyclerView>(Resource.Id.recyler);

                var viewPagerView = DialogAddCredits.FindViewById<ViewPager>(Resource.Id.viewPager);
                var indicator = DialogAddCredits.FindViewById<CircleIndicator>(Resource.Id.indicator);

                var titleText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainTitelText);
                titleText.Text = ActivityContext.GetText(Resource.String.Lbl_Your) + " " + AppSettings.ApplicationName + " " + ActivityContext.GetText(Resource.String.Lbl_CreditsBalance);

                var mainText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainText);
                var data = ListUtils.MyUserInfo?.FirstOrDefault();
                mainText.Text = data?.Balance.Replace(".00", "") + " " + ActivityContext.GetText(Resource.String.Lbl_Credits);

                var btnSkip = DialogAddCredits.FindViewById<TextView>(Resource.Id.skippButton);

                var creditsClass = new List<CreditsFeaturesClass>
                {
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits1), ColorCircle = "#00bee7",ImageFromResource = Resource.Drawable.viewPager_rocket},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits2), ColorCircle = "#0456C4" ,ImageFromResource = Resource.Drawable.viewPager_msg},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits3), ColorCircle = "#ff7102" ,ImageFromResource = Resource.Drawable.viewPager_gift},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits4), ColorCircle = "#4caf50" ,ImageFromResource = Resource.Drawable.viewPager_target},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits5), ColorCircle = "#8c4fe6" ,ImageFromResource = Resource.Drawable.viewPager_crown},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits6), ColorCircle = "#22e271" ,ImageFromResource = Resource.Drawable.viewPager_sticker},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits7), ColorCircle = "#f44336",ImageFromResource = Resource.Drawable.viewPager_heart}
                };

                var imageDescViewPager = new ImageDescViewPager(ActivityContext, creditsClass);
                viewPagerView.Adapter = imageDescViewPager;
                indicator.SetViewPager(viewPagerView);

                CreditAdapter = new CreditAdapter(ActivityContext);
                recyclerView.SetLayoutManager(new LinearLayoutManager(ActivityContext));
                CreditAdapter.OnItemClick += CreditAdapterOnItemClick;
                recyclerView.SetAdapter(CreditAdapter);

                btnSkip.Click += BtnSkipOnClick;
                DialogAddCredits.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Open walletFragment with Google
        private void CreditAdapterOnItemClick(object sender, CreditAdapterViewHolderClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    if (!AppSettings.ShowPaypal && !AppSettings.ShowCreditCard && !AppSettings.ShowBankTransfer && !AppSettings.ShowInAppBilling && !InitializeQuickDate.IsExtended)
                        return;

                    CreditsClass item = CreditAdapter.GetItem(position);
                    if (item != null)
                    {
                        GlobalContext.Id = item.Id.ToString();
                        GlobalContext.Credits = item.TotalCoins;
                        GlobalContext.Price = item.Price;
                        GlobalContext.PayType = "credits";

                        switch (AppSettings.PaymentsSystem)
                        {
                            case PaymentsSystem.All:
                                {
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("Id", item.Id.ToString());
                                    bundle.PutString("Credits", item.TotalCoins);
                                    bundle.PutString("Price", item.Price);
                                    bundle.PutString("payType", "credits"); // credits|membership

                                    PaymentXBottomSheetDialog bottomSheetDialog = new PaymentXBottomSheetDialog()
                                    {
                                        Arguments = bundle
                                    };
                                    bottomSheetDialog.Show(ActivityContext.SupportFragmentManager, bottomSheetDialog.Tag);

                                    break;
                                }
                            case PaymentsSystem.JustInAppBillingGoogle when AppSettings.ShowInAppBilling && InitializeQuickDate.IsExtended:
                                {
                                    var option = ListUtils.SettingsSiteList;

                                    string type = "";
                                    if (GlobalContext.Credits == option?.BagOfCreditsAmount)
                                        type = InAppBillingGoogle.BagOfCredits;
                                    else if (GlobalContext.Credits == option?.BoxOfCreditsAmount)
                                        type = InAppBillingGoogle.BoxofCredits;
                                    else if (GlobalContext.Credits == option?.ChestOfCreditsAmount)
                                        type = InAppBillingGoogle.ChestofCredits;

                                    if (!string.IsNullOrEmpty(type))
                                        GlobalContext?.BillingSupport?.PurchaseNow(type);
                                }
                                break;
                        }

                        DialogAddCredits?.Hide();
                        DialogAddCredits?.Dismiss();

                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnTermsOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", InitializeQuickDate.WebsiteUrl + "/terms");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_TermsOfUse));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnSkipOnClick(object sender, EventArgs e)
        {
            try
            {
                DialogAddCredits.Hide();
                DialogAddCredits.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }


        #endregion

        //////////////////////////////////////////////////////

    }
}