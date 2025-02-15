using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Com.Airbnb.Lottie;
using QuickDate.Helpers.Fonts;
using System;

namespace QuickDate.Helpers.Utils
{
    public class EmptyStateInflater
    {
        public AppCompatButton EmptyStateButton;
        public TextView EmptyStateIcon;
        public TextView DescriptionText;
        public TextView TitleText;
        public LottieAnimationView LottieAnimationView;
        public ImageView EmptyImage;

        public enum Type
        {
            NoConnection,
            NoSearchResult,
            SomeThingWentWrong,
            NoUsers,
            NoMatches,
            NoNotifications,
            NoMessage,
            NoBlock,
            NoArticle,
            NoSessions,
            NoMedia,
            NoFriendsRequests,
            GetPremium,
            NoTransactions,
        }

        public void InflateLayout(View inflated, Type type)
        {
            try
            {

                EmptyStateIcon = (TextView)inflated.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)inflated.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)inflated.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = inflated.FindViewById<AppCompatButton>(Resource.Id.button);
                LottieAnimationView = inflated.FindViewById<LottieAnimationView>(Resource.Id.animation_view);
                EmptyImage = inflated.FindViewById<ImageView>(Resource.Id.iv_empty);

                switch (type)
                {
                    case Type.NoConnection:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.IosThunderstorm);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_TitleText);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_DescriptionText);
                        EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_Button);
                        break;
                    case Type.NoSearchResult:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Search);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_TitleText);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_DescriptionText);
                        EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_Button);
                        break;
                    case Type.SomeThingWentWrong:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Close);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_TitleText);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_DescriptionText);
                        EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_Button);
                        break;
                    case Type.NoMatches:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Pin);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMoreUsers);
                        DescriptionText.Text = " ";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoUsers:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Gone;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                        EmptyImage.Visibility = ViewStates.Visible;
                        EmptyImage.SetImageResource(Resource.Drawable.icon_no_user_vector);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMoreUsers);
                        DescriptionText.Text = " ";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoNotifications:
                        LottieAnimationView.Visibility = ViewStates.Visible;
                        EmptyStateIcon.Visibility = ViewStates.Gone;
                        //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Notifications);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoNotification_TitleText);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoNotification_DescriptionText);
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoMessage:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Chatboxes);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMessage_TitleText);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoMessage_DescriptionText) + " " + AppSettings.ApplicationName;
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoBlock:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoBlockUsers);
                        DescriptionText.Text = " ";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoArticle:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.FileAlt);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Article);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Article);
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoSessions:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.Fingerprint);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Sessions);
                        DescriptionText.Text = "";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoMedia:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.Images);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Media);
                        DescriptionText.Text = "";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoFriendsRequests:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.UserFriends);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_FriendsRequests);
                        DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoFriendsRequests);
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.GetPremium:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_GetPremium);
                        DescriptionText.Text = "";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                    case Type.NoTransactions:
                        LottieAnimationView.Visibility = ViewStates.Gone;
                        EmptyStateIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Pulse);
                        EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                        TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Transactions);
                        DescriptionText.Text = "";
                        EmptyStateButton.Visibility = ViewStates.Gone;
                        break;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }

        }
    }
}