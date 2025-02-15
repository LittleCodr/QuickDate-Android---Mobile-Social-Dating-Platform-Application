using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using JetBrains.Annotations;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.CardStackView.Internal;
using System;
using Exception = System.Exception;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class CardStackView : RecyclerView
    {
        private CardStackDataObserver Observer;

        protected CardStackView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CardStackView([NotNull] Context context) : base(context)
        {
            Initialize();
        }

        public CardStackView([NotNull] Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public CardStackView([NotNull] Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize();
        }

        public override void SetLayoutManager(LayoutManager layout)
        {
            try
            {
                if (layout is CardStackLayoutManager manager)
                {
                    base.SetLayoutManager(layout);
                }
                else
                {
                    throw new IllegalArgumentException("CardStackView must be set CardStackLayoutManager.");
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void SetAdapter(Adapter adapter)
        {
            try
            {
                if (GetLayoutManager() == null)
                {
                    SetLayoutManager(new CardStackLayoutManager(Context));
                }
                // Imitate RecyclerView's implementation
                // http://tools.oesf.biz/android-9.0.0_r1.0/xref/frameworks/base/core/java/com/android/internal/widget/RecyclerView.java#1005
                if (GetAdapter() != null)
                {
                    GetAdapter().UnregisterAdapterDataObserver(Observer);
                    GetAdapter().OnDetachedFromRecyclerView(this);
                }
                adapter.RegisterAdapterDataObserver(Observer);

                base.SetAdapter(adapter);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                if (ev.Action == MotionEventActions.Down)
                {
                    CardStackLayoutManager manager = (CardStackLayoutManager)GetLayoutManager();
                    manager?.UpdateProportion(ev.GetX(), ev.GetY());
                }
                return base.OnInterceptTouchEvent(ev);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.OnInterceptTouchEvent(ev);
            }

        }

        public void Swipe()
        {
            try
            {
                if (GetLayoutManager() is CardStackLayoutManager manager)
                {
                    SmoothScrollToPosition(manager.GetTopPosition() + 1);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Rewind()
        {
            try
            {
                if (GetLayoutManager() is CardStackLayoutManager manager)
                {
                    SmoothScrollToPosition(manager.GetTopPosition() - 1);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Initialize()
        {
            try
            {
                Observer = new CardStackDataObserver(this);
                new CardStackSnapHelper().AttachToRecyclerView(this);
                OverScrollMode = OverScrollMode.Never;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}