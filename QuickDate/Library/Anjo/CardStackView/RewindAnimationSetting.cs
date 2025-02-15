using Android.Views.Animations;
using QuickDate.Helpers.Utils;
using QuickDate.Library.Anjo.CardStackView.Internal;
using System;
using DecelerateInterpolator = Android.Views.Animations.DecelerateInterpolator;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class RewindAnimationSetting : IAnimationSetting
    {
        private readonly SwipeDirection Direction;
        private readonly int Duration;
        private readonly IInterpolator Interpolator;

        private RewindAnimationSetting(SwipeDirection direction, int duration, IInterpolator interpolator)
        {
            try
            {
                Direction = direction;
                Duration = duration;
                Interpolator = interpolator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }


        public SwipeDirection GetDirection()
        {
            return Direction;
        }

        public int GetDuration()
        {
            return Duration;
        }

        public IInterpolator GetInterpolator()
        {
            return Interpolator;
        }

        public class Builder
        {
            internal SwipeDirection Direction = SwipeDirection.Bottom;
            internal int Duration = SwipeDuration.Normal.Duration;
            internal IInterpolator Interpolator = new DecelerateInterpolator();

            public virtual Builder SetDirection(SwipeDirection direction)
            {
                Direction = direction;
                return this;
            }

            public virtual Builder SetDuration(int duration)
            {
                Duration = duration;
                return this;
            }

            public virtual Builder SetInterpolator(IInterpolator interpolator)
            {
                Interpolator = interpolator;
                return this;
            }

            public virtual RewindAnimationSetting Build()
            {
                return new RewindAnimationSetting(Direction, Duration, Interpolator);
            }
        }
    }
}