using Android.App;
using Android.Graphics;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Adapters
{
    public class CategoriesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CategoriesAdapterClickEventArgs> ItemClick;
        public event EventHandler<CategoriesAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<Classes.CategoriesClass> MCategoriesList = new ObservableCollection<Classes.CategoriesClass>();

        public CategoriesAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => MCategoriesList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_CategoriesView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_CategoriesView, parent, false);
                var vh = new CategoriesAdapterViewHolder(itemView, Click, LongClick);
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
                    case CategoriesAdapterViewHolder holder:
                        {
                            var item = MCategoriesList[position];
                            if (item != null)
                            {
                                holder.Button.Text = item.Name;

                                //holder.Button.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(item.Color1));
                                holder.Button.SetTextColor(Color.ParseColor(item.Color2));
                            }
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                Console.WriteLine(ActivityContext);
            }
        }

        public Classes.CategoriesClass GetItem(int position)
        {
            return MCategoriesList[position];
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

        private void Click(CategoriesAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(CategoriesAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class CategoriesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public CategoriesAdapterViewHolder(View itemView, Action<CategoriesAdapterClickEventArgs> clickListener,
            Action<CategoriesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Button = MainView.FindViewById<AppCompatButton>(Resource.Id.cont);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new CategoriesAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CategoriesAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }



        public AppCompatButton Button { get; set; }

        #endregion
    }

    public class CategoriesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}