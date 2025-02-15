using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Gift.Adapters
{
    public class GiftAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        private ObservableCollection<DataFile> GiftsList = new ObservableCollection<DataFile>();
        public event EventHandler<GiftAdapterClickEventArgs> OnItemClick;
        public event EventHandler<GiftAdapterClickEventArgs> OnItemLongClick;
        private readonly string Type = "Normal";
        #endregion

        public GiftAdapter(Activity context, string type)
        {
            try
            {
                ActivityContext = context;
                Type = type;
                HasStableIds = true;
                GetGift(type);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => GiftsList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_GiftView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_GiftView, parent, false);
                var vh = new GiftAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is GiftAdapterViewHolder holder)
                {
                    var item = GiftsList[position];
                    if (item != null)
                    {
                        var imageSplit = item.File.Split('/').Last();
                        string folderName = Type == "Chat" ? Methods.Path.FolderDiskSticker : Methods.Path.FolderDiskGif;
                        string getImage = Methods.MultiMedia.GetMediaFrom_Disk(folderName, imageSplit);
                        if (getImage == "File Dont Exists")
                        {
                            var url = item.File.Contains("media3.giphy.com/");
                            if (url)
                            {
                                item.File = item.File.Replace(InitializeQuickDate.WebsiteUrl, "");
                            }

                            //Methods.MultiMedia.DownloadMediaTo_DiskAsync(folderName, item.File);
                        }

                        Glide.With(ActivityContext?.BaseContext).Load(item.File).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.ImgGift);

                        //GlideImageLoader.LoadImage(ActivityContext, item.File, holder.ImgGift, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void GetGift(string type)
        {
            try
            {
                GiftsList = type == "Chat" ? new ObservableCollection<DataFile>(ListUtils.StickersList.Where(a => a.File.Contains(".gif")).ToList()) : ListUtils.GiftsList;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public DataFile GetItem(int position)
        {
            return GiftsList[position];
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

        public void Click(GiftAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public void LongClick(GiftAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = GiftsList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.File != "")
                {
                    d.Add(item.File);
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
                .Apply(new RequestOptions().CenterCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class GiftAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public ImageView ImgGift { get; private set; }
        public View MainView { get; }

        #endregion

        public GiftAdapterViewHolder(View itemView, Action<GiftAdapterClickEventArgs> clickListener, Action<GiftAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ImgGift = MainView.FindViewById<ImageView>(Resource.Id.Image);

                itemView.Click += (sender, e) => clickListener(new GiftAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new GiftAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class GiftAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}