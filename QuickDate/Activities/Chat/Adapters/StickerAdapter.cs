using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuickDate.Activities.Chat.Adapters
{
    public class StickerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<StickerAdapterClickEventArgs> OnItemClick;
        public event EventHandler<StickerAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        private ObservableCollection<DataFile> StickerList = new ObservableCollection<DataFile>();

        public StickerAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                GetSticker();
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
                //Setup your layout here >> Sticker_view
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_StickerView, parent, false);
                var vh = new StickerAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is StickerAdapterViewHolder holder)
                {
                    var item = StickerList[position];
                    if (item != null)
                    {
                        var imageSplit = item.File.Split('/').Last();
                        var getImage = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskSticker, imageSplit);
                        if (getImage != "File Dont Exists")
                        {
                            Glide.With(ActivityContext?.BaseContext).Load(item.File).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.Image);
                        }
                        else
                        {
                            var url = item.File.Contains("media3.giphy.com/");
                            if (url)
                            {
                                item.File = item.File.Replace(InitializeQuickDate.WebsiteUrl, "");
                            }

                            //Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskSticker, item.File);
                            Glide.With(ActivityContext?.BaseContext).Load(item.File).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.Image);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void GetSticker()
        {
            try
            {
                StickerList = new ObservableCollection<DataFile>(ListUtils.StickersList.Where(a => !a.File.Contains(".gif")).ToList());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => StickerList?.Count ?? 0;

        public DataFile GetItem(int position)
        {
            return StickerList[position];
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

        void Click(StickerAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(StickerAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);
    }

    public class StickerAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public ImageView Image { get; private set; }

        #endregion

        public StickerAdapterViewHolder(View itemView, Action<StickerAdapterClickEventArgs> clickListener, Action<StickerAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = itemView.FindViewById<ImageView>(Resource.Id.stickerImage);

                //Event
                itemView.Click += (sender, e) => clickListener(new StickerAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new StickerAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class StickerAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}