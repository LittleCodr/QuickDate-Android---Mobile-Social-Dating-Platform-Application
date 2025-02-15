using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;

namespace QuickDate.Activities.Premium.Adapters
{

    public class ImageDescViewPager : PagerAdapter
    {

        private Context Context;
        private readonly List<CreditsFeaturesClass> ListDescriptions;
        private readonly LayoutInflater Inflater;

        public ImageDescViewPager(Context context, List<CreditsFeaturesClass> listDescriptions)
        {
            Context = context;
            ListDescriptions = listDescriptions;
            Inflater = LayoutInflater.From(context);
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup view, int position)
        {
            try
            {
                View layout = Inflater.Inflate(Resource.Layout.Style_ImageForPagerVeiw, view, false);
                ImageView iconImage = layout.FindViewById<ImageView>(Resource.Id.Iconimage2);
                TextView description = layout.FindViewById<TextView>(Resource.Id.desc);

                description.Text = ListDescriptions[position].Description;
                iconImage.SetImageResource(ListDescriptions[position].ImageFromResource);

                view.AddView(layout);

                return layout;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }

        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view.Equals(@object);
        }



        public override int Count
        {
            get
            {
                if (ListDescriptions != null)
                {
                    return ListDescriptions.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            try
            {
                View view = (View)@object;
                container.RemoveView(view);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }
    }
}