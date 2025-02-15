using Android.App;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Bumptech.Glide.Util;
using Com.Airbnb.Lottie;
using QuickDate.Activities.HotOrNot;
using QuickDate.Activities.HotOrNot.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.IntegrationRecyclerView;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Priority = Bumptech.Glide.Priority;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class TrendingAdapter : RecyclerView.Adapter
    {
        public event EventHandler<NearByAdapterClickEventArgs> UsersLikeButtonItemClick;
        public event EventHandler<NearByAdapterClickEventArgs> UsersItemClick;
        public event EventHandler<NearByAdapterClickEventArgs> UsersItemLongClick;

        private readonly Activity ActivityContext;
        private readonly HomeActivity GlobalContext;
        private ProUserAdapter ProUserAdapter;
        private HotOrNotUserAdapter HotOrNotUserAdapter;
        public ObservableCollection<Classes.TrendingClass> TrendingList = new ObservableCollection<Classes.TrendingClass>();
        private readonly RequestBuilder FullGlideRequestBuilder;

        public TrendingAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                GlobalContext = HomeActivity.GetInstance();
                var glideRequestOptions = new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).SetPriority(Priority.High);
                FullGlideRequestBuilder = Glide.With(context?.BaseContext).AsBitmap().Apply(glideRequestOptions).Transition(new BitmapTransitionOptions().CrossFade(100));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                switch (viewType)
                {
                    case (int)ItemType.ProUser:
                    case (int)ItemType.HotOrNot:
                        {
                            //Setup your layout here >> TemplateRecyclerViewLayout
                            View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.TemplateRecyclerViewLayout, parent, false);
                            var vh = new TrendingAdapterViewHolder(itemView);
                            return vh;
                        }
                    case (int)ItemType.Users:
                        {
                            //Setup your layout here >> TemplateRecyclerViewLayout
                            View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_NearByView, parent, false);
                            var vh = new NearByAdapterViewHolder(itemView, UsersLikeButtonOnClick, UsersOnClick, UsersOnLongClick);
                            return vh;
                        }
                    case (int)ItemType.EmptyPage:
                        {
                            //Setup your layout here >> EmptyStateLayout
                            View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.EmptyStateLayout, parent, false);
                            var vh = new EmptyStateViewHolder(itemView);
                            return vh;
                        }
                    default:
                        return null;
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                var item = TrendingList[position];
                if (item != null)
                {
                    switch (item.Type)
                    {
                        case ItemType.ProUser:
                        case ItemType.HotOrNot:
                            {
                                if (viewHolder is TrendingAdapterViewHolder holder)
                                {
                                    switch (item.Type)
                                    {
                                        case ItemType.ProUser:
                                            {
                                                if (ProUserAdapter == null)
                                                {
                                                    ProUserAdapter = new ProUserAdapter(ActivityContext)
                                                    {
                                                        ProUserList = new ObservableCollection<UserInfoObject>()
                                                    };

                                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                                                    holder.MRecycler.NestedScrollingEnabled = false;

                                                    var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                                    var preLoader = new RecyclerViewPreloader<UserInfoObject>(ActivityContext, ProUserAdapter, sizeProvider, 10);
                                                    holder.MRecycler.AddOnScrollListener(preLoader);
                                                    holder.MRecycler.SetAdapter(ProUserAdapter);
                                                    ProUserAdapter.OnItemClick += ProUserAdapterOnOnItemClick;
                                                }

                                                int countList = ProUserAdapter.ProUserList.Count;
                                                if (item.ProUserList.Count > 0)
                                                {
                                                    if (countList > 0)
                                                    {
                                                        foreach (var user in from user in item.ProUserList let check = ProUserAdapter.ProUserList.FirstOrDefault(a => a.Id == user.Id) where check == null select user)
                                                        {
                                                            ProUserAdapter.ProUserList.Add(user);
                                                        }

                                                        // ActivityContext?.RunOnUiThread(() => { ProUserAdapter.NotifyItemRangeInserted(countList - 1, ProUserAdapter.ProUserList.Count - countList); }); 
                                                    }
                                                    else
                                                    {
                                                        ProUserAdapter.ProUserList = new ObservableCollection<UserInfoObject>(item.ProUserList);
                                                    }
                                                    ActivityContext?.RunOnUiThread(() => { ProUserAdapter.NotifyDataSetChanged(); });
                                                }

                                                holder.MainLinear.Visibility = ViewStates.Invisible;
                                                holder.MoreText.Visibility = ViewStates.Invisible;
                                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_ProUser);
                                                break;
                                            }
                                        case ItemType.HotOrNot:
                                            {
                                                if (HotOrNotUserAdapter == null)
                                                {
                                                    HotOrNotUserAdapter = new HotOrNotUserAdapter(ActivityContext)
                                                    {
                                                        UsersDateList = new ObservableCollection<UserInfoObject>()
                                                    };

                                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                                                    holder.MRecycler.NestedScrollingEnabled = false;

                                                    var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                                    var preLoader = new RecyclerViewPreloader<UserInfoObject>(ActivityContext, HotOrNotUserAdapter, sizeProvider, 10);
                                                    holder.MRecycler.AddOnScrollListener(preLoader);
                                                    holder.MRecycler.SetAdapter(HotOrNotUserAdapter);
                                                    HotOrNotUserAdapter.HotItemClick += HotOrNotUserAdapterOnHotItemClick;
                                                    HotOrNotUserAdapter.NotItemClick += HotOrNotUserAdapterOnNotItemClick;
                                                    HotOrNotUserAdapter.ImageItemClick += HotOrNotUserAdapterOnImageItemClick;

                                                    holder.MainLinear.Click += MoreTextOnClick;
                                                }

                                                int countList = HotOrNotUserAdapter.UsersDateList.Count;
                                                if (item.HotOrNotList.Count > 0)
                                                {
                                                    if (countList > 0)
                                                    {
                                                        foreach (var user in from user in item.HotOrNotList let check = HotOrNotUserAdapter.UsersDateList.FirstOrDefault(a => a.Id == user.Id) where check == null select user)
                                                        {
                                                            HotOrNotUserAdapter.UsersDateList.Add(user);
                                                        }

                                                        //ActivityContext?.RunOnUiThread(() => { HotOrNotUserAdapter.NotifyItemRangeInserted(countList - 1, HotOrNotUserAdapter.UsersDateList.Count - countList); });
                                                    }
                                                    else
                                                    {
                                                        HotOrNotUserAdapter.UsersDateList = new ObservableCollection<UserInfoObject>(item.HotOrNotList);
                                                    }
                                                    ActivityContext?.RunOnUiThread(() => { HotOrNotUserAdapter.NotifyDataSetChanged(); });
                                                }
                                                holder.MainLinear.Visibility = ViewStates.Visible;
                                                holder.MoreText.Visibility = ProUserAdapter.ProUserList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_HotORNot);
                                                break;
                                            }
                                    }
                                }

                                break;
                            }
                        case ItemType.Users:
                            {
                                if (viewHolder is NearByAdapterViewHolder nearByAdapter)
                                {
                                    FullGlideRequestBuilder.Load(item.UsersData.Avater).Into(nearByAdapter.ImgUser);

                                    nearByAdapter.TxtName.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item.UsersData), 20) + ", " + QuickDateTools.GetAgeUser(item.UsersData);

                                    var distanceBetween = QuickDateTools.DistanceBetween(new LatLng(Convert.ToDouble(UserDetails.Lat), Convert.ToDouble(UserDetails.Lng)), new LatLng(Convert.ToDouble(item.UsersData.Lat), Convert.ToDouble(item.UsersData.Lng)));
                                    nearByAdapter.TxtLastSeen.Text = QuickDateTools.ToSi(distanceBetween);

                                    // nearByAdapter.TxtLastSeen.Text = !string.IsNullOrEmpty(item.UsersData.LastseenTxt) ? item.UsersData.LastseenTxt : Methods.Time.TimeAgo(int.Parse(item.UsersData.Lastseen), false);
                                    nearByAdapter.OnlineIcon.Visibility = QuickDateTools.GetStatusOnline(item.UsersData.Lastseen, item.UsersData.Online) ? ViewStates.Visible : ViewStates.Gone;

                                    if (item.UsersData.IsLiked != null && item.UsersData.IsLiked.Value)
                                    {
                                        nearByAdapter.LottieAnimation.Visibility = ViewStates.Visible;
                                        nearByAdapter.LottieAnimation.PlayAnimation();
                                    }
                                    else
                                    {
                                        nearByAdapter.LottieAnimation.Visibility = ViewStates.Gone;
                                        nearByAdapter.LottieAnimation.CancelAnimation();
                                        nearByAdapter.LottieAnimation.Progress = 0;
                                    }
                                }

                                break;
                            }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region Event

        private void MoreTextOnClick(object sender, EventArgs e)
        {
            try
            {
                var globalContext = HomeActivity.GetInstance();
                if (globalContext?.TrendingFragment != null)
                {
                    globalContext.TrendingFragment.HotOrNotFragment = new HotOrNotFragment();
                    globalContext.FragmentBottomNavigator.DisplayFragment(globalContext.TrendingFragment.HotOrNotFragment);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
        //Open profile user  >> Hot Or Not
        private void HotOrNotUserAdapterOnImageItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = HotOrNotUserAdapter.GetItem(e.Position);
                if (item != null)
                {
                    QuickDateTools.OpenProfile(ActivityContext, "likeAndClose", item, e.Image);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add Not
        private void HotOrNotUserAdapterOnNotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = HotOrNotUserAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.SetNotAsync(item.Id.ToString()) });

                        var index = HotOrNotUserAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            Animation animation = AnimationUtils.LoadAnimation(ActivityContext, Android.Resource.Animation.FadeOut);
                            e.View.StartAnimation(animation);
                            Handler handle = new Handler(Looper.MainLooper);
                            handle.PostDelayed(() =>
                            {
                                ActivityContext.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        HotOrNotUserAdapter.UsersDateList.RemoveAt(index);
                                        HotOrNotUserAdapter.NotifyItemRemoved(index);
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });
                                animation.Cancel();
                            }, 100);
                        }
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Add Hot
        private void HotOrNotUserAdapterOnHotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = HotOrNotUserAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddHotAsync(item.Id.ToString()) });

                        var index = HotOrNotUserAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            Animation animation = AnimationUtils.LoadAnimation(ActivityContext, Android.Resource.Animation.FadeOut);
                            e.View.StartAnimation(animation);
                            Handler handle = new Handler(Looper.MainLooper);
                            handle.PostDelayed(() =>
                            {
                                ActivityContext.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        HotOrNotUserAdapter.UsersDateList.RemoveAt(index);
                                        HotOrNotUserAdapter.NotifyItemRemoved(index);
                                    }
                                    catch (Exception exception)
                                    {
                                        Methods.DisplayReportResultTrack(exception);
                                    }
                                });
                                animation.Cancel();
                            }, 100);
                        }
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        //Open profile user  >> Pro User
        private void ProUserAdapterOnOnItemClick(object sender, ProUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = ProUserAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (item.Type == "Your")
                    {
                        var window = new PopupController(GlobalContext);
                        window.DisplayPremiumWindow();
                    }
                    else
                    {
                        QuickDateTools.OpenProfile(ActivityContext, "Close", item, e.Image);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        public override int ItemCount => TrendingList?.Count ?? 0;
        public Classes.TrendingClass GetItem(int position)
        {
            return TrendingList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                var item = TrendingList[position];
                if (item != null)
                {
                    return item.Type switch
                    {
                        ItemType.ProUser => (int)ItemType.ProUser,
                        ItemType.HotOrNot => (int)ItemType.HotOrNot,
                        ItemType.Users => (int)ItemType.Users,
                        ItemType.EmptyPage => (int)ItemType.EmptyPage,
                        _ => (int)ItemType.EmptyPage
                    };
                }

                return (int)ItemType.EmptyPage;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return (int)ItemType.EmptyPage;
            }
        }

        void UsersLikeButtonOnClick(NearByAdapterClickEventArgs args) => UsersLikeButtonItemClick?.Invoke(ActivityContext, args);
        void UsersOnClick(NearByAdapterClickEventArgs args) => UsersItemClick?.Invoke(ActivityContext, args);
        void UsersOnLongClick(NearByAdapterClickEventArgs args) => UsersItemLongClick?.Invoke(ActivityContext, args);

    }

    public class TrendingAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public RelativeLayout MainLinear { get; private set; }
        public TextView TitleText { get; private set; }
        public TextView MoreText { get; private set; }
        public RecyclerView MRecycler { get; private set; }

        #endregion

        public TrendingAdapterViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;

                MainLinear = MainView.FindViewById<RelativeLayout>(Resource.Id.mainLinear);
                TitleText = MainView.FindViewById<TextView>(Resource.Id.textTitle);
                MoreText = MainView.FindViewById<TextView>(Resource.Id.textMore);
                MRecycler = MainView.FindViewById<RecyclerView>(Resource.Id.recyler);

                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);

                //Create an Event
                //itemView.Click += (sender, e) => clickListener(new TrendingAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new TrendingAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

    }

    public class EmptyStateViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public AppCompatButton EmptyStateButton { get; private set; }
        public TextView EmptyStateIcon { get; private set; }
        public TextView DescriptionText { get; private set; }
        public TextView TitleText { get; private set; }
        public LottieAnimationView LottieAnimationView { get; private set; }

        #endregion

        public EmptyStateViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;

                EmptyStateIcon = (TextView)itemView.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)itemView.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)itemView.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = itemView.FindViewById<AppCompatButton>(Resource.Id.button);
                LottieAnimationView = itemView.FindViewById<LottieAnimationView>(Resource.Id.animation_view);

                LottieAnimationView.Visibility = ViewStates.Gone;
                EmptyStateIcon.Visibility = ViewStates.Visible;
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Search);
                EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_TitleText);
                DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_DescriptionText);
                EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_Button);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class NearByAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public ImageView ImgUser { get; private set; }
        public TextView TxtName { get; private set; }
        public TextView TxtLastSeen { get; private set; }
        public ImageView LikeButton { get; private set; }
        public View OnlineIcon { get; private set; }
        public LottieAnimationView LottieAnimation { get; private set; }

        #endregion

        public NearByAdapterViewHolder(View itemView, Action<NearByAdapterClickEventArgs> likeButtonClickListener, Action<NearByAdapterClickEventArgs> clickListener, Action<NearByAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                ImgUser = itemView.FindViewById<ImageView>(Resource.Id.imgUser);
                TxtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                TxtLastSeen = itemView.FindViewById<TextView>(Resource.Id.LastSeen);
                LikeButton = itemView.FindViewById<ImageView>(Resource.Id.LikeButton);
                OnlineIcon = itemView.FindViewById<View>(Resource.Id.OnlineIcon);
                LottieAnimation = itemView.FindViewById<LottieAnimationView>(Resource.Id.animation_view_of_like);
                LottieAnimation.CancelAnimation();
                LottieAnimation.Visibility = ViewStates.Gone;
                OnlineIcon.Visibility = ViewStates.Gone;

                //var cardView = itemView.FindViewById<CardView>(Resource.Id.maincardview);
                //if(AdapterPosition % 2 = 1)
                //var f = (CardView.LayoutParams)cardView.LayoutParameters;
                //f.Height = 150;
                //cardView.LayoutParameters = f;
                //ImgIcon = itemView.FindViewById<TextView>(Resource.Id.imgIcon);
                //CountImage = itemView.FindViewById<TextView>(Resource.Id.countImage);

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ImgIcon, IonIconsFonts.Camera);

                //Event
                LikeButton.Click += (sender, e) => likeButtonClickListener(new NearByAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = ImgUser });
                itemView.Click += (sender, e) => clickListener(new NearByAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = ImgUser });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


    }

    public class NearByAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }

    }
}