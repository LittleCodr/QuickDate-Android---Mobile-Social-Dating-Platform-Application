using Android.Content;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public static class DisplayUtil
    {
        public static int DpToPx(Context context, float dp)
        {
            float density = context.Resources.DisplayMetrics.Density;
            return (int)(dp * density + 0.5f);
        }
    }
}