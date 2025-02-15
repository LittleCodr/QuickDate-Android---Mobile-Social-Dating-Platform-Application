using System;

namespace QuickDate.Helpers.Model
{
    public class SwipeLimitDetails
    {
        public int SwapCount { get; set; }
        public DateTime LastSwapDate { get; set; }

        public bool CanSwipe(int maxSwapLimit)
        {
            var isSwapCountNotReachedTotalSwapLimit = SwapCount < maxSwapLimit;

            var canSwipe = isSwapCountNotReachedTotalSwapLimit || IsNextDay();

            return canSwipe;
        }

        public int GetSwipeCount()
        {
            return IsNextDay() ? 0 : SwapCount;
        }

        private bool IsNextDay()
        {
            TimeSpan timeSpan = DateTime.UtcNow.Date.Subtract(LastSwapDate.Date);
            var isNextDay = timeSpan.TotalDays >= 1;
            return isNextDay;
        }
    }
}