using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide.Util;
using Google.Android.Material.BottomSheet;
using QuickDate.Activities.Gift.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.IntegrationRecyclerView;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Common;
using QuickDateClient.Requests;
using System;
using System.Linq;

namespace QuickDate.Activities.Gift
{
    public class GiftDialogFragment : BottomSheetDialogFragment
    {
        #region Variables Basic

        private RecyclerView GiftRecyclerView;
        private GiftAdapter GiftAdapter;
        private LinearLayout LayoutPremium;
        private TextView TxtCountCart;
        private AppCompatButton BtnGetPremium, BtnBuyCredits;
        private string UserId;

        #endregion

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                UserId = Arguments?.GetString("UserId");
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
                View view = localInflater?.Inflate(Resource.Layout.ButtomSheetGift, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);

                InitComponent(view);
                SetRecyclerViewAdapters();

                BtnGetPremium.Click += BtnGetPremiumOnClick;
                BtnBuyCredits.Click += BtnBuyCreditsOnClick;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);

            }
        }

        #region Functions

        private void InitComponent(View contentView)
        {
            try
            {
                GiftRecyclerView = contentView.FindViewById<RecyclerView>(Resource.Id.rvGift);
                LayoutPremium = contentView.FindViewById<LinearLayout>(Resource.Id.LnyPremium);
                TxtCountCart = contentView.FindViewById<TextView>(Resource.Id.countCartTextView);
                BtnGetPremium = contentView.FindViewById<AppCompatButton>(Resource.Id.GetPremiumButton);
                BtnBuyCredits = contentView.FindViewById<AppCompatButton>(Resource.Id.BuyCreditsButton);

                var costPerGift = ListUtils.SettingsSiteList?.CostPerGift ?? "50";
                TxtCountCart.Text = GetText(Resource.String.Lbl_countCartGift) + " " + costPerGift + " " + GetText(Resource.String.Lbl_Credits);

                if (AppSettings.EnableAppFree)
                {
                    LayoutPremium.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (AppSettings.PremiumSystemEnabled)
                    {
                        BtnGetPremium.Visibility = UserDetails.IsPro == "0" ? ViewStates.Visible : ViewStates.Gone;
                    }
                    else
                    {
                        BtnGetPremium.Visibility = ViewStates.Gone;
                    }
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
                GiftRecyclerView.NestedScrollingEnabled = false;
                GiftAdapter = new GiftAdapter(Activity, "Normal");
                GiftAdapter.OnItemClick += GiftAdapterOnItemClick;

                var gridLayoutManager = new GridLayoutManager(Activity, 2);
                GiftRecyclerView.SetLayoutManager(gridLayoutManager);
                GiftRecyclerView.SetItemViewCacheSize(20);
                GiftRecyclerView.HasFixedSize = true;
                GiftRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<DataFile>(Activity, GiftAdapter, sizeProvider, 10);
                GiftRecyclerView.AddOnScrollListener(preLoader);
                GiftRecyclerView.SetAdapter(GiftAdapter);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void BtnBuyCreditsOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayCreditWindow();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnGetPremiumOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayPremiumWindow();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private async void GiftAdapterOnItemClick(object sender, GiftAdapterClickEventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo?.FirstOrDefault();
                if (!AppSettings.EnableAppFree && (dataUser?.Balance == "0.00" || dataUser?.Balance == "0.0" || dataUser?.Balance == "0"))
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_ErrorNotHaveCredit), ToastLength.Short)?.Show();
                    var window = new PopupController(Activity);
                    window.DisplayCreditWindow();
                    return;
                }

                int position = e.Position;
                if (position > -1)
                {
                    var item = GiftAdapter.GetItem(position);
                    if (item != null)
                    {
                        var (apiStatus, respond) = await RequestsAsync.Users.SendGiftAsync(UserId, item.Id.ToString());
                        if (apiStatus == 200)
                        {
                            if (respond is AmountObject result)
                            {
                                Activity?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        Toast.MakeText(Context, GetText(Resource.String.Lbl_SentSuccessfully), ToastLength.Short)?.Show();

                                        if (HomeActivity.GetInstance().ProfileFragment?.WalletNumber != null)
                                            HomeActivity.GetInstance().ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString();
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });

                                //Close Fragment 
                                Dismiss();
                            }
                        }
                        else Methods.DisplayReportResult(Activity, respond);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

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
    }
}