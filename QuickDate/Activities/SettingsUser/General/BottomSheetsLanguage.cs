using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.BottomSheet;
using QuickDate.Adapters;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.SettingsUser.General
{
    public class BottomSheetsLanguage : BottomSheetDialogFragment
    {
        #region Variables Basic

        private TextView TitleText;
        private ImageView IconClose;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ItemOptionAdapter MAdapter;

        #endregion

        #region General

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Context contextThemeWrapper = QuickDateTools.IsTabDark() ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater?.Inflate(Resource.Layout.BottomSheetDefaultLayout, container, false);
                return view;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null!;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                base.OnViewCreated(view, savedInstanceState);
                InitComponent(view);
                SetRecyclerViewAdapters(view);

                LoadDataChat();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
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

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                IconClose = view.FindViewById<ImageView>(Resource.Id.iconClose);
                IconClose.Click += IconCloseOnClick;

                TitleText = view.FindViewById<TextView>(Resource.Id.titleHeader);
                TitleText.Text = GetText(Resource.String.Lbl_Display_your_app_Language);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetRecyclerViewAdapters(View view)
        {
            try
            {
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);

                MAdapter = new ItemOptionAdapter(Activity)
                {
                    ItemOptionList = new ObservableCollection<Classes.ItemOptionObject>()
                };
                MAdapter.ItemClick += MAdapterOnItemClick;
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.SetAdapter(MAdapter);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(50);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.GetRecycledViewPool().Clear();
                MRecycler.SetAdapter(MAdapter);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Event

        private void IconCloseOnClick(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void MAdapterOnItemClick(object sender, ItemOptionAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position > -1)
                {
                    var item = MAdapter.GetItem(position);
                    if (item.Id != AppSettings.Lang) //Create Ad
                    {
                        AppSettings.Lang = item.Id;
                        MainSettings.SharedData.Edit()?.PutString("Lang_key", AppSettings.Lang)?.Commit();

                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.remove_Settings();

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            Activity.Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            Activity.Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(Activity, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        intent.AddFlags(ActivityFlags.NoAnimation);
                        Activity.FinishAffinity();
                        Activity.OverridePendingTransition(0, 0);
                        StartActivity(intent);
                    }

                    Dismiss();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        private void LoadDataChat()
        {
            try
            {
                foreach (var lang in AppSettings.LanguageList)
                {
                    MAdapter.ItemOptionList.Add(new Classes.ItemOptionObject()
                    {
                        Id = lang.Key,
                        Text = lang.Value,
                        Icon = 0,
                    });
                }

                MAdapter.NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}