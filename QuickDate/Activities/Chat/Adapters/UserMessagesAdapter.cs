using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Com.Luseen.Autolinklibrary;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace QuickDate.Activities.Chat.Adapters
{
    public class UserMessagesAdapter : RecyclerView.Adapter
    {
        #region Variables Basic

        private readonly MessagesBoxActivity ActivityContext;
        public ObservableCollection<GetChatConversationsObject.Messages> MessageList = new ObservableCollection<GetChatConversationsObject.Messages>();

        private readonly SparseBooleanArray SelectedItems;
        private IOnClickListenerSelectedMessages ClickListener;
        private int CurrentSelectedIdx = -1;

        #endregion

        public UserMessagesAdapter(MessagesBoxActivity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                SelectedItems = new SparseBooleanArray();
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
                //Setup your layout here >> 
                var itemView = MessageList[viewType];
                if (itemView != null)
                {
                    if (itemView.From == UserDetails.UserId && itemView.MessageType == "text")
                    {
                        View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Right_MS_view, parent, false);
                        TextViewHolder textViewHolder = new TextViewHolder(row, ActivityContext);
                        return textViewHolder;
                    }

                    if (itemView.To == UserDetails.UserId && itemView.MessageType == "text")
                    {
                        View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Left_MS_view, parent, false);
                        TextViewHolder textViewHolder = new TextViewHolder(row, ActivityContext);
                        return textViewHolder;
                    }
                    if (itemView.From == UserDetails.UserId && itemView.MessageType == "media")
                    {
                        View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Right_MS_image, parent, false);
                        ImageViewHolder imageViewHolder = new ImageViewHolder(row);
                        return imageViewHolder;
                    }
                    if (itemView.To == UserDetails.UserId && itemView.MessageType == "media")
                    {
                        View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Left_MS_image, parent, false);
                        ImageViewHolder imageViewHolder = new ImageViewHolder(row);
                        return imageViewHolder;
                    }
                    if (itemView.From == UserDetails.UserId && itemView.MessageType == "sticker")
                    {
                        if (itemView.Sticker.Contains(".gif"))
                        {
                            View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Right_MS_image, parent, false);
                            ImageViewHolder viewHolder = new ImageViewHolder(row);
                            return viewHolder;
                        }
                        else
                        {
                            View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Right_MS_sticker, parent, false);
                            StickerViewHolder stickerViewHolder = new StickerViewHolder(row);
                            return stickerViewHolder;
                        }
                    }
                    if (itemView.To == UserDetails.UserId && itemView.MessageType == "sticker")
                    {
                        if (itemView.Sticker.Contains(".gif"))
                        {
                            View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Left_MS_image, parent, false);
                            ImageViewHolder viewHolder = new ImageViewHolder(row);
                            return viewHolder;
                        }
                        else
                        {
                            View row = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Left_MS_sticker, parent, false);
                            StickerViewHolder stickerViewHolder = new StickerViewHolder(row);
                            return stickerViewHolder;
                        }
                    }

                    return null;
                }

                return null;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder vh, int position)
        {
            try
            {
                int type = GetItemViewType(position);
                var item = MessageList[type];
                if (item == null) return;
                switch (item.MessageType)
                {
                    case "text":
                        {
                            TextViewHolder holder = vh as TextViewHolder;
                            LoadTextOfChatItem(holder, position, item);
                            break;
                        }
                    case "media":
                        {
                            ImageViewHolder holder = vh as ImageViewHolder;
                            LoadImageOfChatItem(holder, position, item);
                            break;
                        }
                    case "sticker" when item.Sticker.Contains(".gif"):
                        {
                            ImageViewHolder holder = vh as ImageViewHolder;
                            LoadGifOfChatItem(holder, position, item);
                            break;
                        }
                    case "sticker":
                        {
                            StickerViewHolder holder = vh as StickerViewHolder;
                            LoadStickerOfChatItem(holder, position, item);
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void SetOnClickListener(IOnClickListenerSelectedMessages onClickListener)
        {
            ClickListener = onClickListener;
        }

        #region Load Holder

        private void LoadTextOfChatItem(TextViewHolder holder, int position, GetChatConversationsObject.Messages item)
        {
            try
            {
                try { holder.Time.Text = Methods.Time.TimeAgo(Convert.ToInt32(item.CreatedAt)); }
                catch
                {
                    // ignored
                }

                holder.TextSanitizerAutoLink?.Load(Methods.FunString.DecodeString(item.Text), item.Type);

                holder.LytParent.Activated = SelectedItems.Get(position, false);

                if (!holder.LytParent.HasOnClickListeners)
                {
                    holder.LytParent.Click += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };

                    holder.LytParent.LongClick += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemLongClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void LoadImageOfChatItem(ImageViewHolder holder, int position, GetChatConversationsObject.Messages message)
        {
            try
            {
                string imageUrl = message.Media.Contains("upload/chat/") && !message.Media.Contains(InitializeQuickDate.WebsiteUrl) ? InitializeQuickDate.WebsiteUrl + "/" + message.Media : message.Media;

                string fileSavedPath;

                try { holder.Time.Text = Methods.Time.TimeAgo(Convert.ToInt32(message.CreatedAt)); }
                catch
                {
                    // ignored
                }

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('/').Last();
                    string imageFile = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimImage, fileName);
                    if (imageFile == "File Dont Exists")
                    {
                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Visible;

                        string mediaFile = Methods.Path.FolderDcimImage + fileName;
                        fileSavedPath = mediaFile;

                        HttpClient client;
                        if (AppSettings.TurnSecurityProtocolType3072On)
                        {
                            HttpClientHandler clientHandler = new HttpClientHandler();
                            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                            //clientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Default;

                            // Pass the handler to httpClient(from you are calling api)
                            client = new HttpClient(clientHandler);
                        }
                        else
                        {
                            client = new HttpClient();
                        }

                        var s = await client.GetStreamAsync(new System.Uri(imageUrl));
                        if (s.CanRead)
                        {
                            if (File.Exists(mediaFile)) return;
                            await using FileStream fs = new FileStream(mediaFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                            await s.CopyToAsync(fs);

                            // Tell the media scanner about the new file so that it is
                            // immediately available to the user.
                            MediaScannerConnection.ScanFile(Application.Context, new[] { mediaFile }, null, null);

                            GlideImageLoader.LoadImage(ActivityContext, mediaFile, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                            holder.LoadingProgressView.Indeterminate = false;
                            holder.LoadingProgressView.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        fileSavedPath = imageFile;

                        GlideImageLoader.LoadImage(ActivityContext, imageFile, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    fileSavedPath = imageUrl;

                    GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                    holder.LoadingProgressView.Indeterminate = false;
                    holder.LoadingProgressView.Visibility = ViewStates.Gone;
                }

                if (!holder.ImageView.HasOnClickListeners)
                {
                    holder.ImageView.Click += (sender, args) =>
                    {
                        try
                        {
                            string imageFile = Methods.MultiMedia.CheckFileIfExits(fileSavedPath);
                            if (imageFile != "File Dont Exists")
                            {
                                Java.IO.File file2 = new Java.IO.File(fileSavedPath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetDataAndType(photoUri, "image/*");
                                ActivityContext.StartActivity(intent);
                            }
                            else
                            {
                                Intent intent = new Intent(Intent.ActionView, Uri.Parse(imageUrl));
                                ActivityContext.StartActivity(intent);
                            }
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }

                holder.LytParent.Activated = SelectedItems.Get(position, false);
                if (!holder.LytParent.HasOnClickListeners)
                {
                    holder.LytParent.Click += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };

                    holder.LytParent.LongClick += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemLongClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }


                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private async void LoadGifOfChatItem(ImageViewHolder holder, int position, GetChatConversationsObject.Messages item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Sticker))
                    return;

                string imageUrl = item.Sticker;
                string fileSavedPath;

                try { holder.Time.Text = Methods.Time.TimeAgo(Convert.ToInt32(item.CreatedAt)); }
                catch
                {
                    // ignored
                }
                // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('_').Last();
                    string imageFile = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskGif, fileName);
                    if (imageFile == "File Dont Exists")
                    {
                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Visible;

                        string filePath = Path.Combine(Methods.Path.FolderDiskGif);
                        string mediaFile = filePath + "/" + fileName;
                        fileSavedPath = mediaFile;

                        var url = imageUrl.Contains("media3.giphy.com/");
                        if (url)
                        {
                            imageUrl = imageUrl.Replace(InitializeQuickDate.WebsiteUrl, "");
                        }

                        HttpClient client;
                        if (AppSettings.TurnSecurityProtocolType3072On)
                        {
                            HttpClientHandler clientHandler = new HttpClientHandler();
                            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                            //clientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Default;

                            // Pass the handler to httpClient(from you are calling api)
                            client = new HttpClient(clientHandler);
                        }
                        else
                        {
                            client = new HttpClient();
                        }

                        var s = await client.GetStreamAsync(new System.Uri(imageUrl));
                        if (s.CanRead)
                        {
                            if (File.Exists(mediaFile)) return;
                            await using FileStream fs = new FileStream(mediaFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                            await s.CopyToAsync(fs);

                            // Tell the media scanner about the new file so that it is
                            // immediately available to the user.
                            MediaScannerConnection.ScanFile(Application.Context, new[] { mediaFile }, null, null);

                            GlideImageLoader.LoadImage(ActivityContext, mediaFile, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                            holder.LoadingProgressView.Indeterminate = false;
                            holder.LoadingProgressView.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        fileSavedPath = imageFile;

                        GlideImageLoader.LoadImage(ActivityContext, imageFile, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    fileSavedPath = imageUrl;

                    GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);

                    holder.LoadingProgressView.Indeterminate = false;
                    holder.LoadingProgressView.Visibility = ViewStates.Gone;
                }

                if (!holder.ImageView.HasOnClickListeners)
                {
                    holder.ImageView.Click += (sender, args) =>
                    {
                        try
                        {
                            string imageFile = Methods.MultiMedia.CheckFileIfExits(fileSavedPath);
                            if (imageFile != "File Dont Exists")
                            {
                                Java.IO.File file2 = new Java.IO.File(fileSavedPath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetDataAndType(photoUri, "image/*");
                                ActivityContext.StartActivity(intent);
                            }
                            else
                            {
                                Intent intent = new Intent(Intent.ActionView, Uri.Parse(imageUrl));
                                ActivityContext.StartActivity(intent);
                            }
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }

                holder.LytParent.Activated = SelectedItems.Get(position, false);
                if (!holder.LytParent.HasOnClickListeners)
                {
                    holder.LytParent.Click += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };

                    holder.LytParent.LongClick += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemLongClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }

                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void LoadStickerOfChatItem(StickerViewHolder holder, int position, GetChatConversationsObject.Messages message)
        {
            try
            {
                string imageUrl = message.Sticker;

                try { holder.Time.Text = Methods.Time.TimeAgo(Convert.ToInt32(message.CreatedAt)); }
                catch
                {
                    // ignored
                }

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('_').Last();
                    string imageFile = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskSticker, fileName);
                    if (imageFile == "File Dont Exists")
                    {
                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Visible;

                        var url = imageUrl.Contains("media3.giphy.com/");
                        if (url)
                        {
                            imageUrl = imageUrl.Replace(InitializeQuickDate.WebsiteUrl, "");
                        }

                        //Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskSticker, imageUrl);

                        GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.LoadingProgressView.Indeterminate = false;
                        holder.LoadingProgressView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    GlideImageLoader.LoadImage(ActivityContext, imageUrl, holder.ImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    holder.LoadingProgressView.Indeterminate = false;
                    holder.LoadingProgressView.Visibility = ViewStates.Gone;
                }


                holder.LytParent.Activated = SelectedItems.Get(position, false);
                if (!holder.LytParent.HasOnClickListeners)
                {
                    holder.LytParent.Click += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };

                    holder.LytParent.LongClick += delegate
                    {
                        try
                        {
                            if (ClickListener == null) return;

                            var dataItem = MessageList[holder.BindingAdapterPosition];
                            ClickListener.ItemLongClick(holder.LytParent, dataItem, holder.BindingAdapterPosition);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    };
                }
                ToggleCheckedBackground(holder, position);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        public override int ItemCount => MessageList?.Count ?? 0;

        public GetChatConversationsObject.Messages GetItem(int position)
        {
            return MessageList[position];
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

        #region Toolbar & Selected

        private void ToggleCheckedBackground(dynamic holder, int position)
        {
            try
            {
                if (SelectedItems.Get(position, false))
                {
                    holder.MainView.SetBackgroundColor(Color.LightBlue);
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
                else
                {
                    holder.MainView.SetBackgroundColor(Color.Transparent);
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResetCurrentItems()
        {
            try
            {
                CurrentSelectedIdx = -1;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public int GetSelectedItemCount()
        {
            return SelectedItems.Size();
        }

        public List<int> GetSelectedItems()
        {
            List<int> items = new List<int>(SelectedItems.Size());
            for (int i = 0; i < SelectedItems.Size(); i++)
            {
                items.Add(SelectedItems.KeyAt(i));
            }
            return items;
        }

        public void RemoveData(int position, GetChatConversationsObject.Messages users)
        {
            try
            {
                var index = MessageList.IndexOf(MessageList.FirstOrDefault(a => a.Id == users.Id));
                if (index != -1)
                {
                    MessageList.Remove(users);
                    NotifyItemRemoved(index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }

                ResetCurrentItems();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ClearSelections()
        {
            try
            {
                SelectedItems.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ToggleSelection(int pos)
        {
            try
            {
                CurrentSelectedIdx = pos;
                if (SelectedItems.Get(pos, false))
                {
                    SelectedItems.Delete(pos);
                }
                else
                {
                    SelectedItems.Put(pos, true);
                }
                NotifyItemChanged(pos);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

    }

    public class TextViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public LinearLayout LytParent { get; private set; }
        public TextView Time { get; private set; }
        public View MainView { get; private set; }
        public AutoLinkTextView AutoLinkTextView { get; private set; }
        public TextSanitizer TextSanitizerAutoLink { get; private set; }

        #endregion

        public TextViewHolder(View itemView, AppCompatActivity activity) : base(itemView)
        {
            try
            {
                MainView = itemView;

                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                AutoLinkTextView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

                TextSanitizerAutoLink = new TextSanitizer(AutoLinkTextView, activity);

                //itemView.Click += (sender, e) => clickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new UserMessagesAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class ImageViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public LinearLayout LytParent { get; private set; }
        public ImageView ImageView { get; private set; }
        public ProgressBar LoadingProgressView { get; private set; }
        public TextView Time { get; private set; }

        #endregion

        public ImageViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                LoadingProgressView = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class StickerViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public LinearLayout LytParent { get; private set; }
        public ImageView ImageView { get; private set; }
        public ProgressBar LoadingProgressView { get; private set; }
        public TextView Time { get; private set; }

        #endregion

        public StickerViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                LoadingProgressView = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}