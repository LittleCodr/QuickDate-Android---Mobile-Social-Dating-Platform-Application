using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Users;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.SettingsUser.Adapters
{
    public class TransactionsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<TransactionsAdapterClickEventArgs> ItemClick;
        public event EventHandler<TransactionsAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;

        public ObservableCollection<TransactionsDataObject> TransactionsList = new ObservableCollection<TransactionsDataObject>();

        public TransactionsAdapter(Activity context)
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

        public override int ItemCount => TransactionsList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_TransactionsView
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_TransactionsView, parent, false);
                var vh = new TransactionsAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is TransactionsAdapterViewHolder holder)
                {
                    var item = TransactionsList[position];
                    if (item != null)
                    {
                        holder.ProcessedText.Text = item.Via;

                        if (item.Type == "PRO")
                        {
                            switch (item.ProPlan)
                            {
                                case "1":
                                    holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Pro) + " - " + ActivityContext.GetText(Resource.String.Lbl_Weekly);
                                    break;
                                case "2":
                                    holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Pro) + " - " + ActivityContext.GetText(Resource.String.Lbl_Monthly);
                                    break;
                                case "3":
                                    holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Pro) + " - " + ActivityContext.GetText(Resource.String.Lbl_Yearly);
                                    break;
                                case "4":
                                    holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Pro) + " - " + ActivityContext.GetText(Resource.String.Lbl_Lifetime);
                                    break;
                                default:
                                    holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Pro) + " - " + ActivityContext.GetText(Resource.String.Lbl_Upgrade);
                                    break;
                            }
                        }
                        else if (item.Type == "CREDITS")
                        {
                            holder.TypeText.Text = ActivityContext.GetText(Resource.String.Lbl_Credits) + " - " + item.CreditAmount + " " + ActivityContext.GetText(Resource.String.Lbl_Credits);
                        }
                        else
                        {
                            holder.TypeText.Text = item.Type;
                        }

                        holder.AmountValue.Text = "$" + item.Amount;
                        holder.DateText.Text = item.Date;
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public TransactionsDataObject GetItem(int position)
        {
            return TransactionsList[position];
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

        private void Click(TransactionsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(TransactionsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class TransactionsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TransactionsAdapterViewHolder(View itemView, Action<TransactionsAdapterClickEventArgs> clickListener, Action<TransactionsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ProcessedText = MainView.FindViewById<TextView>(Resource.Id.ProcessedText);
                TypeText = MainView.FindViewById<TextView>(Resource.Id.TypeText);
                AmountValue = MainView.FindViewById<TextView>(Resource.Id.AmountValue);
                DateText = MainView.FindViewById<TextView>(Resource.Id.DateText);

                //Event   
                itemView.Click += (sender, e) => clickListener(new TransactionsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new TransactionsAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public TextView ProcessedText { get; private set; }
        public TextView TypeText { get; private set; }
        public TextView AmountValue { get; private set; }
        public TextView DateText { get; private set; }

        #endregion
    }

    public class TransactionsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}