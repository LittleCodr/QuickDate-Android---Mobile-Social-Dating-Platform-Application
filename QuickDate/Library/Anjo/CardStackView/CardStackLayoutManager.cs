using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.CardStackView.Internal;
using System.Collections.Generic;
using Exception = System.Exception;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class CardStackLayoutManager : RecyclerView.LayoutManager, RecyclerView.SmoothScroller.IScrollVectorProvider
    {
        private readonly Context Context;

        private readonly ICardStackListener Listener;
        private readonly CardStackSetting Setting = new CardStackSetting();
        private readonly CardStackState State = new CardStackState();

        public CardStackLayoutManager(Context context)
        {
            Context = context;
        }

        public CardStackLayoutManager(Context context, ICardStackListener listener)
        {
            try
            {
                Context = context;
                Listener = listener;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override RecyclerView.LayoutParams GenerateDefaultLayoutParams()
        {
            return new RecyclerView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            try
            {
                base.OnLayoutChildren(recycler, state);
                Update(recycler);
                if (state != null && state.DidStructureChange())
                {
                    View topView = GetTopView();
                    if (topView != null)
                    {
                        Listener?.OnCardAppeared(GetTopView(), State.TopPosition);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public override bool CanScrollHorizontally()
        {
            try
            {
                return Setting.SwipeableMethod.CanSwipe() && Setting.CanScrollHorizontal;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.CanScrollHorizontally();
            }
        }

        public override bool CanScrollVertically()
        {
            try
            {
                return Setting.SwipeableMethod.CanSwipe() && Setting.CanScrollVertical;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.CanScrollVertically();
            }
        }

        public override int ScrollHorizontallyBy(int dx, RecyclerView.Recycler recycler, RecyclerView.State s)
        {
            try
            {
                if (State.TopPosition == ItemCount)
                {
                    return 0;
                }

                if (State.StatusCard == CardStackState.Status.Idle)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dx -= dx;
                        Update(recycler);
                        return dx;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.Dragging)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dx -= dx;
                        Update(recycler);
                        return dx;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.RewindAnimating)
                {
                    State.Dx -= dx;
                    Update(recycler);
                    return dx;
                }
                else if (State.StatusCard == CardStackState.Status.AutomaticSwipeAnimating)
                {
                    if (Setting.SwipeableMethod.CanSwipeAutomatically())
                    {
                        State.Dx -= dx;
                        Update(recycler);
                        return dx;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.AutomaticSwipeAnimated)
                {
                }
                else if (State.StatusCard == CardStackState.Status.ManualSwipeAnimating)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dx -= dx;
                        Update(recycler);
                        return dx;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.ManualSwipeAnimated)
                {
                }

                return 0;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.ScrollHorizontallyBy(dx, recycler, s);
            }
        }

        public override int ScrollVerticallyBy(int dy, RecyclerView.Recycler recycler, RecyclerView.State s)
        {
            try
            {
                if (State.TopPosition == ItemCount)
                {
                    return 0;
                }

                if (State.StatusCard == CardStackState.Status.Idle)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dy -= dy;
                        Update(recycler);
                        return dy;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.Dragging)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dy -= dy;
                        Update(recycler);
                        return dy;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.RewindAnimating)
                {
                    State.Dy -= dy;
                    Update(recycler);
                    return dy;
                }
                else if (State.StatusCard == CardStackState.Status.AutomaticSwipeAnimating)
                {
                    if (Setting.SwipeableMethod.CanSwipeAutomatically())
                    {
                        State.Dy -= dy;
                        Update(recycler);
                        return dy;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.AutomaticSwipeAnimated)
                {
                }
                else if (State.StatusCard == CardStackState.Status.ManualSwipeAnimating)
                {
                    if (Setting.SwipeableMethod.CanSwipeManually())
                    {
                        State.Dy -= dy;
                        Update(recycler);
                        return dy;
                    }
                }
                else if (State.StatusCard == CardStackState.Status.ManualSwipeAnimated)
                {
                }

                return 0;

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return base.ScrollVerticallyBy(dy, recycler, s);
            }
        }

        public override void OnScrollStateChanged(int s)
        {
            try
            {
                base.OnScrollStateChanged(s);
                switch (s)
                {
                    // Timing when scrolling stopped
                    case RecyclerView.ScrollStateIdle:
                        if (State.TargetPosition == RecyclerView.NoPosition)
                        {
                            // Processing when Swipe is completed
                            State.Next(CardStackState.Status.Idle);
                            State.TargetPosition = RecyclerView.NoPosition;
                        }
                        else if (State.TopPosition == State.TargetPosition)
                        {
                            // What to do when Rewind is complete
                            State.Next(CardStackState.Status.Idle);
                            State.TargetPosition = RecyclerView.NoPosition;
                        }
                        else
                        {
                            // Processing when swiping two or more cards at the same time
                            if (State.TopPosition < State.TargetPosition)
                            {
                                // When you swipe the first card, SCROLL_STATE_IDLE will be played once.
                                // Make it look like you are swiping continuously by running the next animation at that timing
                                SmoothScrollToNext(State.TargetPosition);
                            }
                            else
                            {
                                // Timing when the card is released from the finger
                                SmoothScrollToPrevious(State.TargetPosition);
                            }
                        }
                        break;
                    // While dragging the card
                    case RecyclerView.ScrollStateDragging:
                        if (Setting.SwipeableMethod.CanSwipeManually())
                        {
                            State.Next(CardStackState.Status.Dragging);
                        }
                        break;
                    // Timing when the card is released from the finger
                    case RecyclerView.ScrollStateSettling:
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public PointF ComputeScrollVectorForPosition(int targetPosition)
        {
            return null;
        }

        public override void ScrollToPosition(int position)
        {
            try
            {
                base.ScrollToPosition(position);
                if (Setting.SwipeableMethod.CanSwipeAutomatically())
                {
                    if (State.CanScrollToPosition(position, ItemCount))
                    {
                        State.TopPosition = position;
                        RequestLayout();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State s, int position)
        {
            try
            {
                base.SmoothScrollToPosition(recyclerView, s, position);

                if (Setting.SwipeableMethod.CanSwipeAutomatically())
                {
                    if (State.CanScrollToPosition(position, ItemCount))
                    {
                        SmoothScrollToPosition(position);
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public CardStackSetting GetCardStackSetting()
        {
            return Setting;
        }


        public CardStackState GetCardStackState()
        {
            return State;
        }

        public ICardStackListener GetCardStackListener()
        {
            return Listener;
        }

        public void UpdateProportion(float x, float y)
        {
            try
            {
                if (GetTopPosition() < ItemCount)
                {
                    View view = FindViewByPosition(GetTopPosition());
                    if (view != null)
                    {
                        float half = Height / 2.0f;
                        State.Proportion = -(y - half - view.Top) / half;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Update(RecyclerView.Recycler recycler)
        {
            try
            {
                State.Width = Width;
                State.Height = Height;

                if (State.IsSwipeCompleted())
                {
                    // ■ Overview
                    // Delete the swiped View from the cache when the swipe is complete
                    // If you do not delete the cache, the swiped card will be displayed at the next update
                    // If you see a swiped card, the data source is correct and only the display is out of date
                    // //
                    // ■ Reproducibility procedure
                    // 1. Comment out `removeAndRecycleView (getTopView (), recycler);`
                    // 2. Set VisibleCount = 1 to paging when the last card is swiped
                    // 3. Display only one card on the screen (let this card be A)
                    // 4. Swipe A
                    // 5. Display only one card on the screen (let this card be B)
                    // 6. B should be displayed after paging is completed, but A is displayed on the screen
                    RemoveAndRecycleView(GetTopView(), recycler);

                    SwipeDirection direction = State.GetDirection();

                    State.Next(State.StatusCard.ToAnimatedStatus());
                    State.TopPosition++;
                    State.Dx = 0;
                    State.Dy = 0;
                    if (State.TopPosition == State.TargetPosition)
                    {
                        State.TargetPosition = RecyclerView.NoPosition;
                    }

                    new Handler(Looper.MainLooper).Post(() =>
                    {
                        try
                        {
                            Listener?.OnCardSwiped(direction);
                            View topView = GetTopView();
                            if (topView != null)
                            {
                                Listener?.OnCardAppeared(GetTopView(), State.TopPosition);
                            }
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e);
                        }
                    });
                }

                DetachAndScrapAttachedViews(recycler);

                int parentTop = PaddingTop;
                int parentLeft = PaddingLeft;
                int parentRight = Width - PaddingLeft;
                int parentBottom = Height - PaddingBottom;
                for (int i = State.TopPosition; i < State.TopPosition + Setting.VisibleCount && i < ItemCount; i++)
                {
                    View child = recycler.GetViewForPosition(i);
                    AddView(child, 0);
                    MeasureChildWithMargins(child, 0, 0);
                    LayoutDecoratedWithMargins(child, parentLeft, parentTop, parentRight, parentBottom);

                    ResetTranslation(child);
                    ResetScale(child);
                    ResetRotation(child);
                    ResetOverlay(child);

                    if (i == State.TopPosition)
                    {
                        UpdateTranslation(child);
                        ResetScale(child);
                        UpdateRotation(child);
                        UpdateOverlay(child);
                    }
                    else
                    {
                        int currentIndex = i - State.TopPosition;
                        UpdateTranslation(child, currentIndex);
                        UpdateScale(child, currentIndex);
                        ResetRotation(child);
                        ResetOverlay(child);
                    }
                }

                if (State.StatusCard.IsDragging)
                {
                    Listener?.OnCardDragging(State.GetDirection(), State.GetRatio());
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdateTranslation(View view)
        {
            try
            {
                view.TranslationX = State.Dx;
                view.TranslationY = State.Dy;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdateTranslation(View view, int index)
        {
            try
            {
                int nextIndex = index - 1;
                int translationPx = DisplayUtil.DpToPx(Context, Setting.TranslationInterval);
                float currentTranslation = index * translationPx;
                float nextTranslation = nextIndex * translationPx;
                float targetTranslation = currentTranslation - (currentTranslation - nextTranslation) * State.GetRatio();
                if (Setting.StackFrom == StackFrom.None)
                {
                    // Do nothing
                }
                else if (Setting.StackFrom == StackFrom.Top)
                {
                    view.TranslationY = -targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.TopAndLeft)
                {
                    view.TranslationY = -targetTranslation;
                    view.TranslationX = -targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.TopAndRight)
                {
                    view.TranslationY = -targetTranslation;
                    view.TranslationX = targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.Bottom)
                {
                    view.TranslationY = targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.BottomAndLeft)
                {
                    view.TranslationY = targetTranslation;
                    view.TranslationX = -targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.BottomAndRight)
                {
                    view.TranslationY = targetTranslation;
                    view.TranslationX = targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.Left)
                {
                    view.TranslationX = -targetTranslation;
                }
                else if (Setting.StackFrom == StackFrom.Right)
                {
                    view.TranslationX = targetTranslation;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResetTranslation(View view)
        {
            try
            {
                view.TranslationX = 0.0f;
                view.TranslationY = 0.0f;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdateScale(View view, int index)
        {
            try
            {
                int nextIndex = index - 1;
                float currentScale = 1.0f - index * (1.0f - Setting.ScaleInterval);
                float nextScale = 1.0f - nextIndex * (1.0f - Setting.ScaleInterval);
                float targetScale = currentScale + (nextScale - currentScale) * State.GetRatio();
                switch (Setting.StackFrom)
                {
                    case StackFrom.None:
                        view.ScaleX = targetScale;
                        view.ScaleY = targetScale;
                        break;
                    case StackFrom.Top:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.TopAndLeft:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.TopAndRight:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.Bottom:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.BottomAndLeft:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.BottomAndRight:
                        view.ScaleX = targetScale;
                        // TODO Should handle ScaleY
                        break;
                    case StackFrom.Left:
                        // TODO Should handle ScaleX
                        view.ScaleY = targetScale;
                        break;
                    case StackFrom.Right:
                        // TODO Should handle ScaleX
                        view.ScaleY = targetScale;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResetScale(View view)
        {
            try
            {
                view.ScaleX = 1.0f;
                view.ScaleY = 1.0f;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdateRotation(View view)
        {
            try
            {
                float degree = State.Dx * Setting.MaxDegree / Width * State.Proportion;
                view.Rotation = degree;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResetRotation(View view)
        {
            try
            {
                view.Rotation = 0.0f;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void UpdateOverlay(View view)
        {
            try
            {
                View leftOverlay = view.FindViewById(Resource.Id.left_overlay);
                if (leftOverlay != null)
                {
                    leftOverlay.Alpha = 0.0f;
                }
                View rightOverlay = view.FindViewById(Resource.Id.right_overlay);
                if (rightOverlay != null)
                {
                    rightOverlay.Alpha = 0.0f;
                }
                View topOverlay = view.FindViewById(Resource.Id.top_overlay);
                if (topOverlay != null)
                {
                    topOverlay.Alpha = 0.0f;
                }
                View bottomOverlay = view.FindViewById(Resource.Id.bottom_overlay);
                if (bottomOverlay != null)
                {
                    bottomOverlay.Alpha = 0.0f;
                }
                SwipeDirection direction = State.GetDirection();
                float alpha = Setting.OverlayInterpolator.GetInterpolation(State.GetRatio());
                if (direction == SwipeDirection.Left)
                {
                    if (leftOverlay != null)
                    {
                        leftOverlay.Alpha = alpha;
                    }
                }
                else if (direction == SwipeDirection.Right)
                {
                    if (rightOverlay != null)
                    {
                        rightOverlay.Alpha = alpha;
                    }
                }
                else if (direction == SwipeDirection.Top)
                {
                    if (topOverlay != null)
                    {
                        topOverlay.Alpha = alpha;
                    }
                }
                else if (direction == SwipeDirection.Bottom)
                {
                    if (bottomOverlay != null)
                    {
                        bottomOverlay.Alpha = alpha;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void ResetOverlay(View view)
        {
            try
            {
                View leftOverlay = view.FindViewById(Resource.Id.left_overlay);
                if (leftOverlay != null)
                {
                    leftOverlay.Alpha = 0.0f;
                }
                View rightOverlay = view.FindViewById(Resource.Id.right_overlay);
                if (rightOverlay != null)
                {
                    rightOverlay.Alpha = 0.0f;
                }
                View topOverlay = view.FindViewById(Resource.Id.top_overlay);
                if (topOverlay != null)
                {
                    topOverlay.Alpha = 0.0f;
                }
                View bottomOverlay = view.FindViewById(Resource.Id.bottom_overlay);
                if (bottomOverlay != null)
                {
                    bottomOverlay.Alpha = 0.0f;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SmoothScrollToPosition(int position)
        {
            try
            {
                if (State.TopPosition < position)
                {
                    SmoothScrollToNext(position);
                }
                else
                {
                    SmoothScrollToPrevious(position);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SmoothScrollToNext(int position)
        {
            try
            {
                State.Proportion = 0.0f;
                State.TargetPosition = position;
                CardStackSmoothScrolled scrolled = new CardStackSmoothScrolled(CardStackSmoothScrolled.ScrollType.AutomaticSwipe, this);
                scrolled.TargetPosition = State.TopPosition;
                StartSmoothScroll(scrolled);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SmoothScrollToPrevious(int position)
        {
            try
            {
                View topView = GetTopView();
                if (topView != null)
                {
                    Listener?.OnCardDisappeared(GetTopView(), State.TopPosition);
                }

                State.Proportion = 0.0f;
                State.TargetPosition = position;
                State.TopPosition--;
                CardStackSmoothScrolled scrolled = new CardStackSmoothScrolled(CardStackSmoothScrolled.ScrollType.AutomaticRewind, this);
                scrolled.TargetPosition = State.TopPosition;
                StartSmoothScroll(scrolled);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public View GetTopView()
        {
            return FindViewByPosition(State.TopPosition);
        }

        public int GetTopPosition()
        {
            return State.TopPosition;
        }

        public void SetTopPosition(int topPosition)
        {
            try
            {
                State.TopPosition = topPosition;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetStackFrom(StackFrom stackFrom)
        {
            try
            {
                Setting.StackFrom = stackFrom;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetVisibleCount(int visibleCount)
        {
            try
            {
                if (visibleCount < 1)
                {
                    throw new IllegalArgumentException("VisibleCount must be greater than 0.");
                }
                Setting.VisibleCount = visibleCount;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetTranslationInterval(float translationInterval)
        {
            try
            {
                if (translationInterval < 0.0f)
                {
                    throw new IllegalArgumentException("TranslationInterval must be greater than or equal 0.0f");
                }
                Setting.TranslationInterval = translationInterval;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetScaleInterval(float scaleInterval)
        {
            try
            {
                if (scaleInterval < 0.0f)
                {
                    throw new IllegalArgumentException("ScaleInterval must be greater than or equal 0.0f.");
                }
                Setting.ScaleInterval = scaleInterval;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetSwipeThreshold(float swipeThreshold)
        {
            try
            {
                if (swipeThreshold < 0.0f || 1.0f < swipeThreshold)
                {
                    throw new IllegalArgumentException("SwipeThreshold must be 0.0f to 1.0f.");
                }
                Setting.SwipeThreshold = swipeThreshold;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetMaxDegree(float maxDegree)
        {
            try
            {
                if (maxDegree < -360.0f || 360.0f < maxDegree)
                {
                    throw new IllegalArgumentException("MaxDegree must be -360.0f to 360.0f");
                }
                Setting.MaxDegree = maxDegree;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetDirections(List<SwipeDirection> directions)
        {
            try
            {
                Setting.Directions = directions;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetCanScrollHorizontal(bool canScrollHorizontal)
        {
            try
            {
                Setting.CanScrollHorizontal = canScrollHorizontal;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetCanScrollVertical(bool canScrollVertical)
        {
            try
            {
                Setting.CanScrollVertical = canScrollVertical;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetSwipeableMethod(SwipeableMethod swipeableMethod)
        {
            try
            {
                Setting.SwipeableMethod = swipeableMethod;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetSwipeAnimationSetting(SwipeAnimationSetting swipeAnimationSetting)
        {
            try
            {
                Setting.SwipeAnimationSetting = swipeAnimationSetting;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetRewindAnimationSetting(RewindAnimationSetting rewindAnimationSetting)
        {
            try
            {
                Setting.RewindAnimationSetting = rewindAnimationSetting;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetOverlayInterpolator(IInterpolator overlayInterpolator)
        {
            try
            {
                Setting.OverlayInterpolator = overlayInterpolator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


    }
}
