using Android.Views;
using AndroidX.RecyclerView.Widget;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public class CardStackSmoothScrolled : RecyclerView.SmoothScroller
    {
        public enum ScrollType
        {
            AutomaticSwipe,
            AutomaticRewind,
            ManualSwipe,
            ManualCancel
        }

        private readonly ScrollType Type;
        private readonly CardStackLayoutManager Manager;

        public CardStackSmoothScrolled(ScrollType type, CardStackLayoutManager manager)
        {
            try
            {
                Type = type;
                Manager = manager;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnSeekTargetStep(int p0, int p1, RecyclerView.State p2, Action action)
        {
            try
            {
                if (Type == ScrollType.AutomaticRewind)
                {
                    RewindAnimationSetting setting = Manager.GetCardStackSetting().RewindAnimationSetting;
                    action.Update(-GetDx(setting), -GetDy(setting), setting.GetDuration(), setting.GetInterpolator());
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStart()
        {
            try
            {
                ICardStackListener listener = Manager.GetCardStackListener();
                CardStackState state = Manager.GetCardStackState();
                switch (Type)
                {
                    case ScrollType.AutomaticSwipe:
                        state.Next(CardStackState.Status.AutomaticSwipeAnimating);
                        listener?.OnCardDisappeared(Manager.GetTopView(), Manager.GetTopPosition());
                        break;
                    case ScrollType.AutomaticRewind:
                        state.Next(CardStackState.Status.RewindAnimating);
                        break;
                    case ScrollType.ManualSwipe:
                        state.Next(CardStackState.Status.ManualSwipeAnimating);
                        listener?.OnCardDisappeared(Manager.GetTopView(), Manager.GetTopPosition());
                        break;
                    case ScrollType.ManualCancel:
                        state.Next(CardStackState.Status.RewindAnimating);
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                ICardStackListener listener = Manager.GetCardStackListener();
                switch (Type)
                {
                    case ScrollType.AutomaticSwipe:
                        // Notify callback from CardStackLayoutManager
                        break;
                    case ScrollType.AutomaticRewind:
                        listener?.OnCardRewound();
                        listener?.OnCardAppeared(Manager.GetTopView(), Manager.GetTopPosition());
                        break;
                    case ScrollType.ManualSwipe:
                        // Notify callback from CardStackLayoutManager
                        break;
                    case ScrollType.ManualCancel:
                        listener?.OnCardCanceled();
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        protected override void OnTargetFound(View targetView, RecyclerView.State state, Action action)
        {
            try
            {
                int x = (int)targetView.TranslationX;
                int y = (int)targetView.TranslationY;
                IAnimationSetting setting;
                switch (Type)
                {
                    case ScrollType.AutomaticSwipe:
                        setting = Manager.GetCardStackSetting().SwipeAnimationSetting;
                        action.Update(-GetDx(setting), -GetDy(setting), setting.GetDuration(), setting.GetInterpolator());
                        break;
                    case ScrollType.AutomaticRewind:
                        setting = Manager.GetCardStackSetting().RewindAnimationSetting;
                        action.Update(x, y, setting.GetDuration(), setting.GetInterpolator());
                        break;
                    case ScrollType.ManualSwipe:
                        int dx = -x * 10;
                        int dy = -y * 10;
                        setting = Manager.GetCardStackSetting().SwipeAnimationSetting;
                        action.Update(dx, dy, setting.GetDuration(), setting.GetInterpolator());
                        break;
                    case ScrollType.ManualCancel:
                        setting = Manager.GetCardStackSetting().RewindAnimationSetting;
                        action.Update(x, y, setting.GetDuration(), setting.GetInterpolator());
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private int GetDx(IAnimationSetting setting)
        {
            try
            {
                CardStackState state = Manager.GetCardStackState();
                int dx = 0;
                if (setting.GetDirection() == SwipeDirection.Left)
                    dx = -state.Width * 2;
                else if (setting.GetDirection() == SwipeDirection.Right)
                    dx = state.Width * 2;
                else if (setting.GetDirection() == SwipeDirection.Top || setting.GetDirection() == SwipeDirection.Bottom)
                    dx = 0;

                return dx;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }

        private int GetDy(IAnimationSetting setting)
        {
            try
            {
                CardStackState state = Manager.GetCardStackState();
                int dy = 0;
                if (setting.GetDirection() == SwipeDirection.Left || setting.GetDirection() == SwipeDirection.Right)
                    dy = state.Height / 4;
                else if (setting.GetDirection() == SwipeDirection.Top)
                    dy = -state.Height * 2;
                else if (setting.GetDirection() == SwipeDirection.Bottom) dy = state.Height * 2;

                return dy;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return 0;
            }
        }
    }
}