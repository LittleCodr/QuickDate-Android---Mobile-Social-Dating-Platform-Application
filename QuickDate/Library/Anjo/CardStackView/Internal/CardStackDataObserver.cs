using AndroidX.RecyclerView.Widget;
using Java.Lang;
using QuickDate.Helpers.Utils;
using Exception = Java.Lang.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public class CardStackDataObserver : RecyclerView.AdapterDataObserver
    {
        private readonly RecyclerView RecyclerView;

        public CardStackDataObserver(RecyclerView recyclerView)
        {
            RecyclerView = recyclerView;
        }

        public override void OnChanged()
        {
            try
            {
                base.OnChanged();
                CardStackLayoutManager manager = GetCardStackLayoutManager();
                manager.SetTopPosition(0);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnItemRangeChanged(int positionStart, int itemCount)
        {
            try
            {
                base.OnItemRangeChanged(positionStart, itemCount);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnItemRangeChanged(int positionStart, int itemCount, Object payload)
        {
            try
            {
                base.OnItemRangeChanged(positionStart, itemCount, payload);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnItemRangeInserted(int positionStart, int itemCount)
        {
            try
            {
                base.OnItemRangeInserted(positionStart, itemCount);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnItemRangeRemoved(int positionStart, int itemCount)
        {
            try
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                // TopPosition may need to be adjusted if the element is deleted
                // Specifically, adjustment is required when all elements are deleted and when elements before TopPosition are deleted.
                CardStackLayoutManager manager = GetCardStackLayoutManager();
                int topPosition = manager.GetTopPosition();
                if (manager.ItemCount == 0)
                {
                    // When all elements are deleted
                    manager.SetTopPosition(0);
                }
                else if (positionStart < topPosition)
                {
                    // When elements before TopPosition are deleted
                    int diff = topPosition - positionStart;
                    manager.SetTopPosition(Math.Min(topPosition - diff, manager.ItemCount - 1));
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount)
        {
            try
            {
                base.OnItemRangeMoved(fromPosition, toPosition, itemCount);
                CardStackLayoutManager manager = GetCardStackLayoutManager();
                manager.RemoveAllViews();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CardStackLayoutManager GetCardStackLayoutManager()
        {
            try
            {
                RecyclerView.LayoutManager manager = RecyclerView.GetLayoutManager();
                if (manager is CardStackLayoutManager layoutManager)
                {
                    return layoutManager;
                }
                throw new IllegalStateException("CardStackView must be set CardStackLayoutManager.");
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

    }
}