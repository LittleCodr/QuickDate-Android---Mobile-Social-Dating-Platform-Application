using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.UserProfile.Adapters
{
    public class MultiMediaAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public ObservableCollection<MediaFile> UsersMultiMediaList = new ObservableCollection<MediaFile>();

        private readonly Activity ActivityContext;
        private readonly RequestBuilder FullGlideRequestBuilder;
        public MultiMediaAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                var glideRequestOptions = new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).SetPriority(Priority.High);
                FullGlideRequestBuilder = Glide.With(context?.BaseContext).AsBitmap().Apply(glideRequestOptions).Transition(new BitmapTransitionOptions().CrossFade(100));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public MediaFile GetItem(int position)
        {
            return UsersMultiMediaList[position];
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is MultiMediaAdapterViewHolder holder)
                {
                    var item = UsersMultiMediaList[position];
                    if (item != null)
                    {
                        if (item.IsVideo == "1" && item.IsApproved == "1" && item.IsPrivate == "0" && !string.IsNullOrEmpty(item.VideoFile))
                            holder.IconImageView.Visibility = ViewStates.Visible;
                        else
                            holder.IconImageView.Visibility = ViewStates.Gone;

                        if (item.IsPrivate == "1")
                            FullGlideRequestBuilder.Load(item.PrivateFileFull).Into(holder.ImgUser);
                        else
                            FullGlideRequestBuilder.Load(item.Full).Into(holder.ImgUser);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SuggestionsView
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_MultiMediaView, parent, false);
                var vh = new MultiMediaAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        void Click(MultiMediaAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(MultiMediaAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public event EventHandler<MultiMediaAdapterClickEventArgs> OnItemClick;
        public event EventHandler<MultiMediaAdapterClickEventArgs> OnItemLongClick;

        public override int ItemCount => UsersMultiMediaList?.Count ?? 0;

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

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UsersMultiMediaList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.IsPrivate == "1" && !string.IsNullOrEmpty(item.PrivateFileFull))
                    d.Add(item.PrivateFileFull);
                else if (!string.IsNullOrEmpty(item.Full))
                    d.Add(item.Full);

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
            return Glide.With(ActivityContext?.BaseContext).Load(p0.ToString()).Apply(new RequestOptions().CenterCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class MultiMediaAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public ImageView ImgUser { get; private set; }
        public ImageView IconImageView { get; private set; }


        #endregion

        public MultiMediaAdapterViewHolder(View itemView, Action<MultiMediaAdapterClickEventArgs> clickListener, Action<MultiMediaAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {

                ImgUser = itemView.FindViewById<ImageView>(Resource.Id.imgUser);
                IconImageView = itemView.FindViewById<ImageView>(Resource.Id.Icon);

                //Event
                itemView.Click += (sender, e) => clickListener(new MultiMediaAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = ImgUser });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class MultiMediaAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }

    }
}