using System.Collections.Generic;

namespace QuickDate.Library.Anjo.CardStackView
{
    public class SwipeDirection
    {
        public static readonly SwipeDirection Left = new SwipeDirection("Left", InnerEnum.Left);
        public static readonly SwipeDirection Right = new SwipeDirection("Right", InnerEnum.Right);
        public static readonly SwipeDirection Top = new SwipeDirection("Top", InnerEnum.Top);
        public static readonly SwipeDirection Bottom = new SwipeDirection("Bottom", InnerEnum.Bottom);

        private static readonly List<SwipeDirection> ValueList = new List<SwipeDirection>();

        static SwipeDirection()
        {
            ValueList.Add(Left);
            ValueList.Add(Right);
            ValueList.Add(Top);
            ValueList.Add(Bottom);
        }

        public enum InnerEnum
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public readonly InnerEnum InnerEnumValue;
        private readonly string NameValue;
        private readonly int OrdinalValue;
        private static int NextOrdinal = 0;

        private SwipeDirection(string name, InnerEnum innerEnum)
        {
            NameValue = name;
            OrdinalValue = NextOrdinal++;
            InnerEnumValue = innerEnum;
        }

        public static readonly List<SwipeDirection> Horizontal = new List<SwipeDirection> { Left, Right };
        public static readonly List<SwipeDirection> Vertical = new List<SwipeDirection> { Top, Bottom };
        public static readonly List<SwipeDirection> Freedom = new List<SwipeDirection>(Values());

        public static List<SwipeDirection> Values()
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

        public static SwipeDirection ValueOf(string name)
        {
            foreach (SwipeDirection enumInstance in ValueList)
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