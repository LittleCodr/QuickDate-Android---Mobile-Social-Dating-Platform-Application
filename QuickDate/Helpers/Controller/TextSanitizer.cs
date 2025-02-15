using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Com.Luseen.Autolinklibrary;
using QuickDate.Helpers.Utils;
using System;
using System.Linq;

namespace QuickDate.Helpers.Controller
{
    public class TextSanitizer
    {
        private readonly AutoLinkTextView AutoLinkTextView;
        private readonly AppCompatActivity Activity;

        public TextSanitizer(AutoLinkTextView linkTextView, AppCompatActivity activity)
        {
            try
            {
                AutoLinkTextView = linkTextView;
                Activity = activity;
                AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void Load(string autoLinkText, string position = "Left")
        {
            try
            {
                AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention, AutoLinkMode.ModeCustom);

                if (position == "Sent" || position == "sent")
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                }
                else
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeUrl_color));
                }
                var text = autoLinkText.Split('/');
                if (text.Count() > 1)
                {
                    AutoLinkTextView.SetCustomRegex(@"\b(" + text.LastOrDefault() + @")\b");
                }

                string lastString = autoLinkText.Replace(" /", " ");
                if (!string.IsNullOrEmpty(lastString))
                    AutoLinkTextView.SetAutoLinkText(Methods.FunString.DecodeString(lastString));
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void AutoLinkTextViewOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1.Replace(" ", ""));
                if (typetext == "Email" || autoLinkOnClickEventArgs.P0 == AutoLinkMode.ModeEmail)
                {
                    Methods.App.SendEmail(Activity, autoLinkOnClickEventArgs.P1.Replace(" ", ""));
                }
                else if (typetext == "Website" || autoLinkOnClickEventArgs.P0 == AutoLinkMode.ModeUrl)
                {
                    string url = autoLinkOnClickEventArgs.P1.Replace(" ", "");
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1.Replace(" ", "");
                    }

                    //var intent = new Intent(Activity, typeof(LocalWebViewActivity));
                    //intent.PutExtra("URL", url);
                    //intent.PutExtra("Type", url);
                    //Activity.StartActivity(intent);
                    new IntentController(Activity).OpenBrowserFromApp(url);
                }
                else if (typetext == "Hashtag" || autoLinkOnClickEventArgs.P0 == AutoLinkMode.ModeHashtag)
                {

                }
                else if (typetext == "Mention" || autoLinkOnClickEventArgs.P0 == AutoLinkMode.ModeMention)
                {

                }
                else if (typetext == "Number" || autoLinkOnClickEventArgs.P0 == AutoLinkMode.ModePhone)
                {
                    Methods.App.SaveContacts(Activity, autoLinkOnClickEventArgs.P1.Replace(" ", ""), "", "2");
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }
    }
}