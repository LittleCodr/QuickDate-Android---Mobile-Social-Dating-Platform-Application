using System;

namespace QuickDate.Helpers.Utils
{
    public class DoubleClickHandler
    {
        private readonly int DelayInMs;
        private DateTime PreviousClickTime = DateTime.UtcNow;

        public DoubleClickHandler(int delayInMs = 200)
        {
            DelayInMs = delayInMs;
        }

        public void HandleDoubleClick(Action a)
        {
            if (CanClick(DateTime.UtcNow))
            {
                a.Invoke();
            }
        }

        private bool CanClick(DateTime newClickTime)
        {
            var diff = newClickTime.Subtract(PreviousClickTime).TotalMilliseconds;
            PreviousClickTime = newClickTime;
            return diff > DelayInMs;
        }
    }
}