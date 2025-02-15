using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.Premium.Adapters
{
    public class CreditAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CreditAdapterViewHolderClickEventArgs> OnItemClick;
        public event EventHandler<CreditAdapterViewHolderClickEventArgs> OnItemLongClick;

        private readonly Context ActivityContext;
        private readonly ObservableCollection<CreditsClass> CreditList = new ObservableCollection<CreditsClass>();
        private string CurrencySymbol = "$";

        public CreditAdapter(Context context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                GetCredits();
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
                //Setup your layout here >> Style_HContact_view
                View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_CreditView, parent, false);
                var vh = new CreditAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is CreditAdapterViewHolder holder)
                {
                    var item = CreditList[position];

                    if (item == null)
                        return;

                    holder.Price.Text = CurrencySymbol + " " + item.Price;
                    holder.Title.Text = item.Description;
                    holder.CoinCount.Text = item.TotalCoins;

                    holder.CreditImage.SetImageResource(item.ImageFromResource);
                    holder.CreditImage.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(item.Color));

                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void GetCredits()
        {
            try
            {
                var option = ListUtils.SettingsSiteList;
                if (option != null)
                {
                    CurrencySymbol = option.CurrencySymbol ?? "$";

                    CreditList.Add(new CreditsClass { Id = 1, TotalCoins = option.BagOfCreditsAmount, Price = option.BagOfCreditsPrice, Description = ActivityContext.GetString(Resource.String.Lbl_BagOfCredits), ImageFromResource = Resource.Drawable.credits1, Color = "#FFF9E7" });
                    CreditList.Add(new CreditsClass { Id = 2, TotalCoins = option.BoxOfCreditsAmount, Price = option.BoxOfCreditsPrice, Description = ActivityContext.GetString(Resource.String.Lbl_BoxOfCredits), ImageFromResource = Resource.Drawable.credits2, Color = "#FCF2FF" });
                    CreditList.Add(new CreditsClass { Id = 3, TotalCoins = option.ChestOfCreditsAmount, Price = option.ChestOfCreditsPrice, Description = ActivityContext.GetString(Resource.String.Lbl_ChestOfCredits), ImageFromResource = Resource.Drawable.credits3, Color = "#FFF2F8" });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CreditsClass GetItem(int position)
        {
            return CreditList[position];
        }

        public override int ItemCount => CreditList?.Count ?? 0;

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


        void Click(CreditAdapterViewHolderClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(CreditAdapterViewHolderClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

    }

    public class CreditAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic


        public View MainView { get; private set; }
        public TextView Title { get; private set; }
        public ImageView CreditImage { get; private set; }
        public TextView Price { get; private set; }
        public TextView CoinCount { get; private set; }

        #endregion

        public CreditAdapterViewHolder(View itemView, Action<CreditAdapterViewHolderClickEventArgs> clickListener, Action<CreditAdapterViewHolderClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Title = MainView.FindViewById<TextView>(Resource.Id.name);
                CreditImage = MainView.FindViewById<ImageView>(Resource.Id.image);
                Price = MainView.FindViewById<TextView>(Resource.Id.price);
                CoinCount = MainView.FindViewById<TextView>(Resource.Id.count);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new CreditAdapterViewHolderClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CreditAdapterViewHolderClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class CreditAdapterViewHolderClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}