using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace QuickDate.Library.Anjo.CardStackView.Internal
{
    public class CardStackState
    {
        public Status StatusCard = Status.Idle;
        public int Width = 0;
        public int Height = 0;
        public int Dx = 0;
        public int Dy = 0;
        public int TopPosition = 0;
        public int TargetPosition = RecyclerView.NoPosition;
        public float Proportion = 0.0f;


        public class Status
        {
            public static readonly Status Idle = new Status("Idle", InnerEnum.Idle);
            public static readonly Status Dragging = new Status("Dragging", InnerEnum.Dragging);
            public static readonly Status RewindAnimating = new Status("RewindAnimating", InnerEnum.RewindAnimating);
            public static readonly Status AutomaticSwipeAnimating = new Status("AutomaticSwipeAnimating", InnerEnum.AutomaticSwipeAnimating);
            public static readonly Status AutomaticSwipeAnimated = new Status("AutomaticSwipeAnimated", InnerEnum.AutomaticSwipeAnimated);
            public static readonly Status ManualSwipeAnimating = new Status("ManualSwipeAnimating", InnerEnum.ManualSwipeAnimating);
            public static readonly Status ManualSwipeAnimated = new Status("ManualSwipeAnimated", InnerEnum.ManualSwipeAnimated);

            private static readonly IList<Status> ValueList = new List<Status>();

            static Status()
            {
                ValueList.Add(Idle);
                ValueList.Add(Dragging);
                ValueList.Add(RewindAnimating);
                ValueList.Add(AutomaticSwipeAnimating);
                ValueList.Add(AutomaticSwipeAnimated);
                ValueList.Add(ManualSwipeAnimating);
                ValueList.Add(ManualSwipeAnimated);
            }

            public enum InnerEnum
            {
                Idle,
                Dragging,
                RewindAnimating,
                AutomaticSwipeAnimating,
                AutomaticSwipeAnimated,
                ManualSwipeAnimating,
                ManualSwipeAnimated
            }

            public readonly InnerEnum InnerEnumValue;
            private readonly string NameValue;
            private readonly int OrdinalValue;
            private static int NextOrdinal;

            private Status(string name, InnerEnum innerEnum)
            {
                NameValue = name;
                OrdinalValue = NextOrdinal++;
                InnerEnumValue = innerEnum;
            }

            public bool IsBusy => this != Idle;

            public bool IsDragging => this == Dragging;

            public bool IsSwipeAnimating => this == ManualSwipeAnimating || this == AutomaticSwipeAnimating;

            public Status ToAnimatedStatus()
            {
                if (this == ManualSwipeAnimating)
                    return ManualSwipeAnimated;
                else if (this == AutomaticSwipeAnimating)
                    return AutomaticSwipeAnimated;
                else
                    return Idle;
            }

            public static IList<Status> Values()
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

            public static Status ValueOf(string name)
            {
                foreach (Status enumInstance in ValueList)
                {
                    if (enumInstance.NameValue == name)
                    {
                        return enumInstance;
                    }
                }
                throw new ArgumentException(name);
            }
        }

        public void Next(Status state)
        {
            StatusCard = state;
        }

        public SwipeDirection GetDirection()
        {
            if (Math.Abs(Dy) < Math.Abs(Dx))
            {
                if (Dx < 0.0f)
                {
                    return SwipeDirection.Left;
                }
                else
                {
                    return SwipeDirection.Right;
                }
            }
            else
            {
                if (Dy < 0.0f)
                {
                    return SwipeDirection.Top;
                }
                else
                {
                    return SwipeDirection.Bottom;
                }
            }
        }

        public float GetRatio()
        {
            int absDx = Math.Abs(Dx);
            int absDy = Math.Abs(Dy);
            float ratio;
            if (absDx < absDy)
            {
                ratio = absDy / (Height / 2.0f);
            }
            else
            {
                ratio = absDx / (Width / 2.0f);
            }
            return Math.Min(ratio, 1.0f);
        }

        public bool IsSwipeCompleted()
        {
            if (StatusCard.IsSwipeAnimating)
            {
                if (TopPosition < TargetPosition)
                {
                    if (Width < Math.Abs(Dx) || Height < Math.Abs(Dy))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanScrollToPosition(int position, int itemCount)
        {
            if (position == TopPosition)
            {
                return false;
            }
            if (position < 0)
            {
                return false;
            }
            if (itemCount < position)
            {
                return false;
            }
            if (StatusCard.IsBusy)
            {
                return false;
            }
            return true;
        }

    }
}