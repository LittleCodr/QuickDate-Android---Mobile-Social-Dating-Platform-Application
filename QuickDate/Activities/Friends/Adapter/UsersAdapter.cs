using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Java.Util;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Friends.Adapter
{
    public class UsersAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        private readonly HomeActivity HomeActivity;
        public ObservableCollection<UserInfoObject> UserList = new ObservableCollection<UserInfoObject>();
        public event EventHandler<UsersAdapterClickEventArgs> OnItemClick;
        public event EventHandler<UsersAdapterClickEventArgs> OnItemLongClick;
        public readonly string NameButton;

        #endregion

        public UsersAdapter(Activity context, HomeActivity homeActivity, string nameButton)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                HomeActivity = homeActivity;
                NameButton = nameButton;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_UsersView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_UsersView, parent, false);
                var vh = new UsersAdapterViewHolder(this, itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is UsersAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        if (!item.Avater.Contains(InitializeQuickDate.WebsiteUrl))
                            item.Avater = InitializeQuickDate.WebsiteUrl + "/" + item.Avater;

                        GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.ImageOnline.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online) ? ViewStates.Visible : ViewStates.Gone;

                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 14);

                        bool success = int.TryParse(item.Lastseen, out var number);
                        if (success)
                        {
                            holder.LastTimeOnline.Text = Methods.Time.TimeAgo(number, false);
                        }
                        else
                        {
                            holder.LastTimeOnline.Text = item.Lastseen;
                        }

                        switch (NameButton)
                        {
                            case "Likes":
                                holder.Button.Visibility = ViewStates.Gone;
                                break;
                            case "ILikedUsers":
                            case "IDislikedUsers":
                                holder.Button.SetImageResource(Resource.Drawable.icon_remove_heart_vector);
                                break;
                            case "Friends":
                                holder.Button.SetImageResource(Resource.Drawable.icon_person_add_disabled_vector);
                                break;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void LikedButtonClick(UsersClickEventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (e.UserClass == null)
                    e.UserClass = GetItem(e.Position);

                if (e.UserClass != null)
                {
                    var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                    if (index != -1)
                    {
                        UserList.Remove(e.UserClass);
                        NotifyItemRemoved(index);
                        NotifyItemRangeRemoved(0, ItemCount);
                    }

                    //send api Delete Likes 
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.DeleteLikesAsync(e.UserClass.Id.ToString()) });

                    var countList = HomeActivity?.ProfileFragment?.LikedFragment?.MAdapter?.ItemCount;
                    if (countList == 0)
                    {
                        HomeActivity?.ProfileFragment?.LikedFragment?.ShowEmptyPage();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void DislikedButtonClick(UsersClickEventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (e.UserClass == null)
                    e.UserClass = GetItem(e.Position);

                if (e.UserClass != null)
                {
                    var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                    if (index != -1)
                    {
                        UserList.Remove(e.UserClass);
                        NotifyItemRemoved(index);
                        NotifyItemRangeRemoved(0, ItemCount);
                    }

                    //send api Delete Dislikes 
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.DeleteDislikesAsync(e.UserClass.Id.ToString()), () => RequestsAsync.Users.DeleteDislikesAsync(e.UserClass.Id.ToString()) });

                    var countList = HomeActivity?.ProfileFragment?.DislikedFragment?.MAdapter?.ItemCount;
                    if (countList == 0)
                    {
                        HomeActivity?.ProfileFragment?.DislikedFragment?.ShowEmptyPage();
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void FriendButtonClick(UsersClickEventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)?.Show();
                    return;
                }

                if (e.UserClass == null)
                    e.UserClass = GetItem(e.Position);

                if (e.UserClass != null)
                {
                    e.ButtonFollow.Tag = "false";
                    e.UserClass.IsFriend = false;
                    e.UserClass.IsFriendRequest = false;

                    var index = UserList.IndexOf(UserList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                    if (index != -1)
                    {
                        UserList.Remove(e.UserClass);
                        NotifyItemRemoved(index);
                        NotifyItemRangeRemoved(0, ItemCount);
                    }

                    var countList = HomeActivity?.ProfileFragment?.FriendsFragment?.MAdapter?.ItemCount;
                    if (countList == 0)
                    {
                        HomeActivity?.ProfileFragment?.FriendsFragment?.ShowEmptyPage();
                    }

                    // Send Api Remove
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Friends.AddOrRemoveFriendsAsync(e.UserClass.Id.ToString()) });
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }



        public UserInfoObject GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }

        private void Click(UsersAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(UsersAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avater != "")
                {
                    d.Add(item.Avater);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext?.BaseContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class UsersAdapterViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        #region Variables Basic

        private readonly UsersAdapter UsersAdapter;
        public View MainView { get; }
        public ImageView Image { get; private set; }
        public CircleImageView ImageOnline { get; private set; }

        public TextView Name { get; private set; }
        public TextView LastTimeOnline { get; private set; }
        public CircleButton Button { get; private set; }

        #endregion

        public UsersAdapterViewHolder(UsersAdapter users, View itemView, Action<UsersAdapterClickEventArgs> clickListener, Action<UsersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                UsersAdapter = users;

                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<CircleButton>(Resource.Id.action_button);

                Button.SetOnClickListener(this);

                itemView.Click += (sender, e) => clickListener(new UsersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = Image });
                itemView.LongClick += (sender, e) => longClickListener(new UsersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (BindingAdapterPosition == RecyclerView.NoPosition) return;

                if (v.Id == Button.Id)
                {
                    switch (UsersAdapter.NameButton)
                    {
                        case "ILikedUsers":
                            UsersAdapter.LikedButtonClick(new UsersClickEventArgs { View = MainView, UserClass = null, Position = BindingAdapterPosition, ButtonFollow = Button });
                            break;
                        case "IDislikedUsers":
                            UsersAdapter.DislikedButtonClick(new UsersClickEventArgs { View = MainView, UserClass = null, Position = BindingAdapterPosition, ButtonFollow = Button });
                            break;
                        case "Friends":
                            UsersAdapter.FriendButtonClick(new UsersClickEventArgs { View = MainView, UserClass = null, Position = BindingAdapterPosition, ButtonFollow = Button });
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class UsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }

    public class UsersClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public UserInfoObject UserClass { get; set; }
        public CircleButton ButtonFollow { get; set; }
    }
}