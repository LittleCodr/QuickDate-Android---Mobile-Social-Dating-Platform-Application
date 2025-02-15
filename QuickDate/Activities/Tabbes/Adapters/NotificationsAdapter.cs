using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class NotificationsAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<NotificationsAdapterClickEventArgs> DeleteButtonItemClick;
        public event EventHandler<NotificationsAdapterClickEventArgs> AddButtonItemClick;
        public event EventHandler<NotificationsAdapterClickEventArgs> OnItemClick;
        public event EventHandler<NotificationsAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<GetNotificationsObject.Datum> NotificationsList = new ObservableCollection<GetNotificationsObject.Datum>();

        public NotificationsAdapter(Activity context)
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

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Notifications_view
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_NotificationsView, parent, false);
                var vh = new NotificationsAdapterViewHolder(itemView, AddButtonClick, DeleteButtonClick, Click, LongClick);
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
                if (viewHolder is NotificationsAdapterViewHolder holder)
                {
                    var item = NotificationsList[position];
                    if (item != null)
                    {
                        holder.UserNameNoitfy.Text = QuickDateTools.GetNameFinal(item.Notifier);

                        GlideImageLoader.LoadImage(ActivityContext, item.Notifier.Avater, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                        switch (item.Type)
                        {
                            case "got_new_match":
                                {
                                    holder.CircleIcon.SetBackgroundResource(Resource.Drawable.img_matches);

                                    holder.CircleIcon.Visibility = ViewStates.Visible;
                                    holder.AddButton.Visibility = ViewStates.Gone;
                                    holder.DeleteButton.Visibility = ViewStates.Gone;
                                    holder.TimeText.Visibility = ViewStates.Visible;
                                    break;
                                }
                            case "like":
                                {
                                    holder.CircleIcon.SetBackgroundResource(Resource.Drawable.img_fire);

                                    holder.CircleIcon.Visibility = ViewStates.Visible;
                                    holder.AddButton.Visibility = ViewStates.Gone;
                                    holder.DeleteButton.Visibility = ViewStates.Gone;
                                    holder.TimeText.Visibility = ViewStates.Visible;
                                    break;
                                }
                            case "visit":
                                {
                                    holder.CircleIcon.SetBackgroundResource(Resource.Drawable.img_eye);
                                    holder.CircleIcon.Visibility = ViewStates.Visible;
                                    holder.AddButton.Visibility = ViewStates.Gone;
                                    holder.DeleteButton.Visibility = ViewStates.Gone;
                                    holder.TimeText.Visibility = ViewStates.Visible;
                                    break;
                                }
                            case "friend_request_accepted":
                                {
                                    holder.CircleIcon.SetBackgroundResource(Resource.Drawable.img_request);

                                    holder.CircleIcon.Visibility = ViewStates.Visible;
                                    holder.AddButton.Visibility = ViewStates.Gone;
                                    holder.DeleteButton.Visibility = ViewStates.Gone;
                                    holder.TimeText.Visibility = ViewStates.Visible;
                                    break;
                                }
                            case "friend_request":
                                {
                                    holder.CircleIcon.SetBackgroundResource(Resource.Drawable.img_request);

                                    holder.CircleIcon.Visibility = ViewStates.Visible;
                                    holder.AddButton.Visibility = ViewStates.Visible;
                                    holder.DeleteButton.Visibility = ViewStates.Visible;
                                    holder.TimeText.Visibility = ViewStates.Invisible;
                                    break;
                                }
                        }

                        holder.Description.Text = QuickDateTools.GetNotificationsText(item);

                        bool success = int.TryParse(item.CreatedAt, out var number);
                        if (success)
                        {
                            holder.TimeText.Text = Methods.Time.TimeAgo(number, false);
                        }
                        else
                        {
                            bool successData = DateTime.TryParse(item.CreatedAt, out var data);
                            if (successData)
                            {
                                holder.TimeText.Text = Methods.Time.TimeAgo(data, false);
                            }
                            else
                            {
                                holder.TimeText.Text = item.CreatedAt;
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

        public override int ItemCount => NotificationsList?.Count ?? 0;

        public GetNotificationsObject.Datum GetItem(int position)
        {
            return NotificationsList[position];
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

        void AddButtonClick(NotificationsAdapterClickEventArgs args) => AddButtonItemClick?.Invoke(this, args);
        void DeleteButtonClick(NotificationsAdapterClickEventArgs args) => DeleteButtonItemClick?.Invoke(this, args);
        void Click(NotificationsAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(NotificationsAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = NotificationsList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Notifier.Avater != "")
                {
                    d.Add(item.Notifier.Avater);
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

    public class NotificationsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView ImageUser { get; private set; }
        public View CircleIcon { get; private set; }
        public TextView UserNameNoitfy { get; private set; }
        public TextView Description { get; private set; }
        public TextView TimeText { get; private set; }
        public CircleButton AddButton { get; private set; }
        public CircleButton DeleteButton { get; private set; }

        #endregion

        public NotificationsAdapterViewHolder(View itemView, Action<NotificationsAdapterClickEventArgs> addButtonClickListener, Action<NotificationsAdapterClickEventArgs> deleteButtonClickListener
            , Action<NotificationsAdapterClickEventArgs> clickListener, Action<NotificationsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                ImageUser = (ImageView)MainView.FindViewById(Resource.Id.ImageUser);
                CircleIcon = MainView.FindViewById<View>(Resource.Id.CircleIcon);
                UserNameNoitfy = (TextView)MainView.FindViewById(Resource.Id.NotificationsName);
                Description = (TextView)MainView.FindViewById(Resource.Id.NotificationsText);
                TimeText = (TextView)MainView.FindViewById(Resource.Id.TimeText);
                AddButton = MainView.FindViewById<CircleButton>(Resource.Id.Add_button);
                DeleteButton = MainView.FindViewById<CircleButton>(Resource.Id.delete_button);

                AddButton.Visibility = ViewStates.Gone;
                DeleteButton.Visibility = ViewStates.Gone;

                //Create an Event
                AddButton.Click += (sender, e) => addButtonClickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                DeleteButton.Click += (sender, e) => deleteButtonClickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.Click += (sender, e) => clickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = ImageUser });
                itemView.LongClick += (sender, e) => longClickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition, Image = ImageUser });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class NotificationsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }
}