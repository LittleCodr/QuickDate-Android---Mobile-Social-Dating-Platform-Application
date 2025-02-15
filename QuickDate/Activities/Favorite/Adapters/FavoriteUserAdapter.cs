using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using DE.Hdodenhof.CircleImageViewLib;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Favorites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Favorite.Adapters
{
    public class FavoriteUserAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        public ObservableCollection<FavoritesObject> UserList = new ObservableCollection<FavoritesObject>();
        public event EventHandler<FavoriteUserAdapterClickEventArgs> FavItemClick;
        public event EventHandler<FavoriteUserAdapterClickEventArgs> OnItemClick;
        public event EventHandler<FavoriteUserAdapterClickEventArgs> OnItemLongClick;

        #endregion

        public FavoriteUserAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
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
                //Setup your layout here >> Style_FavoriteView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_FavoriteView, parent, false);
                var vh = new FavoriteUserAdapterViewHolder(itemView, FavClick, Click, LongClick);
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
                if (viewHolder is FavoriteUserAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.UserData.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                        holder.ImageOnline.Visibility = QuickDateTools.GetStatusOnline(item.UserData.Lastseen, item.UserData.Online) ? ViewStates.Visible : ViewStates.Gone;
                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item.UserData), 14);
                        holder.Button.Text = ActivityContext.GetString(Resource.String.Lbl_UnFavorite);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public FavoritesObject GetItem(int position)
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

        private void FavClick(FavoriteUserAdapterClickEventArgs args)
        {
            FavItemClick?.Invoke(this, args);
        }

        private void Click(FavoriteUserAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(FavoriteUserAdapterClickEventArgs args)
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

                if (item.UserData?.Avater != "")
                {
                    d.Add(item.UserData.Avater);
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
            return Glide.With(ActivityContext?.BaseContext).Load(p0.ToString()).Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class FavoriteUserAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; }
        public ImageView Image { get; private set; }
        public CircleImageView ImageOnline { get; private set; }

        public TextView Name { get; private set; }
        public TextView LastTimeOnline { get; private set; }
        public AppCompatButton Button { get; private set; }

        #endregion

        public FavoriteUserAdapterViewHolder(View itemView, Action<FavoriteUserAdapterClickEventArgs> favClickListener, Action<FavoriteUserAdapterClickEventArgs> clickListener, Action<FavoriteUserAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<AppCompatButton>(Resource.Id.btn_UnFavorite);

                Button.Click += (sender, e) => favClickListener(new FavoriteUserAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = Image });
                itemView.Click += (sender, e) => clickListener(new FavoriteUserAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = Image });
                itemView.LongClick += (sender, e) => longClickListener(new FavoriteUserAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class FavoriteUserAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }

    public class UsersClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public FavoritesObject UserClass { get; set; }
        public AppCompatButton ButtonFollow { get; set; }
    }
}