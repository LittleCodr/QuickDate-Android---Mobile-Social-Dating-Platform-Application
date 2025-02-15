using Android.Graphics;

using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace QuickDate.Helpers.Utils
{
    public class GridSpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int SpanCount;
        private readonly int Spacing;
        private readonly bool IncludeEdge;

        public GridSpacingItemDecoration(int spanCount, int spacing, bool includeEdge)
        {
            SpanCount = spanCount;
            Spacing = spacing;
            IncludeEdge = includeEdge;
        }


        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            int position = parent.GetChildAdapterPosition(view);
            int column = position % SpanCount;

            if (IncludeEdge)
            {
                outRect.Left = Spacing - column * Spacing / SpanCount;
                outRect.Right = (column + 1) * Spacing / SpanCount;

                if (position < SpanCount)
                {
                    outRect.Top = Spacing;
                }
                outRect.Bottom = Spacing;
            }
            else
            {
                outRect.Left = column * Spacing / SpanCount;
                outRect.Right = Spacing - (column + 1) * Spacing / SpanCount;

                if (position >= SpanCount)
                {
                    outRect.Top = Spacing;
                }
            }
        }
    }
}