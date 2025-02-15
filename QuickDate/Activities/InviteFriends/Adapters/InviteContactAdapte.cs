using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.InviteFriends.Adapters
{
    public class InviteContactAdapter : RecyclerView.Adapter
    {
        public event EventHandler<InviteContactAdapterClickEventArgs> ButtonItemClick;
        public event EventHandler<InviteContactAdapterClickEventArgs> OnItemClick;
        public event EventHandler<InviteContactAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<Methods.PhoneContactManager.UserContact> UsersPhoneContacts = new ObservableCollection<Methods.PhoneContactManager.UserContact>();

        public InviteContactAdapter(Activity context)
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
                //Setup your layout here >> Style_HContactView
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_HContactView, parent, false);
                var vh = new InviteContactAdapterViewHolder(itemView, ButtonClick, Click, LongClick);
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

                if (viewHolder is InviteContactAdapterViewHolder holder)
                {
                    var item = UsersPhoneContacts[position];
                    if (item != null)
                    {
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void Initialize(InviteContactAdapterViewHolder holder, Methods.PhoneContactManager.UserContact users)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, "no_profile_image_circle", holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                string name = Methods.FunString.DecodeString(users.UserDisplayName);
                holder.UserName.Text = name;
                holder.Description.Text = users.PhoneNumber;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => UsersPhoneContacts?.Count ?? 0;

        public Methods.PhoneContactManager.UserContact GetItem(int position)
        {
            return UsersPhoneContacts[position];
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

        void ButtonClick(InviteContactAdapterClickEventArgs args) => ButtonItemClick?.Invoke(this, args);
        void Click(InviteContactAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(InviteContactAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);
    }

    public class InviteContactAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView ImageUser { get; private set; }
        public TextView UserName { get; private set; }
        public TextView Description { get; private set; }
        public AppCompatButton ButtonSend { get; private set; }

        #endregion

        public InviteContactAdapterViewHolder(View itemView, Action<InviteContactAdapterClickEventArgs> ButtonClickListener, Action<InviteContactAdapterClickEventArgs> clickListener, Action<InviteContactAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                ImageUser = (ImageView)MainView.FindViewById(Resource.Id.card_pro_pic);

                UserName = (TextView)MainView.FindViewById(Resource.Id.card_name);
                Description = (TextView)MainView.FindViewById(Resource.Id.card_dist);
                ButtonSend = (AppCompatButton)MainView.FindViewById(Resource.Id.cont);

                //Event
                ButtonSend.Click += (sender, e) => ButtonClickListener(new InviteContactAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.Click += (sender, e) => clickListener(new InviteContactAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new InviteContactAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class InviteContactAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}