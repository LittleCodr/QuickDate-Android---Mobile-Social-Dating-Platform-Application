using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Bumptech.Glide.Util;
using Com.Google.Android.Gms.Ads;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Chat.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.IntegrationRecyclerView;
using QuickDate.SQLite;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Common;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Object = Java.Lang.Object;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Chat
{
    public class LastChatFragment : AndroidX.Fragment.App.Fragment, IOnClickListenerSelected, IDialogListCallBack
    {
        #region Variables Basic

        public LastChatAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private RecyclerViewOnScrollListener MainScrollEvent;
        public static ActionMode ActionMode;
        public static Toolbar ToolBar;
        private AdView MAdView;
        private ActionModeCallback ModeCallback;
        private static LastChatFragment Instance;
        private HomeActivity MainContext;
        private ImageView MoreButton;
        public static bool ApiRun;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainContext = HomeActivity.GetInstance();
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.TLastChatLayout, container, false);
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

                Instance = this;

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                GetLastChatLocal();

                AdsGoogle.Ad_Interstitial(Activity);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnResume()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Resume");
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                AdsGoogle.LifecycleAdView(MAdView, "Pause");
                base.OnPause();
                AddOrRemoveEvent(false);
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
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            {
                MAdapter?.UserList.Clear();
                MAdapter?.NotifyDataSetChanged();

                AdsGoogle.LifecycleAdView(MAdView, "Destroy");

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                var topLayout = view.FindViewById(Resource.Id.TopLayout);
                QuickDateTools.SetBackgroundTheme(topLayout);

                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                MoreButton = view.FindViewById<ImageView>(Resource.Id.moreButton);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                //SwipeRefreshLayout.SetPadding(5, 0, 0, 0);

                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, MRecycler);
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
                MAdapter = new LastChatAdapter(Activity);
                LayoutManager = new LinearLayoutManager(Context);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<GetConversationListObject.DataConversation>(this, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
                MAdapter.SetOnClickListener(this);

                ModeCallback = new ActionModeCallback(this);

                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += MainScrollEventOnLoadMoreEvent;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void InitToolbar(View view)
        {
            try
            {
                ToolBar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    var title = GetText(Resource.String.Lbl_Chats);
                    MainContext.SetToolBar(ToolBar, title, true, false);
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
                    MoreButton.Click += MoreButtonOnClick;
                    SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                }
                else
                {
                    MoreButton.Click -= MoreButtonOnClick;
                    SwipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static LastChatFragment GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        #endregion

        #region Scroll

        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Event Scroll #LastChat
                var item = MAdapter.UserList.LastOrDefault();
                if (item != null && MAdapter.UserList.Count > 10)
                {
                    StartApiService(item.Id.ToString());
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Toolbar & Selected

        public class ActionModeCallback : Object, ActionMode.ICallback
        {
            private readonly LastChatFragment Activity;
            public ActionModeCallback(LastChatFragment activity)
            {
                Activity = activity;
            }

            public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
            {
                int id = item.ItemId;
                switch (id)
                {
                    case Resource.Id.action_delete:
                        DeleteItems();
                        mode.Finish();
                        return true;
                    case Android.Resource.Id.Home:
                        HomeActivity.GetInstance()?.SetService();

                        Instance.MAdapter.ClearSelections();

                        ToolBar.Visibility = ViewStates.Visible;
                        ActionMode.Finish();

                        return true;
                    default:
                        return false;
                }
            }

            public bool OnCreateActionMode(ActionMode mode, IMenu menu)
            {
                SetSystemBarColor(Activity, AppSettings.MainColor);
                mode.MenuInflater.Inflate(Resource.Menu.menu_delete, menu);
                return true;
            }

            public void SetSystemBarColor(LastChatFragment act, string color)
            {
                try
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        Window window = act.Activity.Window;
                        window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        //window.SetStatusBarColor(Color.ParseColor(color));
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnDestroyActionMode(ActionMode mode)
            {
                try
                {
                    Instance.MAdapter.ClearSelections();
                    ActionMode.Finish();
                    ActionMode = null;

                    SetSystemBarColor(Activity, AppSettings.MainColor);

                    HomeActivity.GetInstance()?.SetService();

                    ToolBar.Visibility = ViewStates.Visible;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
            {
                return false;
            }

            //Delete Chat 
            private void DeleteItems()
            {
                try
                {
                    HomeActivity.GetInstance()?.SetService();

                    if (ToolBar.Visibility != ViewStates.Visible)
                        ToolBar.Visibility = ViewStates.Visible;

                    if (Methods.CheckConnectivity())
                    {
                        List<int> selectedItemPositions = Instance.MAdapter.GetSelectedItems();
                        for (int i = selectedItemPositions.Count - 1; i >= 0; i--)
                        {
                            var datItem = Instance.MAdapter.GetItem(selectedItemPositions[i]);
                            if (datItem != null)
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.DeleteUserLastChat(datItem.User.Id.ToString());
                                dbDatabase.DeleteAllMessagesUser(UserDetails.UserId.ToString(), datItem.User.Id.ToString());


                                var index = Instance.MAdapter.UserList.IndexOf(Instance.MAdapter.UserList.FirstOrDefault(a => a.User.Id == datItem.User.Id));
                                if (index != -1)
                                {
                                    Instance.MAdapter.UserList.Remove(datItem);
                                    Instance.MAdapter.NotifyItemRemoved(index);
                                }

                                Instance.MAdapter.RemoveData();

                                //Send Api Delete 
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.DeleteMessagesAsync(datItem.User.Id.ToString()) });
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(Activity.Activity, Activity.Activity.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        private Intent MessagesBoxIntent;

        //Event 
        public void ItemClick(View view, GetConversationListObject.DataConversation obj, int pos)
        {
            try
            {
                if (MAdapter.GetSelectedItemCount() > 0) // Add Select  New Item 
                {
                    EnableActionMode(pos);
                }
                else
                {
                    HomeActivity.GetInstance()?.SetService();

                    if (ToolBar.Visibility != ViewStates.Visible)
                        ToolBar.Visibility = ViewStates.Visible;

                    // read the item which removes bold from the row >> event click open ChatBox by user id
                    var item = MAdapter.GetItem(pos);
                    if (item != null)
                    {
                        item.NewMessages = 0;
                        item.Seen = 1;
                        MessagesBoxIntent = new Intent(Activity, typeof(MessagesBoxActivity));
                        MessagesBoxIntent.PutExtra("UserId", item.User.Id.ToString());
                        MessagesBoxIntent.PutExtra("TypeChat", "LastChat");
                        MessagesBoxIntent.PutExtra("UserItem", JsonConvert.SerializeObject(item.User));

                        // Check if we're running on Android 5.0 or higher
                        if ((int)Build.VERSION.SdkInt < 23)
                        {
                            StartActivity(MessagesBoxIntent);
                            MAdapter.NotifyItemChanged(pos);
                        }
                        else
                        {
                            if (PermissionsController.CheckPermissionStorage(Activity))
                            {
                                StartActivity(MessagesBoxIntent);
                                MAdapter.NotifyItemChanged(pos);
                            }
                            else
                            {
                                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                                {
                                    //below android 13
                                    ActivityCompat.RequestPermissions(Activity, new[]
                                    {
                                        Manifest.Permission.ReadMediaImages,
                                        Manifest.Permission.ReadMediaVideo,
                                        Manifest.Permission.ReadMediaAudio,
                                        Manifest.Permission.ReadExternalStorage,
                                    }, 1300);
                                }
                                else if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                                {
                                    //below android 11
                                    ActivityCompat.RequestPermissions(Activity, new[]
                                    {
                                        Manifest.Permission.ReadExternalStorage,
                                        Manifest.Permission.WriteExternalStorage,
                                        Manifest.Permission.AccessMediaLocation,
                                    }, 1300);
                                }
                                else
                                {
                                    ActivityCompat.RequestPermissions(Activity, new[]
                                    {
                                        Manifest.Permission.ReadExternalStorage,
                                        Manifest.Permission.WriteExternalStorage,
                                        Manifest.Permission.AccessMediaLocation,
                                    }, 1300);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OpenMessagesBoxIntent()
        {
            try
            {
                if (MessagesBoxIntent != null)
                    StartActivity(MessagesBoxIntent);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ItemLongClick(View view, GetConversationListObject.DataConversation obj, int pos)
        {
            EnableActionMode(pos);
        }

        private void EnableActionMode(int position)
        {
            ActionMode ??= MainContext.StartActionMode(ModeCallback);
            ToggleSelection(position);
        }

        private void ToggleSelection(int position)
        {
            try
            {
                MAdapter.ToggleSelection(position);
                int count = MAdapter.GetSelectedItemCount();

                if (count == 0)
                {
                    HomeActivity.GetInstance()?.SetService();

                    ToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();
                }
                else
                {
                    HomeActivity.GetInstance()?.SetService(false);

                    ToolBar.Visibility = ViewStates.Gone;
                    ActionMode.SetTitle(count);
                    ActionMode.Invalidate();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Events

        private void MoreButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(Context);

                dialogList.SetTitle(Resource.String.Lbl_More);

                //wael add after fix api
                //arrayAdapter.Add(GetText(Resource.String.Lbl_MessagesRequests));
                arrayAdapter.Add(GetText(Resource.String.Lbl_DeleteAllChat));

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetNegativeButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Refresh
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MainScrollEvent.IsLoading = false;
                ApiRun = false;
                MAdapter.UserList.Clear();
                MAdapter.NotifyDataSetChanged();

                SqLiteDatabase database = new SqLiteDatabase();
                database.ClearLastChat();
                database.ClearAll_Messages();

                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Load Data Api 

        private void GetLastChatLocal()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var list = dbDatabase.GetAllLastChat();
                if (list.Count > 0)
                {
                    MAdapter.UserList = new ObservableCollection<GetConversationListObject.DataConversation>(list);
                    MAdapter.NotifyDataSetChanged();
                }
                else
                {
                    Activity?.RunOnUiThread(() => { SwipeRefreshLayout.Refreshing = true; });
                }

                Task.Factory.StartNew(() => StartApiService());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void StartApiService(string offset = "0")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadDataAsync(offset) });
        }

        public async Task LoadDataAsync(string offset = "0")
        {
            if (MainScrollEvent != null && MainScrollEvent.IsLoading)
                return;

            if (Methods.CheckConnectivity())
            {
                ApiRun = true;

                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = true;

                int countList = MAdapter.UserList.Count;

                var (apiStatus, respond) = await RequestsAsync.Chat.GetConversationListAsync("15", offset);
                if (apiStatus != 200 || respond is not GetConversationListObject result || result.Data == null)
                {
                    ApiRun = false;
                    if (MainScrollEvent != null)
                        MainScrollEvent.IsLoading = false;
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        if (countList > 0)
                        {
                            LoadDataLastChatNewV(result);
                        }
                        else
                        {
                            ApiRun = false;
                            MAdapter.UserList = new ObservableCollection<GetConversationListObject.DataConversation>(result.Data);
                            Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (MAdapter.UserList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short)?.Show();
                    }
                }

                MainScrollEvent.IsLoading = false;
                Activity?.RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
            }
        }

        public void LoadDataLastChatNewV(GetConversationListObject result)
        {
            try
            {
                var countList = MAdapter.UserList.Count;
                var respondList = result.Data?.Count;
                if (respondList > 0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            bool add = false;
                            foreach (var item in result.Data)
                            {
                                var checkUser = MAdapter?.UserList?.FirstOrDefault(a => a.Id == item.Id);

                                int index = -1;
                                if (checkUser != null)
                                    index = MAdapter.UserList.IndexOf(checkUser);

                                if (checkUser == null)
                                {
                                    add = true;

                                    if (countList > 0)
                                    {
                                        MAdapter?.UserList?.Insert(0, item);
                                    }
                                    else
                                    {
                                        MAdapter?.UserList?.Add(item);
                                    }
                                }
                                else
                                {
                                    add = false;

                                    checkUser.Seen = item.Seen;
                                    checkUser.Time = item.Time;
                                    checkUser.CreatedAt = item.CreatedAt;

                                    if (checkUser.Text != item.Text || checkUser.Media != item.Media || checkUser.Sticker != item.Sticker)
                                    {
                                        checkUser = item;
                                        checkUser.Id = item.Id;
                                        checkUser.Owner = item.Owner;
                                        checkUser.Text = item.Text;
                                        checkUser.Media = item.Media;
                                        checkUser.Sticker = item.Sticker;
                                        checkUser.CreatedAt = item.CreatedAt;
                                        checkUser.NewMessages = item.NewMessages;
                                        checkUser.MessageType = item.MessageType;
                                        checkUser.Accepted = item.Accepted;
                                        checkUser.ConversationStatus = item.ConversationStatus;
                                        checkUser.FromId = item.FromId;
                                        checkUser.ToId = item.ToId;
                                        checkUser.User = item.User;
                                        checkUser.ConversationCreatedAt = item.ConversationCreatedAt;

                                        if (index > 0)
                                        {
                                            MAdapter.UserList[index] = checkUser;

                                            MAdapter?.UserList?.Move(index, 0);

                                            Activity?.RunOnUiThread(() =>
                                            {
                                                MAdapter?.NotifyItemMoved(index, 0);
                                                MAdapter?.NotifyItemChanged(0, "WithoutBlobText");
                                            });
                                        }
                                        else
                                        {
                                            if (index == 0)
                                            {
                                                MAdapter.UserList[index] = checkUser;
                                                Activity?.RunOnUiThread(() => { MAdapter?.NotifyItemChanged(index, "WithoutBlobText"); });
                                            }
                                        }
                                    }
                                }
                            }

                            //sort by time
                            //var list = MAdapter.UserList.OrderByDescending(o => o.LastChat?.ChatTime).ToList();
                            //MAdapter.UserList = new ObservableCollection<Classes.LastChatsClass>(list);

                            if (add)
                                Activity?.RunOnUiThread(() => { MAdapter?.NotifyDataSetChanged(); });

                            Activity?.RunOnUiThread(ShowEmptyPage);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);

                        }
                    }); 
                }
            }
            catch (Exception e)
            {
                Activity?.RunOnUiThread(ShowEmptyPage);
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                ApiRun = false;
                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = false;

                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.UserList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.InsertOrReplaceLastChatTable(new ObservableCollection<GetConversationListObject.DataConversation>(MAdapter?.UserList));
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    Inflated ??= EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoMessage);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                ApiRun = false;
                if (MainScrollEvent != null)
                    MainScrollEvent.IsLoading = false;

                SwipeRefreshLayout.Refreshing = false;
                Methods.DisplayReportResultTrack(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                if (itemString == GetString(Resource.String.Lbl_DeleteAllChat))
                {
                    if (MAdapter.UserList.Count == 0)
                        return;

                    var list = MAdapter.UserList;
                    foreach (var conversation in list)
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.DeleteMessagesAsync(conversation.User?.Id.ToString()) });
                    }

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.ClearLastChat();
                    database.ClearAll_Messages();

                    MAdapter.UserList.Clear();
                    MAdapter.NotifyDataSetChanged();

                    Activity?.RunOnUiThread(ShowEmptyPage);
                }
                //else if (itemString == GetString(Resource.String.Lbl_MessagesRequests))
                //{ 
                //    var intent = new Intent(Context, typeof(MessagesRequestsActivity)); 
                //    StartActivity(intent);
                //} 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public static void LoadCall(Context context, dynamic respond)
        {
            try
            {
                var dataSettings = ListUtils.SettingsSiteList;

                var videoCall = dataSettings?.VideoChat == "1";
                var audioCall = dataSettings?.AudioChat == "1";

                if (respond == null || CallConstant.CallActive)
                    return;

                if (respond is GetNotificationsObject chatObject)
                {
                    string typeCalling = "";
                    DataCallObject callUser = null!;
                    string title = Application.Context.GetText(Resource.String.Lbl_Voice_call);

                    var dataVideoCall = chatObject.VideoCall?.DatumClass;
                    var dataAudioCall = chatObject.AudioCall?.DatumClass;

                    if (dataSettings?.TwilioChatCall == "1")
                    {
                        if (dataVideoCall != null && videoCall)
                        {
                            typeCalling = "Twilio_video_call";
                            callUser = dataVideoCall;
                            title = Application.Context.GetText(Resource.String.Lbl_Video_call);
                        }
                        else if (dataAudioCall != null && audioCall)
                        {
                            typeCalling = "Twilio_audio_call";
                            callUser = dataAudioCall;
                            title = Application.Context.GetText(Resource.String.Lbl_Voice_call);
                        }
                    }
                    else if (dataSettings?.AgoraChatCall == "1")
                    {
                        if (dataVideoCall != null && videoCall)
                        {
                            typeCalling = "Agora_video_call_recieve";
                            callUser = dataVideoCall;
                            title = Application.Context.GetText(Resource.String.Lbl_Video_call);
                        }
                        else if (dataAudioCall != null && audioCall)
                        {
                            typeCalling = "Agora_audio_call_recieve";
                            callUser = dataAudioCall;
                            title = Application.Context.GetText(Resource.String.Lbl_Voice_call);
                        }
                    }

                    if (callUser != null && !string.IsNullOrEmpty(typeCalling))
                    {
                        var check = CallConstant.CallUserList.FirstOrDefault(a => a.Id == callUser.Id);
                        if (check != null)
                            return;

                        Intent intent = new Intent(context, typeof(CallingService));
                        intent.PutExtra("callUserObject", JsonConvert.SerializeObject(callUser));
                        intent.PutExtra("type", typeCalling);
                        intent.PutExtra("title", title);

                        intent.SetAction(CallingService.ActionStartIncoming);
                        context.StartService(intent);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                CallConstant.CallActive = false;
            }
        }

    }
}