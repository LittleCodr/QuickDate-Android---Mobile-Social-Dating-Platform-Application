using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Helpers.ShimmerUtils
{
    public class ShimmerTemplateLayout : LinearLayout
    {
        private int TemplateType;

        protected ShimmerTemplateLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ShimmerTemplateLayout(Context context) : base(context)
        {
        }

        public ShimmerTemplateLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ShimmerTemplateLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ShimmerTemplateLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }
        private void InitView(Context context, IAttributeSet attributeSet)
        {
            TypedArray attributes = null;
            try
            {
                if (attributeSet != null)
                {
                    attributes = context?.Theme?.ObtainStyledAttributes(attributeSet, Resource.Styleable.TemplateView, 0, 0);
                    TemplateType = attributes.GetResourceId(Resource.Styleable.TemplateShimmer_template_res, Resource.Layout.Style_RowShimmerView);
                    LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                    inflater.Inflate(TemplateType, this);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
            finally
            {
                attributes?.Recycle();
            }
        }

        public void SetTemplateStyle(ShimmerTemplateStyle style)
        {
            try
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                switch (style)
                {
                    case ShimmerTemplateStyle.UserProfileTemplate:
                        TemplateType = Resource.Layout.Style_RowShimmerView;
                        break;
                }

                inflater.Inflate(TemplateType, this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ShimmerTemplateStyle GetTemplateTypeName()
        {
            switch (TemplateType)
            {
                case Resource.Layout.Style_RowShimmerView:
                    return ShimmerTemplateStyle.UserProfileTemplate;

                default:
                    return ShimmerTemplateStyle.UserProfileTemplate;
            }
        }


    }
}