using Android.Views.Animations;
using System.Collections.Generic;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public class CardStackSetting
    {
        public StackFrom StackFrom = StackFrom.None;
        public int VisibleCount = 3;
        public float TranslationInterval = 8.0f;
        public float ScaleInterval = 0.95f; // 0.0f - 1.0f
        public float SwipeThreshold = 0.3f; // 0.0f - 1.0f
        public float MaxDegree = 20.0f;


        public List<SwipeDirection> Directions = Horizontal;
        public bool CanScrollHorizontal = true;
        public bool CanScrollVertical = true;
        public SwipeableMethod SwipeableMethod = SwipeableMethod.AutomaticAndManual;
        public SwipeAnimationSetting SwipeAnimationSetting = new SwipeAnimationSetting.Builder().Build();
        public RewindAnimationSetting RewindAnimationSetting = new RewindAnimationSetting.Builder().Build();
        public IInterpolator OverlayInterpolator = new LinearInterpolator();

        //Direction
        public static readonly List<SwipeDirection> Horizontal = new List<SwipeDirection> { SwipeDirection.Left, SwipeDirection.Right };
        public static readonly List<SwipeDirection> Vertical = new List<SwipeDirection> { SwipeDirection.Top, SwipeDirection.Bottom };
        public static readonly List<SwipeDirection> Freedom = new List<SwipeDirection> { SwipeDirection.Top, SwipeDirection.Right, SwipeDirection.Bottom, SwipeDirection.Left };

    }
}