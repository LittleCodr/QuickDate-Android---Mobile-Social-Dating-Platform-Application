using AmulyaKhare.TextDrawableLib;
using Android.Database;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Google.Places;
using Java.Lang;
using QuickDate.Helpers.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Double = Java.Lang.Double;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace QuickDate.PlacesAsync.Adapters
{
    public class MyPlace : Object
    {
        public string Address { get; set; }
        public AddressComponents AddressComponents { get; set; }
        public IList<string> Attributions { get; set; }
        public string Id { get; set; }
        public LatLng LatLng { get; set; }
        public string Name { get; set; }
        public OpeningHours OpeningHours { get; set; }
        public string PhoneNumber { get; set; }
        public IList<PhotoMetadata> PhotoMetadatas { get; set; }
        public PlusCode PlusCode { get; set; }
        public Integer PriceLevel { get; set; }
        public Double Rating { get; set; }
        public Integer UserRatingsTotal { get; set; }
        public LatLngBounds Viewport { get; set; }
        public Uri WebsiteUri { get; set; }
    }

    public class PlacesAdapter : Object, IListAdapter
    {
        public ObservableCollection<MyPlace> PlacesList = new ObservableCollection<MyPlace>();

        public Object GetItem(int position)
        {
            return PlacesList[position];
        }

        public long GetItemId(int position)
        {
            return position;
        }

        public int GetItemViewType(int position)
        {
            return position;
        }

        public View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                View view = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.Style_PlacesView, parent, false);
                if (view != null)
                {
                    var Image = view.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                    var Title = view.FindViewById<TextView>(Resource.Id.card_name);
                    var Description = view.FindViewById<TextView>(Resource.Id.card_dist);


                    var item = PlacesList[position];
                    if (item != null)
                    {
                        var drawable = TextDrawable.InvokeBuilder().BeginConfig().FontSize(35).EndConfig().BuildRound(item.Name.Substring(0, 1), Color.ParseColor(AppSettings.MainColor));
                        Image.SetImageDrawable(drawable);

                        Title.Text = item.Name;
                        Description.Text = item.Address;
                    }
                }

                return view;

            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return null;
            }
        }

        public void RegisterDataSetObserver(DataSetObserver observer)
        {

        }

        public void UnregisterDataSetObserver(DataSetObserver observer)
        {

        }

        public int Count => PlacesList?.Count ?? 0;
        public bool HasStableIds { get; }
        public bool IsEmpty => PlacesList?.Count == 0;
        public int ViewTypeCount { get; }
        public bool AreAllItemsEnabled()
        {
            return true;
        }

        public bool IsEnabled(int position)
        {
            return true;
        }
    }
}