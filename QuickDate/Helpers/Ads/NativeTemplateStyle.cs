using Android.Graphics;
using Android.Graphics.Drawables;

namespace QuickDate.Helpers.Ads
{
    /// <summary>
    /// A class containing the optional styling options for the Native Template.
    /// </summary>
    public class NativeTemplateStyle
    {

        // Call to action typeface.
        private Typeface CallToActionTextTypeface;

        // Size of call to action text.
        private float CallToActionTextSize;

        // Call to action typeface color in the form 0xAARRGGBB.
        private Color CallToActionTypefaceColor;

        // Call to action background color.
        private ColorDrawable CallToActionBackgroundColor;

        // All templates have a primary text area which is populated by the native ad's headline.

        // Primary text typeface.
        private Typeface PrimaryTextTypeface;

        // Size of primary text.
        private float PrimaryTextSize;

        // Primary text typeface color in the form 0xAARRGGBB.
        private Color PrimaryTextTypefaceColor;

        // Primary text background color.
        private ColorDrawable PrimaryTextBackgroundColor;

        // The typeface, typeface color, and background color for the second row of text in the template.
        // All templates have a secondary text area which is populated either by the body of the ad or
        // by the rating of the app.

        // Secondary text typeface.
        private Typeface SecondaryTextTypeface;

        // Size of secondary text.
        private float SecondaryTextSize;

        // Secondary text typeface color in the form 0xAARRGGBB.
        private Color SecondaryTextTypefaceColor;

        // Secondary text background color.
        private ColorDrawable SecondaryTextBackgroundColor;

        // The typeface, typeface color, and background color for the third row of text in the template.
        // The third row is used to display store name or the default tertiary text.

        // Tertiary text typeface.
        private Typeface TertiaryTextTypeface;

        // Size of tertiary text.
        private float TertiaryTextSize;

        // Tertiary text typeface color in the form 0xAARRGGBB.
        private Color TertiaryTextTypefaceColor;

        // Tertiary text background color.
        private ColorDrawable TertiaryTextBackgroundColor;

        // The background color for the bulk of the ad.
        private ColorDrawable MainBackgroundColor;

        public Typeface GetCallToActionTextTypeface()
        {
            return CallToActionTextTypeface;
        }

        public float GetCallToActionTextSize()
        {
            return CallToActionTextSize;
        }

        public Color GetCallToActionTypefaceColor()
        {
            return CallToActionTypefaceColor;
        }

        public ColorDrawable GetCallToActionBackgroundColor()
        {
            return CallToActionBackgroundColor;
        }

        public Typeface GetPrimaryTextTypeface()
        {
            return PrimaryTextTypeface;
        }

        public float GetPrimaryTextSize()
        {
            return PrimaryTextSize;
        }

        public Color GetPrimaryTextTypefaceColor()
        {
            return PrimaryTextTypefaceColor;
        }

        public ColorDrawable GetPrimaryTextBackgroundColor()
        {
            return PrimaryTextBackgroundColor;
        }

        public Typeface GetSecondaryTextTypeface()
        {
            return SecondaryTextTypeface;
        }

        public float GetSecondaryTextSize()
        {
            return SecondaryTextSize;
        }

        public Color GetSecondaryTextTypefaceColor()
        {
            return SecondaryTextTypefaceColor;
        }

        public ColorDrawable GetSecondaryTextBackgroundColor()
        {
            return SecondaryTextBackgroundColor;
        }

        public Typeface GetTertiaryTextTypeface()
        {
            return TertiaryTextTypeface;
        }

        public float GetTertiaryTextSize()
        {
            return TertiaryTextSize;
        }

        public Color GetTertiaryTextTypefaceColor()
        {
            return TertiaryTextTypefaceColor;
        }

        public ColorDrawable GetTertiaryTextBackgroundColor()
        {
            return TertiaryTextBackgroundColor;
        }

        public ColorDrawable GetMainBackgroundColor()
        {
            return MainBackgroundColor;
        }

        /** A class that provides helper methods to build a style object. * */
        public class Builder
        {

            private readonly NativeTemplateStyle Styles;

            public Builder()
            {
                Styles = new NativeTemplateStyle();
            }

            public Builder WithCallToActionTextTypeface(Typeface callToActionTextTypeface)
            {
                Styles.CallToActionTextTypeface = callToActionTextTypeface;
                return this;
            }

            public Builder WithCallToActionTextSize(float callToActionTextSize)
            {
                Styles.CallToActionTextSize = callToActionTextSize;
                return this;
            }

            public Builder WithCallToActionTypefaceColor(Color callToActionTypefaceColor)
            {
                Styles.CallToActionTypefaceColor = callToActionTypefaceColor;
                return this;
            }

            public Builder WithCallToActionBackgroundColor(ColorDrawable callToActionBackgroundColor)
            {
                Styles.CallToActionBackgroundColor = callToActionBackgroundColor;
                return this;
            }

            public Builder WithPrimaryTextTypeface(Typeface primaryTextTypeface)
            {
                Styles.PrimaryTextTypeface = primaryTextTypeface;
                return this;
            }

            public Builder WithPrimaryTextSize(float primaryTextSize)
            {
                Styles.PrimaryTextSize = primaryTextSize;
                return this;
            }

            public Builder WithPrimaryTextTypefaceColor(Color primaryTextTypefaceColor)
            {
                Styles.PrimaryTextTypefaceColor = primaryTextTypefaceColor;
                return this;
            }

            public Builder WithPrimaryTextBackgroundColor(ColorDrawable primaryTextBackgroundColor)
            {
                Styles.PrimaryTextBackgroundColor = primaryTextBackgroundColor;
                return this;
            }

            public Builder WithSecondaryTextTypeface(Typeface secondaryTextTypeface)
            {
                Styles.SecondaryTextTypeface = secondaryTextTypeface;
                return this;
            }

            public Builder WithSecondaryTextSize(float secondaryTextSize)
            {
                Styles.SecondaryTextSize = secondaryTextSize;
                return this;
            }

            public Builder WithSecondaryTextTypefaceColor(Color secondaryTextTypefaceColor)
            {
                Styles.SecondaryTextTypefaceColor = secondaryTextTypefaceColor;
                return this;
            }

            public Builder WithSecondaryTextBackgroundColor(ColorDrawable secondaryTextBackgroundColor)
            {
                Styles.SecondaryTextBackgroundColor = secondaryTextBackgroundColor;
                return this;
            }

            public Builder WithTertiaryTextTypeface(Typeface tertiaryTextTypeface)
            {
                Styles.TertiaryTextTypeface = tertiaryTextTypeface;
                return this;
            }

            public Builder WithTertiaryTextSize(float tertiaryTextSize)
            {
                Styles.TertiaryTextSize = tertiaryTextSize;
                return this;
            }

            public Builder WithTertiaryTextTypefaceColor(Color tertiaryTextTypefaceColor)
            {
                Styles.TertiaryTextTypefaceColor = tertiaryTextTypefaceColor;
                return this;
            }

            public Builder WithTertiaryTextBackgroundColor(ColorDrawable tertiaryTextBackgroundColor)
            {
                Styles.TertiaryTextBackgroundColor = tertiaryTextBackgroundColor;
                return this;
            }

            public Builder WithMainBackgroundColor(ColorDrawable mainBackgroundColor)
            {
                Styles.MainBackgroundColor = mainBackgroundColor;
                return this;
            }

            public NativeTemplateStyle Build()
            {
                return Styles;
            }
        }
    }
}