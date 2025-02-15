using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Adapters
{
    public class ItemOptionAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ItemOptionAdapterClickEventArgs> ItemClick;
        public event EventHandler<ItemOptionAdapterClickEventArgs> ItemLongClick;

        private Activity ActivityContext;
        public ObservableCollection<Classes.ItemOptionObject> ItemOptionList = new ObservableCollection<Classes.ItemOptionObject>();

        public ItemOptionAdapter(Activity context)
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

        public override int ItemCount => ItemOptionList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_Categories_View
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_ItemOptionView, parent, false);
                var vh = new ItemOptionAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null!;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is ItemOptionAdapterViewHolder holder)
                {
                    var item = ItemOptionList[position];
                    if (item == null) return;

                    holder.ContentText.Text = item.Text;

                    holder.IconContent.SetImageResource(item.Icon);
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public Classes.ItemOptionObject GetItem(int position)
        {
            return ItemOptionList[position];
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

        private void Click(ItemOptionAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(ItemOptionAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class ItemOptionAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; }

        public LinearLayout ContentLayout { get; set; }
        public ImageView IconContent { get; set; }
        public TextView ContentText { get; set; }

        #endregion

        public ItemOptionAdapterViewHolder(View itemView, Action<ItemOptionAdapterClickEventArgs> clickListener, Action<ItemOptionAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ContentLayout = itemView.FindViewById<LinearLayout>(Resource.Id.ContentLayout);
                IconContent = itemView.FindViewById<ImageView>(Resource.Id.IconContent);
                ContentText = itemView.FindViewById<TextView>(Resource.Id.ContentText);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new ItemOptionAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ItemOptionAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }

    public class ItemOptionAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}