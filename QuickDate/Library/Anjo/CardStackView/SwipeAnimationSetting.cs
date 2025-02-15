using Android.Views.Animations;
using QuickDate.Library.Anjo.CardStackView.Internal;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class SwipeAnimationSetting : IAnimationSetting
    {
        private readonly SwipeDirection Direction;
        private readonly int Duration;
        private readonly IInterpolator Interpolator;

        private SwipeAnimationSetting(SwipeDirection direction, int duration, IInterpolator interpolator)
        {
            Direction = direction;
            Duration = duration;
            Interpolator = interpolator;
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
            private SwipeDirection Direction = SwipeDirection.Right;
            private int Duration = SwipeDuration.Normal.Duration;
            private IInterpolator Interpolator = new AccelerateInterpolator();

            public Builder SetDirection(SwipeDirection direction)
            {
                Direction = direction;
                return this;
            }

            public Builder SetDuration(int duration)
            {
                Duration = duration;
                return this;
            }

            public Builder SetInterpolator(IInterpolator interpolator)
            {
                Interpolator = interpolator;
                return this;
            }

            public SwipeAnimationSetting Build()
            {
                return new SwipeAnimationSetting(Direction, Duration, Interpolator);
            }
        }

    }
}