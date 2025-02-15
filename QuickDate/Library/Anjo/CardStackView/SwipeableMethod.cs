using QuickDate.Helpers.Utils;
using System;
using System.Collections.Generic;

namespace QuickDate.Library.Anjo.CardStackView
{
    public sealed class SwipeableMethod
    {
        public static readonly SwipeableMethod AutomaticAndManual = new SwipeableMethod("AutomaticAndManual", InnerEnum.AutomaticAndManual);
        public static readonly SwipeableMethod Automatic = new SwipeableMethod("Automatic", InnerEnum.Automatic);
        public static readonly SwipeableMethod Manual = new SwipeableMethod("Manual", InnerEnum.Manual);
        public static readonly SwipeableMethod None = new SwipeableMethod("None", InnerEnum.None);

        private static readonly IList<SwipeableMethod> ValueList = new List<SwipeableMethod>();

        static SwipeableMethod()
        {
            try
            {
                ValueList.Add(AutomaticAndManual);
                ValueList.Add(Automatic);
                ValueList.Add(Manual);
                ValueList.Add(None);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public enum InnerEnum
        {
            AutomaticAndManual,
            Automatic,
            Manual,
            None
        }

        public readonly InnerEnum InnerEnumValue;
        private readonly string NameValue;
        private readonly int OrdinalValue;
        private static int NextOrdinal;

        private SwipeableMethod(string name, InnerEnum innerEnum)
        {
            NameValue = name;
            OrdinalValue = NextOrdinal++;
            InnerEnumValue = innerEnum;
        }

        internal bool CanSwipe()
        {
            return CanSwipeAutomatically() || CanSwipeManually();
        }

        internal bool CanSwipeAutomatically()
        {
            return this == AutomaticAndManual || this == Automatic;
        }

        internal bool CanSwipeManually()
        {
            return this == AutomaticAndManual || this == Manual;
        }

        public static IList<SwipeableMethod> Values()
        {
            return ValueList;
        }

        public int Ordinal()
        {
            return OrdinalValue;
        }

        public override string ToString()
        {
            return NameValue;
        }

        public static SwipeableMethod ValueOf(string name)
        {
            foreach (SwipeableMethod enumInstance in ValueList)
            {
                if (enumInstance.NameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new ArgumentException(name);
        }
    }

}