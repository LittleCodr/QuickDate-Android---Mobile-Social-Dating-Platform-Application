using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;

namespace QuickDate.Adapters
{
    public class MainTabAdapter : FragmentStateAdapter
    {
        #region Variables

        public List<Fragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        #endregion

        public MainTabAdapter(Fragment fragment) : base(fragment)
        {
            try
            {
                Fragments = new List<Fragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public MainTabAdapter(FragmentActivity fragmentActivity) : base(fragmentActivity)
        {
            try
            {
                Fragments = new List<Fragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public MainTabAdapter(FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
        {
            try
            {
                Fragments = new List<Fragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public override int ItemCount => Fragments.Count;
        public override Fragment CreateFragment(int position)
        {
            if (Fragments[position] != null)
            {
                return Fragments[position];
            }
            else
            {
                return Fragments[0];
            }
        }

        public string GetFragment(int position)
        {
            try
            {
                if (FragmentNames[position] != null)
                {
                    return FragmentNames[position];
                }
                else
                {
                    return FragmentNames[0];
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return "";
            }
        }

        public void AddFragment(Fragment fragment, string name)
        {
            try
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void ClaerFragment()
        {
            try
            {
                Fragments.Clear();
                FragmentNames.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void RemoveFragment(Fragment fragment, string name)
        {
            try
            {
                Fragments.Remove(fragment);
                FragmentNames.Remove(name);
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        public void InsertFragment(int index, Fragment fragment, string name)
        {
            try
            {
                Fragments.Insert(index, fragment);
                FragmentNames.Insert(index, name);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

    }
}