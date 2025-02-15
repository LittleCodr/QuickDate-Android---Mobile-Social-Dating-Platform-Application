using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace QuickDate.Library.Anjo.IntegrationRecyclerView
{
    public class RecyclerToListViewScrollListener : RecyclerView.OnScrollListener
    {
        public static readonly int UnknownScrollState = int.MinValue;
        private readonly AbsListView.IOnScrollListener ScrollListener;
        private int LastFirstVisible = -1;
        private int LastVisibleCount = -1;
        private int LastItemCount = -1;

        public RecyclerToListViewScrollListener(AbsListView.IOnScrollListener scrollListener)
        {
            ScrollListener = scrollListener;
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);

            ScrollState listViewState = newState switch
            {
                RecyclerView.ScrollStateDragging => ScrollState.TouchScroll,
                RecyclerView.ScrollStateIdle => ScrollState.Idle,
                RecyclerView.ScrollStateSettling => ScrollState.Fling,
                _ => ScrollState.TouchScroll
            };

            ScrollListener.OnScrollStateChanged(null /*view*/, listViewState);

        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            LinearLayoutManager layoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager();

            int firstVisible = layoutManager.FindFirstVisibleItemPosition();
            int visibleCount = Math.Abs(firstVisible - layoutManager.FindLastVisibleItemPosition());
            int itemCount = recyclerView.GetAdapter().ItemCount;

            if (firstVisible != LastFirstVisible
                || visibleCount != LastVisibleCount
                || itemCount != LastItemCount)
            {
                ScrollListener.OnScroll(null, firstVisible, visibleCount, itemCount);
                LastFirstVisible = firstVisible;
                LastVisibleCount = visibleCount;
                LastItemCount = itemCount;
            }
        }

    }

}