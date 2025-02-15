using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using Com.Google.Android.Gms.Ads.Nativead;
using QuickDate.Helpers.Utils;

namespace QuickDate.Helpers.Ads
{
    public class TemplateView : FrameLayout
    {
        private int TemplateType;
        private NativeTemplateStyle Styles;
        private NativeAd NativeAd;
        private NativeAdView NativeAdView;

        private TextView PrimaryView;
        private TextView SecondaryView;

        private RatingBar RatingBar;
        private TextView TertiaryView;
        private ImageView IconView;

        private MediaView MediaView;

        private AppCompatButton CallToActionView;

        private new ConstraintLayout Background;

        public static readonly string MediumTemplate = "medium_template";
        public static readonly string NativeContentAd = "NativeContentAd";
        public static readonly string FullTemplate = "full_template";
        public static readonly string SmallTemplate = "small_template";


        protected TemplateView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TemplateView(Context context) : base(context)
        {

        }

        public TemplateView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitView(context, attrs);
        }

        public TemplateView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitView(context, attrs);
        }

        public TemplateView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            InitView(context, attrs);
        }

        public void SetStyles(NativeTemplateStyle styles)
        {
            Styles = styles;
            ApplyStyles();
        }

        public NativeAdView GetNativeAdView()
        {
            return NativeAdView;
        }

        private void ApplyStyles()
        {
            try
            {
                Drawable mainBackground = Styles.GetMainBackgroundColor();
                if (mainBackground != null)
                {
                    Background.Background = mainBackground;
                    if (PrimaryView != null)
                    {
                        PrimaryView.Background = mainBackground;
                    }

                    if (SecondaryView != null)
                    {
                        SecondaryView.Background = mainBackground;
                    }

                    if (TertiaryView != null)
                    {
                        TertiaryView.Background = mainBackground;
                    }
                }

                Typeface primary = Styles.GetPrimaryTextTypeface();
                if (primary != null)
                {
                    PrimaryView?.SetTypeface(primary, TypefaceStyle.Normal);
                }

                Typeface secondary = Styles.GetSecondaryTextTypeface();
                if (secondary != null)
                {
                    SecondaryView?.SetTypeface(secondary, TypefaceStyle.Normal);
                }

                Typeface tertiary = Styles.GetTertiaryTextTypeface();
                if (tertiary != null)
                {
                    TertiaryView?.SetTypeface(tertiary, TypefaceStyle.Normal);
                }

                Typeface ctaTypeface = Styles.GetCallToActionTextTypeface();
                if (ctaTypeface != null)
                {
                    CallToActionView?.SetTypeface(ctaTypeface, TypefaceStyle.Normal);
                }


                Color primaryTypefaceColor = Styles.GetPrimaryTextTypefaceColor();
                if (primaryTypefaceColor > 0)
                {
                    PrimaryView?.SetTextColor(primaryTypefaceColor);
                }

                Color secondaryTypefaceColor = Styles.GetSecondaryTextTypefaceColor();
                if (secondaryTypefaceColor > 0)
                {
                    SecondaryView?.SetTextColor(secondaryTypefaceColor);
                }

                Color tertiaryTypefaceColor = Styles.GetTertiaryTextTypefaceColor();
                if (tertiaryTypefaceColor > 0)
                {
                    TertiaryView?.SetTextColor(tertiaryTypefaceColor);
                }

                var ctaTypefaceColor = Styles.GetCallToActionTypefaceColor();
                if (ctaTypefaceColor > 0)
                {
                    CallToActionView?.SetTextColor(ctaTypefaceColor);
                }

                float ctaTextSize = Styles.GetCallToActionTextSize();
                if (ctaTextSize > 0)
                {
                    CallToActionView?.SetTextSize(ComplexUnitType.Sp, ctaTextSize);
                }

                float primaryTextSize = Styles.GetPrimaryTextSize();
                if (primaryTextSize > 0)
                {
                    PrimaryView?.SetTextSize(ComplexUnitType.Sp, primaryTextSize);
                }

                float secondaryTextSize = Styles.GetSecondaryTextSize();
                if (secondaryTextSize > 0)
                {
                    SecondaryView?.SetTextSize(ComplexUnitType.Sp, secondaryTextSize);
                }

                float tertiaryTextSize = Styles.GetTertiaryTextSize();
                if (tertiaryTextSize > 0)
                {
                    TertiaryView?.SetTextSize(ComplexUnitType.Sp, tertiaryTextSize);
                }

                Drawable ctaBackground = Styles.GetCallToActionBackgroundColor();
                if (ctaBackground != null && CallToActionView != null)
                {
                    CallToActionView.Background = ctaBackground;
                }

                Drawable primaryBackground = Styles.GetPrimaryTextBackgroundColor();
                if (primaryBackground != null && PrimaryView != null)
                {
                    PrimaryView.Background = primaryBackground;
                }

                Drawable secondaryBackground = Styles.GetSecondaryTextBackgroundColor();
                if (secondaryBackground != null && SecondaryView != null)
                {
                    SecondaryView.Background = secondaryBackground;
                }

                Drawable tertiaryBackground = Styles.GetTertiaryTextBackgroundColor();
                if (tertiaryBackground != null && TertiaryView != null)
                {
                    TertiaryView.Background = tertiaryBackground;
                }

                Invalidate();
                RequestLayout();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private bool AdHasOnlyStore(NativeAd nativeAd)
        {
            string store = nativeAd.Store;
            string advertiser = nativeAd.Advertiser;
            return !TextUtils.IsEmpty(store) && TextUtils.IsEmpty(advertiser);
        }

        public void SetNativeAd(NativeAd nativeAd)
        {
            try
            {
                NativeAd = nativeAd;

                string store = nativeAd.Store;
                string advertiser = nativeAd.Advertiser;
                string headline = nativeAd.Headline;
                string body = nativeAd.Body;
                string cta = nativeAd.CallToAction;
                var starRating = nativeAd.StarRating;
                var icon = nativeAd.Icon;

                string secondaryText;

                NativeAdView.CallToActionView = CallToActionView;
                NativeAdView.HeadlineView = PrimaryView;
                NativeAdView.MediaView = MediaView;
                SecondaryView.Visibility = ViewStates.Visible;
                if (AdHasOnlyStore(nativeAd))
                {
                    NativeAdView.StoreView = SecondaryView;
                    secondaryText = store;
                }
                else if (!TextUtils.IsEmpty(advertiser))
                {
                    NativeAdView.AdvertiserView = SecondaryView;
                    secondaryText = advertiser;
                }
                else
                {
                    secondaryText = "";
                }

                PrimaryView.Text = headline;

                if (CallToActionView != null)
                    CallToActionView.Text = cta;

                //  Set the secondary view to be the star rating if available.
                if (starRating != null && starRating.FloatValue() > 0)
                {
                    SecondaryView.Visibility = ViewStates.Gone;
                    RatingBar.Visibility = ViewStates.Visible;
                    RatingBar.Rating = starRating.FloatValue();
                    NativeAdView.StarRatingView = RatingBar;
                }
                else
                {
                    RatingBar.Visibility = ViewStates.Gone;

                    if (string.IsNullOrEmpty(secondaryText))
                    {
                        SecondaryView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        SecondaryView.Visibility = ViewStates.Visible;
                        SecondaryView.Text = secondaryText;
                    }
                }

                if (icon != null)
                {
                    IconView.Visibility = ViewStates.Visible;
                    IconView.SetImageDrawable(icon.Drawable);
                }
                else
                {
                    IconView.Visibility = ViewStates.Gone;
                }

                if (TertiaryView != null && !string.IsNullOrEmpty(body))
                {
                    TertiaryView.Text = body;
                    NativeAdView.BodyView = TertiaryView;
                }
                else if (TertiaryView != null)
                {
                    TertiaryView.Visibility = ViewStates.Gone;
                }

                NativeAdView.SetNativeAd(nativeAd);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        /// <summary>
        /// To prevent memory leaks, make sure to destroy your ad when you don't need it anymore.
        /// This method does not destroy the template view.
        /// </summary>
        public void DestroyNativeAd()
        {
            NativeAd.Destroy();
        }

        public string GetTemplateTypeName()
        {
            return TemplateType switch
            {
                Resource.Layout.gnt_medium_template_view => MediumTemplate,
                Resource.Layout.gnt_NativeContentAd_view => NativeContentAd,
                Resource.Layout.gnt_full_template_view => FullTemplate,
                Resource.Layout.gnt_small_template_view => SmallTemplate,
                _ => ""
            };
        }

        private void InitView(Context context, IAttributeSet attributeSet)
        {
            try
            {
                TypedArray attributes = context.Theme.ObtainStyledAttributes(attributeSet, Resource.Styleable.TemplateView, 0, 0);

                try
                {
                    TemplateType = attributes.GetResourceId(Resource.Styleable.TemplateView_gnt_template_type, Resource.Layout.gnt_medium_template_view);
                }
                finally
                {
                    attributes.Recycle();
                }

                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                inflater.Inflate(TemplateType, this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnFinishInflate()
        {
            try
            {
                base.OnFinishInflate();

                NativeAdView = (NativeAdView)FindViewById(Resource.Id.nativeAdView);

                switch (AppSettings.ShowAdMobNative)
                {
                    case false:
                        {
                            if (NativeAdView != null) NativeAdView.Visibility = ViewStates.Gone;
                            break;
                        }
                    default:
                        PrimaryView = (TextView)FindViewById(Resource.Id.primary);
                        SecondaryView = (TextView)FindViewById(Resource.Id.secondary);
                        TertiaryView = (TextView)FindViewById(Resource.Id.body);

                        RatingBar = (RatingBar)FindViewById(Resource.Id.rating_bar);
                        if (RatingBar != null) RatingBar.Enabled = false;

                        CallToActionView = (AppCompatButton)FindViewById(Resource.Id.cta);
                        IconView = (ImageView)FindViewById(Resource.Id.icon);
                        MediaView = (MediaView)FindViewById(Resource.Id.media);
                        Background = (ConstraintLayout)FindViewById(Resource.Id.background);
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void NativeContentAdView(NativeAd nativeAd)
        {
            try
            {
                var adView = (NativeAdView)FindViewById(Resource.Id.nativeAdView);

                // Set the media view.
                NativeAdView.MediaView = ((MediaView)adView.FindViewById(Resource.Id.media));

                // Set other ad assets.
                NativeAdView.HeadlineView = adView.FindViewById(Resource.Id.headline);
                NativeAdView.BodyView = adView.FindViewById(Resource.Id.body);
                NativeAdView.CallToActionView = adView.FindViewById(Resource.Id.call_to_action);
                NativeAdView.IconView = adView.FindViewById(Resource.Id.icon);
                NativeAdView.PriceView = adView.FindViewById(Resource.Id.price);
                NativeAdView.StarRatingView = adView.FindViewById(Resource.Id.stars);
                NativeAdView.StoreView = adView.FindViewById(Resource.Id.store);
                NativeAdView.AdvertiserView = adView.FindViewById(Resource.Id.advertiser);

                // The headline and mediaContent are guaranteed to be in every NativeAd.
                ((TextView)NativeAdView.HeadlineView).Text = nativeAd.Headline;
                //NativeAdView.MediaView.MediaContent = (nativeAd.MediaContent);

                // These assets aren't guaranteed to be in every NativeAd, so it's important to
                // check before trying to display them.
                if (string.IsNullOrEmpty(nativeAd.Body))
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.BodyView).Text = nativeAd.Body;
                }

                if (string.IsNullOrEmpty(nativeAd.CallToAction))
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Visible;
                    ((AppCompatButton)NativeAdView.CallToActionView).Text = nativeAd.CallToAction;
                }

                if (nativeAd.Icon == null)
                {
                    NativeAdView.IconView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((ImageView)NativeAdView.IconView).SetImageDrawable(nativeAd.Icon.Drawable);
                    NativeAdView.IconView.Visibility = ViewStates.Visible;
                }

                if (nativeAd.Price == null)
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Invisible;
                }
                else
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Gone;
                    ((TextView)NativeAdView.PriceView).Text = (nativeAd.Price);
                }

                if (nativeAd.Store == null)
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Invisible;
                }
                else
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.StoreView).Text = (nativeAd.Store);
                }

                if (nativeAd.StarRating == null)
                {
                    NativeAdView.StarRatingView.Visibility = ViewStates.Invisible;
                }
                else
                {
                    ((RatingBar)NativeAdView.StarRatingView).Rating = (nativeAd.StarRating.FloatValue());
                    NativeAdView.StarRatingView.Visibility = ViewStates.Visible;
                }

                if (string.IsNullOrEmpty(nativeAd.Advertiser))
                {
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((TextView)NativeAdView.AdvertiserView).Text = nativeAd.Advertiser;
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Visible;
                }

                // This method tells the Google Mobile Ads SDK that you have finished populating your
                // native ad view with this native ad.
                NativeAdView.SetNativeAd(nativeAd);

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}