using AmulyaKhare.TextDrawableLib;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AT.Markushi.UI;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Users;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.SettingsUser.Adapters
{
    public class ManageSessionsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ManageSessionsAdapterClickEventArgs> CloseItemClick;
        public event EventHandler<ManageSessionsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ManageSessionsAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;

        public ObservableCollection<FetchSessionsObject.SessionsDataObject> SessionsList = new ObservableCollection<FetchSessionsObject.SessionsDataObject>();

        public ManageSessionsAdapter(Activity context)
        {
            try
            {
                //HasStableIds = true;
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => SessionsList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SessionsView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_SessionsView, parent, false);
                var vh = new ManageSessionsAdapterViewHolder(itemView, CloseClick, Click, LongClick);
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
                if (viewHolder is ManageSessionsAdapterViewHolder holder)
                {
                    var item = SessionsList[position];
                    if (item != null)
                    {
                        holder.Platform.Text = item.Os;
                        holder.Browser.Text = ActivityContext.GetText(Resource.String.Lbl_Browser) + " : " + item.Platform;
                        holder.Seen.Text = ActivityContext.GetText(Resource.String.Lbl_Last_seen) + " : " + item.TimeText;

                        if (item.Platform != null)
                        {
                            var drawable = TextDrawable.InvokeBuilder().BeginConfig().FontSize(35).EndConfig().BuildRound(item.Platform.Substring(0, 1), Color.ParseColor(AppSettings.MainColor));
                            holder.Image.SetImageDrawable(drawable);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public FetchSessionsObject.SessionsDataObject GetItem(int position)
        {
            return SessionsList[position];
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

        private void CloseClick(ManageSessionsAdapterClickEventArgs args)
        {
            CloseItemClick?.Invoke(this, args);
        }

        private void Click(ManageSessionsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(ManageSessionsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class ManageSessionsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ManageSessionsAdapterViewHolder(View itemView, Action<ManageSessionsAdapterClickEventArgs> closeClickListener, Action<ManageSessionsAdapterClickEventArgs> clickListener, Action<ManageSessionsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                Platform = MainView.FindViewById<TextView>(Resource.Id.card_name);
                Browser = MainView.FindViewById<TextView>(Resource.Id.card_Browser);
                Seen = MainView.FindViewById<TextView>(Resource.Id.card_Seen);
                Button = MainView.FindViewById<CircleButton>(Resource.Id.ImageCircle);

                //Event  
                Button.Click += (sender, e) => closeClickListener(new ManageSessionsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.Click += (sender, e) => clickListener(new ManageSessionsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ManageSessionsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public ImageView Image { get; private set; }
        public TextView Platform { get; private set; }
        public TextView Browser { get; private set; }
        public TextView Seen { get; private set; }
        public CircleButton Button { get; private set; }

        #endregion
    }

    public class ManageSessionsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}