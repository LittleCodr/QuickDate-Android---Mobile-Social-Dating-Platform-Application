using Android.Views;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public class CardStackSnapHelper : SnapHelper
    {
        private int VelocityX;
        private int VelocityY;

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
        {
            try
            {
                if (layoutManager is CardStackLayoutManager manager)
                {

                    if (manager.FindViewByPosition(manager.GetTopPosition()) != null)
                    {
                        int x = (int)targetView.TranslationX;
                        int y = (int)targetView.TranslationY;
                        if (x != 0 || y != 0)
                        {
                            CardStackSetting setting = manager.GetCardStackSetting();
                            float horizontal = Math.Abs(x) / (float)targetView.Width;
                            float vertical = Math.Abs(y) / (float)targetView.Height;
                            SwipeDuration duration = SwipeDuration.FromVelocity(VelocityY < VelocityX ? VelocityX : VelocityY);
                            if (duration == SwipeDuration.Fast || setting.SwipeThreshold < horizontal || setting.SwipeThreshold < vertical)
                            {
                                CardStackState state = manager.GetCardStackState();
                                if (setting.Directions.Contains(state.GetDirection()))
                                {
                                    state.TargetPosition = state.TopPosition + 1;

                                    SwipeAnimationSetting swipeAnimationSetting = new SwipeAnimationSetting.Builder()
                                        .SetDirection(setting.SwipeAnimationSetting.GetDirection())
                                        .SetDuration(duration.Duration)
                                        .SetInterpolator(setting.SwipeAnimationSetting.GetInterpolator())
                                        .Build();
                                    manager.SetSwipeAnimationSetting(swipeAnimationSetting);

                                    VelocityX = 0;
                                    VelocityY = 0;

                                    CardStackSmoothScrolled scrolled = new CardStackSmoothScrolled(CardStackSmoothScrolled.ScrollType.ManualSwipe, manager);
                                    scrolled.TargetPosition = manager.GetTopPosition();
                                    manager.StartSmoothScroll(scrolled);
                                }
                                else
                                {
                                    CardStackSmoothScrolled scrolled = new CardStackSmoothScrolled(CardStackSmoothScrolled.ScrollType.ManualCancel, manager);
                                    scrolled.TargetPosition = manager.GetTopPosition();
                                    manager.StartSmoothScroll(scrolled);
                                }
                            }
                            else
                            {
                                CardStackSmoothScrolled scrolled = new CardStackSmoothScrolled(CardStackSmoothScrolled.ScrollType.ManualCancel, manager);
                                scrolled.TargetPosition = manager.GetTopPosition();
                                manager.StartSmoothScroll(scrolled);
                            }
                        }
                    }
                }
                return new int[2];
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new int[2];
            }
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            try
            {
                if (layoutManager is CardStackLayoutManager manager)
                {

                    View view = manager.FindViewByPosition(manager.GetTopPosition());
                    if (view != null)
                    {
                        int x = (int)view.TranslationX;
                        int y = (int)view.TranslationY;
                        if (x == 0 && y == 0)
                        {
                            return null;
                        }
                        return view;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public override int FindTargetSnapPosition(RecyclerView.LayoutManager layoutManager, int velocityX, int velocityY)
        {
            VelocityX = Math.Abs(velocityX);
            VelocityY = Math.Abs(velocityY);
            if (layoutManager is CardStackLayoutManager manager)
            {
                return manager.GetTopPosition();
            }
            return RecyclerView.NoPosition;
        }
    }
}