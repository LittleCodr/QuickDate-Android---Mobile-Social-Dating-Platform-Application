using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using System;
using System.Linq;

namespace QuickDate.Helpers.Controller
{
    public class DialogController
    {
        private Dialog AlertDialogMatchFound, AlertDialogSkipTutorial, AlertDialogFinishTutorial, AlertDialogYouArePremium, AlertDialogGoToPremium;
        private readonly FragmentActivity Activity;
        public static UserInfoObject DataUser;
        private const int TutorialDialogBackgroundAlpha = 150;

        public DialogController(FragmentActivity activity)
        {
            Activity = activity;
        }

        //============================ DialogMatch ================================

        #region Dialog >> Match

        public void OpenDialogMatchFound(UserInfoObject dataUser)
        {
            try
            {
                DataUser = dataUser;

                AlertDialogMatchFound = new Dialog(Activity, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AlertDialogMatchFound.SetContentView(Resource.Layout.DialogMatchFound);

                SetTransparentBackground(AlertDialogMatchFound.Window);
                AlertDialogMatchFound.Window?.SetGravity(GravityFlags.Center);

                var image1 = AlertDialogMatchFound.FindViewById<ImageView>(Resource.Id.Iconimage);
                var image2 = AlertDialogMatchFound.FindViewById<ImageView>(Resource.Id.Iconimage2);

                GlideImageLoader.LoadImage(Activity, dataUser.Avater, image1, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(Activity, UserDetails.Avatar, image2, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                var subTitle = AlertDialogMatchFound.FindViewById<TextView>(Resource.Id.secondarytextview);
                subTitle.Text = Activity.GetText(Resource.String.Lbl_YouAnd) + " " + QuickDateTools.GetNameFinal(dataUser) + " " + Activity.GetText(Resource.String.Lbl_SubTitle_Match);

                Button btnSkipMatch = AlertDialogMatchFound.FindViewById<AppCompatButton>(Resource.Id.skippButton);
                Button btnNextMatch = AlertDialogMatchFound.FindViewById<AppCompatButton>(Resource.Id.NextButton);

                btnSkipMatch.Click += BtnSkipMatchOnClick;
                btnNextMatch.Click += BtnNextMatchOnClick;

                AlertDialogMatchFound.Show();

                //got_new_match
                var data = ListUtils.MatchList.FirstOrDefault(a => a.Notifier.Id == dataUser.Id);
                if (data == null)
                {
                    ListUtils.MatchList.Add(new GetNotificationsObject.Datum
                    {
                        Notifier = dataUser,
                        Type = "got_new_match"
                    });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void BtnNextMatchOnClick(object sender, EventArgs e)
        {
            try
            {
                HomeActivity.GetInstance().ShowMessagesBox(DataUser);

                AlertDialogMatchFound.Hide();
                AlertDialogMatchFound.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnSkipMatchOnClick(object sender, EventArgs e)
        {
            try
            {
                AlertDialogMatchFound.Hide();
                AlertDialogMatchFound.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //============================ Premium ================================ 

        #region Dialog >> Go To Premium

        public void OpenDialogGetToPremium()
        {
            try
            {
                AlertDialogGoToPremium = new Dialog(Activity, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AlertDialogGoToPremium.SetContentView(Resource.Layout.DialogGoToPremium);

                SetTransparentBackground(AlertDialogGoToPremium.Window);
                AlertDialogGoToPremium.Window?.SetGravity(GravityFlags.Center);

                var btnUpgrade = AlertDialogGoToPremium.FindViewById<AppCompatButton>(Resource.Id.upgradeButton);
                btnUpgrade.Click += BtnUpgrade_Click;

                AlertDialogGoToPremium.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnUpgrade_Click(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayPremiumWindow();

                AlertDialogGoToPremium.Hide();
                AlertDialogGoToPremium.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //============================ You Are Premium ================================  

        #region Dialog >> You Are Premium

        private AppCompatButton BtnUpgrade;
        public void OpenDialogYouArePremium(string time, string proType)
        {
            try
            {
                AlertDialogYouArePremium = new Dialog(Activity, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AlertDialogYouArePremium.SetContentView(Resource.Layout.DialogYouArePremium);

                SetTransparentBackground(AlertDialogYouArePremium.Window);
                AlertDialogYouArePremium.Window?.SetGravity(GravityFlags.Center);

                var txtSec = AlertDialogYouArePremium.FindViewById<TextView>(Resource.Id.secondarytextview);

                DateTime startDate = Methods.Time.UnixTimeStampToDateTime(int.Parse(time));

                DateTime endDate = startDate;
                switch (proType)
                {
                    case "1"://Weekly
                        endDate = startDate.AddDays(+7);
                        break;
                    case "2"://Monthly
                        endDate = startDate.AddMonths(+1);
                        break;
                    case "3"://Yearly
                        endDate = startDate.AddYears(+1);
                        break;
                }

                BtnUpgrade = AlertDialogYouArePremium.FindViewById<AppCompatButton>(Resource.Id.skippButton);
                BtnUpgrade.Click += BtnUpgradeOnClick;

                if (proType == "4")
                {
                    txtSec.Text = Activity.GetText(Resource.String.Lbl_Lifetime);

                    BtnUpgrade.Text = Activity.GetText(Resource.String.Lbl_Skip);
                    BtnUpgrade.Tag = "skip";
                }
                else
                {
                    var dateTime = (endDate - startDate).TotalDays;
                    txtSec.Text = Activity.GetText(Resource.String.Lbl_ExpirationDate) + " : " + dateTime + " " + Activity.GetText(Resource.String.Lbl_days);

                    BtnUpgrade.Text = Activity.GetText(Resource.String.Lbl_Renew);
                    BtnUpgrade.Tag = "Renew";
                }

                AlertDialogYouArePremium.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnUpgradeOnClick(object sender, EventArgs e)
        {
            try
            {
                if (BtnUpgrade?.Tag?.ToString() == "Renew")
                {
                    var window = new PopupController(Activity);
                    window.DisplayPremiumWindow();
                }

                AlertDialogYouArePremium.Hide();
                AlertDialogYouArePremium.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        //============================ Tutorial ================================   

        #region Dialog >> Tutorial

        public void OpenDialogSkipTutorial()
        {
            try
            {
                AlertDialogSkipTutorial = new Dialog(Activity, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AlertDialogSkipTutorial.SetContentView(Resource.Layout.DialogSkipTutorial);

                SetTransparentBackground(AlertDialogSkipTutorial.Window);
                AlertDialogSkipTutorial.Window?.SetGravity(GravityFlags.Center);

                Button btnSkipTutorial = AlertDialogSkipTutorial.FindViewById<AppCompatButton>(Resource.Id.skippButton);
                Button btnNextTutorial = AlertDialogSkipTutorial.FindViewById<AppCompatButton>(Resource.Id.NextButton);

                btnSkipTutorial.Click += BtnSkipTutorialOnClick;
                btnNextTutorial.Click += BtnNextTutorialOnClick;

                AlertDialogSkipTutorial.Show();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void BtnSkipTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                SaveAppSettings();
                SetActive();

                AlertDialogSkipTutorial.Hide();
                AlertDialogSkipTutorial.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnNextTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                SaveAppSettings();
                SetActive();

                AlertDialogSkipTutorial.Hide();
                AlertDialogSkipTutorial.Dismiss();

                //Open Dialog Finish Tutorial
                AlertDialogFinishTutorial = new Dialog(Activity, QuickDateTools.IsTabDark() ? Resource.Style.MyTheme_Dark : Resource.Style.MyTheme);
                AlertDialogFinishTutorial.SetContentView(Resource.Layout.DialogFinishTutorial);

                var backgroundDrawable = new ColorDrawable(Color.Black);
                backgroundDrawable.SetAlpha(TutorialDialogBackgroundAlpha);
                AlertDialogFinishTutorial.Window?.SetBackgroundDrawable(backgroundDrawable);
                AlertDialogFinishTutorial.Window?.SetGravity(GravityFlags.Center);

                Button btnFinishTutorial = AlertDialogFinishTutorial.FindViewById<AppCompatButton>(Resource.Id.finishButton);

                btnFinishTutorial.Click += BtnFinishTutorialOnClick;

                AlertDialogFinishTutorial.Show();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void BtnFinishTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                AlertDialogFinishTutorial.Dismiss();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        private void SaveAppSettings()
        {
            try
            {
                MainSettings.StoreShowTutorialDialogValue(false);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
            }
        }

        #endregion

        /////////////////////////////////////////////////

        private void SetActive()
        {
            try
            {
                var data = ListUtils.DataUserLoginList.FirstOrDefault();
                if (data != null)
                {
                    if (data.Status != "Active")
                    {
                        data.Status = "Active";
                        UserDetails.Status = "Active";

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.InsertOrUpdateLogin_Credentials(data);

                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        private void SetTransparentBackground(Window window)
        {
            try
            {
                var backgroundDrawable = new ColorDrawable(Color.Black);
                backgroundDrawable.SetAlpha(TutorialDialogBackgroundAlpha);
                window.SetBackgroundDrawable(backgroundDrawable);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}