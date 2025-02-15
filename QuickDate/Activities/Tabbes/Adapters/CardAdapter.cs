using Android.App;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using System;
using System.Collections.ObjectModel;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class CardAdapter : RecyclerView.Adapter
    {

        private readonly Activity ActivityContext;
        private readonly RequestBuilder FullGlideRequestBuilder;

        public ObservableCollection<UserInfoObject> UsersDateList = new ObservableCollection<UserInfoObject>();

        public CardAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                var glideRequestOptions = new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).SetPriority(Priority.High);
                FullGlideRequestBuilder = Glide.With(context?.BaseContext).AsBitmap().Apply(glideRequestOptions).Transition(new BitmapTransitionOptions().CrossFade(100));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override int ItemCount => UsersDateList?.Count ?? 0;

        public event EventHandler<CardAdapterClickEventArgs> ItemClick;
        public event EventHandler<CardAdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_DatingCardview
                var itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_DatingCardview, parent, false);
                var vh = new CardAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is CardAdapterViewHolder holder)
                {
                    var item = UsersDateList[position];
                    if (item != null)
                    {
                        FullGlideRequestBuilder.Load(item.Avater).Into(holder.Image);

                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.Status, IonIconsFonts.Recording);
                        holder.Status.SetTextColor(Color.Green);

                        holder.Status.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online) ? ViewStates.Visible : ViewStates.Gone;

                        string name = !string.IsNullOrEmpty(item.FirstName) ? item.FirstName : item.Username;
                        holder.Name.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(name), 20) + ", " + QuickDateTools.GetAgeUser(item);
                        holder.Name.SetCompoundDrawablesWithIntrinsicBounds(0, 0, item.Verified == "1" ? Resource.Drawable.icon_checkmark_vector : 0, 0);

                        //holder.Name.Text = QuickDateTools.GetNameFinal(item); 
                        if (!string.IsNullOrEmpty(item.CountryTxt))
                        {
                            var distanceBetween = QuickDateTools.DistanceBetween(new LatLng(Convert.ToDouble(UserDetails.Lat), Convert.ToDouble(UserDetails.Lng)), new LatLng(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Lng)));
                            var textKm = QuickDateTools.ToSi(distanceBetween);
                            if (string.IsNullOrEmpty(textKm))
                            {
                                holder.City.Text = Methods.FunString.DecodeString(item.CountryTxt) + "(" + textKm + ")";
                            }
                            else
                            {
                                holder.City.Text = Methods.FunString.DecodeString(item.CountryTxt);
                            }
                        }
                        else
                            holder.City.Text = ActivityContext.GetText(Resource.String.Lbl_Unknown);
                    }
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public UserInfoObject GetItem(int position)
        {
            return UsersDateList[position];
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

        private void Click(CardAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(CardAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        //public IList GetPreloadItems(int p0)
        //{
        //    try
        //    {
        //        var d = new List<string>();
        //        var item = UsersDateList[p0];

        //        if (item == null)
        //            return Collections.SingletonList(p0);

        //        if (item.Avater != "")
        //        {
        //            d.Add(item.Avater);
        //            return d;
        //        }

        //        return d;
        //    }
        //    catch (Exception e)
        //    {
        //        Methods.DisplayReportResultTrack(e);
        //        return Collections.SingletonList(p0);
        //    }
        //}

        //public RequestBuilder GetPreloadRequestBuilder(Object p0)
        //{
        //    return Glide.With(ActivityContext?.BaseContext).Load(p0.ToString()).Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        //}
    }

    public class CardAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        private View MainView { get; }

        public TextView Name { get; private set; }
        public TextView City { get; private set; }
        public TextView Status { get; private set; }
        public ImageView Image { get; private set; }

        #endregion
        public CardAdapterViewHolder(View itemView, Action<CardAdapterClickEventArgs> clickListener, Action<CardAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Name = (TextView)itemView.FindViewById(Resource.Id.item_tourist_spot_card_name);
                City = (TextView)itemView.FindViewById(Resource.Id.item_tourist_spot_card_city);
                Image = (ImageView)itemView.FindViewById(Resource.Id.item_tourist_spot_card_image);
                Status = (TextView)itemView.FindViewById(Resource.Id.status);

                //Event
                itemView.Click += (sender, e) => clickListener(new CardAdapterClickEventArgs { View = itemView, Image = Image, Position = BindingAdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CardAdapterClickEventArgs { View = itemView, Image = Image, Position = BindingAdapterPosition });
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }

    public class CardAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }
}