using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Java.Lang;
using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Services
{
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class AppApiService : JobService
    {
        private static AppApiService Instance;

        public AppApiService()
        {
            Instance = this;
        }

        public static AppApiService GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AppApiService();
            }
            return Instance;
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                base.OnStartCommand(intent, flags, startId);

                // Perform your background task here
                ThreadPool.RunOnUiThread(new AppUpdaterHelper(Application.Context));

                return StartCommandResult.Sticky;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return StartCommandResult.NotSticky;
            }
        }

        public override bool OnStartJob(JobParameters jobParams)
        {
            //Toast.MakeText(Application.Context, "On Start Job " + Methods.AppLifecycleObserver.AppState, ToastLength.Short)?.Show();

            // Perform your background task here
            ThreadPool.RunOnUiThread(new AppUpdaterHelper(Application.Context));

            // Our task will run in background, we will take care of notifying the finish.
            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            //Toast.MakeText(Application.Context, "On Stop Job 321" + Methods.AppLifecycleObserver.AppState, ToastLength.Short)?.Show();
            // I want it to reschedule so returned true, if we would have returned false, then job would have ended here.
            // It would not fire onStartJob() when constraints are re satisfied.

            return true;
        }

        public void StartService(Context context)
        {
            try
            {
                Intent serviceIntent = new Intent(context, typeof(AppApiService));
                serviceIntent.PutExtra("inputExtra", "Foreground Service");
                context.StartService(serviceIntent);

                ScheduleJob(context);
            }
            catch (ForegroundServiceStartNotAllowedException e)
            {
                Methods.DisplayReportResultTrack(e);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void ScheduleJob(Context context)
        {
            try
            {
                ComponentName serviceComponent = new ComponentName(context, Class.FromType(typeof(AppApiService)));

                //JobScheduler js = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
                //JobInfo.Builder builder = new JobInfo.Builder(0, serviceComponent);
                //builder.AddTriggerContentUri(new JobInfo.TriggerContentUri(MediaStore.Images.Media.ExternalContentUri, TriggerContentUriFlags.NotifyForDescendants));
                //js.Schedule(builder.Build());

                JobInfo jobInfo;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    JobInfo.Builder builder = new JobInfo.Builder(1021, serviceComponent);

                    // Set the job parameters
                    builder.SetRequiredNetworkType(NetworkType.Any);  // Requires network connectivity
                    //builder.SetRequiresCharging(false);  // Requires device to be charging
                    // builder.SetPeriodic(15 * 60 * 1000, 60 * 1000);  // Repeat every 15 minutes
                    builder.SetMinimumLatency(3600000);  //5 * 1000 Minimum latency of 5 minutes
                    //builder.SetRequiresDeviceIdle(false);  // the device should be idle

                    //builder.SetPersisted(true);  // Keep the job after a device reboot
                    jobInfo = builder?.Build();
                }
                else
                {
                    jobInfo = new JobInfo.Builder(1021, serviceComponent)?.SetPeriodic(3600000)?.Build();
                }

                var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
                if (jobInfo != null)
                {
                    var resultCode = jobScheduler?.Schedule(jobInfo);
                    if (resultCode == JobScheduler.ResultSuccess)
                    {

                        Console.WriteLine("MyJobService scheduled!");
                    }
                    else
                    {
                        Console.WriteLine("MyJobService not scheduled!");
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void StopJob(Context context)
        {
            try
            {
                var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
                jobScheduler?.CancelAll();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }

    public class AppUpdaterHelper : Java.Lang.Object, IRunnable
    {

        private static Context Context;

        public AppUpdaterHelper(Context context)
        {
            Context = context;
        }

        public async void Run()
        {
            try
            {
                if (string.IsNullOrEmpty(Methods.AppLifecycleObserver.AppState))
                    Methods.AppLifecycleObserver.AppState = "Background";

                //Toast.MakeText(Context, "Started", ToastLength.Short)?.Show(); 

                if (string.IsNullOrEmpty(InitializeQuickDate.WebsiteUrl))
                {
                    InitializeQuickDate.Initialize(AppSettings.TripleDesAppServiceProvider, Context.PackageName, AppSettings.TurnTrustFailureOnWebException, new MyReportModeApp());
                    var sqLiteDatabase = new SqLiteDatabase();
                    sqLiteDatabase.CheckTablesStatus();
                }

                if (Methods.AppLifecycleObserver.AppState == "Background" || string.IsNullOrEmpty(Current.AccessToken))
                {
                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    var login = dbDatabase.Get_data_Login_Credentials();
                    dbDatabase.GetSettings();
                    Console.WriteLine(login);
                }

                if (string.IsNullOrEmpty(Current.AccessToken))
                    return;

                if (Methods.CheckConnectivity())
                {
                    var count = await ApiRequest.GetCountNotifications(Context);

                    if (Methods.AppLifecycleObserver.AppState == "Foreground")
                    {
                        var instance = HomeActivity.GetInstance();
                        instance?.GetNotifications(count.Item1, count.Item2);
                    }
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadChatAsync }, 0);
                }

                ThreadPool.RunOnUiThread(this);
            }
            catch (Exception e)
            {
                //ToastUtils.ShowToast(Context, "ResultSender failed",ToastLength.Short);
                ThreadPool.RunOnUiThread(this);
                Methods.DisplayReportResultTrack(e);
            }
        }

        public static async Task LoadChatAsync()
        {
            try
            {
                if (LastChatFragment.ApiRun)
                    return;

                //ToastUtils.ShowToast(Context, "LoadChatAsync", ToastLength.Short);

                LastChatFragment.ApiRun = true;

                var (apiStatus, respond) = await RequestsAsync.Chat.GetConversationListAsync("10", "0");
                if (apiStatus != 200 || respond is not GetConversationListObject result || result.Data == null)
                {
                    LastChatFragment.ApiRun = false;
                    //Methods.DisplayReportResult(new Activity(), respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        if (Methods.AppLifecycleObserver.AppState == "Foreground")
                        {
                            var instance = LastChatFragment.GetInstance();
                            if (instance != null)
                            {
                                HomeActivity.GetInstance()?.OnReceiveResult(JsonConvert.SerializeObject(result)); 
                            }
                            else
                            {
                                LastChatFragment.ApiRun = false;

                                //Insert All data users to database
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertOrReplaceLastChatTable(new ObservableCollection<GetConversationListObject.DataConversation>(result.Data));
                            }
                        }
                        else
                        {
                            LastChatFragment.ApiRun = false;

                            //Insert All data users to database
                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.InsertOrReplaceLastChatTable(new ObservableCollection<GetConversationListObject.DataConversation>(result.Data));
                        }
                    }
                    else
                    {
                        LastChatFragment.ApiRun = false;
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                LastChatFragment.ApiRun = false;
            }
        }
    }

    public abstract class ThreadPool
    {
        private static Handler SUiThreadHandler;

        private ThreadPool()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="runnable"></param>
        public static void RunOnUiThread(AppUpdaterHelper runnable)
        {
            try
            {
                SUiThreadHandler ??= new Handler(Looper.MainLooper);
                SUiThreadHandler.PostDelayed(runnable, 3500);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}