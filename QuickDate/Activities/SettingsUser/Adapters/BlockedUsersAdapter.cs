using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.SettingsUser.Adapters
{
    public class BlockedUsersAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<BlockedUsersAdapterClickEventArgs> OnItemClick;
        public event EventHandler<BlockedUsersAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<Block> BlockedUsersList = new ObservableCollection<Block>();

        public BlockedUsersAdapter(Activity context)
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
                //Setup your layout here >> Style_BlockedUsersView
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_BlockedUsersView, parent, false);
                var vh = new BlockedUsersAdapterViewHolder(itemView, Click, LongClick);
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

                if (viewHolder is BlockedUsersAdapterViewHolder holder)
                {
                    var item = BlockedUsersList[position];
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

        public void Initialize(BlockedUsersAdapterViewHolder holder, Block users)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, users.Data.Avater, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                string name = Methods.FunString.DecodeString(users.Data.FullName);
                holder.UserName.Text = Methods.FunString.SubStringCutOf(name, 25);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Remove(Block item)
        {
            try
            {
                var index = BlockedUsersList.IndexOf(BlockedUsersList.FirstOrDefault(a => a.Id == item.Id));
                if (index != -1)
                {
                    BlockedUsersList.Remove(item);
                    NotifyItemRemoved(index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override int ItemCount => BlockedUsersList?.Count ?? 0;

        public Block GetItem(int position)
        {
            return BlockedUsersList[position];
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

        void Click(BlockedUsersAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(BlockedUsersAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = BlockedUsersList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Data?.Avater != "")
                {
                    d.Add(item.Data?.Avater);
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

    public class BlockedUsersAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView ImageUser { get; private set; }

        public TextView UserName { get; private set; }

        public AppCompatButton BtnBlockControl { get; private set; }

        #endregion

        public BlockedUsersAdapterViewHolder(View itemView, Action<BlockedUsersAdapterClickEventArgs> clickListener, Action<BlockedUsersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                ImageUser = (ImageView)MainView.FindViewById(Resource.Id.ImageUser);
                UserName = (TextView)MainView.FindViewById(Resource.Id.userName);

                BtnBlockControl = (AppCompatButton)MainView.FindViewById(Resource.Id.btn_block_control);

                //Event
                BtnBlockControl.Click += (sender, e) => clickListener(new BlockedUsersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new BlockedUsersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class BlockedUsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}