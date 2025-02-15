using Android.Views.Animations;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public interface IAnimationSetting
    {
        SwipeDirection GetDirection();
        int GetDuration();
        IInterpolator GetInterpolator();
    }
}