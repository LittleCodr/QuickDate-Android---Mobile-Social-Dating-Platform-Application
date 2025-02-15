using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Core.Content;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Library.Anjo.Call
{
    public class CallAnswerDeclineButton : LinearLayout, View.IOnTouchListener
    {
        private const int TotalTime = 1000;
        private const int ShakeTime = 200;

        private static readonly int UpTime = (TotalTime - ShakeTime) / 2;
        private static readonly int DownTime = (TotalTime - ShakeTime) / 2;
        private const int FadeOutTime = 300;
        private const int FadeInTime = 100;
        private static readonly int ShimmerTotal = UpTime + ShakeTime;

        private const int AnswerThreshold = 112;
        private const int DeclineThreshold = 56;

        private TextView SwipeUpText;
        private ImageView Fab;
        private TextView SwipeDownText;

        private ImageView ArrowOne;
        private ImageView ArrowTwo;
        private ImageView ArrowThree;
        private ImageView ArrowFour;

        private float LastY;

        private bool Animating = false;
        private bool Complete = false;

        private AnimatorSet AnimatorSet;
        private IAnswerDeclineListener Listener;


        protected CallAnswerDeclineButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public CallAnswerDeclineButton(Context context) : base(context)
        {
            Initialize();
        }

        public CallAnswerDeclineButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public CallAnswerDeclineButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize();
        }

        public CallAnswerDeclineButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize();
        }


        private void Initialize()
        {
            try
            {
                Orientation = Orientation.Vertical;
                LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                Inflate(Context, Resource.Layout.CallAnswerDeclineButton, this);

                SwipeUpText = FindViewById<TextView>(Resource.Id.swipe_up_text);
                Fab = FindViewById<ImageView>(Resource.Id.answer);
                SwipeDownText = FindViewById<TextView>(Resource.Id.swipe_down_text);

                ArrowOne = FindViewById<ImageView>(Resource.Id.arrow_one);
                ArrowTwo = FindViewById<ImageView>(Resource.Id.arrow_two);
                ArrowThree = FindViewById<ImageView>(Resource.Id.arrow_three);
                ArrowFour = FindViewById<ImageView>(Resource.Id.arrow_four);

                Fab.SetOnTouchListener(this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StartRingingAnimation()
        {
            try
            {
                if (!Animating)
                {
                    Animating = true;
                    AnimateElements(0);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopRingingAnimation()
        {
            try
            {
                if (Animating)
                {
                    Animating = false;
                    ResetElements();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetAnswerDeclineListener(IAnswerDeclineListener listener)
        {
            Listener = listener;
        }

        public bool OnTouch(View v, MotionEvent @event)
        {

            try
            {
                switch (@event.Action)
                {
                    case MotionEventActions.Down:
                        ResetElements();
                        SwipeUpText.Animate().Alpha(0).SetDuration(200).Start();
                        SwipeDownText.Animate().Alpha(0).SetDuration(200).Start();
                        LastY = @event.RawY;
                        break;
                    case MotionEventActions.Cancel:
                    case MotionEventActions.Up:
                        SwipeUpText.ClearAnimation();
                        SwipeDownText.ClearAnimation();
                        SwipeUpText.Alpha = 1;
                        SwipeDownText.Alpha = 1;
                        Fab.Rotation = 0;

                        if ((int)Build.VERSION.SdkInt >= 21)
                        {
                            Fab.Drawable.SetTint(Color.ParseColor("#00c853"));
                            Fab.Background.SetTint(Color.White);
                        }

                        Animating = true;
                        AnimateElements(0);
                        break;
                    case MotionEventActions.Move:
                        float difference = @event.RawY - LastY;

                        float differenceThreshold;
                        float percentageToThreshold;
                        int backgroundColor;
                        int foregroundColor;

                        if (difference <= 0)
                        {
                            differenceThreshold = PixelUtil.DpToPx(Context, AnswerThreshold);
                            percentageToThreshold = Math.Min(1, difference * -1 / differenceThreshold);
                            backgroundColor = (int)new ArgbEvaluator().Evaluate(percentageToThreshold, ContextCompat.GetColor(Context, Resource.Color.green100), ContextCompat.GetColor(Context, Resource.Color.greenA700));

                            if (percentageToThreshold > 0.5)
                            {
                                foregroundColor = Color.White;
                            }
                            else
                            {
                                foregroundColor = ContextCompat.GetColor(Context, Resource.Color.greenA700);
                            }

                            Fab.TranslationY = difference;

                            if (percentageToThreshold == 1 && Listener != null)
                            {
                                Fab.Visibility = ViewStates.Invisible;
                                LastY = @event.RawY;
                                if (!Complete)
                                {
                                    Complete = true;
                                    Listener.OnAnswered();
                                }
                            }
                        }
                        else
                        {
                            differenceThreshold = PixelUtil.DpToPx(Context, DeclineThreshold);
                            percentageToThreshold = Math.Min(1, difference / differenceThreshold);
                            backgroundColor = (int)new ArgbEvaluator().Evaluate(percentageToThreshold, ContextCompat.GetColor(Context, Resource.Color.red100), ContextCompat.GetColor(Context, Resource.Color.redA700));

                            if (percentageToThreshold > 0.5)
                            {
                                foregroundColor = Color.White;
                            }
                            else
                            {
                                foregroundColor = ContextCompat.GetColor(Context, Resource.Color.redA700);
                            }

                            Fab.Rotation = 135 * percentageToThreshold;

                            if (percentageToThreshold == 1 && Listener != null)
                            {
                                Fab.Visibility = ViewStates.Invisible;
                                LastY = @event.RawY;

                                if (!Complete)
                                {
                                    Complete = true;
                                    Listener.OnDeclined();
                                }
                            }
                        }

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            Fab.Background.SetTint(backgroundColor);
                            Fab.Drawable.SetTint(foregroundColor);
                        }

                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

            return true;
        }

        private void AnimateElements(int delay)
        {
            try
            {
                ObjectAnimator fabUp = GetUpAnimation(Fab);
                ObjectAnimator fabDown = GetDownAnimation(Fab);
                ObjectAnimator fabShake = GetShakeAnimation(Fab);

                AnimatorSet = new AnimatorSet();
                AnimatorSet.Play(fabUp).With(GetUpAnimation(SwipeUpText));
                AnimatorSet.Play(fabShake).After(fabUp);
                AnimatorSet.Play(fabDown).With(GetDownAnimation(SwipeUpText)).After(fabShake);

                AnimatorSet.Play(GetFadeOut(SwipeDownText)).With(fabUp);
                AnimatorSet.Play(GetFadeIn(SwipeDownText)).After(fabDown);

                AnimatorSet.Play(GetShimmer(ArrowFour, ArrowThree, ArrowTwo, ArrowOne));

                AnimatorSet.AddListener(new AnimatorListenerAnonymousInnerClass(this));

                AnimatorSet.StartDelay = delay;
                AnimatorSet.Start();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class AnimatorListenerAnonymousInnerClass : Java.Lang.Object, Animator.IAnimatorListener
        {
            private readonly CallAnswerDeclineButton OuterInstance;

            public AnimatorListenerAnonymousInnerClass(CallAnswerDeclineButton outerInstance)
            {
                OuterInstance = outerInstance;
            }

            public void OnAnimationCancel(Animator animation)
            {

            }

            public void OnAnimationEnd(Animator animation)
            {
                try
                {
                    if (OuterInstance.Animating)
                    {
                        OuterInstance.AnimateElements(1000);
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {

            }

            public void OnAnimationStart(Animator animation)
            {

            }
        }

        private Animator GetShimmer(params View[] targets)
        {
            try
            {
                AnimatorSet animatorSet = new AnimatorSet();
                int evenDuration = ShimmerTotal / targets.Length;
                int interval = 75;

                for (int i = 0; i < targets.Length; i++)
                {
                    animatorSet.Play(GetShimmer(targets[i], evenDuration + (evenDuration - interval))).After(interval * i);
                }

                return animatorSet;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetShimmer(View target, int duration)
        {
            try
            {
                ObjectAnimator shimmer = ObjectAnimator.OfFloat(target, "alpha", 0, 1, 0);
                shimmer.SetDuration(duration);

                return shimmer;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetShakeAnimation(View target)
        {
            try
            {
                ObjectAnimator animator = ObjectAnimator.OfFloat(target, "translationX", 0, 25, -25, 25, -25, 15, -15, 6, -6, 0);
                animator.SetDuration(ShakeTime);

                return animator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetUpAnimation(View target)
        {
            try
            {
                ObjectAnimator animator = ObjectAnimator.OfFloat(target, "translationY", 0, -1 * PixelUtil.DpToPx(Context, 32));
                animator.SetInterpolator(new AccelerateInterpolator());
                animator.SetDuration(UpTime);

                return animator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetDownAnimation(View target)
        {
            try
            {
                ObjectAnimator animator = ObjectAnimator.OfFloat(target, "translationY", 0);
                animator.SetInterpolator(new DecelerateInterpolator());
                animator.SetDuration(DownTime);

                return animator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetFadeOut(View target)
        {
            try
            {
                ObjectAnimator animator = ObjectAnimator.OfFloat(target, "alpha", 1, 0);
                animator.SetDuration(FadeOutTime);
                return animator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private ObjectAnimator GetFadeIn(View target)
        {
            try
            {
                ObjectAnimator animator = ObjectAnimator.OfFloat(target, "alpha", 0, 1);
                animator.SetDuration(FadeInTime);
                return animator;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        private void ResetElements()
        {
            try
            {
                Animating = false;
                Complete = false;

                if (AnimatorSet != null)
                {
                    AnimatorSet.Cancel();
                }

                SwipeUpText.TranslationY = 0;
                Fab.TranslationY = 0;
                SwipeDownText.Alpha = 1;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public interface IAnswerDeclineListener
        {
            void OnAnswered();

            void OnDeclined();
        }

    }
}