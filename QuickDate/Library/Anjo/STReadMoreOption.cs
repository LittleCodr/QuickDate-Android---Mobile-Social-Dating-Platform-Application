using Android.Animation;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.Utils;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace QuickDate.Library.Anjo
{
    public enum StTypeText
    {
        ReadMore,
        ReadLess
    }

    public class StReadMoreOption
    {
        public static readonly string Tag = "STReadMoreOption";
        public static readonly int TypeLine = 1;
        public static readonly int TypeCharacter = 2;

        // optional
        private readonly int TextLength;
        private readonly int TextLengthType;
        private readonly string MoreLabel;
        private readonly string LessLabel;
        private readonly Color MoreLabelColor;
        private readonly Color LessLabelColor;
        private readonly bool LabelUnderLine;
        private readonly bool ExpandAnimation;

        private StReadMoreOption(Builder builder)
        {

            TextLength = builder.MTextLength;
            TextLengthType = builder.MTextLengthType;
            MoreLabel = builder.MMoreLabel;
            LessLabel = builder.MLessLabel;
            MoreLabelColor = builder.MMoreLabelColor;
            LessLabelColor = builder.MLessLabelColor;
            LabelUnderLine = builder.MLabelUnderLine;
            ExpandAnimation = builder.MExpandAnimation;
        }

        public void AddReadMoreTo(TextView textView, ICharSequence text)
        {
            try
            {
                if (TextLengthType == TypeCharacter)
                {
                    if (text.Length() <= TextLength)
                    {
                        textView.SetText(text, TextView.BufferType.Spannable);
                        //wael textView.SetTextFuture(PrecomputedTextCompat.GetTextFuture(text, TextViewCompat.GetTextMetricsParams(textView), null));
                        return;
                    }
                }
                else
                {
                    // If TYPE_LINE
                    textView.SetLines(TextLength);
                    //wael textView.SetTextFuture(PrecomputedTextCompat.GetTextFuture(text, TextViewCompat.GetTextMetricsParams(textView), null));
                    textView.SetText(text, TextView.BufferType.Spannable);
                }
                textView.Post(new StRunnable(this, textView, text));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void AddReadLess(TextView textView, ICharSequence text)
        {
            try
            {
                textView.SetMaxLines(Integer.MaxValue);

                SpannableStringBuilder spendableStringBuilder = new SpannableStringBuilder(text);
                spendableStringBuilder.Append("\n ");
                spendableStringBuilder.Append(LessLabel);

                SpannableString ss = SpannableString.ValueOf(spendableStringBuilder);
                ClickableSpan rclickableSpan = new StRclickableSpan(this, textView, text, StTypeText.ReadLess);
                ss.SetSpan(rclickableSpan, ss.Length() - LessLabel.Length, ss.Length(), SpanTypes.ExclusiveExclusive);

                // textView.SetTextFuture(PrecomputedTextCompat.GetTextFuture(ss, TextViewCompat.GetTextMetricsParams(textView), null));
                textView.SetText(ss, TextView.BufferType.Spannable);
                textView.MovementMethod = LinkMovementMethod.Instance;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private class StRunnable : Object, IRunnable
        {
            private readonly StReadMoreOption Option;
            private readonly TextView TextView;
            private readonly ICharSequence Text;
            public StRunnable(StReadMoreOption option, TextView textView, ICharSequence text)
            {
                try
                {
                    Option = option;
                    TextView = textView;
                    Text = text;
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
                    int textLengthNew = Option.TextLength;

                    if (Option.TextLengthType == TypeLine)
                    {
                        if (TextView.Layout.LineCount <= Option.TextLength)
                        {
                            TextView.SetText(Text, TextView.BufferType.Spannable);
                            return;
                        }

                        ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams)TextView.LayoutParameters;

                        string subString = Text.ToString().Substring(TextView.Layout.GetLineStart(0), TextView.Layout.GetLineEnd(Option.TextLength - 1));
                        textLengthNew = subString.Length - (Option.MoreLabel.Length + 4 + lp.RightMargin / 6);
                    }

                    SpannableStringBuilder spendableStringBuilder = new SpannableStringBuilder(Text.SubSequence(0, textLengthNew));
                    spendableStringBuilder.Append(" ...");
                    spendableStringBuilder.Append(Option.MoreLabel);

                    SpannableString ss = SpannableString.ValueOf(spendableStringBuilder);
                    ClickableSpan rclickableSpan = new StRclickableSpan(Option, TextView, Text, StTypeText.ReadMore);

                    ss.SetSpan(rclickableSpan, ss.Length() - Option.MoreLabel.Length, ss.Length(), SpanTypes.ExclusiveExclusive);

                    switch (Build.VERSION.SdkInt)
                    {
                        case >= BuildVersionCodes.JellyBean when Option.ExpandAnimation:
                            {
                                LayoutTransition layoutTransition = new LayoutTransition();
                                layoutTransition.EnableTransitionType(LayoutTransitionType.Changing);
                                ((ViewGroup)TextView?.Parent).LayoutTransition = layoutTransition;
                                break;
                            }
                    }
                    //TextView.SetTextFuture(PrecomputedTextCompat.GetTextFuture(ss, TextViewCompat.GetTextMetricsParams(TextView), null));
                    TextView.SetText(ss, TextView.BufferType.Spannable);
                    TextView.MovementMethod = LinkMovementMethod.Instance;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        private class StRclickableSpan : ClickableSpan
        {
            private readonly StReadMoreOption Option;
            private readonly TextView TextView;
            private readonly ICharSequence Text;
            private readonly StTypeText Type;
            public StRclickableSpan(StReadMoreOption option, TextView textView, ICharSequence text, StTypeText type)
            {
                Option = option;
                TextView = textView;
                Text = text;
                Type = type;
            }

            public override void OnClick(View widget)
            {
                try
                {
                    switch (Type)
                    {
                        case StTypeText.ReadMore:
                            Option.AddReadLess(TextView, Text);
                            break;
                        case StTypeText.ReadLess:
                            Option.AddReadMoreTo(TextView, Text);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                try
                {
                    base.UpdateDrawState(ds);
                    ds.UnderlineText = Option.LabelUnderLine;
                    ds.Color = Option.MoreLabelColor;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e);
                }
            }
        }

        public class Builder
        {

            // optional
            public int MTextLength = 100;
            public int MTextLengthType = TypeCharacter;
            public string MMoreLabel = " Read More";
            public string MLessLabel = " Read Less";
            public Color MMoreLabelColor = Color.ParseColor("#ff00ff");
            public Color MLessLabelColor = Color.ParseColor("#ff00ff");
            public bool MLabelUnderLine;
            public bool MExpandAnimation;

            public Builder()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="length">can be no. of line OR no. of characters - default is 100 character</param>
            /// <param name="textLengthType">StReadMoreOption.TYPE_LINE for no. of line OR
            /// StReadMoreOption.TYPE_CHARACTER for no. of character
            /// - default is ReadMoreOption.TYPE_CHARACTER
            /// </param>
            /// <returns>Builder</returns>
            public Builder TextLength(int length, int textLengthType)
            {
                MTextLength = length;
                MTextLengthType = textLengthType;
                return this;
            }

            public Builder MoreLabel(string moreLabel)
            {
                MMoreLabel = moreLabel;
                return this;
            }

            public Builder LessLabel(string lessLabel)
            {
                MLessLabel = lessLabel;
                return this;
            }

            public Builder MoreLabelColor(Color moreLabelColor)
            {
                MMoreLabelColor = moreLabelColor;
                return this;
            }

            public Builder LessLabelColor(Color lessLabelColor)
            {
                MLessLabelColor = lessLabelColor;
                return this;
            }

            public Builder LabelUnderLine(bool labelUnderLine)
            {
                MLabelUnderLine = labelUnderLine;
                return this;
            }

            /// <summary> 
            /// expandAnimation either true to enable animation on expand or false to disable animation - default is false 
            /// </summary>
            /// <param name="expandAnimation"></param>
            /// <returns>Builder</returns>
            public Builder ExpandAnimation(bool expandAnimation)
            {
                MExpandAnimation = expandAnimation;
                return this;
            }

            public StReadMoreOption Build()
            {
                return new StReadMoreOption(this);
            }

        }
    }
}
