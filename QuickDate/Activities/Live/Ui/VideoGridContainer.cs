using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Activities.Live.Stats;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Exception = System.Exception;

namespace QuickDate.Activities.Live.Ui
{
    public class VideoGridContainer : RelativeLayout, IRunnable
    {
        private static readonly int MaxUser = 4;
        private static readonly int StatsRefreshInterval = 2000;
        private static readonly int StatLeftMargin = 34;
        private static readonly int StatTextSize = 10;

        private Dictionary<int, ViewGroup> MUserViewList;
        private readonly List<int> MUidList = new List<int>(MaxUser);
        private StatsManager MStatsManager;
        private Handler MHandler;
        private int MStatMarginBottom;

        protected VideoGridContainer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public VideoGridContainer(Context context) : base(context)
        {
            Init();
        }

        public VideoGridContainer(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public VideoGridContainer(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public VideoGridContainer(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                //SetBackgroundResource(Resource.Drawable.live_room_bg);
                MStatMarginBottom = Resources.GetDimensionPixelSize(Resource.Dimension.live_stat_margin_bottom);
                MHandler = new Handler(Looper.MainLooper);
                MUserViewList = new Dictionary<int, ViewGroup>(MaxUser);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void SetStatsManager(StatsManager manager)
        {
            MStatsManager = manager;
        }

        public void AddUserVideotextureView(int uid, SurfaceView surfaceView, bool isLocal)
        {
            try
            {
                switch (surfaceView)
                {
                    case null:
                        return;
                }

                int id = -1;
                switch (isLocal)
                {
                    case true:
                        {
                            if (MUidList.Contains(0))
                            {
                                MUidList.Remove(0);
                                MUserViewList.Remove(0);
                            }

                            if (MUidList.Count == MaxUser)
                            {
                                MUidList.Remove(0);
                                MUserViewList.Remove(0);
                            }
                            id = 0;
                            break;
                        }
                    default:
                        {
                            if (MUidList.Contains(uid))
                            {
                                MUidList.Remove(uid);
                                MUserViewList.Remove(uid);
                            }

                            if (MUidList.Count < MaxUser)
                            {
                                id = uid;
                            }

                            break;
                        }
                }

                switch (id)
                {
                    case 0:
                        MUidList.Add(uid);
                        break;
                    default:
                        MUidList.Add(uid);
                        break;
                }

                if (id != -1)
                {
                    MUserViewList.Add(uid, CreateVideoView(surfaceView));

                    if (MStatsManager != null)
                    {
                        MStatsManager.AddUserStats(uid, isLocal);
                        if (MStatsManager.IsEnabled())
                        {
                            MHandler.RemoveCallbacks(this);
                            MHandler.PostDelayed(this, StatsRefreshInterval);
                        }
                    }

                    RequestGridLayout();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private ViewGroup CreateVideoView(SurfaceView surface)
        {
            try
            {
                RelativeLayout layout = new RelativeLayout(Context) { Id = surface.GetHashCode() };

                LayoutParams videoLayoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                layout.AddView(surface, videoLayoutParams);

                TextView text = new TextView(Context) { Id = layout.GetHashCode() };
                LayoutParams textParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                textParams.AddRule(LayoutRules.AlignParentBottom, 1);
                textParams.BottomMargin = MStatMarginBottom;
                textParams.LeftMargin = StatLeftMargin;
                text.SetTextColor(Color.White);
                text.SetTextSize(ComplexUnitType.Sp, StatTextSize);

                layout.AddView(text, textParams);
                return layout;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public void RemoveUserVideo(int uid, bool isLocal)
        {
            try
            {
                switch (isLocal)
                {
                    case true when MUidList.Contains(0):
                        MUidList.Remove(0);
                        MUserViewList.Remove(0);
                        break;
                    default:
                        {
                            if (MUidList.Contains(uid))
                            {
                                MUidList.Remove(uid);
                                MUserViewList.Remove(uid);
                            }

                            break;
                        }
                }

                MStatsManager.RemoveUserStats(uid);
                RequestGridLayout();

                switch (ChildCount)
                {
                    case 0:
                        MHandler.RemoveCallbacks(this);
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void RequestGridLayout()
        {
            try
            {
                RemoveAllViews();
                Layout(MUidList.Count);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void Layout(int size)
        {
            try
            {
                var params1 = GetParams(size);
                for (int i = 0; i < size; i++)
                {
                    var value = MUserViewList.FirstOrDefault(async => async.Key == MUidList[i]).Value;
                    if (value != null)
                        AddView(value, params1[i]);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private LayoutParams[] GetParams(int size)
        {
            try
            {
                int width = MeasuredWidth;
                int height = MeasuredHeight;

                LayoutParams[] array = new LayoutParams[size];

                for (int i = 0; i < size; i++)
                {
                    switch (i)
                    {
                        case 0:
                            array[0] = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                            array[0].AddRule(LayoutRules.AlignParentTop, 0);
                            array[0].AddRule(LayoutRules.AlignParentLeft, 0);
                            break;
                        case 1:
                            array[1] = new LayoutParams(width, height / 2);
                            array[0].Height = array[1].Height;
                            var value10 = MUserViewList.FirstOrDefault(async => async.Key == MUidList[0]).Value;
                            array[1].AddRule(LayoutRules.Below, value10.Id);
                            array[1].AddRule(LayoutRules.AlignParentLeft, 0);
                            break;
                        case 2:
                            array[i] = new LayoutParams(width / 2, height / 2);
                            array[i - 1].Width = array[i].Width;
                            var value = MUserViewList.FirstOrDefault(async => async.Key == MUidList[i - 1]).Value;
                            array[i].AddRule(LayoutRules.RightOf, value.Id);
                            array[i].AddRule(LayoutRules.AlignTop, value.Id);
                            break;
                        case 3:
                            array[i] = new LayoutParams(width / 2, height / 2);
                            array[0].Width = width / 2;
                            array[1].AddRule(LayoutRules.Below, 0);
                            array[1].AddRule(LayoutRules.AlignParentLeft, 0);
                            var value30 = MUserViewList.FirstOrDefault(async => async.Key == MUidList[0]).Value;
                            array[1].AddRule(LayoutRules.RightOf, value30.Id);
                            array[1].AddRule(LayoutRules.AlignParentTop, 0);
                            array[2].AddRule(LayoutRules.AlignParentLeft, 0);
                            array[2].AddRule(LayoutRules.RightOf, 0);
                            array[2].AddRule(LayoutRules.AlignTop, 0);
                            array[2].AddRule(LayoutRules.Below, value30.Id);
                            var value31 = MUserViewList.FirstOrDefault(async => async.Key == MUidList[1]).Value;
                            var value32 = MUserViewList.FirstOrDefault(async => async.Key == MUidList[2]).Value;
                            array[3].AddRule(LayoutRules.Below, value31.Id);
                            array[3].AddRule(LayoutRules.RightOf, value32.Id);
                            break;
                    }
                }

                return array;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return new LayoutParams[size];
            }
        }

        protected override void OnDetachedFromWindow()
        {
            try
            {
                base.OnDetachedFromWindow();
                ClearAllVideo();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        private void ClearAllVideo()
        {
            try
            {
                RemoveAllViews();
                MUserViewList.Clear();
                MUidList.Clear();
                MHandler.RemoveCallbacks(this);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
        public void Run()
        {
            try
            {
                if (MStatsManager != null && MStatsManager.IsEnabled())
                {
                    int count = ChildCount;
                    for (int i = 0; i < count; i++)
                    {
                        RelativeLayout layout = (RelativeLayout)GetChildAt(i);
                        TextView text = layout?.FindViewById<TextView>(layout.GetHashCode());
                        if (text != null)
                        {
                            StatsData data = MStatsManager.GetStatsData(MUidList[i]);
                            string info = data?.ToString();
                            if (info != null) text.SetText(info, TextView.BufferType.Normal);
                        }
                    }

                    MHandler.PostDelayed(this, StatsRefreshInterval);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}