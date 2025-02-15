using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Content.Res;
using AndroidX.AppCompat.Widget;
using AndroidX.Interpolator.View.Animation;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AT.Markushi.UI;
using Developer.SEmojis.Actions;
using Developer.SEmojis.Helper;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using QuickDate.Activities.Base;
using QuickDate.Activities.Call.Tools;
using QuickDate.Activities.Chat.Adapters;
using QuickDate.Activities.Chat.ChatBoxStates;
using QuickDate.Activities.Chat.Fragments;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using ActionMode = AndroidX.AppCompat.View.ActionMode;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using Object = Java.Lang.Object;
using SupportFragment = AndroidX.Fragment.App.Fragment;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace QuickDate.Activities.Chat
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MessagesBoxActivity : AppCompatActivity, IOnClickListenerSelectedMessages, IDialogListCallBack
    {
        #region Variables Basic

        private ImageView AudioCallButton, VideoCallButton, MoreButton;
        public TextView ActionBarTitle, ActionBarSubTitle;
        private ImageView UserChatProfile;

        private ImageView ChatEmojisImage;
        private RelativeLayout RootView;
        private EmojiconEditText EmojisIconEditTextView;
        private CircleButton ChatSendButton;
        private ImageView GiftButton, ImageButton;
        private static Toolbar TopChatToolBar;
        public RecyclerView ChatBoxRecyclerView;
        private LinearLayoutManager MLayoutManager;
        public UserMessagesAdapter MAdapter;
        private string LastSeenUser = "", TaskWork = "";
        private long BeforeMessageId, FirstMessageId;
        public static int Userid;// to_id
        private static Timer Timer;
        private SwipeRefreshLayout SwipeRefreshLayout;
        public UserInfoObject UserInfoData;
        private ActionModeCallback ModeCallback;
        private static ActionMode ActionMode;
        public FrameLayout TopFragmentHolder, ButtonFragmentHolder;
        public StickersFragment ChatStickersFragment;
        public GiftFragment ChatGiftFragment;
        private SupportFragment MainFragmentOpened;
        private FastOutSlowInInterpolator Interpolation;
        private HomeActivity GlobalContext;
        private static MessagesBoxActivity Instance;
        private TypeCall CallType;

        private ChatBoxButtonWindowState LastChatBoxButtonWindowState;
        private ViewGroup PremiumChatLayout, ChatBoxonButtom;
        private AppCompatButton ButtonGetPremium;
        

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Window?.SetSoftInputMode(SoftInput.AdjustResize);
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);

                // Set our view from the "MessagesBox_Layout" layout resource
                SetContentView(Resource.Layout.MessagesBoxLayout);

                Instance = this;

                Methods.Path.Chack_MyFolder();
                 
                var data = Intent?.GetStringExtra("UserId") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data)) Userid = int.Parse(data); // to_id

                var item = JsonConvert.DeserializeObject<UserInfoObject>(Intent?.GetStringExtra("UserItem") ?? "");
                if (item != null) UserInfoData = item;

                Interpolation = new FastOutSlowInInterpolator();

                ChatStickersFragment = new StickersFragment();
                ChatGiftFragment = new GiftFragment();

                Bundle args = new Bundle();
                args.PutString("userid", Userid.ToString());
                ChatStickersFragment.Arguments = args;
                ChatGiftFragment.Arguments = args;

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();
                GetStickersGiftsLists();
                CanDoChat();
                InitBackPressed();

                //Set Title ToolBar and data chat user After that get messages 
                loadData_ItemUser();

                AdsGoogle.Ad_RewardedVideo(this);

                GlobalContext = HomeActivity.GetInstance();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static MessagesBoxActivity GetInstance()
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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);

                if (Timer != null)
                {
                    Timer.Enabled = true;
                    Timer.Start();
                }


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

                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }


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
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                    Timer = null;
                }

                MAdapter?.MessageList.Clear();
                MAdapter?.NotifyDataSetChanged();



                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        MainSettings.ApplyTheme(MainSettings.LightMode);
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        MainSettings.ApplyTheme(MainSettings.DarkMode);
                        break;
                }

                Delegate.SetLocalNightMode(QuickDateTools.IsTabDark() ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);
                SetTheme(QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                RootView = FindViewById<RelativeLayout>(Resource.Id.rootChatWindowView);

                UserChatProfile = FindViewById<ImageView>(Resource.Id.userProfileImage);
                ActionBarTitle = FindViewById<TextView>(Resource.Id.Txt_Username);
                ActionBarSubTitle = FindViewById<TextView>(Resource.Id.Txt_last_time);

                AudioCallButton = FindViewById<ImageView>(Resource.Id.IconCall);
                VideoCallButton = FindViewById<ImageView>(Resource.Id.IconvideoCall);
                MoreButton = FindViewById<ImageView>(Resource.Id.IconMore);

                ChatEmojisImage = FindViewById<ImageView>(Resource.Id.emojiicon);
                EmojisIconEditTextView = FindViewById<EmojiconEditText>(Resource.Id.EmojiconEditText5);
                ChatSendButton = FindViewById<CircleButton>(Resource.Id.sendButton);
                ChatBoxRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyler);
                SwipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(QuickDateTools.IsTabDark() ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                GiftButton = FindViewById<ImageView>(Resource.Id.giftButton);
                ImageButton = FindViewById<ImageView>(Resource.Id.imageButton);
                TopFragmentHolder = FindViewById<FrameLayout>(Resource.Id.TopFragmentHolder);
                ButtonFragmentHolder = FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                PremiumChatLayout = FindViewById<ViewGroup>(Resource.Id.PremiumChatLayout);
                PremiumChatLayout.Visibility = ViewStates.Gone;
                ButtonGetPremium = FindViewById<AppCompatButton>(Resource.Id.ButtonGetPremium);
                ChatBoxonButtom = FindViewById<ViewGroup>(Resource.Id.chatBoxonButtom);
                SupportFragmentManager.BeginTransaction().Add(ButtonFragmentHolder.Id, ChatStickersFragment, "ChatStickersFragment");

                ChatSendButton.Tag = "Text";
                ChatSendButton.SetImageResource(Resource.Drawable.ic_send_vector);

                ModeCallback = new ActionModeCallback(this);

                var emojisIcon = new EmojIconActions(this, RootView, EmojisIconEditTextView, ChatEmojisImage);
                emojisIcon.ShowEmojIcon();

                if (!AppSettings.EnableAppFree)
                {
                    var dataSettings = ListUtils.SettingsSiteList;
                    if (dataSettings?.AvcallPro == "1") //just pro user can chat
                    {
                        var dataUser = ListUtils.MyUserInfo?.FirstOrDefault()?.IsPro;
                        if (dataUser == "0") // Not Pro remove call
                        {
                            VideoCallButton.Visibility = ViewStates.Invisible;
                            AudioCallButton.Visibility = ViewStates.Invisible;
                        }
                    }
                    else //all users can chat
                    {
                        if (dataSettings?.VideoChat == "0")
                        {
                            VideoCallButton.Visibility = ViewStates.Invisible;
                        }

                        if (dataSettings?.AudioChat == "0")
                        {
                            AudioCallButton.Visibility = ViewStates.Invisible;
                        }
                    }
                }
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
                TopChatToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (TopChatToolBar != null)
                {
                    TopChatToolBar.SetTitleTextColor(QuickDateTools.IsTabDark() ? AppSettings.TitleTextColorDark : AppSettings.TitleTextColor);
                    SetSupportActionBar(TopChatToolBar);
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

        private void InitBackPressed()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    OnBackInvokedDispatcher.RegisterOnBackInvokedCallback(0, new BackCallAppBase2(this, "MessagesBoxActivity"));
                }
                else
                {
                    OnBackPressedDispatcher.AddCallback(new BackCallAppBase1(this, "MessagesBoxActivity", true));
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

                MAdapter = new UserMessagesAdapter(this);

                ChatBoxRecyclerView.SetItemAnimator(null);

                MLayoutManager = new LinearLayoutManager(this);
                ChatBoxRecyclerView.SetLayoutManager(MLayoutManager);
                ChatBoxRecyclerView.SetAdapter(MAdapter);

                MAdapter.SetOnClickListener(this);

                //Set Event Scroll
                XamarinRecyclerViewOnScrollListener onScrollListener = new XamarinRecyclerViewOnScrollListener(MLayoutManager, SwipeRefreshLayout);
                onScrollListener.LoadMoreEvent += Messages_OnScroll_OnLoadMoreEvent;
                ChatBoxRecyclerView.AddOnScrollListener(onScrollListener);
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
                    AudioCallButton.Click += AudioCallButtonOnClick;
                    VideoCallButton.Click += VideoCallButtonOnClick;
                    MoreButton.Click += MoreButtonOnClick;
                    UserChatProfile.Click += UserChatProfileOnClick;
                    ActionBarTitle.Click += UserChatProfileOnClick;
                    ActionBarSubTitle.Click += UserChatProfileOnClick;

                    ChatSendButton.Touch += Chat_sendButton_Touch;
                    GiftButton.Click += GiftButtonOnClick;
                    ImageButton.Click += ImageButtonOnClick;
                    ButtonGetPremium.Click += BtnGetPremiumOnClick;
                    EmojisIconEditTextView.AfterTextChanged += EmojisIconEditTextViewOnAfterTextChanged;
                }
                else
                {
                    AudioCallButton.Click -= AudioCallButtonOnClick;
                    VideoCallButton.Click -= VideoCallButtonOnClick;
                    MoreButton.Click -= MoreButtonOnClick;
                    UserChatProfile.Click -= UserChatProfileOnClick;
                    ActionBarTitle.Click -= UserChatProfileOnClick;
                    ActionBarSubTitle.Click -= UserChatProfileOnClick;

                    ChatSendButton.Touch -= Chat_sendButton_Touch;
                    GiftButton.Click -= GiftButtonOnClick;
                    ImageButton.Click -= ImageButtonOnClick;
                    ButtonGetPremium.Click -= BtnGetPremiumOnClick;
                    EmojisIconEditTextView.AfterTextChanged -= EmojisIconEditTextViewOnAfterTextChanged;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Set ToolBar and data chat user

        //Set ToolBar and data chat user
        private void loadData_ItemUser()
        {
            try
            {
                if (UserInfoData != null)
                {
                    ActionBarTitle.Text = QuickDateTools.GetNameFinal(UserInfoData);
                    ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(Convert.ToInt32(UserInfoData.Lastseen), false);
                    LastSeenUser = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(Convert.ToInt32(UserInfoData.Lastseen), false);

                    GlideImageLoader.LoadImage(this, UserInfoData.Avater, UserChatProfile, ImageStyle.CircleCrop, ImagePlaceholders.DrawableUser);
                }

                Get_Messages();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Get Messages

        //Get Messages Local Or Api
        private void Get_Messages()
        {
            try
            {
                BeforeMessageId = 0;
                MAdapter.MessageList.Clear();
                MAdapter.NotifyDataSetChanged();

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.GetMessagesList(UserDetails.UserId, Userid, BeforeMessageId);
                if (localList?.Count > 0) //Database.. Get Messages Local
                {
                    MAdapter.MessageList = new ObservableCollection<GetChatConversationsObject.Messages>(localList);
                    MAdapter.NotifyDataSetChanged();

                    //Scroll Down >> 
                    ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.Count - 1);
                    SwipeRefreshLayout.Refreshing = false;
                    SwipeRefreshLayout.Enabled = false;
                }
                else //Or server.. Get Messages Api
                {
                    SwipeRefreshLayout.Refreshing = true;
                    SwipeRefreshLayout.Enabled = true;

                    TaskWork = "Working";

                    //Run timer
                    Timer = new Timer { Interval = AppSettings.MessageRequestSpeed, Enabled = true };
                    Timer.Elapsed += TimerOnElapsed_MessageUpdater;
                    Timer.Start();

                    GetMessages_API();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Messages From API 
        private async void GetMessages_API()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    SwipeRefreshLayout.Refreshing = false;
                    SwipeRefreshLayout.Enabled = false;

                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }
                else
                {
                    BeforeMessageId = 0;

                    var (apiStatus, respond) = await RequestsAsync.Chat.GetChatConversationsAsync(Userid.ToString());
                    if (apiStatus == 200)
                    {
                        if (respond is GetChatConversationsObject result)
                        {
                            if (result.Data.Count > 0)
                            {
                                MAdapter.MessageList = new ObservableCollection<GetChatConversationsObject.Messages>(result.Data);
                                MAdapter.NotifyDataSetChanged();

                                //Insert to DataBase
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertOrReplaceMessages(MAdapter.MessageList);


                                //Scroll Down >> 
                                ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.Count - 1);

                                SwipeRefreshLayout.Refreshing = false;
                                SwipeRefreshLayout.Enabled = false;
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    SwipeRefreshLayout.Refreshing = false;
                    SwipeRefreshLayout.Enabled = false;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = false;
            }
        }

        #endregion

        //Timer Message Updater >> Get New Message
        private void TimerOnElapsed_MessageUpdater(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Code get last Message id where Updater >>
                MessageUpdater();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region Updater

        private async void MessageUpdater()
        {
            try
            {
                if (TaskWork == "Working")
                {
                    TaskWork = "Stop";

                    if (!Methods.CheckConnectivity())
                    {
                        SwipeRefreshLayout.Refreshing = false;
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                    }
                    else
                    {
                        int countList = MAdapter.MessageList.Count;
                        string afterId = MAdapter.MessageList.LastOrDefault()?.Id.ToString() ?? "";
                        var (apiStatus, respond) = await RequestsAsync.Chat.GetChatConversationsAsync(Userid.ToString(), "30", afterId);
                        if (apiStatus == 200)
                        {
                            if (respond is GetChatConversationsObject result)
                            {
                                int responseList = result.Data.Count;
                                if (responseList > 0)
                                {
                                    if (countList > 0)
                                    {
                                        foreach (var item in from item in result.Data let check = MAdapter.MessageList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                                        {
                                            MAdapter.MessageList.Add(item);

                                            int index = MAdapter.MessageList.IndexOf(item);
                                            if (index > -1)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    try
                                                    {
                                                        MAdapter.NotifyItemInserted(index);

                                                        //Scroll Down >> 
                                                        ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.Count - 1);
                                                    }
                                                    catch (Exception ee)
                                                    {
                                                        Methods.DisplayReportResultTrack(ee);
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MAdapter.MessageList = new ObservableCollection<GetChatConversationsObject.Messages>(result.Data);
                                    }

                                    RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            var lastCountItem = MAdapter.ItemCount;
                                            if (countList > 0)
                                                MAdapter.NotifyItemRangeInserted(lastCountItem, MAdapter.MessageList.Count - 1);
                                            else
                                                MAdapter.NotifyDataSetChanged();

                                            //Insert to DataBase
                                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                            dbDatabase.InsertOrReplaceMessages(MAdapter.MessageList);


                                            //Scroll Down >> 
                                            ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.Count - 1);

                                            var lastMessage = MAdapter.MessageList.LastOrDefault();
                                            if (lastMessage != null)
                                            {
                                                var instance = LastChatFragment.GetInstance();
                                                var dataUser = instance?.MAdapter?.UserList?.FirstOrDefault(a => a.User?.Id == Userid);
                                                if (dataUser != null)
                                                {
                                                    dataUser.Text = lastMessage.Text;
                                                    int index = instance.MAdapter.UserList.IndexOf(dataUser);

                                                    instance.MAdapter.UserList.Move(index, 0);
                                                    instance.MAdapter.NotifyItemMoved(index, 0);

                                                    var data = instance.MAdapter.UserList.FirstOrDefault(a => a.User?.Id == dataUser.User.Id);
                                                    if (data != null)
                                                    {
                                                        data.Id = dataUser.Id;
                                                        data.Owner = dataUser.Owner;
                                                        data.User = dataUser.User;
                                                        data.Seen = dataUser.Seen;
                                                        data.Text = dataUser.Text;
                                                        data.Media = dataUser.Media;
                                                        data.Sticker = dataUser.Sticker;
                                                        data.Time = dataUser.Time;
                                                        data.CreatedAt = dataUser.CreatedAt;
                                                        data.NewMessages = dataUser.NewMessages;
                                                        data.MessageType = dataUser.MessageType;

                                                        instance.MAdapter.NotifyItemChanged(instance.MAdapter.UserList.IndexOf(data));
                                                    }
                                                }
                                            }

                                            if (AppSettings.RunSoundControl)
                                                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_GetMesseges.mp3");

                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e);
                                        }
                                    });
                                }
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                    TaskWork = "Working";
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                TaskWork = "Working";
            }
        }

        public void UpdateOneMessage(GetChatConversationsObject.Messages message)
        {
            try
            {
                var checker = MAdapter.MessageList.FirstOrDefault(a => a.Id == message.Id);
                if (checker != null)
                {
                    checker.Id = message.Id;
                    checker.FromName = message.FromName;
                    checker.FromAvater = message.FromAvater;
                    checker.ToName = message.ToName;
                    checker.ToAvater = message.ToAvater;
                    checker.From = message.From;
                    checker.To = message.To;
                    checker.Text = message.Text;
                    checker.Media = message.Media;
                    checker.FromDelete = message.FromDelete;
                    checker.ToDelete = message.ToDelete;
                    checker.Sticker = message.Sticker;
                    checker.CreatedAt = message.CreatedAt;
                    checker.Seen = message.Seen;
                    checker.Type = message.Type;
                    checker.MessageType = message.MessageType;

                    MAdapter.NotifyItemChanged(MAdapter.MessageList.IndexOf(checker));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Load More

        private async void LoadMoreMessages()
        {
            try
            {
                //Run Load More Api 
                var local = LoadMoreMessagesDatabase();
                if (local == "1")
                {

                }
                else
                {
                    var api = await LoadMoreMessagesApi();
                    if (api == "1")
                    {

                    }
                    else
                    {
                        SwipeRefreshLayout.Refreshing = false;
                        SwipeRefreshLayout.Enabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private string LoadMoreMessagesDatabase()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.GetMessageList(UserDetails.UserId, Userid, FirstMessageId);
                if (localList?.Count > 0) //Database.. Get Messages Local
                {
                    localList = new List<DataTables.MessageTb>(localList.OrderByDescending(a => a.Id));

                    foreach (var message in localList.Select(messages => new GetChatConversationsObject.Messages
                    {
                        Id = messages.Id,
                        FromName = messages.FromName,
                        FromAvater = messages.FromAvater,
                        ToName = messages.ToName,
                        ToAvater = messages.ToAvater,
                        From = messages.FromId,
                        To = messages.ToId,
                        Text = messages.Text,
                        Media = messages.Media,
                        FromDelete = messages.FromDelete,
                        ToDelete = messages.ToDelete,
                        Sticker = messages.Sticker,
                        CreatedAt = messages.CreatedAt,
                        Seen = messages.Seen,
                        Type = messages.Type,
                        MessageType = messages.MessageType
                    }))
                    {
                        MAdapter.MessageList.Insert(0, message);
                        MAdapter.NotifyItemInserted(MAdapter.MessageList.IndexOf(MAdapter.MessageList.FirstOrDefault()));

                        var index = MAdapter.MessageList.FirstOrDefault(a => a.Id == FirstMessageId);
                        if (index == null) continue;
                        MAdapter.NotifyItemChanged(MAdapter.MessageList.IndexOf(index));
                        //Scroll Down >> 
                        ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.IndexOf(index));
                    }


                    return "1";
                }


                return "0";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return "0";
            }
        }

        private async Task<string> LoadMoreMessagesApi()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    SwipeRefreshLayout.Refreshing = false;
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }
                else
                {
                    var (apiStatus, respond) = await RequestsAsync.Chat.GetChatConversationsAsync(Userid.ToString(), "30", FirstMessageId.ToString(), "1").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {
                        if (respond is GetChatConversationsObject result)
                        {
                            if (result.Data.Count > 0)
                            {
                                var listApi = new ObservableCollection<GetChatConversationsObject.Messages>();

                                foreach (var messages in result.Data.OrderBy(a => a.CreatedAt))
                                {
                                    MAdapter.MessageList.Insert(0, messages);
                                    RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            MAdapter.NotifyItemInserted(MAdapter.MessageList.IndexOf(MAdapter.MessageList.FirstOrDefault()));

                                            var index = MAdapter.MessageList.FirstOrDefault(a => a.Id == FirstMessageId);
                                            if (index != null)
                                            {
                                                MAdapter.NotifyItemChanged(MAdapter.MessageList.IndexOf(index));
                                                //Scroll Down >> 
                                                ChatBoxRecyclerView.ScrollToPosition(MAdapter.MessageList.IndexOf(index));
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e);
                                        }
                                    });

                                    listApi.Insert(0, messages);

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    // Insert data user in database
                                    dbDatabase.InsertOrReplaceMessages(listApi);

                                }
                                return "1";
                            }
                            return "0";
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
                return "0";
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return "0";
            }
        }

        #endregion

        #region Events

        private void EmojisIconEditTextViewOnAfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            try
            {
                if (EmojisIconEditTextView.Text?.Length > 0)
                {
                    GiftButton.Visibility = ViewStates.Gone;
                    ImageButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    GiftButton.Visibility = ViewStates.Visible;
                    ImageButton.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Get Premium 
        private void BtnGetPremiumOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(this);
                window.DisplayPremiumWindow();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Send Message type => "right_text"
        private void OnClick_OfSendButton()
        {
            try
            {
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time2 = unixTimestamp.ToString(CultureInfo.InvariantCulture);

                if (string.IsNullOrEmpty(EmojisIconEditTextView.Text) || string.IsNullOrWhiteSpace(EmojisIconEditTextView.Text))
                {

                }
                else
                {
                    //Here on This function will send Text Messages to the user 

                    //remove \n in a string
                    string replacement = Regex.Replace(EmojisIconEditTextView.Text, @"\t|\n|\r", "");

                    if (Methods.CheckConnectivity())
                    {
                        GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
                        {
                            Id = Convert.ToInt32(unixTimestamp),
                            FromName = UserDetails.FullName,
                            FromAvater = UserDetails.Avatar,
                            ToName = UserInfoData?.FullName ?? "",
                            ToAvater = UserInfoData?.Avater ?? "",
                            From = UserDetails.UserId,
                            To = Userid,
                            Text = replacement,
                            Media = "",
                            FromDelete = 0,
                            ToDelete = 0,
                            Sticker = "",
                            CreatedAt = time2,
                            Seen = 0,
                            Type = "Sent",
                            MessageType = "text"
                        };

                        MAdapter.MessageList.Add(message);

                        int index = MAdapter.MessageList.IndexOf(MAdapter.MessageList.Last());
                        if (index > -1)
                        {
                            MAdapter.NotifyItemInserted(index);
                            //Scroll Down >> 
                            ChatBoxRecyclerView.ScrollToPosition(index);
                        }

                        var messageText = EmojisIconEditTextView.Text;
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => MessageController.SendMessageTask(this, Userid, messageText, "", "", time2, UserInfoData) });
                        CanDoChat();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                    }

                    EmojisIconEditTextView.Text = "";
                }

                ChatSendButton.Tag = "Text";
                ChatSendButton.SetImageResource(Resource.Drawable.ic_send_vector);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Send messages >>  type text
        private void Chat_sendButton_Touch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e?.Event?.Action == MotionEventActions.Down)
                {
                    if (CanDoChat())
                    {
                        OnClick_OfSendButton();
                    }
                }
                e.Handled = false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void UpdateChatBoxAttachmentWindowState(ChatBoxButtonWindowState chatBoxButtonWindowState)
        {
            UpdateActionButtonsTintColor(chatBoxButtonWindowState);

            switch (chatBoxButtonWindowState)
            {
                case ChatBoxButtonWindowState.AllClosed:
                    CloseChatAttachmentWindow();
                    break;
                case ChatBoxButtonWindowState.ShowImages:
                    CloseChatAttachmentWindow();
                    break;
                case ChatBoxButtonWindowState.ShowStrickers:
                    ReplaceButtonFragment(ChatStickersFragment);
                    break;
                case ChatBoxButtonWindowState.ShowGifts:
                    ReplaceButtonFragment(ChatGiftFragment);
                    break;
            }

            LastChatBoxButtonWindowState = chatBoxButtonWindowState;
        }

        private void CloseChatAttachmentWindow()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();

                if (SupportFragmentManager.Fragments.Count <= 0) return;

                FragmentTransaction fragmentManager = SupportFragmentManager.BeginTransaction();
                foreach (var vrg in SupportFragmentManager.Fragments)
                {
                    if (SupportFragmentManager.Fragments.Contains(ChatStickersFragment))
                    {
                        fragmentManager.Remove(ChatStickersFragment);
                    }
                    else if (SupportFragmentManager.Fragments.Contains(ChatGiftFragment))
                    {
                        fragmentManager.Remove(ChatGiftFragment);
                    }
                }
                fragmentManager.Commit();
            }
        }

        private void UpdateActionButtonsTintColor(ChatBoxButtonWindowState chatBoxButtonWindowState)
        {
            try
            {
                var normalStateColor = QuickDateTools.IsTabDark() ? Color.ParseColor("#ffffff") : Color.ParseColor("#4D4D4D");
                var selectedStateColor = Color.ParseColor(AppSettings.MainColor);

                //StickerButton.Drawable.SetTint(chatBoxButtonWindowState == ChatBoxButtonWindowState.ShowStrickers ? selectedStateColor : normalStateColor);
                //GiftButton.Drawable.SetTint(chatBoxButtonWindowState == ChatBoxButtonWindowState.ShowGifts ? selectedStateColor : normalStateColor);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Sent image >> type media (OnActivityResult)
        private void ImageButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CanDoChat())
                {
                    PixImagePickerUtils.OpenDialogGallery(this);

                    UpdateChatBoxAttachmentWindowState(ChatBoxButtonWindowState.ShowImages);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Sent Sticker >> type sticker (ChatStickersFragment)
        private void StickerButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CanDoChat())
                {

                    var currentState = LastChatBoxButtonWindowState == ChatBoxButtonWindowState.ShowStrickers ? ChatBoxButtonWindowState.AllClosed : ChatBoxButtonWindowState.ShowStrickers;

                    UpdateChatBoxAttachmentWindowState(currentState);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Sent Gift >> type sticker (ChatGiftFragment)
        private void GiftButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CanDoChat())
                {
                    var currentState = LastChatBoxButtonWindowState == ChatBoxButtonWindowState.ShowGifts ? ChatBoxButtonWindowState.AllClosed : ChatBoxButtonWindowState.ShowGifts;

                    UpdateChatBoxAttachmentWindowState(currentState);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        #region Scroll

        //Event Scroll #Messages
        private void Messages_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Start Loader Get from Database or API Request >>
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;

                FirstMessageId = 0;

                //Code get first Message id where LoadMore >>
                var mes = MAdapter.MessageList.FirstOrDefault();
                if (mes != null)
                {
                    FirstMessageId = mes.Id;
                }

                if (FirstMessageId > 0)
                {
                    LoadMoreMessages();
                }

                SwipeRefreshLayout.Refreshing = false;
                SwipeRefreshLayout.Enabled = false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);

            public event LoadMoreEventHandler LoadMoreEvent;

            public readonly LinearLayoutManager LayoutManager;
            public readonly SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, SwipeRefreshLayout swipeRefreshLayout)
            {
                LayoutManager = layoutManager;
                SwipeRefreshLayout = swipeRefreshLayout;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    var pastVisibleItems = LayoutManager.FindFirstVisibleItemPosition();
                    if (pastVisibleItems == 0 && visibleItemCount != totalItemCount)
                    {
                        //Load More  from API Request
                        LoadMoreEvent?.Invoke(this, null);
                        //Start Load More messages From Database
                    }
                    else
                    {
                        if (SwipeRefreshLayout.Refreshing)
                        {
                            SwipeRefreshLayout.Refreshing = false;
                            SwipeRefreshLayout.Enabled = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
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

        private void SetSystemBarColor(Activity act, Color color)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Window window = act.Window;
                    window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    //window.SetStatusBarColor(color);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UserChatProfileOnClick(object sender, EventArgs e)
        {
            OpenUserProfile();
        }

        private void MoreButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialAlertDialogBuilder(this);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Block));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Clear_chat));

                dialogList.SetItems(arrayAdapter.ToArray(), new MaterialDialogUtils(arrayAdapter, this));
                dialogList.SetPositiveButton(GetText(Resource.String.Lbl_Close), new MaterialDialogUtils());

                dialogList.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void VideoCallButtonOnClick(object sender, EventArgs e)
        {
            MenuVideoCallIcon_Click();
        }

        private void AudioCallButtonOnClick(object sender, EventArgs e)
        {
            MenuPhoneCallIcon_Click();
        }

        private void OpenUserProfile()
        {
            try
            {
                if (UserInfoData != null)
                {
                    QuickDateTools.OpenProfile(this, "HideButton", UserInfoData, null);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        //Block User
        private void MenuBlockClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var list = GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter?.UserList;
                    if (list?.Count > 0)
                    {
                        var dataFav = list.FirstOrDefault(a => a.Id == Userid);
                        if (dataFav != null)
                        {
                            list.Remove(dataFav);
                            GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter?.NotifyDataSetChanged();
                        }
                    }

                    MenuClearChatClick();

                    Toast.MakeText(this, GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Short)?.Show();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.BlockAsync(Userid.ToString()) });
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void MenuClearChatClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Chat.DeleteMessagesAsync(Userid.ToString()) });

                    MAdapter.MessageList.Clear();
                    MAdapter.NotifyDataSetChanged();

                    var instance = LastChatFragment.GetInstance();
                    var userDelete = instance?.MAdapter?.UserList?.FirstOrDefault(a => a.User?.Id == Userid);
                    if (userDelete != null)
                    {
                        instance?.MAdapter?.UserList?.Remove(userDelete);
                        instance?.MAdapter?.NotifyDataSetChanged();
                    }

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.DeleteUserLastChat(Userid.ToString());
                    dbDatabase.DeleteAllMessagesUser(UserDetails.UserId.ToString(), Userid.ToString());

                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void MenuPhoneCallIcon_Click()
        {
            try
            {
                CallType = TypeCall.Audio;
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        StartCall(TypeCall.Audio);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(102);
                    }
                }
                else
                {
                    StartCall(TypeCall.Audio);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void MenuVideoCallIcon_Click()
        {
            try
            {
                CallType = TypeCall.Video;
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        StartCall(TypeCall.Video);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(103);
                    }
                }
                else
                {
                    StartCall(TypeCall.Video);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Call

        private void StartCall(TypeCall type)
        {
            try
            {
                var dataSettings = ListUtils.SettingsSiteList;
                Intent intentCall = new Intent(this, typeof(CallingService));
                if (type == TypeCall.Audio)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Agora_audio_calling_start");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Twilio_audio_calling_start");
                    }
                }
                else if (type == TypeCall.Video)
                {
                    if (dataSettings?.AgoraChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Agora_video_calling_start");
                    }
                    else if (dataSettings?.TwilioChatCall == "1")
                    {
                        intentCall.PutExtra("type", "Twilio_video_calling_start");
                    }
                }

                if (UserInfoData != null)
                {
                    var callUserObject = new DataCallObject
                    {
                        ToId = Userid.ToString(),
                        Avater = UserInfoData.Avater,
                        Fullname = QuickDateTools.GetNameFinal(UserInfoData),
                    };
                    intentCall.PutExtra("callUserObject", JsonConvert.SerializeObject(callUserObject));
                }

                intentCall.SetAction(CallingService.ActionStartNewCall);
                StartService(intentCall);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion


        #region Toolbar & Selected

        public class ActionModeCallback : Object, ActionMode.ICallback
        {
            private readonly MessagesBoxActivity Activity;
            public ActionModeCallback(MessagesBoxActivity activity)
            {
                Activity = activity;
            }

            public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
            {
                int id = item.ItemId;
                if (id == Resource.Id.action_copy)
                {
                    CopyItems();
                    mode.Finish();
                    return true;
                }
                return false;
            }

            public bool OnCreateActionMode(ActionMode mode, IMenu menu)
            {
                Activity.SetSystemBarColor(Activity, Color.ParseColor(AppSettings.MainColor));
                mode.MenuInflater.Inflate(Resource.Menu.menuChat, menu);
                return true;
            }

            public void OnDestroyActionMode(ActionMode mode)
            {
                try
                {
                    Activity.MAdapter.ClearSelections();
                    ActionMode.Finish();
                    ActionMode = null;

                    Activity.SetSystemBarColor(Activity, Color.ParseColor(AppSettings.MainColor));

                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    TopChatToolBar.Visibility = ViewStates.Visible;

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

            //Copy Messages
            private void CopyItems()
            {
                try
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    if (TopChatToolBar.Visibility != ViewStates.Visible)
                        TopChatToolBar.Visibility = ViewStates.Visible;

                    string allText = "";
                    List<int> selectedItemPositions = Activity.MAdapter.GetSelectedItems();
                    for (int i = selectedItemPositions.Count - 1; i >= 0; i--)
                    {
                        var datItem = Activity.MAdapter.GetItem(selectedItemPositions[i]);
                        if (datItem != null)
                        {
                            allText = allText + " \n" + datItem.Text;
                        }
                    }

                    Methods.CopyToClipboard(Activity, allText);

                    Activity.MAdapter.NotifyDataSetChanged();

                    Toast.MakeText(Activity, Activity.GetText(Resource.String.Lbl_Text_copied), ToastLength.Short)?.Show();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        public void ItemClick(View view, GetChatConversationsObject.Messages obj, int pos)
        {
            try
            {
                if (MAdapter.GetSelectedItemCount() > 0) // Add Select New Item 
                {
                    EnableActionMode(pos);
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    if (TopChatToolBar.Visibility != ViewStates.Visible)
                        TopChatToolBar.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ItemLongClick(View view, GetChatConversationsObject.Messages obj, int pos)
        {
            EnableActionMode(pos);
        }

        private void EnableActionMode(int position)
        {
            try
            {
                ActionMode ??= StartSupportActionMode(ModeCallback);
                ToggleSelection(position);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }

        private void ToggleSelection(int position)
        {
            try
            {
                MAdapter.ToggleSelection(position);
                int count = MAdapter.GetSelectedItemCount();

                if (count == 0)
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    TopChatToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = false;
                        Timer.Stop();
                    }

                    TopChatToolBar.Visibility = ViewStates.Gone;
                    ActionMode.Title = count.ToString();
                    ActionMode.Invalidate();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region SupportFragment

        private void ReplaceButtonFragment(SupportFragment fragmentView)
        {
            try
            {
                HideKeyboard();
                if (fragmentView.IsVisible)
                    return;

                var trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(ButtonFragmentHolder.Id, fragmentView);

                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    trans.AddToBackStack(null);
                }

                trans.Commit();

                ButtonFragmentHolder.TranslationY = 1200;
                ButtonFragmentHolder.Animate().SetInterpolator(new FastOutSlowInInterpolator()).TranslationYBy(-1200).SetDuration(500);
                MainFragmentOpened = fragmentView;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == PixImagePickerActivity.RequestCode && resultCode == Result.Ok) // right_image 
                {
                    var listPath = JsonConvert.DeserializeObject<ResultIntentPixImage>(data.GetStringExtra("ResultPixImage") ?? "");
                    if (listPath?.List?.Count > 0)
                    {
                        var filepath = listPath.List.FirstOrDefault();
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                            if (type == "Image")
                            {
                                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                                string time2 = unixTimestamp.ToString(CultureInfo.InvariantCulture);

                                //Sent image 
                                GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
                                {
                                    Id = Convert.ToInt32(unixTimestamp),
                                    FromName = UserDetails.FullName,
                                    FromAvater = UserDetails.Avatar,
                                    ToName = UserInfoData?.FullName ?? "",
                                    ToAvater = UserInfoData?.Avater ?? "",
                                    From = UserDetails.UserId,
                                    To = Userid,
                                    Text = "",
                                    Media = filepath,
                                    FromDelete = 0,
                                    ToDelete = 0,
                                    Sticker = "",
                                    CreatedAt = time2,
                                    Seen = 0,
                                    Type = "Sent",
                                    MessageType = "media"
                                };

                                MAdapter.MessageList.Add(message);

                                int index = MAdapter.MessageList.IndexOf(MAdapter.MessageList.Last());
                                if (index > -1)
                                {
                                    MAdapter.NotifyItemInserted(index);

                                    //Scroll Down >> 
                                    ChatBoxRecyclerView.ScrollToPosition(index);
                                }

                                await MessageController.SendMessageTask(this, Userid, "", "", filepath, time2, UserInfoData);
                                CanDoChat();
                                EmojisIconEditTextView.Text = "";
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

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                switch (requestCode)
                {
                    case 108 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        PixImagePickerUtils.OpenDialogGallery(this);
                        break;
                    case 108:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 102 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        StartCall(TypeCall.Audio);
                        break;
                    case 102:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                    case 103 when grantResults.Length > 0 && grantResults[0] == Permission.Granted:
                        StartCall(TypeCall.Video);
                        break;
                    case 103:
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)?.Show();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public void BackPressed()
        {
            try
            {
                int count = MAdapter.GetSelectedItemCount();
                if (SupportFragmentManager.BackStackEntryCount > 0)
                {
                    UpdateChatBoxAttachmentWindowState(ChatBoxButtonWindowState.AllClosed);
                }
                else if (count > 0)
                {
                    if (Timer != null)
                    {
                        Timer.Enabled = true;
                        Timer.Start();
                    }

                    TopChatToolBar.Visibility = ViewStates.Visible;
                    ActionMode.Finish();
                }
                else
                {
                    Finish();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void GetStickersGiftsLists()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                var listGifts = sqlEntity.GetGiftsList();
                var listStickers = sqlEntity.GetStickersList();

                if (ListUtils.StickersList.Count == 0 && listStickers?.Count > 0)
                    ListUtils.StickersList = listStickers;

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetStickers(this) });

                if (ListUtils.GiftsList.Count == 0 && listGifts?.Count > 0)
                    ListUtils.GiftsList = listGifts;

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetGifts(this) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private bool CanDoChat()
        {
            bool canChat = true;

            var isPro = ListUtils.MyUserInfo?.FirstOrDefault()?.IsPro ?? "0";
            var totalMessageSent = MAdapter.MessageList.Where(data => data.From == UserDetails.UserId).ToList()?.Count;

            if (AppSettings.ShouldHaveLimitOnMessages && isPro == "0" && totalMessageSent >= AppSettings.MaxMessageLimitForNonProUser)
            {
                canChat = false;
                HideChatWindowAndShowPremiumMessage(canChat);
            }

            return canChat;
        }

        private void HideChatWindowAndShowPremiumMessage(bool canChat)
        {
            if (!canChat)
                HideKeyboard();

            PremiumChatLayout.Visibility = canChat ? ViewStates.Gone : ViewStates.Visible;
            //ChatBoxonButtom.Visibility = canChat ? ViewStates.Visible : ViewStates.Gone;
        }

        private void HideKeyboard()
        {
            try
            {
                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region MaterialDialog

        public void OnSelection(IDialogInterface dialog, int position, string itemString)
        {
            try
            {
                if (itemString == GetText(Resource.String.Lbl_Block))
                {
                    MenuBlockClick();
                }
                else if (itemString == GetText(Resource.String.Lbl_Clear_chat))
                {
                    MenuClearChatClick();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion
    }
}