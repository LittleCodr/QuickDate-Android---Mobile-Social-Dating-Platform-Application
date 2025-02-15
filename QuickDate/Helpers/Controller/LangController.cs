using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Java.Util;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using System;
using System.Globalization;
using System.Threading;

namespace QuickDate.Helpers.Controller
{
    public class LangController : ContextWrapper
    {
        private Context Context;
        public static string Language = "";

        protected LangController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LangController(Context context) : base(context)
        {
            Context = context;
        }

        public static LangController SetApplicationLang(Context context, string language)
        {
            try
            {
                Locale newLocale = new Locale(language);

                Locale locale = newLocale;
                Locale.Default = locale;

                Resources res = context.Resources;
                Configuration configuration = res.Configuration;

                switch (Build.VERSION.SdkInt)
                {
                    case >= BuildVersionCodes.N:
                        {
                            configuration.SetLocale(newLocale);

                            LocaleList localeList = new LocaleList(newLocale);
                            LocaleList.Default = localeList;
                            Locale.SetDefault(Locale.Category.Display, newLocale);
                            configuration.Locales = localeList;
                            configuration.SetLayoutDirection(newLocale);

                            CultureInfo myCulture = new CultureInfo(language);
                            CultureInfo.DefaultThreadCurrentCulture = myCulture;

                            context = context.CreateConfigurationContext(configuration);
                            break;
                        }
                    case >= BuildVersionCodes.JellyBeanMr1:
                        {
#pragma warning disable CS0618 // Type or member is obsolete
                            configuration.Locale = newLocale;
#pragma warning restore CS0618 // Type or member is obsolete

                            configuration.SetLocale(newLocale);

                            CultureInfo myCulture = new CultureInfo(language);
                            CultureInfo.DefaultThreadCurrentCulture = myCulture;

                            context = context.CreateConfigurationContext(configuration);
                            break;
                        }
                    default:
#pragma warning disable 618
                        configuration.Locale = newLocale;
#pragma warning restore 618
                        break;
                }

#pragma warning disable 618
                res.UpdateConfiguration(configuration, res.DisplayMetrics);
#pragma warning restore 618

                UserDetails.LangName = language;
                AppSettings.Lang = language;
                AppSettings.FlowDirectionRightToLeft = language.Contains("ar");
                MainSettings.SharedData.Edit()?.PutString("Lang_key", AppSettings.Lang)?.Commit();

                SetCulture(language);

                return new LangController(context);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception);
                return new LangController(context);
            }
        }

        private static void SetCulture(string language)
        {
            try
            {
                CultureInfo myCulture = new CultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = myCulture;
                Thread.CurrentThread.CurrentCulture = myCulture;
                Thread.CurrentThread.CurrentUICulture = myCulture;

                new ChineseLunisolarCalendar();
                new HebrewCalendar();
                new HijriCalendar();
                new JapaneseCalendar();
                new JapaneseLunisolarCalendar();
                new KoreanCalendar();
                new KoreanLunisolarCalendar();
                new PersianCalendar();
                new TaiwanCalendar();
                new TaiwanLunisolarCalendar();
                new ThaiBuddhistCalendar();
                new UmAlQuraCalendar();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }
    }
}