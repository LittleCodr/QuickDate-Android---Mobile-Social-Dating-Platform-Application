using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Live;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Live.Adapters
{
    public class LiveMessageAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<LiveMessageAdapterClickEventArgs> ItemClick;
        public event EventHandler<LiveMessageAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<CommentDataObject> CommentList = new ObservableCollection<CommentDataObject>();

        public LiveMessageAdapter(Activity context)
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

        public override int ItemCount => CommentList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_LiveStreemChatView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_LiveStreemChatView, parent, false);
                var vh = new LiveMessageAdapterViewHolder(itemView, Click, LongClick);
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
                    case LiveMessageAdapterViewHolder holder:
                        {
                            var item = CommentList[position];
                            if (item != null)
                            {
                                GlideImageLoader.LoadImage(ActivityContext, item.Publisher.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                                holder.TxtName.Text = QuickDateTools.GetNameFinal(item.Publisher);
                                holder.TxtMessage.Text = Methods.FunString.DecodeString(item.Text);
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

        public override void OnViewRecycled(Object holder)
        {
            try
            {
                if (ActivityContext?.IsDestroyed != false)
                    return;

                switch (holder)
                {
                    case LiveMessageAdapterViewHolder viewHolder:
                        Glide.With(ActivityContext?.BaseContext).Clear(viewHolder.Image);
                        break;
                }
                base.OnViewRecycled(holder);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public CommentDataObject GetItem(int position)
        {
            return CommentList[position];
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

        private void Click(LiveMessageAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(LiveMessageAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = CommentList[p0];
                switch (item)
                {
                    case null:
                        return Collections.SingletonList(p0);
                }

                switch (string.IsNullOrEmpty(item.Publisher?.Avater))
                {
                    case false:
                        d.Add(item.Publisher?.Avater);
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

    public class LiveMessageAdapterViewHolder : RecyclerView.ViewHolder
    {
        public LiveMessageAdapterViewHolder(View itemView, Action<LiveMessageAdapterClickEventArgs> clickListener, Action<LiveMessageAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;
                Image = itemView.FindViewById<ImageView>(Resource.Id.userImageView);
                TxtName = itemView.FindViewById<TextView>(Resource.Id.displayName);
                TxtMessage = itemView.FindViewById<TextView>(Resource.Id.displayMessage);

                //Event
                itemView.Click += (sender, e) => clickListener(new LiveMessageAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LiveMessageAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #region Variables Basic

        public View MainView { get; }
        public ImageView Image { get; private set; }
        public TextView TxtName { get; private set; }
        public TextView TxtMessage { get; private set; }

        #endregion Variables Basic

    }

    public class LiveMessageAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}