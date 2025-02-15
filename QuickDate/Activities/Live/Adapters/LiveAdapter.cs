using Android.App;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Live;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;


namespace QuickDate.Activities.Live.Adapters
{
    public class LiveAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<LiveAdapterClickEventArgs> ItemClick;
        public event EventHandler<LiveAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<LiveDataObject> LiveList = new ObservableCollection<LiveDataObject>();

        public LiveAdapter(Activity context)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => LiveList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_LiveView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_LiveView, parent, false);
                var vh = new LiveAdapterViewHolder(itemView, Click, LongClick);
                return vh;
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
                switch (viewHolder)
                {
                    case LiveAdapterViewHolder holder:
                        {
                            var item = LiveList[position];
                            if (item != null)
                            {
                                GlideImageLoader.LoadImage(ActivityContext, item.UserData.Avater, holder.ImgUser, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                                holder.TxtName.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item.UserData), 20) + ", " + QuickDateTools.GetAgeUser(item.UserData);

                                var distanceBetween = QuickDateTools.DistanceBetween(new LatLng(Convert.ToDouble(UserDetails.Lat), Convert.ToDouble(UserDetails.Lng)), new LatLng(Convert.ToDouble(item.UserData.Lat), Convert.ToDouble(item.UserData.Lng)));
                                holder.TxtLastSeen.Text = QuickDateTools.ToSi(distanceBetween);

                                // nearByAdapter.TxtLastSeen.Text = !string.IsNullOrEmpty(item.UserData.LastseenTxt) ? item.UserData.LastseenTxt : Methods.Time.TimeAgo(int.Parse(item.UserData.Lastseen), false);
                                holder.OnlineIcon.Visibility = QuickDateTools.GetStatusOnline(item.UserData.Lastseen, item.UserData.Online) ? ViewStates.Visible : ViewStates.Gone;
                            }
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public LiveDataObject GetItem(int position)
        {
            return LiveList[position];
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
                return position;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return 0;
            }
        }

        private void Click(LiveAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(LiveAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = LiveList[p0];
                switch (item)
                {
                    case null:
                        return Collections.SingletonList(p0);
                }

                switch (string.IsNullOrEmpty(item.UserData?.Avater))
                {
                    case false:
                        d.Add(item.UserData?.Avater);
                        break;
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
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.RoundedCrop);
        }
    }

    public class LiveAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public ImageView ImgUser { get; private set; }
        public TextView TxtName { get; private set; }
        public TextView TxtLastSeen { get; private set; }
        public View OnlineIcon { get; private set; }

        #endregion  

        public LiveAdapterViewHolder(View itemView, Action<LiveAdapterClickEventArgs> clickListener, Action<LiveAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                ImgUser = itemView.FindViewById<ImageView>(Resource.Id.imgUser);
                TxtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                TxtLastSeen = itemView.FindViewById<TextView>(Resource.Id.LastSeen);
                OnlineIcon = itemView.FindViewById<View>(Resource.Id.OnlineIcon);
                OnlineIcon.Visibility = ViewStates.Gone;

                //Event
                itemView.Click += (sender, e) => clickListener(new LiveAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LiveAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }



    }

    public class LiveAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}