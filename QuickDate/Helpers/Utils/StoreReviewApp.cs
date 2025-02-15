using Android.Content;
using Android.OS;
using Com.Google.Android.Play.Core.Review;
using Com.Google.Android.Play.Core.Tasks;
using Java.Lang;
using Exception = System.Exception;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.Utils
{
    public class StoreReviewApp : Java.Lang.Object, IOnCompleteListener, IOnFailureListener
    {
        private Intent GetRateIntent(string url)
        {
            try
            {
                var intent = new Intent(Intent.ActionView, Uri.Parse(url));

                intent.AddFlags(ActivityFlags.NoHistory);
                intent.AddFlags(ActivityFlags.MultipleTask);
                if ((int)Build.VERSION.SdkInt >= 21)
                {
                    intent.AddFlags(ActivityFlags.NewDocument);
                }
                else
                {
                    intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                }
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                return intent;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        /// <summary>
        /// Opens the store review page.
        /// </summary>
        /// <param name="appId">App identifier.</param>
        public void OpenStoreReviewPage(Context context, string appId)
        {
            //try
            //{
            //    var manager = ReviewManagerFactory.Create(context);
            //    var request = manager.RequestReviewFlow();
            //    request.AddOnCompleteListener(this);
            //    request.AddOnFailureListener(this);
            //    return;
            //}
            //catch (Exception ex)
            //{
            //    //Unable to launch app store
            //    Methods.DisplayReportResultTrack(ex);
            //}

            //Unable to launch app store
            var url = $"market://details?id={appId}";
            try
            {
                var intent = GetRateIntent(url);
                context.StartActivity(intent);
                return;
            }
            catch (Exception ex)
            {
                //Unable to launch app store
                Methods.DisplayReportResultTrack(ex);
            }

            url = $"https://play.google.com/store/apps/details?id={appId}";
            try
            {
                var intent = GetRateIntent(url);
                context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                //Unable to launch app store:
                Methods.DisplayReportResultTrack(ex);
            }
        }

        /// <summary>
        /// The flow has finished. The API does not indicate whether the user
        /// reviewed or not, or even whether the review dialog was shown. Thus, no
        /// matter the result, we continue our app flow.
        /// </summary>
        /// <param name="task"></param>
        public void OnComplete(Task task)
        {
            try
            {
                if (task.IsSuccessful)
                {
                    // We can get the ReviewInfo object
                    var reviewInfo = task.GetResult(Class.FromType(typeof(ReviewInfo)));
                }
                else
                {
                    // There was some problem, log or handle the error code.
                    var reviewErrorCode = task.Exception;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void OnFailure(Java.Lang.Exception p0)
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}