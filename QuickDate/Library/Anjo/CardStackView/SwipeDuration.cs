using System.Collections.Generic;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class SwipeDuration
    {
        public static readonly SwipeDuration Fast = new SwipeDuration("Fast", InnerEnum.Fast, 100);
        public static readonly SwipeDuration Normal = new SwipeDuration("Normal", InnerEnum.Normal, 200);
        public static readonly SwipeDuration Slow = new SwipeDuration("Slow", InnerEnum.Slow, 500);

        private static readonly IList<SwipeDuration> ValueList = new List<SwipeDuration>();

        static SwipeDuration()
        {
            ValueList.Add(Fast);
            ValueList.Add(Normal);
            ValueList.Add(Slow);
        }

        public enum InnerEnum
        {
            Fast,
            Normal,
            Slow
        }

        public readonly InnerEnum InnerEnumValue;
        private readonly string NameValue;
        private readonly int OrdinalValue;
        private static int NextOrdinal;

        public readonly int Duration;

        internal SwipeDuration(string name, InnerEnum innerEnum, int duration)
        {
            Duration = duration;

            NameValue = name;
            OrdinalValue = NextOrdinal++;
            InnerEnumValue = innerEnum;
        }

        public static SwipeDuration FromVelocity(int velocity)
        {
            if (velocity < 1000)
            {
                return Slow;
            }
            else if (velocity < 5000)
            {
                return Normal;
            }
            return Fast;
        }

        public static IList<SwipeDuration> Values()
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

        public static SwipeDuration ValueOf(string name)
        {
            foreach (SwipeDuration enumInstance in ValueList)
            {
                if (enumInstance.NameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }

}