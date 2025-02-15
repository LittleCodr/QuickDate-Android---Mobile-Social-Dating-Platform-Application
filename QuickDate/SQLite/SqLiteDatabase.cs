using Newtonsoft.Json;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace QuickDate.SQLite
{
    public class SqLiteDatabase
    {
        //############# DON'T MODIFY HERE #############
        private static readonly string Folder = AppDomain.CurrentDomain.BaseDirectory; // Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string PathCombine = Path.Combine(Folder, AppSettings.DatabaseName + "_.db");

        //Open Connection in Database
        //*********************************************************

        #region Connection

        private SQLiteConnection OpenConnection()
        {
            try
            {
                var connection = new SQLiteConnection(new SQLiteConnectionString(PathCombine, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex, true));
                return connection;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public void CheckTablesStatus()
        {
            try
            {
                using var connection = OpenConnection();
                connection?.CreateTable<DataTables.LoginTb>();
                connection?.CreateTable<DataTables.SettingsTb>();
                connection?.CreateTable<DataTables.InfoUsersTb>();
                connection?.CreateTable<DataTables.GiftsTb>();
                connection?.CreateTable<DataTables.StickersTb>();
                connection?.CreateTable<DataTables.LastChatTb>();
                connection?.CreateTable<DataTables.MessageTb>();
                connection?.CreateTable<DataTables.FilterOptionsTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    CheckTablesStatus();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Delete table
        public void DropAll()
        {
            try
            {
                using var connection = OpenConnection();
                connection?.DropTable<DataTables.LoginTb>();
                connection?.DropTable<DataTables.SettingsTb>();
                connection?.DropTable<DataTables.InfoUsersTb>();
                connection?.DropTable<DataTables.GiftsTb>();
                connection?.DropTable<DataTables.StickersTb>();
                connection?.DropTable<DataTables.LastChatTb>();
                connection?.DropTable<DataTables.MessageTb>();
                connection?.DropTable<DataTables.FilterOptionsTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DropAll();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion Connection

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                connection.Insert(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                connection.Update(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using var connection = OpenConnection();
                connection.Delete(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using var connection = OpenConnection();
                connection.InsertAll(row);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion General

        //Start SQL_Commander >>  Custom
        //*********************************************************

        #region Login

        //Get data Login
        public DataTables.LoginTb Get_data_Login_Credentials()
        {
            try
            {
                using var connection = OpenConnection();
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    UserDetails.Username = dataUser.Username;
                    UserDetails.FullName = dataUser.Username;
                    UserDetails.Password = dataUser.Password;
                    UserDetails.AccessToken = dataUser.AccessToken;
                    UserDetails.UserId = Convert.ToInt32(dataUser.UserId);
                    UserDetails.Status = dataUser.Status;
                    UserDetails.Cookie = dataUser.Cookie;
                    UserDetails.Email = dataUser.Email;
                    UserDetails.DeviceId = dataUser.DeviceId;
                    AppSettings.Lang = dataUser.Lang;
                    Current.AccessToken = dataUser.AccessToken;

                    ListUtils.DataUserLoginList.Clear();
                    ListUtils.DataUserLoginList.Add(dataUser);

                    return dataUser;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_data_Login_Credentials();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
        }

        //Insert Or Update data Login
        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using var connection = OpenConnection();
                var dataUser = connection.Table<DataTables.LoginTb>().FirstOrDefault();
                if (dataUser != null)
                {
                    dataUser.UserId = UserDetails.UserId.ToString();
                    dataUser.AccessToken = UserDetails.AccessToken;
                    dataUser.Cookie = UserDetails.Cookie;
                    dataUser.Username = UserDetails.Username;
                    dataUser.Password = UserDetails.Password;
                    dataUser.Status = UserDetails.Status;
                    dataUser.Lang = AppSettings.Lang;
                    dataUser.DeviceId = UserDetails.DeviceId;
                    dataUser.Email = UserDetails.Email;

                    connection.Update(dataUser);
                }
                else
                {
                    connection.Insert(db);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateLogin_Credentials(db);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Filter Options 


        //Get data Filter Options
        public DataTables.FilterOptionsTb Get_data_Filter_Options()
        {
            try
            {
                using var connection = OpenConnection();
                var dataFilterOptions = connection.Table<DataTables.FilterOptionsTb>().FirstOrDefault();
                if (dataFilterOptions != null)
                {
                    UserDetails.AgeMin = dataFilterOptions.AgeMin;
                    UserDetails.AgeMax = dataFilterOptions.AgeMax;
                    UserDetails.Gender = dataFilterOptions.Gender;
                    UserDetails.Location = dataFilterOptions.Location;
                    UserDetails.SwitchState = dataFilterOptions.IsOnline;
                    UserDetails.Located = dataFilterOptions.Distance;
                    UserDetails.Language = dataFilterOptions.Language;
                    UserDetails.Ethnicity = dataFilterOptions.Ethnicity;
                    UserDetails.Religion = dataFilterOptions.Religion;
                    UserDetails.RelationShip = dataFilterOptions.RelationShip;
                    UserDetails.Smoke = dataFilterOptions.Smoke;
                    UserDetails.Drink = dataFilterOptions.Drink;
                    UserDetails.Body = dataFilterOptions.Body;
                    UserDetails.FromHeight = dataFilterOptions.FromHeight;
                    UserDetails.ToHeight = dataFilterOptions.ToHeight;
                    UserDetails.Interest = dataFilterOptions.Interest;
                    UserDetails.Education = dataFilterOptions.Education;
                    UserDetails.Pets = dataFilterOptions.Pets;

                    return dataFilterOptions;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return Get_data_Filter_Options();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
        }

        //Insert Or Update data filter options
        public void InsertOrUpdateFilter_Options(DataTables.FilterOptionsTb db)
        {
            try
            {
                using var connection = OpenConnection();
                var dataFilterOptions = connection.Table<DataTables.FilterOptionsTb>().FirstOrDefault();
                if (dataFilterOptions != null)
                {
                    dataFilterOptions.AgeMin = UserDetails.AgeMin;
                    dataFilterOptions.AgeMax = UserDetails.AgeMax;
                    dataFilterOptions.Gender = UserDetails.Gender;
                    dataFilterOptions.Location = UserDetails.Location;
                    dataFilterOptions.IsOnline = UserDetails.SwitchState;
                    dataFilterOptions.Distance = UserDetails.Located;
                    dataFilterOptions.Language = UserDetails.Language;
                    dataFilterOptions.Ethnicity = UserDetails.Ethnicity;
                    dataFilterOptions.Religion = UserDetails.Religion;
                    dataFilterOptions.RelationShip = UserDetails.RelationShip;
                    dataFilterOptions.Smoke = UserDetails.Smoke;
                    dataFilterOptions.Drink = UserDetails.Drink;
                    dataFilterOptions.Body = UserDetails.Body;
                    dataFilterOptions.FromHeight = UserDetails.FromHeight;
                    dataFilterOptions.ToHeight = UserDetails.ToHeight;
                    dataFilterOptions.Interest = UserDetails.Interest;
                    dataFilterOptions.Education = UserDetails.Education;
                    dataFilterOptions.Pets = UserDetails.Pets;

                    connection.Update(dataFilterOptions);
                }
                else
                {
                    connection.Insert(db);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateFilter_Options(db);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Settings

        public void InsertOrUpdateSettings(GetOptionsObject.DataOptions settingsData)
        {
            try
            {
                using var connection = OpenConnection();
                if (settingsData != null)
                {
                    var select = connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                    if (select == null)
                    {
                        var db = new DataTables.SettingsTb
                        {
                            LoadConfigInSession = settingsData.LoadConfigInSession,
                            MetaDescription = settingsData.MetaDescription,
                            MetaKeywords = settingsData.MetaKeywords,
                            DefaultTitle = settingsData.DefaultTitle,
                            SiteName = settingsData.SiteName,
                            DefaultLanguage = settingsData.DefaultLanguage,
                            SmtpOrMail = settingsData.SmtpOrMail,
                            SmtpHost = settingsData.SmtpHost,
                            SmtpUsername = settingsData.SmtpUsername,
                            SmtpPassword = settingsData.SmtpPassword,
                            SmtpEncryption = settingsData.SmtpEncryption,
                            SmtpPort = settingsData.SmtpPort,
                            SiteEmail = settingsData.SiteEmail,
                            Theme = settingsData.Theme,
                            AllLogin = settingsData.AllLogin,
                            GoogleLogin = settingsData.GoogleLogin,
                            FacebookLogin = settingsData.FacebookLogin,
                            TwitterLogin = settingsData.TwitterLogin,
                            LinkedinLogin = settingsData.LinkedinLogin,
                            VkontakteLogin = settingsData.VkontakteLogin,
                            FacebookAppId = settingsData.FacebookAppId,
                            FacebookAppKey = settingsData.FacebookAppKey,
                            GoogleAppId = settingsData.GoogleAppId,
                            GoogleAppKey = settingsData.GoogleAppKey,
                            TwitterAppId = settingsData.TwitterAppId,
                            TwitterAppKey = settingsData.TwitterAppKey,
                            LinkedinAppId = settingsData.LinkedinAppId,
                            LinkedinAppKey = settingsData.LinkedinAppKey,
                            VkontakteAppId = settingsData.VkontakteAppId,
                            VkontakteAppKey = settingsData.VkontakteAppKey,
                            InstagramAppId = settingsData.InstagramAppId,
                            InstagramAppkey = settingsData.InstagramAppkey,
                            InstagramLogin = settingsData.InstagramLogin,
                            SmsOrEmail = settingsData.SmsOrEmail,
                            SmsPhoneNumber = settingsData.SmsPhoneNumber,
                            PaypalId = settingsData.PaypalId,
                            PaypalSecret = settingsData.PaypalSecret,
                            PaypalMode = settingsData.PaypalMode,
                            Currency = settingsData.Currency,
                            LastBackup = settingsData.LastBackup,
                            AmazoneS3 = settingsData.AmazoneS3,
                            BucketName = settingsData.BucketName,
                            AmazoneS3Key = settingsData.AmazoneS3Key,
                            AmazoneS3SKey = settingsData.AmazoneS3SKey,
                            Region = settingsData.Region,
                            SmsTPhoneNumber = settingsData.SmsTPhoneNumber,
                            SmsTwilioUsername = settingsData.SmsTwilioUsername,
                            SmsTwilioPassword = settingsData.SmsTwilioPassword,
                            SmsProvider = settingsData.SmsProvider,
                            ProfilePictureWidthCrop = settingsData.ProfilePictureWidthCrop,
                            ProfilePictureHeightCrop = settingsData.ProfilePictureHeightCrop,
                            UserDefaultAvatar = settingsData.UserDefaultAvatar,
                            ProfilePictureImageQuality = settingsData.ProfilePictureImageQuality,
                            EmailValidation = settingsData.EmailValidation,
                            StripeSecret = settingsData.StripeSecret,
                            StripeId = settingsData.StripeId,
                            PushId = settingsData.PushId,
                            PushKey = settingsData.PushKey,
                            PushId2 = settingsData.PushId2,
                            PushKey2 = settingsData.PushKey2,
                            Terms = settingsData.Terms,
                            About = settingsData.About,
                            PrivacyPolicy = settingsData.PrivacyPolicy,
                            FacebookUrl = settingsData.FacebookUrl,
                            TwitterUrl = settingsData.TwitterUrl,
                            GoogleUrl = settingsData.GoogleUrl,
                            CurrencySymbol = settingsData.CurrencySymbol,
                            BagOfCreditsPrice = settingsData.BagOfCreditsPrice,
                            BagOfCreditsAmount = settingsData.BagOfCreditsAmount,
                            BoxOfCreditsPrice = settingsData.BoxOfCreditsPrice,
                            BoxOfCreditsAmount = settingsData.BoxOfCreditsAmount,
                            ChestOfCreditsPrice = settingsData.ChestOfCreditsPrice,
                            ChestOfCreditsAmount = settingsData.ChestOfCreditsAmount,
                            WeeklyProPlan = settingsData.WeeklyProPlan,
                            MonthlyProPlan = settingsData.MonthlyProPlan,
                            YearlyProPlan = settingsData.YearlyProPlan,
                            LifetimeProPlan = settingsData.LifetimeProPlan,
                            WorkerUpdateDelay = settingsData.WorkerUpdateDelay,
                            ProfileRecordViewsMinute = settingsData.ProfileRecordViewsMinute,
                            CostPerGift = settingsData.CostPerGift,
                            DeleteAccount = settingsData.DeleteAccount,
                            UserRegistration = settingsData.UserRegistration,
                            MaxUpload = settingsData.MaxUpload,
                            MimeTypes = settingsData.MimeTypes,
                            NormalBoostMeCreditsPrice = settingsData.NormalBoostMeCreditsPrice,
                            MoreStickersCreditsPrice = settingsData.MoreStickersCreditsPrice,
                            ProBoostMeCreditsPrice = settingsData.ProBoostMeCreditsPrice,
                            BoostExpireTime = settingsData.BoostExpireTime,
                            NotProChatLimitDaily = settingsData.NotProChatLimitDaily,
                            NotProChatCredit = settingsData.NotProChatCredit,
                            NotProChatStickersCredit = settingsData.NotProChatStickersCredit,
                            NotProChatStickersLimit = settingsData.NotProChatStickersLimit,
                            CostPerXvisits = settingsData.CostPerXvisits,
                            XvisitsExpireTime = settingsData.XvisitsExpireTime,
                            CostPerXmatche = settingsData.CostPerXmatche,
                            XmatcheExpireTime = settingsData.XmatcheExpireTime,
                            CostPerXlike = settingsData.CostPerXlike,
                            XlikeExpireTime = settingsData.XlikeExpireTime,
                            GooglePlaceApi = settingsData.GooglePlaceApi,
                            WowonderLogin = settingsData.WowonderLogin,
                            WowonderAppId = settingsData.WowonderAppId,
                            WowonderAppKey = settingsData.WowonderAppKey,
                            WowonderDomainUri = settingsData.WowonderDomainUri,
                            WowonderDomainIcon = settingsData.WowonderDomainIcon,
                            BankTransferNote = settingsData.BankTransferNote,
                            MaxSwaps = settingsData.MaxSwaps,
                            StripeVersion = settingsData.StripeVersion,
                            PayseraProjectId = settingsData.PayseraProjectId,
                            PayseraPassword = settingsData.PayseraPassword,
                            PayseraTestMode = settingsData.PayseraTestMode,
                            MessageRequestSystem = settingsData.MessageRequestSystem,
                            VideoChat = settingsData.VideoChat,
                            AudioChat = settingsData.AudioChat,
                            VideoAccountSid = settingsData.VideoAccountSid,
                            VideoApiKeySid = settingsData.VideoApiKeySid,
                            VideoApiKeySecret = settingsData.VideoApiKeySecret,
                            GiphyApi = settingsData.GiphyApi,
                            DefaultUnit = settingsData.DefaultUnit,
                            MaintenanceMode = settingsData.MaintenanceMode,
                            Displaymode = settingsData.Displaymode,
                            BankDescription = settingsData.BankDescription,
                            Version = settingsData.Version,
                            GoogleTagCode = settingsData.GoogleTagCode,
                            AvcallPro = settingsData.AvcallPro,
                            ProSystem = settingsData.ProSystem,
                            ImgBlurAmount = settingsData.ImgBlurAmount,
                            EmailNotification = settingsData.EmailNotification,
                            ActivationLimitSystem = settingsData.ActivationLimitSystem,
                            MaxActivationRequest = settingsData.MaxActivationRequest,
                            ActivationRequestTimeLimit = settingsData.ActivationRequestTimeLimit,
                            FreeFeatures = settingsData.FreeFeatures,
                            OppositeGender = settingsData.OppositeGender,
                            ImageVerification = settingsData.ImageVerification,
                            PendingVerification = settingsData.PendingVerification,
                            Push = settingsData.Push,
                            SpamWarning = settingsData.SpamWarning,
                            ImageVerificationStart = settingsData.ImageVerificationStart,
                            TwoFactor = settingsData.TwoFactor,
                            TwoFactorType = settingsData.TwoFactorType,
                            AffiliateSystem = settingsData.AffiliateSystem,
                            AffiliateType = settingsData.AffiliateType,
                            MWithdrawal = settingsData.MWithdrawal,
                            AmountRef = settingsData.AmountRef,
                            AmountPercentRef = settingsData.AmountPercentRef,
                            ConnectivitySystem = settingsData.ConnectivitySystem,
                            ConnectivitySystemLimit = settingsData.ConnectivitySystemLimit,
                            ShowUserOnHomepage = settingsData.ShowUserOnHomepage,
                            ShowedUser = settingsData.ShowedUser,
                            MaxPhotoPerUser = settingsData.MaxPhotoPerUser,
                            ReviewMediaFiles = settingsData.ReviewMediaFiles,
                            FfmpegSys = settingsData.FfmpegSys,
                            MaxVideoDuration = settingsData.MaxVideoDuration,
                            FfmpegBinary = settingsData.FfmpegBinary,
                            DisablePhoneField = settingsData.DisablePhoneField,
                            SocialMediaLinks = settingsData.SocialMediaLinks,
                            YtApi = settingsData.YtApi,
                            Seo = settingsData.Seo,
                            LockPrivatePhoto = settingsData.LockPrivatePhoto,
                            LockPrivatePhotoFee = settingsData.LockPrivatePhotoFee,
                            LockProVideo = settingsData.LockProVideo,
                            LockProVideoFee = settingsData.LockProVideoFee,
                            VerificationOnSignup = settingsData.VerificationOnSignup,
                            CreditEarnSystem = settingsData.CreditEarnSystem,
                            CreditEarnMaxDays = settingsData.CreditEarnMaxDays,
                            CreditEarnDayAmount = settingsData.CreditEarnDayAmount,
                            SpecificEmailSignup = settingsData.SpecificEmailSignup,
                            Push1 = settingsData.Push1,
                            CheckoutPayment = settingsData.CheckoutPayment,
                            CheckoutMode = settingsData.CheckoutMode,
                            CheckoutCurrency = settingsData.CheckoutCurrency,
                            CheckoutSellerId = settingsData.CheckoutSellerId,
                            CheckoutPublishableKey = settingsData.CheckoutPublishableKey,
                            CheckoutPrivateKey = settingsData.CheckoutPrivateKey,
                            CashfreePayment = settingsData.CashfreePayment,
                            CashfreeMode = settingsData.CashfreeMode,
                            CashfreeClientKey = settingsData.CashfreeClientKey,
                            CashfreeSecretKey = settingsData.CashfreeSecretKey,
                            IyzipayPayment = settingsData.IyzipayPayment,
                            IyzipayMode = settingsData.IyzipayMode,
                            IyzipayKey = settingsData.IyzipayKey,
                            IyzipayBuyerId = settingsData.IyzipayBuyerId,
                            IyzipaySecretKey = settingsData.IyzipaySecretKey,
                            IyzipayBuyerName = settingsData.IyzipayBuyerName,
                            IyzipayBuyerSurname = settingsData.IyzipayBuyerSurname,
                            IyzipayBuyerGsmNumber = settingsData.IyzipayBuyerGsmNumber,
                            IyzipayBuyerEmail = settingsData.IyzipayBuyerEmail,
                            IyzipayIdentityNumber = settingsData.IyzipayIdentityNumber,
                            IyzipayAddress = settingsData.IyzipayAddress,
                            IyzipayCity = settingsData.IyzipayCity,
                            IyzipayCountry = settingsData.IyzipayCountry,
                            IyzipayZip = settingsData.IyzipayZip,
                            GoogleMapApiKey = settingsData.GoogleMapApiKey,
                            PayuPayment = settingsData.PayuPayment,
                            PayuMode = settingsData.PayuMode,
                            PayuMerchantId = settingsData.PayuMerchantId,
                            PayuSecretKey = settingsData.PayuSecretKey,
                            PayuBuyerName = settingsData.PayuBuyerName,
                            PayuBuyerSurname = settingsData.PayuBuyerSurname,
                            PayuBuyerGsmNumber = settingsData.PayuBuyerGsmNumber,
                            PayuBuyerEmail = settingsData.PayuBuyerEmail,
                            PreventSystem = settingsData.PreventSystem,
                            BadLoginLimit = settingsData.BadLoginLimit,
                            LockTime = settingsData.LockTime,
                            PaystackPayment = settingsData.PaystackPayment,
                            PaystackSecretKey = settingsData.PaystackSecretKey,
                            TwilioChatCall = settingsData.TwilioChatCall,
                            AgoraChatCall = settingsData.AgoraChatCall,
                            AgoraChatAppId = settingsData.AgoraChatAppId,
                            AgoraChatAppCertificate = settingsData.AgoraChatAppCertificate,
                            AgoraChatCustomerId = settingsData.AgoraChatCustomerId,
                            AgoraChatCustomerSecret = settingsData.AgoraChatCustomerSecret,
                            LiveVideo = settingsData.LiveVideo,
                            LiveVideoSave = settingsData.LiveVideoSave,
                            AgoraLiveVideo = settingsData.AgoraLiveVideo,
                            AgoraAppId = settingsData.AgoraAppId,
                            AgoraAppCertificate = settingsData.AgoraAppCertificate,
                            AgoraCustomerId = settingsData.AgoraCustomerId,
                            AgoraCustomerCertificate = settingsData.AgoraCustomerCertificate,
                            AmazoneS32 = settingsData.AmazoneS32,
                            BucketName2 = settingsData.BucketName2,
                            AmazoneS3Key2 = settingsData.AmazoneS3Key2,
                            AmazoneS3SKey2 = settingsData.AmazoneS3SKey2,
                            Region2 = settingsData.Region2,
                            QqAppId = settingsData.QqAppId,
                            QqAppkey = settingsData.QqAppkey,
                            WeChatAppId = settingsData.WeChatAppId,
                            WeChatAppkey = settingsData.WeChatAppkey,
                            DiscordAppId = settingsData.DiscordAppId,
                            DiscordAppkey = settingsData.DiscordAppkey,
                            MailruAppId = settingsData.MailruAppId,
                            MailruAppkey = settingsData.MailruAppkey,
                            QqLogin = settingsData.QqLogin,
                            WeChatLogin = settingsData.WeChatLogin,
                            DiscordLogin = settingsData.DiscordLogin,
                            MailruLogin = settingsData.MailruLogin,
                            TwilioProvider = settingsData.TwilioProvider,
                            BulksmsProvider = settingsData.BulksmsProvider,
                            BulksmsUsername = settingsData.BulksmsUsername,
                            BulksmsPassword = settingsData.BulksmsPassword,
                            MessagebirdProvider = settingsData.MessagebirdProvider,
                            MessagebirdKey = settingsData.MessagebirdKey,
                            MessagebirdPhone = settingsData.MessagebirdPhone,
                            AuthorizePayment = settingsData.AuthorizePayment,
                            AuthorizeLoginId = settingsData.AuthorizeLoginId,
                            AuthorizeTransactionKey = settingsData.AuthorizeTransactionKey,
                            AuthorizeTestMode = settingsData.AuthorizeTestMode,
                            SecurionpayPayment = settingsData.SecurionpayPayment,
                            SecurionpayPublicKey = settingsData.SecurionpayPublicKey,
                            SecurionpaySecretKey = settingsData.SecurionpaySecretKey,
                            InviteLinksSystem = settingsData.InviteLinksSystem,
                            UserLinksLimit = settingsData.UserLinksLimit,
                            ExpireUserLinks = settingsData.ExpireUserLinks,
                            InfobipProvider = settingsData.InfobipProvider,
                            InfobipUsername = settingsData.InfobipUsername,
                            InfobipPassword = settingsData.InfobipPassword,
                            Msg91Provider = settingsData.Msg91Provider,
                            Msg91AuthKey = settingsData.Msg91AuthKey,
                            AutoUserLike = settingsData.AutoUserLike,
                            DevelopersPage = settingsData.DevelopersPage,
                            IsRtl = settingsData.IsRtl,
                            Uri = settingsData.Uri,
                            S3SiteUrl2 = settingsData.S3SiteUrl2,
                            AamarpayMode = settingsData.AamarpayMode,
                            AamarpayPayment = settingsData.AamarpayPayment,
                            AamarpaySignatureKey = settingsData.AamarpaySignatureKey,
                            AamarpayStoreId = settingsData.AamarpayStoreId,
                            BankPayment = settingsData.BankPayment,
                            CashfreeCurrency = settingsData.CashfreeCurrency,
                            CloudBucketName = settingsData.CloudBucketName,
                            CloudFile = settingsData.CloudFile,
                            CloudFilePath = settingsData.CloudFilePath,
                            CloudUpload = settingsData.CloudUpload,
                            CoinbaseKey = settingsData.CoinbaseKey,
                            CoinbasePayment = settingsData.CoinbasePayment,
                            Coinpayments = settingsData.Coinpayments,
                            CoinpaymentsCoin = settingsData.CoinpaymentsCoin,
                            CoinpaymentsCoins = settingsData.CoinpaymentsCoins,
                            CoinpaymentsId = settingsData.CoinpaymentsId,
                            CoinpaymentsPublicKey = settingsData.CoinpaymentsPublicKey,
                            CoinpaymentsSecret = settingsData.CoinpaymentsSecret,
                            CreditPrice = settingsData.CreditPrice,
                            FilterByCities = settingsData.FilterByCities,
                            FilterByCountry = settingsData.FilterByCountry,
                            FluttewavePayment = settingsData.FluttewavePayment,
                            FluttewaveSecretKey = settingsData.FluttewaveSecretKey,
                            FortumoPayment = settingsData.FortumoPayment,
                            FortumoServiceId = settingsData.FortumoServiceId,
                            FtpEndpoint = settingsData.FtpEndpoint,
                            FtpHost = settingsData.FtpHost,
                            FtpPassword = settingsData.FtpPassword,
                            FtpPath = settingsData.FtpPath,
                            FtpPort = settingsData.FtpPort,
                            FtpUpload = settingsData.FtpUpload,
                            FtpUsername = settingsData.FtpUsername,
                            GeoUsername = settingsData.GeoUsername,
                            InstagramImporter = settingsData.InstagramImporter,
                            InstagramImporterAppId = settingsData.InstagramImporterAppId,
                            InstagramImporterAppSecret = settingsData.InstagramImporterAppSecret,
                            IyzipayCurrency = settingsData.IyzipayCurrency,
                            NativeAndroidUrl = settingsData.NativeAndroidUrl,
                            NativeIosUrl = settingsData.NativeIosUrl,
                            NgeniusApiKey = settingsData.NgeniusApiKey,
                            NgeniusMode = settingsData.NgeniusMode,
                            NgeniusOutletId = settingsData.NgeniusOutletId,
                            NgeniusPayment = settingsData.NgeniusPayment,
                            OkAppId = settingsData.OkAppId,
                            OkAppPublickey = settingsData.OkAppPublickey,
                            OkAppSecretkey = settingsData.OkAppSecretkey,
                            OkLogin = settingsData.OkLogin,
                            PaypalCurrency = settingsData.PaypalCurrency,
                            PaypalPayment = settingsData.PaypalPayment,
                            PayseraPayment = settingsData.PayseraPayment,
                            PaystackCurrency = settingsData.PaystackCurrency,
                            RazorpayKeyId = settingsData.RazorpayKeyId,
                            RazorpayKeySecret = settingsData.RazorpayKeySecret,
                            RazorpayPayment = settingsData.RazorpayPayment,
                            Refund = settingsData.Refund,
                            SpaceName = settingsData.SpaceName,
                            SpaceRegion = settingsData.SpaceRegion,
                            Spaces = settingsData.Spaces,
                            SpacesKey = settingsData.SpacesKey,
                            SpacesSecret = settingsData.SpacesSecret,
                            StripeCurrency = settingsData.StripeCurrency,
                            StripePayment = settingsData.StripePayment,
                            SuccessStoriesSystem = settingsData.SuccessStoriesSystem,
                            WasabiAccessKey = settingsData.WasabiAccessKey,
                            WasabiBucketName = settingsData.WasabiBucketName,
                            WasabiBucketRegion = settingsData.WasabiBucketRegion,
                            WasabiSecretKey = settingsData.WasabiSecretKey,
                            WasabiStorage = settingsData.WasabiStorage,
                            WatermarkSystem = settingsData.WatermarkSystem,
                            YoomoneyNotificationsSecret = settingsData.YoomoneyNotificationsSecret,
                            YoomoneyPayment = settingsData.YoomoneyPayment,
                            YoomoneyWalletId = settingsData.YoomoneyWalletId,
                            AlipayPayment = settingsData.AlipayPayment,
                            AlipayServer = settingsData.AlipayServer,
                            Arabic = settingsData.Arabic,
                            BackblazeAccessKey = settingsData.BackblazeAccessKey,
                            BackblazeAccessKeyId = settingsData.BackblazeAccessKeyId,
                            BackblazeBucketId = settingsData.BackblazeBucketId,
                            BackblazeBucketName = settingsData.BackblazeBucketName,
                            BackblazeBucketRegion = settingsData.BackblazeBucketRegion,
                            BackblazeEndpoint = settingsData.BackblazeEndpoint,
                            BackblazeStorage = settingsData.BackblazeStorage,
                            BulksmsPhoneNumber = settingsData.BulksmsPhoneNumber,
                            CostAdmob = settingsData.CostAdmob,
                            CustomName = settingsData.CustomName,
                            DeveloperMode = settingsData.DeveloperMode,
                            Dutch = settingsData.Dutch,
                            English = settingsData.English,
                            //Exchange = settingsData.Exchange,
                            //ExchangeUpdate = settingsData.ExchangeUpdate,
                            FilesVersion = settingsData.FilesVersion,
                            French = settingsData.French,
                            German = settingsData.German,
                            InfobipPhoneNumber = settingsData.InfobipPhoneNumber,
                            IsDailyCredit = settingsData.IsDailyCredit,
                            Italian = settingsData.Italian,
                            Msg91DltId = settingsData.Msg91DltId,
                            Msg91PhoneNumber = settingsData.Msg91PhoneNumber,
                            PopUp18 = settingsData.PopUp18,
                            Portuguese = settingsData.Portuguese,
                            Recaptcha = settingsData.Recaptcha,
                            RecaptchaSecretKey = settingsData.RecaptchaSecretKey,
                            RecaptchaSiteKey = settingsData.RecaptchaSiteKey,
                            ReservedUsernames = settingsData.ReservedUsernames,
                            ReservedUsernamesSystem = settingsData.ReservedUsernamesSystem,
                            Russian = settingsData.Russian,
                            Spanish = settingsData.Spanish,
                            Time18 = settingsData.Time18,
                            Turkish = settingsData.Turkish,
                            //WithdrawalPaymentMethod = settingsData.WithdrawalPaymentMethod,

                            Height = JsonConvert.SerializeObject(settingsData.Height),
                            Notification = JsonConvert.SerializeObject(settingsData.Notification),
                            Gender = JsonConvert.SerializeObject(settingsData.Gender),
                            BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories),
                            Countries = JsonConvert.SerializeObject(settingsData.Countries),
                            HairColor = JsonConvert.SerializeObject(settingsData.HairColor),
                            Travel = JsonConvert.SerializeObject(settingsData.Travel),
                            Drink = JsonConvert.SerializeObject(settingsData.Drink),
                            Smoke = JsonConvert.SerializeObject(settingsData.Smoke),
                            Religion = JsonConvert.SerializeObject(settingsData.Religion),
                            Car = JsonConvert.SerializeObject(settingsData.Car),
                            LiveWith = JsonConvert.SerializeObject(settingsData.LiveWith),
                            Pets = JsonConvert.SerializeObject(settingsData.Pets),
                            Friends = JsonConvert.SerializeObject(settingsData.Friends),
                            Children = JsonConvert.SerializeObject(settingsData.Children),
                            Character = JsonConvert.SerializeObject(settingsData.Character),
                            Body = JsonConvert.SerializeObject(settingsData.Body),
                            Ethnicity = JsonConvert.SerializeObject(settingsData.Ethnicity),
                            Education = JsonConvert.SerializeObject(settingsData.Education),
                            WorkStatus = JsonConvert.SerializeObject(settingsData.WorkStatus),
                            Relationship = JsonConvert.SerializeObject(settingsData.Relationship),
                            Language = JsonConvert.SerializeObject(settingsData.Language),
                            CustomFields = JsonConvert.SerializeObject(settingsData.CustomFields),
                            CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray),
                            CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray),
                            CashfreeCurrencyArray = JsonConvert.SerializeObject(settingsData.CashfreeCurrencyArray),
                            IyzipayCurrencyArray = JsonConvert.SerializeObject(settingsData.IyzipayCurrencyArray),
                            ReservedUsernamesArray = JsonConvert.SerializeObject(settingsData.ReservedUsernamesArray),
                        };

                        connection.Insert(db);
                    }
                    else
                    {
                        if (select != null)
                        {
                            select.LoadConfigInSession = settingsData.LoadConfigInSession;
                            select.MetaDescription = settingsData.MetaDescription;
                            select.MetaKeywords = settingsData.MetaKeywords;
                            select.DefaultTitle = settingsData.DefaultTitle;
                            select.SiteName = settingsData.SiteName;
                            select.DefaultLanguage = settingsData.DefaultLanguage;
                            select.SmtpOrMail = settingsData.SmtpOrMail;
                            select.SmtpHost = settingsData.SmtpHost;
                            select.SmtpUsername = settingsData.SmtpUsername;
                            select.SmtpPassword = settingsData.SmtpPassword;
                            select.SmtpEncryption = settingsData.SmtpEncryption;
                            select.SmtpPort = settingsData.SmtpPort;
                            select.SiteEmail = settingsData.SiteEmail;
                            select.Theme = settingsData.Theme;
                            select.AllLogin = settingsData.AllLogin;
                            select.GoogleLogin = settingsData.GoogleLogin;
                            select.FacebookLogin = settingsData.FacebookLogin;
                            select.TwitterLogin = settingsData.TwitterLogin;
                            select.LinkedinLogin = settingsData.LinkedinLogin;
                            select.VkontakteLogin = settingsData.VkontakteLogin;
                            select.FacebookAppId = settingsData.FacebookAppId;
                            select.FacebookAppKey = settingsData.FacebookAppKey;
                            select.GoogleAppId = settingsData.GoogleAppId;
                            select.GoogleAppKey = settingsData.GoogleAppKey;
                            select.TwitterAppId = settingsData.TwitterAppId;
                            select.TwitterAppKey = settingsData.TwitterAppKey;
                            select.LinkedinAppId = settingsData.LinkedinAppId;
                            select.LinkedinAppKey = settingsData.LinkedinAppKey;
                            select.VkontakteAppId = settingsData.VkontakteAppId;
                            select.VkontakteAppKey = settingsData.VkontakteAppKey;
                            select.InstagramAppId = settingsData.InstagramAppId;
                            select.InstagramAppkey = settingsData.InstagramAppkey;
                            select.InstagramLogin = settingsData.InstagramLogin;
                            select.SmsOrEmail = settingsData.SmsOrEmail;
                            select.SmsPhoneNumber = settingsData.SmsPhoneNumber;
                            select.PaypalId = settingsData.PaypalId;
                            select.PaypalSecret = settingsData.PaypalSecret;
                            select.PaypalMode = settingsData.PaypalMode;
                            select.Currency = settingsData.Currency;
                            select.LastBackup = settingsData.LastBackup;
                            select.AmazoneS3 = settingsData.AmazoneS3;
                            select.BucketName = settingsData.BucketName;
                            select.AmazoneS3Key = settingsData.AmazoneS3Key;
                            select.AmazoneS3SKey = settingsData.AmazoneS3SKey;
                            select.Region = settingsData.Region;
                            select.SmsTPhoneNumber = settingsData.SmsTPhoneNumber;
                            select.SmsTwilioUsername = settingsData.SmsTwilioUsername;
                            select.SmsTwilioPassword = settingsData.SmsTwilioPassword;
                            select.SmsProvider = settingsData.SmsProvider;
                            select.ProfilePictureWidthCrop = settingsData.ProfilePictureWidthCrop;
                            select.ProfilePictureHeightCrop = settingsData.ProfilePictureHeightCrop;
                            select.UserDefaultAvatar = settingsData.UserDefaultAvatar;
                            select.ProfilePictureImageQuality = settingsData.ProfilePictureImageQuality;
                            select.EmailValidation = settingsData.EmailValidation;
                            select.StripeSecret = settingsData.StripeSecret;
                            select.StripeId = settingsData.StripeId;
                            select.PushId = settingsData.PushId;
                            select.PushKey = settingsData.PushKey;
                            select.PushId2 = settingsData.PushId2;
                            select.PushKey2 = settingsData.PushKey2;
                            select.Terms = settingsData.Terms;
                            select.About = settingsData.About;
                            select.PrivacyPolicy = settingsData.PrivacyPolicy;
                            select.FacebookUrl = settingsData.FacebookUrl;
                            select.TwitterUrl = settingsData.TwitterUrl;
                            select.GoogleUrl = settingsData.GoogleUrl;
                            select.CurrencySymbol = settingsData.CurrencySymbol;
                            select.BagOfCreditsPrice = settingsData.BagOfCreditsPrice;
                            select.BagOfCreditsAmount = settingsData.BagOfCreditsAmount;
                            select.BoxOfCreditsPrice = settingsData.BoxOfCreditsPrice;
                            select.BoxOfCreditsAmount = settingsData.BoxOfCreditsAmount;
                            select.ChestOfCreditsPrice = settingsData.ChestOfCreditsPrice;
                            select.ChestOfCreditsAmount = settingsData.ChestOfCreditsAmount;
                            select.WeeklyProPlan = settingsData.WeeklyProPlan;
                            select.MonthlyProPlan = settingsData.MonthlyProPlan;
                            select.YearlyProPlan = settingsData.YearlyProPlan;
                            select.LifetimeProPlan = settingsData.LifetimeProPlan;
                            select.WorkerUpdateDelay = settingsData.WorkerUpdateDelay;
                            select.ProfileRecordViewsMinute = settingsData.ProfileRecordViewsMinute;
                            select.CostPerGift = settingsData.CostPerGift;
                            select.DeleteAccount = settingsData.DeleteAccount;
                            select.UserRegistration = settingsData.UserRegistration;
                            select.MaxUpload = settingsData.MaxUpload;
                            select.MimeTypes = settingsData.MimeTypes;
                            select.NormalBoostMeCreditsPrice = settingsData.NormalBoostMeCreditsPrice;
                            select.MoreStickersCreditsPrice = settingsData.MoreStickersCreditsPrice;
                            select.ProBoostMeCreditsPrice = settingsData.ProBoostMeCreditsPrice;
                            select.BoostExpireTime = settingsData.BoostExpireTime;
                            select.NotProChatLimitDaily = settingsData.NotProChatLimitDaily;
                            select.NotProChatCredit = settingsData.NotProChatCredit;
                            select.NotProChatStickersCredit = settingsData.NotProChatStickersCredit;
                            select.NotProChatStickersLimit = settingsData.NotProChatStickersLimit;
                            select.CostPerXvisits = settingsData.CostPerXvisits;
                            select.XvisitsExpireTime = settingsData.XvisitsExpireTime;
                            select.CostPerXmatche = settingsData.CostPerXmatche;
                            select.XmatcheExpireTime = settingsData.XmatcheExpireTime;
                            select.CostPerXlike = settingsData.CostPerXlike;
                            select.XlikeExpireTime = settingsData.XlikeExpireTime;
                            select.GooglePlaceApi = settingsData.GooglePlaceApi;
                            select.WowonderLogin = settingsData.WowonderLogin;
                            select.WowonderAppId = settingsData.WowonderAppId;
                            select.WowonderAppKey = settingsData.WowonderAppKey;
                            select.WowonderDomainUri = settingsData.WowonderDomainUri;
                            select.WowonderDomainIcon = settingsData.WowonderDomainIcon;
                            select.BankTransferNote = settingsData.BankTransferNote;
                            select.MaxSwaps = settingsData.MaxSwaps;
                            select.StripeVersion = settingsData.StripeVersion;
                            select.PayseraProjectId = settingsData.PayseraProjectId;
                            select.PayseraPassword = settingsData.PayseraPassword;
                            select.PayseraTestMode = settingsData.PayseraTestMode;
                            select.MessageRequestSystem = settingsData.MessageRequestSystem;
                            select.VideoChat = settingsData.VideoChat;
                            select.AudioChat = settingsData.AudioChat;
                            select.VideoAccountSid = settingsData.VideoAccountSid;
                            select.VideoApiKeySid = settingsData.VideoApiKeySid;
                            select.VideoApiKeySecret = settingsData.VideoApiKeySecret;
                            select.GiphyApi = settingsData.GiphyApi;
                            select.DefaultUnit = settingsData.DefaultUnit;
                            select.MaintenanceMode = settingsData.MaintenanceMode;
                            select.Displaymode = settingsData.Displaymode;
                            select.BankDescription = settingsData.BankDescription;
                            select.Version = settingsData.Version;
                            select.GoogleTagCode = settingsData.GoogleTagCode;
                            select.AvcallPro = settingsData.AvcallPro;
                            select.ProSystem = settingsData.ProSystem;
                            select.ImgBlurAmount = settingsData.ImgBlurAmount;
                            select.EmailNotification = settingsData.EmailNotification;
                            select.ActivationLimitSystem = settingsData.ActivationLimitSystem;
                            select.MaxActivationRequest = settingsData.MaxActivationRequest;
                            select.ActivationRequestTimeLimit = settingsData.ActivationRequestTimeLimit;
                            select.FreeFeatures = settingsData.FreeFeatures;
                            select.OppositeGender = settingsData.OppositeGender;
                            select.ImageVerification = settingsData.ImageVerification;
                            select.PendingVerification = settingsData.PendingVerification;
                            select.Push = settingsData.Push;
                            select.SpamWarning = settingsData.SpamWarning;
                            select.ImageVerificationStart = settingsData.ImageVerificationStart;
                            select.TwoFactor = settingsData.TwoFactor;
                            select.TwoFactorType = settingsData.TwoFactorType;
                            select.AffiliateSystem = settingsData.AffiliateSystem;
                            select.AffiliateType = settingsData.AffiliateType;
                            select.MWithdrawal = settingsData.MWithdrawal;
                            select.AmountRef = settingsData.AmountRef;
                            select.AmountPercentRef = settingsData.AmountPercentRef;
                            select.ConnectivitySystem = settingsData.ConnectivitySystem;
                            select.ConnectivitySystemLimit = settingsData.ConnectivitySystemLimit;
                            select.ShowUserOnHomepage = settingsData.ShowUserOnHomepage;
                            select.ShowedUser = settingsData.ShowedUser;
                            select.MaxPhotoPerUser = settingsData.MaxPhotoPerUser;
                            select.ReviewMediaFiles = settingsData.ReviewMediaFiles;
                            select.FfmpegSys = settingsData.FfmpegSys;
                            select.MaxVideoDuration = settingsData.MaxVideoDuration;
                            select.FfmpegBinary = settingsData.FfmpegBinary;
                            select.DisablePhoneField = settingsData.DisablePhoneField;
                            select.SocialMediaLinks = settingsData.SocialMediaLinks;
                            select.YtApi = settingsData.YtApi;
                            select.Seo = settingsData.Seo;
                            select.LockPrivatePhoto = settingsData.LockPrivatePhoto;
                            select.LockPrivatePhotoFee = settingsData.LockPrivatePhotoFee;
                            select.LockProVideo = settingsData.LockProVideo;
                            select.LockProVideoFee = settingsData.LockProVideoFee;
                            select.VerificationOnSignup = settingsData.VerificationOnSignup;
                            select.CreditEarnSystem = settingsData.CreditEarnSystem;
                            select.CreditEarnMaxDays = settingsData.CreditEarnMaxDays;
                            select.CreditEarnDayAmount = settingsData.CreditEarnDayAmount;
                            select.SpecificEmailSignup = settingsData.SpecificEmailSignup;
                            select.Push1 = settingsData.Push1;
                            select.CheckoutPayment = settingsData.CheckoutPayment;
                            select.CheckoutMode = settingsData.CheckoutMode;
                            select.CheckoutCurrency = settingsData.CheckoutCurrency;
                            select.CheckoutSellerId = settingsData.CheckoutSellerId;
                            select.CheckoutPublishableKey = settingsData.CheckoutPublishableKey;
                            select.CheckoutPrivateKey = settingsData.CheckoutPrivateKey;
                            select.CashfreePayment = settingsData.CashfreePayment;
                            select.CashfreeMode = settingsData.CashfreeMode;
                            select.CashfreeClientKey = settingsData.CashfreeClientKey;
                            select.CashfreeSecretKey = settingsData.CashfreeSecretKey;
                            select.IyzipayPayment = settingsData.IyzipayPayment;
                            select.IyzipayMode = settingsData.IyzipayMode;
                            select.IyzipayKey = settingsData.IyzipayKey;
                            select.IyzipayBuyerId = settingsData.IyzipayBuyerId;
                            select.IyzipaySecretKey = settingsData.IyzipaySecretKey;
                            select.IyzipayBuyerName = settingsData.IyzipayBuyerName;
                            select.IyzipayBuyerSurname = settingsData.IyzipayBuyerSurname;
                            select.IyzipayBuyerGsmNumber = settingsData.IyzipayBuyerGsmNumber;
                            select.IyzipayBuyerEmail = settingsData.IyzipayBuyerEmail;
                            select.IyzipayIdentityNumber = settingsData.IyzipayIdentityNumber;
                            select.IyzipayAddress = settingsData.IyzipayAddress;
                            select.IyzipayCity = settingsData.IyzipayCity;
                            select.IyzipayCountry = settingsData.IyzipayCountry;
                            select.IyzipayZip = settingsData.IyzipayZip;
                            select.GoogleMapApiKey = settingsData.GoogleMapApiKey;
                            select.PayuPayment = settingsData.PayuPayment;
                            select.PayuMode = settingsData.PayuMode;
                            select.PayuMerchantId = settingsData.PayuMerchantId;
                            select.PayuSecretKey = settingsData.PayuSecretKey;
                            select.PayuBuyerName = settingsData.PayuBuyerName;
                            select.PayuBuyerSurname = settingsData.PayuBuyerSurname;
                            select.PayuBuyerGsmNumber = settingsData.PayuBuyerGsmNumber;
                            select.PayuBuyerEmail = settingsData.PayuBuyerEmail;
                            select.PreventSystem = settingsData.PreventSystem;
                            select.BadLoginLimit = settingsData.BadLoginLimit;
                            select.LockTime = settingsData.LockTime;
                            select.PaystackPayment = settingsData.PaystackPayment;
                            select.PaystackSecretKey = settingsData.PaystackSecretKey;
                            select.TwilioChatCall = settingsData.TwilioChatCall;
                            select.AgoraChatCall = settingsData.AgoraChatCall;
                            select.AgoraChatAppId = settingsData.AgoraChatAppId;
                            select.AgoraChatAppCertificate = settingsData.AgoraChatAppCertificate;
                            select.AgoraChatCustomerId = settingsData.AgoraChatCustomerId;
                            select.AgoraChatCustomerSecret = settingsData.AgoraChatCustomerSecret;
                            select.LiveVideo = settingsData.LiveVideo;
                            select.LiveVideoSave = settingsData.LiveVideoSave;
                            select.AgoraLiveVideo = settingsData.AgoraLiveVideo;
                            select.AgoraAppId = settingsData.AgoraAppId;
                            select.AgoraAppCertificate = settingsData.AgoraAppCertificate;
                            select.AgoraCustomerId = settingsData.AgoraCustomerId;
                            select.AgoraCustomerCertificate = settingsData.AgoraCustomerCertificate;
                            select.AmazoneS32 = settingsData.AmazoneS32;
                            select.BucketName2 = settingsData.BucketName2;
                            select.AmazoneS3Key2 = settingsData.AmazoneS3Key2;
                            select.AmazoneS3SKey2 = settingsData.AmazoneS3SKey2;
                            select.Region2 = settingsData.Region2;
                            select.QqAppId = settingsData.QqAppId;
                            select.QqAppkey = settingsData.QqAppkey;
                            select.WeChatAppId = settingsData.WeChatAppId;
                            select.WeChatAppkey = settingsData.WeChatAppkey;
                            select.DiscordAppId = settingsData.DiscordAppId;
                            select.DiscordAppkey = settingsData.DiscordAppkey;
                            select.MailruAppId = settingsData.MailruAppId;
                            select.MailruAppkey = settingsData.MailruAppkey;
                            select.QqLogin = settingsData.QqLogin;
                            select.WeChatLogin = settingsData.WeChatLogin;
                            select.DiscordLogin = settingsData.DiscordLogin;
                            select.MailruLogin = settingsData.MailruLogin;
                            select.TwilioProvider = settingsData.TwilioProvider;
                            select.BulksmsProvider = settingsData.BulksmsProvider;
                            select.BulksmsUsername = settingsData.BulksmsUsername;
                            select.BulksmsPassword = settingsData.BulksmsPassword;
                            select.MessagebirdProvider = settingsData.MessagebirdProvider;
                            select.MessagebirdKey = settingsData.MessagebirdKey;
                            select.MessagebirdPhone = settingsData.MessagebirdPhone;
                            select.AuthorizePayment = settingsData.AuthorizePayment;
                            select.AuthorizeLoginId = settingsData.AuthorizeLoginId;
                            select.AuthorizeTransactionKey = settingsData.AuthorizeTransactionKey;
                            select.AuthorizeTestMode = settingsData.AuthorizeTestMode;
                            select.SecurionpayPayment = settingsData.SecurionpayPayment;
                            select.SecurionpayPublicKey = settingsData.SecurionpayPublicKey;
                            select.SecurionpaySecretKey = settingsData.SecurionpaySecretKey;
                            select.InviteLinksSystem = settingsData.InviteLinksSystem;
                            select.UserLinksLimit = settingsData.UserLinksLimit;
                            select.ExpireUserLinks = settingsData.ExpireUserLinks;
                            select.InfobipProvider = settingsData.InfobipProvider;
                            select.InfobipUsername = settingsData.InfobipUsername;
                            select.InfobipPassword = settingsData.InfobipPassword;
                            select.Msg91Provider = settingsData.Msg91Provider;
                            select.Msg91AuthKey = settingsData.Msg91AuthKey;
                            select.AutoUserLike = settingsData.AutoUserLike;
                            select.DevelopersPage = settingsData.DevelopersPage;
                            select.IsRtl = settingsData.IsRtl;
                            select.Uri = settingsData.Uri;
                            select.S3SiteUrl2 = settingsData.S3SiteUrl2;
                            select.AamarpayMode = settingsData.AamarpayMode;
                            select.AamarpayPayment = settingsData.AamarpayPayment;
                            select.AamarpaySignatureKey = settingsData.AamarpaySignatureKey;
                            select.AamarpayStoreId = settingsData.AamarpayStoreId;
                            select.BankPayment = settingsData.BankPayment;
                            select.CashfreeCurrency = settingsData.CashfreeCurrency;
                            select.CloudBucketName = settingsData.CloudBucketName;
                            select.CloudFile = settingsData.CloudFile;
                            select.CloudFilePath = settingsData.CloudFilePath;
                            select.CloudUpload = settingsData.CloudUpload;
                            select.CoinbaseKey = settingsData.CoinbaseKey;
                            select.CoinbasePayment = settingsData.CoinbasePayment;
                            select.Coinpayments = settingsData.Coinpayments;
                            select.CoinpaymentsCoin = settingsData.CoinpaymentsCoin;
                            select.CoinpaymentsCoins = settingsData.CoinpaymentsCoins;
                            select.CoinpaymentsId = settingsData.CoinpaymentsId;
                            select.CoinpaymentsPublicKey = settingsData.CoinpaymentsPublicKey;
                            select.CoinpaymentsSecret = settingsData.CoinpaymentsSecret;
                            select.CreditPrice = settingsData.CreditPrice;
                            select.FilterByCities = settingsData.FilterByCities;
                            select.FilterByCountry = settingsData.FilterByCountry;
                            select.FluttewavePayment = settingsData.FluttewavePayment;
                            select.FluttewaveSecretKey = settingsData.FluttewaveSecretKey;
                            select.FortumoPayment = settingsData.FortumoPayment;
                            select.FortumoServiceId = settingsData.FortumoServiceId;
                            select.FtpEndpoint = settingsData.FtpEndpoint;
                            select.FtpHost = settingsData.FtpHost;
                            select.FtpPassword = settingsData.FtpPassword;
                            select.FtpPath = settingsData.FtpPath;
                            select.FtpPort = settingsData.FtpPort;
                            select.FtpUpload = settingsData.FtpUpload;
                            select.FtpUsername = settingsData.FtpUsername;
                            select.GeoUsername = settingsData.GeoUsername;
                            select.InstagramImporter = settingsData.InstagramImporter;
                            select.InstagramImporterAppId = settingsData.InstagramImporterAppId;
                            select.InstagramImporterAppSecret = settingsData.InstagramImporterAppSecret;
                            select.IyzipayCurrency = settingsData.IyzipayCurrency;
                            select.NativeAndroidUrl = settingsData.NativeAndroidUrl;
                            select.NativeIosUrl = settingsData.NativeIosUrl;
                            select.NgeniusApiKey = settingsData.NgeniusApiKey;
                            select.NgeniusMode = settingsData.NgeniusMode;
                            select.NgeniusOutletId = settingsData.NgeniusOutletId;
                            select.NgeniusPayment = settingsData.NgeniusPayment;
                            select.OkAppId = settingsData.OkAppId;
                            select.OkAppPublickey = settingsData.OkAppPublickey;
                            select.OkAppSecretkey = settingsData.OkAppSecretkey;
                            select.OkLogin = settingsData.OkLogin;
                            select.PaypalCurrency = settingsData.PaypalCurrency;
                            select.PaypalPayment = settingsData.PaypalPayment;
                            select.PayseraPayment = settingsData.PayseraPayment;
                            select.PaystackCurrency = settingsData.PaystackCurrency;
                            select.RazorpayKeyId = settingsData.RazorpayKeyId;
                            select.RazorpayKeySecret = settingsData.RazorpayKeySecret;
                            select.RazorpayPayment = settingsData.RazorpayPayment;
                            select.Refund = settingsData.Refund;
                            select.SpaceName = settingsData.SpaceName;
                            select.SpaceRegion = settingsData.SpaceRegion;
                            select.Spaces = settingsData.Spaces;
                            select.SpacesKey = settingsData.SpacesKey;
                            select.SpacesSecret = settingsData.SpacesSecret;
                            select.StripeCurrency = settingsData.StripeCurrency;
                            select.StripePayment = settingsData.StripePayment;
                            select.SuccessStoriesSystem = settingsData.SuccessStoriesSystem;
                            select.WasabiAccessKey = settingsData.WasabiAccessKey;
                            select.WasabiBucketName = settingsData.WasabiBucketName;
                            select.WasabiBucketRegion = settingsData.WasabiBucketRegion;
                            select.WasabiSecretKey = settingsData.WasabiSecretKey;
                            select.WasabiStorage = settingsData.WasabiStorage;
                            select.WatermarkSystem = settingsData.WatermarkSystem;
                            select.YoomoneyNotificationsSecret = settingsData.YoomoneyNotificationsSecret;
                            select.YoomoneyPayment = settingsData.YoomoneyPayment;
                            select.YoomoneyWalletId = settingsData.YoomoneyWalletId;
                            select.AlipayPayment = settingsData.AlipayPayment;
                            select.AlipayServer = settingsData.AlipayServer;
                            select.Arabic = settingsData.Arabic;
                            select.BackblazeAccessKey = settingsData.BackblazeAccessKey;
                            select.BackblazeAccessKeyId = settingsData.BackblazeAccessKeyId;
                            select.BackblazeBucketId = settingsData.BackblazeBucketId;
                            select.BackblazeBucketName = settingsData.BackblazeBucketName;
                            select.BackblazeBucketRegion = settingsData.BackblazeBucketRegion;
                            select.BackblazeEndpoint = settingsData.BackblazeEndpoint;
                            select.BackblazeStorage = settingsData.BackblazeStorage;
                            select.BulksmsPhoneNumber = settingsData.BulksmsPhoneNumber;
                            select.CostAdmob = settingsData.CostAdmob;
                            select.CustomName = settingsData.CustomName;
                            select.DeveloperMode = settingsData.DeveloperMode;
                            select.Dutch = settingsData.Dutch;
                            select.English = settingsData.English;
                            //select.Exchange = settingsData.Exchange;
                            //select.ExchangeUpdate = settingsData.ExchangeUpdate;
                            select.FilesVersion = settingsData.FilesVersion;
                            select.French = settingsData.French;
                            select.German = settingsData.German;
                            select.InfobipPhoneNumber = settingsData.InfobipPhoneNumber;
                            select.IsDailyCredit = settingsData.IsDailyCredit;
                            select.Italian = settingsData.Italian;
                            select.Msg91DltId = settingsData.Msg91DltId;
                            select.Msg91PhoneNumber = settingsData.Msg91PhoneNumber;
                            select.PopUp18 = settingsData.PopUp18;
                            select.Portuguese = settingsData.Portuguese;
                            select.Recaptcha = settingsData.Recaptcha;
                            select.RecaptchaSecretKey = settingsData.RecaptchaSecretKey;
                            select.RecaptchaSiteKey = settingsData.RecaptchaSiteKey;
                            select.ReservedUsernames = settingsData.ReservedUsernames;
                            select.ReservedUsernamesSystem = settingsData.ReservedUsernamesSystem;
                            select.Russian = settingsData.Russian;
                            select.Spanish = settingsData.Spanish;
                            select.Time18 = settingsData.Time18;
                            select.Turkish = settingsData.Turkish;
                            //select.WithdrawalPaymentMethod = settingsData.WithdrawalPaymentMethod;


                            select.Height = JsonConvert.SerializeObject(settingsData.Height);
                            select.Notification = JsonConvert.SerializeObject(settingsData.Notification);
                            select.Gender = JsonConvert.SerializeObject(settingsData.Gender);
                            select.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            select.Countries = JsonConvert.SerializeObject(settingsData.Countries);
                            select.HairColor = JsonConvert.SerializeObject(settingsData.HairColor);
                            select.Travel = JsonConvert.SerializeObject(settingsData.Travel);
                            select.Drink = JsonConvert.SerializeObject(settingsData.Drink);
                            select.Smoke = JsonConvert.SerializeObject(settingsData.Smoke);
                            select.Religion = JsonConvert.SerializeObject(settingsData.Religion);
                            select.Car = JsonConvert.SerializeObject(settingsData.Car);
                            select.LiveWith = JsonConvert.SerializeObject(settingsData.LiveWith);
                            select.Pets = JsonConvert.SerializeObject(settingsData.Pets);
                            select.Friends = JsonConvert.SerializeObject(settingsData.Friends);
                            select.Children = JsonConvert.SerializeObject(settingsData.Children);
                            select.Character = JsonConvert.SerializeObject(settingsData.Character);
                            select.Body = JsonConvert.SerializeObject(settingsData.Body);
                            select.Ethnicity = JsonConvert.SerializeObject(settingsData.Ethnicity);
                            select.Education = JsonConvert.SerializeObject(settingsData.Education);
                            select.WorkStatus = JsonConvert.SerializeObject(settingsData.WorkStatus);
                            select.Relationship = JsonConvert.SerializeObject(settingsData.Relationship);
                            select.Language = JsonConvert.SerializeObject(settingsData.Language);
                            select.CustomFields = JsonConvert.SerializeObject(settingsData.CustomFields);
                            select.CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray);
                            select.CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray);
                            select.CashfreeCurrencyArray = JsonConvert.SerializeObject(settingsData.CashfreeCurrencyArray);
                            select.IyzipayCurrencyArray = JsonConvert.SerializeObject(settingsData.IyzipayCurrencyArray);
                            select.ReservedUsernamesArray = JsonConvert.SerializeObject(settingsData.ReservedUsernamesArray);

                            connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateSettings(settingsData);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Settings
        public GetOptionsObject.DataOptions GetSettings()
        {
            try
            {
                using var connection = OpenConnection();
                var settingsData = connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                if (settingsData != null)
                {
                    var db = new GetOptionsObject.DataOptions()
                    {
                        LoadConfigInSession = settingsData.LoadConfigInSession,
                        MetaDescription = settingsData.MetaDescription,
                        MetaKeywords = settingsData.MetaKeywords,
                        DefaultTitle = settingsData.DefaultTitle,
                        SiteName = settingsData.SiteName,
                        DefaultLanguage = settingsData.DefaultLanguage,
                        SmtpOrMail = settingsData.SmtpOrMail,
                        SmtpHost = settingsData.SmtpHost,
                        SmtpUsername = settingsData.SmtpUsername,
                        SmtpPassword = settingsData.SmtpPassword,
                        SmtpEncryption = settingsData.SmtpEncryption,
                        SmtpPort = settingsData.SmtpPort,
                        SiteEmail = settingsData.SiteEmail,
                        Theme = settingsData.Theme,
                        AllLogin = settingsData.AllLogin,
                        GoogleLogin = settingsData.GoogleLogin,
                        FacebookLogin = settingsData.FacebookLogin,
                        TwitterLogin = settingsData.TwitterLogin,
                        LinkedinLogin = settingsData.LinkedinLogin,
                        VkontakteLogin = settingsData.VkontakteLogin,
                        FacebookAppId = settingsData.FacebookAppId,
                        FacebookAppKey = settingsData.FacebookAppKey,
                        GoogleAppId = settingsData.GoogleAppId,
                        GoogleAppKey = settingsData.GoogleAppKey,
                        TwitterAppId = settingsData.TwitterAppId,
                        TwitterAppKey = settingsData.TwitterAppKey,
                        LinkedinAppId = settingsData.LinkedinAppId,
                        LinkedinAppKey = settingsData.LinkedinAppKey,
                        VkontakteAppId = settingsData.VkontakteAppId,
                        VkontakteAppKey = settingsData.VkontakteAppKey,
                        InstagramAppId = settingsData.InstagramAppId,
                        InstagramAppkey = settingsData.InstagramAppkey,
                        InstagramLogin = settingsData.InstagramLogin,
                        SmsOrEmail = settingsData.SmsOrEmail,
                        SmsPhoneNumber = settingsData.SmsPhoneNumber,
                        PaypalId = settingsData.PaypalId,
                        PaypalSecret = settingsData.PaypalSecret,
                        PaypalMode = settingsData.PaypalMode,
                        Currency = settingsData.Currency,
                        LastBackup = settingsData.LastBackup,
                        AmazoneS3 = settingsData.AmazoneS3,
                        BucketName = settingsData.BucketName,
                        AmazoneS3Key = settingsData.AmazoneS3Key,
                        AmazoneS3SKey = settingsData.AmazoneS3SKey,
                        Region = settingsData.Region,
                        SmsTPhoneNumber = settingsData.SmsTPhoneNumber,
                        SmsTwilioUsername = settingsData.SmsTwilioUsername,
                        SmsTwilioPassword = settingsData.SmsTwilioPassword,
                        SmsProvider = settingsData.SmsProvider,
                        ProfilePictureWidthCrop = settingsData.ProfilePictureWidthCrop,
                        ProfilePictureHeightCrop = settingsData.ProfilePictureHeightCrop,
                        UserDefaultAvatar = settingsData.UserDefaultAvatar,
                        ProfilePictureImageQuality = settingsData.ProfilePictureImageQuality,
                        EmailValidation = settingsData.EmailValidation,
                        StripeSecret = settingsData.StripeSecret,
                        StripeId = settingsData.StripeId,
                        PushId = settingsData.PushId,
                        PushKey = settingsData.PushKey,
                        PushId2 = settingsData.PushId2,
                        PushKey2 = settingsData.PushKey2,
                        Terms = settingsData.Terms,
                        About = settingsData.About,
                        PrivacyPolicy = settingsData.PrivacyPolicy,
                        FacebookUrl = settingsData.FacebookUrl,
                        TwitterUrl = settingsData.TwitterUrl,
                        GoogleUrl = settingsData.GoogleUrl,
                        CurrencySymbol = settingsData.CurrencySymbol,
                        BagOfCreditsPrice = settingsData.BagOfCreditsPrice,
                        BagOfCreditsAmount = settingsData.BagOfCreditsAmount,
                        BoxOfCreditsPrice = settingsData.BoxOfCreditsPrice,
                        BoxOfCreditsAmount = settingsData.BoxOfCreditsAmount,
                        ChestOfCreditsPrice = settingsData.ChestOfCreditsPrice,
                        ChestOfCreditsAmount = settingsData.ChestOfCreditsAmount,
                        WeeklyProPlan = settingsData.WeeklyProPlan,
                        MonthlyProPlan = settingsData.MonthlyProPlan,
                        YearlyProPlan = settingsData.YearlyProPlan,
                        LifetimeProPlan = settingsData.LifetimeProPlan,
                        WorkerUpdateDelay = settingsData.WorkerUpdateDelay,
                        ProfileRecordViewsMinute = settingsData.ProfileRecordViewsMinute,
                        CostPerGift = settingsData.CostPerGift,
                        DeleteAccount = settingsData.DeleteAccount,
                        UserRegistration = settingsData.UserRegistration,
                        MaxUpload = settingsData.MaxUpload,
                        MimeTypes = settingsData.MimeTypes,
                        NormalBoostMeCreditsPrice = settingsData.NormalBoostMeCreditsPrice,
                        MoreStickersCreditsPrice = settingsData.MoreStickersCreditsPrice,
                        ProBoostMeCreditsPrice = settingsData.ProBoostMeCreditsPrice,
                        BoostExpireTime = settingsData.BoostExpireTime,
                        NotProChatLimitDaily = settingsData.NotProChatLimitDaily,
                        NotProChatCredit = settingsData.NotProChatCredit,
                        NotProChatStickersCredit = settingsData.NotProChatStickersCredit,
                        NotProChatStickersLimit = settingsData.NotProChatStickersLimit,
                        CostPerXvisits = settingsData.CostPerXvisits,
                        XvisitsExpireTime = settingsData.XvisitsExpireTime,
                        CostPerXmatche = settingsData.CostPerXmatche,
                        XmatcheExpireTime = settingsData.XmatcheExpireTime,
                        CostPerXlike = settingsData.CostPerXlike,
                        XlikeExpireTime = settingsData.XlikeExpireTime,
                        GooglePlaceApi = settingsData.GooglePlaceApi,
                        WowonderLogin = settingsData.WowonderLogin,
                        WowonderAppId = settingsData.WowonderAppId,
                        WowonderAppKey = settingsData.WowonderAppKey,
                        WowonderDomainUri = settingsData.WowonderDomainUri,
                        WowonderDomainIcon = settingsData.WowonderDomainIcon,
                        BankTransferNote = settingsData.BankTransferNote,
                        MaxSwaps = settingsData.MaxSwaps,
                        StripeVersion = settingsData.StripeVersion,
                        PayseraProjectId = settingsData.PayseraProjectId,
                        PayseraPassword = settingsData.PayseraPassword,
                        PayseraTestMode = settingsData.PayseraTestMode,
                        MessageRequestSystem = settingsData.MessageRequestSystem,
                        VideoChat = settingsData.VideoChat,
                        AudioChat = settingsData.AudioChat,
                        VideoAccountSid = settingsData.VideoAccountSid,
                        VideoApiKeySid = settingsData.VideoApiKeySid,
                        VideoApiKeySecret = settingsData.VideoApiKeySecret,
                        GiphyApi = settingsData.GiphyApi,
                        DefaultUnit = settingsData.DefaultUnit,
                        MaintenanceMode = settingsData.MaintenanceMode,
                        Displaymode = settingsData.Displaymode,
                        BankDescription = settingsData.BankDescription,
                        Version = settingsData.Version,
                        GoogleTagCode = settingsData.GoogleTagCode,
                        AvcallPro = settingsData.AvcallPro,
                        ProSystem = settingsData.ProSystem,
                        ImgBlurAmount = settingsData.ImgBlurAmount,
                        EmailNotification = settingsData.EmailNotification,
                        ActivationLimitSystem = settingsData.ActivationLimitSystem,
                        MaxActivationRequest = settingsData.MaxActivationRequest,
                        ActivationRequestTimeLimit = settingsData.ActivationRequestTimeLimit,
                        FreeFeatures = settingsData.FreeFeatures,
                        OppositeGender = settingsData.OppositeGender,
                        ImageVerification = settingsData.ImageVerification,
                        PendingVerification = settingsData.PendingVerification,
                        Push = settingsData.Push,
                        SpamWarning = settingsData.SpamWarning,
                        ImageVerificationStart = settingsData.ImageVerificationStart,
                        TwoFactor = settingsData.TwoFactor,
                        TwoFactorType = settingsData.TwoFactorType,
                        AffiliateSystem = settingsData.AffiliateSystem,
                        AffiliateType = settingsData.AffiliateType,
                        MWithdrawal = settingsData.MWithdrawal,
                        AmountRef = settingsData.AmountRef,
                        AmountPercentRef = settingsData.AmountPercentRef,
                        ConnectivitySystem = settingsData.ConnectivitySystem,
                        ConnectivitySystemLimit = settingsData.ConnectivitySystemLimit,
                        ShowUserOnHomepage = settingsData.ShowUserOnHomepage,
                        ShowedUser = settingsData.ShowedUser,
                        MaxPhotoPerUser = settingsData.MaxPhotoPerUser,
                        ReviewMediaFiles = settingsData.ReviewMediaFiles,
                        FfmpegSys = settingsData.FfmpegSys,
                        MaxVideoDuration = settingsData.MaxVideoDuration,
                        FfmpegBinary = settingsData.FfmpegBinary,
                        DisablePhoneField = settingsData.DisablePhoneField,
                        SocialMediaLinks = settingsData.SocialMediaLinks,
                        YtApi = settingsData.YtApi,
                        Seo = settingsData.Seo,
                        LockPrivatePhoto = settingsData.LockPrivatePhoto,
                        LockPrivatePhotoFee = settingsData.LockPrivatePhotoFee,
                        LockProVideo = settingsData.LockProVideo,
                        LockProVideoFee = settingsData.LockProVideoFee,
                        VerificationOnSignup = settingsData.VerificationOnSignup,
                        CreditEarnSystem = settingsData.CreditEarnSystem,
                        CreditEarnMaxDays = settingsData.CreditEarnMaxDays,
                        CreditEarnDayAmount = settingsData.CreditEarnDayAmount,
                        SpecificEmailSignup = settingsData.SpecificEmailSignup,
                        Push1 = settingsData.Push1,
                        CheckoutPayment = settingsData.CheckoutPayment,
                        CheckoutMode = settingsData.CheckoutMode,
                        CheckoutCurrency = settingsData.CheckoutCurrency,
                        CheckoutSellerId = settingsData.CheckoutSellerId,
                        CheckoutPublishableKey = settingsData.CheckoutPublishableKey,
                        CheckoutPrivateKey = settingsData.CheckoutPrivateKey,
                        CashfreePayment = settingsData.CashfreePayment,
                        CashfreeMode = settingsData.CashfreeMode,
                        CashfreeClientKey = settingsData.CashfreeClientKey,
                        CashfreeSecretKey = settingsData.CashfreeSecretKey,
                        IyzipayPayment = settingsData.IyzipayPayment,
                        IyzipayMode = settingsData.IyzipayMode,
                        IyzipayKey = settingsData.IyzipayKey,
                        IyzipayBuyerId = settingsData.IyzipayBuyerId,
                        IyzipaySecretKey = settingsData.IyzipaySecretKey,
                        IyzipayBuyerName = settingsData.IyzipayBuyerName,
                        IyzipayBuyerSurname = settingsData.IyzipayBuyerSurname,
                        IyzipayBuyerGsmNumber = settingsData.IyzipayBuyerGsmNumber,
                        IyzipayBuyerEmail = settingsData.IyzipayBuyerEmail,
                        IyzipayIdentityNumber = settingsData.IyzipayIdentityNumber,
                        IyzipayAddress = settingsData.IyzipayAddress,
                        IyzipayCity = settingsData.IyzipayCity,
                        IyzipayCountry = settingsData.IyzipayCountry,
                        IyzipayZip = settingsData.IyzipayZip,
                        GoogleMapApiKey = settingsData.GoogleMapApiKey,
                        PayuPayment = settingsData.PayuPayment,
                        PayuMode = settingsData.PayuMode,
                        PayuMerchantId = settingsData.PayuMerchantId,
                        PayuSecretKey = settingsData.PayuSecretKey,
                        PayuBuyerName = settingsData.PayuBuyerName,
                        PayuBuyerSurname = settingsData.PayuBuyerSurname,
                        PayuBuyerGsmNumber = settingsData.PayuBuyerGsmNumber,
                        PayuBuyerEmail = settingsData.PayuBuyerEmail,
                        PreventSystem = settingsData.PreventSystem,
                        BadLoginLimit = settingsData.BadLoginLimit,
                        LockTime = settingsData.LockTime,
                        PaystackPayment = settingsData.PaystackPayment,
                        PaystackSecretKey = settingsData.PaystackSecretKey,
                        TwilioChatCall = settingsData.TwilioChatCall,
                        AgoraChatCall = settingsData.AgoraChatCall,
                        AgoraChatAppId = settingsData.AgoraChatAppId,
                        AgoraChatAppCertificate = settingsData.AgoraChatAppCertificate,
                        AgoraChatCustomerId = settingsData.AgoraChatCustomerId,
                        AgoraChatCustomerSecret = settingsData.AgoraChatCustomerSecret,
                        LiveVideo = settingsData.LiveVideo,
                        LiveVideoSave = settingsData.LiveVideoSave,
                        AgoraLiveVideo = settingsData.AgoraLiveVideo,
                        AgoraAppId = settingsData.AgoraAppId,
                        AgoraAppCertificate = settingsData.AgoraAppCertificate,
                        AgoraCustomerId = settingsData.AgoraCustomerId,
                        AgoraCustomerCertificate = settingsData.AgoraCustomerCertificate,
                        AmazoneS32 = settingsData.AmazoneS32,
                        BucketName2 = settingsData.BucketName2,
                        AmazoneS3Key2 = settingsData.AmazoneS3Key2,
                        AmazoneS3SKey2 = settingsData.AmazoneS3SKey2,
                        Region2 = settingsData.Region2,
                        QqAppId = settingsData.QqAppId,
                        QqAppkey = settingsData.QqAppkey,
                        WeChatAppId = settingsData.WeChatAppId,
                        WeChatAppkey = settingsData.WeChatAppkey,
                        DiscordAppId = settingsData.DiscordAppId,
                        DiscordAppkey = settingsData.DiscordAppkey,
                        MailruAppId = settingsData.MailruAppId,
                        MailruAppkey = settingsData.MailruAppkey,
                        QqLogin = settingsData.QqLogin,
                        WeChatLogin = settingsData.WeChatLogin,
                        DiscordLogin = settingsData.DiscordLogin,
                        MailruLogin = settingsData.MailruLogin,
                        TwilioProvider = settingsData.TwilioProvider,
                        BulksmsProvider = settingsData.BulksmsProvider,
                        BulksmsUsername = settingsData.BulksmsUsername,
                        BulksmsPassword = settingsData.BulksmsPassword,
                        MessagebirdProvider = settingsData.MessagebirdProvider,
                        MessagebirdKey = settingsData.MessagebirdKey,
                        MessagebirdPhone = settingsData.MessagebirdPhone,
                        AuthorizePayment = settingsData.AuthorizePayment,
                        AuthorizeLoginId = settingsData.AuthorizeLoginId,
                        AuthorizeTransactionKey = settingsData.AuthorizeTransactionKey,
                        AuthorizeTestMode = settingsData.AuthorizeTestMode,
                        SecurionpayPayment = settingsData.SecurionpayPayment,
                        SecurionpayPublicKey = settingsData.SecurionpayPublicKey,
                        SecurionpaySecretKey = settingsData.SecurionpaySecretKey,
                        InviteLinksSystem = settingsData.InviteLinksSystem,
                        UserLinksLimit = settingsData.UserLinksLimit,
                        ExpireUserLinks = settingsData.ExpireUserLinks,
                        InfobipProvider = settingsData.InfobipProvider,
                        InfobipUsername = settingsData.InfobipUsername,
                        InfobipPassword = settingsData.InfobipPassword,
                        Msg91Provider = settingsData.Msg91Provider,
                        Msg91AuthKey = settingsData.Msg91AuthKey,
                        AutoUserLike = settingsData.AutoUserLike,
                        DevelopersPage = settingsData.DevelopersPage,
                        IsRtl = settingsData.IsRtl,
                        Uri = settingsData.Uri,
                        S3SiteUrl2 = settingsData.S3SiteUrl2,
                        AamarpayMode = settingsData.AamarpayMode,
                        AamarpayPayment = settingsData.AamarpayPayment,
                        AamarpaySignatureKey = settingsData.AamarpaySignatureKey,
                        AamarpayStoreId = settingsData.AamarpayStoreId,
                        BankPayment = settingsData.BankPayment,
                        CashfreeCurrency = settingsData.CashfreeCurrency,
                        CloudBucketName = settingsData.CloudBucketName,
                        CloudFile = settingsData.CloudFile,
                        CloudFilePath = settingsData.CloudFilePath,
                        CloudUpload = settingsData.CloudUpload,
                        CoinbaseKey = settingsData.CoinbaseKey,
                        CoinbasePayment = settingsData.CoinbasePayment,
                        Coinpayments = settingsData.Coinpayments,
                        CoinpaymentsCoin = settingsData.CoinpaymentsCoin,
                        CoinpaymentsCoins = settingsData.CoinpaymentsCoins,
                        CoinpaymentsId = settingsData.CoinpaymentsId,
                        CoinpaymentsPublicKey = settingsData.CoinpaymentsPublicKey,
                        CoinpaymentsSecret = settingsData.CoinpaymentsSecret,
                        CreditPrice = settingsData.CreditPrice,
                        FilterByCities = settingsData.FilterByCities,
                        FilterByCountry = settingsData.FilterByCountry,
                        FluttewavePayment = settingsData.FluttewavePayment,
                        FluttewaveSecretKey = settingsData.FluttewaveSecretKey,
                        FortumoPayment = settingsData.FortumoPayment,
                        FortumoServiceId = settingsData.FortumoServiceId,
                        FtpEndpoint = settingsData.FtpEndpoint,
                        FtpHost = settingsData.FtpHost,
                        FtpPassword = settingsData.FtpPassword,
                        FtpPath = settingsData.FtpPath,
                        FtpPort = settingsData.FtpPort,
                        FtpUpload = settingsData.FtpUpload,
                        FtpUsername = settingsData.FtpUsername,
                        GeoUsername = settingsData.GeoUsername,
                        InstagramImporter = settingsData.InstagramImporter,
                        InstagramImporterAppId = settingsData.InstagramImporterAppId,
                        InstagramImporterAppSecret = settingsData.InstagramImporterAppSecret,
                        IyzipayCurrency = settingsData.IyzipayCurrency,
                        NativeAndroidUrl = settingsData.NativeAndroidUrl,
                        NativeIosUrl = settingsData.NativeIosUrl,
                        NgeniusApiKey = settingsData.NgeniusApiKey,
                        NgeniusMode = settingsData.NgeniusMode,
                        NgeniusOutletId = settingsData.NgeniusOutletId,
                        NgeniusPayment = settingsData.NgeniusPayment,
                        OkAppId = settingsData.OkAppId,
                        OkAppPublickey = settingsData.OkAppPublickey,
                        OkAppSecretkey = settingsData.OkAppSecretkey,
                        OkLogin = settingsData.OkLogin,
                        PaypalCurrency = settingsData.PaypalCurrency,
                        PaypalPayment = settingsData.PaypalPayment,
                        PayseraPayment = settingsData.PayseraPayment,
                        PaystackCurrency = settingsData.PaystackCurrency,
                        RazorpayKeyId = settingsData.RazorpayKeyId,
                        RazorpayKeySecret = settingsData.RazorpayKeySecret,
                        RazorpayPayment = settingsData.RazorpayPayment,
                        Refund = settingsData.Refund,
                        SpaceName = settingsData.SpaceName,
                        SpaceRegion = settingsData.SpaceRegion,
                        Spaces = settingsData.Spaces,
                        SpacesKey = settingsData.SpacesKey,
                        SpacesSecret = settingsData.SpacesSecret,
                        StripeCurrency = settingsData.StripeCurrency,
                        StripePayment = settingsData.StripePayment,
                        SuccessStoriesSystem = settingsData.SuccessStoriesSystem,
                        WasabiAccessKey = settingsData.WasabiAccessKey,
                        WasabiBucketName = settingsData.WasabiBucketName,
                        WasabiBucketRegion = settingsData.WasabiBucketRegion,
                        WasabiSecretKey = settingsData.WasabiSecretKey,
                        WasabiStorage = settingsData.WasabiStorage,
                        WatermarkSystem = settingsData.WatermarkSystem,
                        YoomoneyNotificationsSecret = settingsData.YoomoneyNotificationsSecret,
                        YoomoneyPayment = settingsData.YoomoneyPayment,
                        YoomoneyWalletId = settingsData.YoomoneyWalletId,
                        AlipayPayment = settingsData.AlipayPayment,
                        AlipayServer = settingsData.AlipayServer,
                        Arabic = settingsData.Arabic,
                        BackblazeAccessKey = settingsData.BackblazeAccessKey,
                        BackblazeAccessKeyId = settingsData.BackblazeAccessKeyId,
                        BackblazeBucketId = settingsData.BackblazeBucketId,
                        BackblazeBucketName = settingsData.BackblazeBucketName,
                        BackblazeBucketRegion = settingsData.BackblazeBucketRegion,
                        BackblazeEndpoint = settingsData.BackblazeEndpoint,
                        BackblazeStorage = settingsData.BackblazeStorage,
                        BulksmsPhoneNumber = settingsData.BulksmsPhoneNumber,
                        CostAdmob = settingsData.CostAdmob,
                        CustomName = settingsData.CustomName,
                        DeveloperMode = settingsData.DeveloperMode,
                        Dutch = settingsData.Dutch,
                        English = settingsData.English,
                        //Exchange = settingsData.Exchange,
                        //ExchangeUpdate = settingsData.ExchangeUpdate,
                        FilesVersion = settingsData.FilesVersion,
                        French = settingsData.French,
                        German = settingsData.German,
                        InfobipPhoneNumber = settingsData.InfobipPhoneNumber,
                        IsDailyCredit = settingsData.IsDailyCredit,
                        Italian = settingsData.Italian,
                        Msg91DltId = settingsData.Msg91DltId,
                        Msg91PhoneNumber = settingsData.Msg91PhoneNumber,
                        PopUp18 = settingsData.PopUp18,
                        Portuguese = settingsData.Portuguese,
                        Recaptcha = settingsData.Recaptcha,
                        RecaptchaSecretKey = settingsData.RecaptchaSecretKey,
                        RecaptchaSiteKey = settingsData.RecaptchaSiteKey,
                        ReservedUsernames = settingsData.ReservedUsernames,
                        ReservedUsernamesSystem = settingsData.ReservedUsernamesSystem,
                        Russian = settingsData.Russian,
                        Spanish = settingsData.Spanish,
                        Time18 = settingsData.Time18,
                        Turkish = settingsData.Turkish,
                        //WithdrawalPaymentMethod = settingsData.WithdrawalPaymentMethod,
                    };

                    if (db != null)
                    {
                        db.Height = new List<Dictionary<string, string>>();
                        db.Notification = new List<Dictionary<string, string>>();
                        db.Gender = new List<Dictionary<string, string>>();
                        db.BlogCategories = new List<Dictionary<string, string>>();
                        db.Countries = new List<Dictionary<string, GetOptionsObject.Country>>();
                        db.HairColor = new List<Dictionary<string, string>>();
                        db.Travel = new List<Dictionary<string, string>>();
                        db.Drink = new List<Dictionary<string, string>>();
                        db.Smoke = new List<Dictionary<string, string>>();
                        db.Religion = new List<Dictionary<string, string>>();
                        db.Car = new List<Dictionary<string, string>>();
                        db.LiveWith = new List<Dictionary<string, string>>();
                        db.Pets = new List<Dictionary<string, string>>();
                        db.Friends = new List<Dictionary<string, string>>();
                        db.Children = new List<Dictionary<string, string>>();
                        db.Character = new List<Dictionary<string, string>>();
                        db.Body = new List<Dictionary<string, string>>();
                        db.Ethnicity = new List<Dictionary<string, string>>();
                        db.Education = new List<Dictionary<string, string>>();
                        db.WorkStatus = new List<Dictionary<string, string>>();
                        db.Relationship = new List<Dictionary<string, string>>();
                        db.Language = new List<Dictionary<string, string>>();
                        db.CustomFields = new List<GetOptionsObject.CustomField>();
                        db.CurrencyArray = new List<string>();
                        db.CurrencySymbolArray = new Dictionary<string, string>();
                        db.CashfreeCurrencyArray = new List<string>();
                        db.IyzipayCurrencyArray = new List<string>();
                        db.ReservedUsernamesArray = new List<string>();


                        if (!string.IsNullOrEmpty(settingsData.Height))
                            db.Height = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Height);

                        if (!string.IsNullOrEmpty(settingsData.Notification))
                            db.Notification = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Notification);

                        if (!string.IsNullOrEmpty(settingsData.BlogCategories))
                            db.BlogCategories = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.BlogCategories);

                        if (!string.IsNullOrEmpty(settingsData.Gender))
                            db.Gender = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Gender);

                        if (!string.IsNullOrEmpty(settingsData.Countries))
                            db.Countries = JsonConvert.DeserializeObject<List<Dictionary<string, GetOptionsObject.Country>>>(settingsData.Countries);

                        if (!string.IsNullOrEmpty(settingsData.HairColor))
                            db.HairColor = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.HairColor);

                        if (!string.IsNullOrEmpty(settingsData.Travel))
                            db.Travel = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Travel);

                        if (!string.IsNullOrEmpty(settingsData.Drink))
                            db.Drink = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Drink);

                        if (!string.IsNullOrEmpty(settingsData.Smoke))
                            db.Smoke = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Smoke);

                        if (!string.IsNullOrEmpty(settingsData.Religion))
                            db.Religion = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Religion);

                        if (!string.IsNullOrEmpty(settingsData.Car))
                            db.Car = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Car);

                        if (!string.IsNullOrEmpty(settingsData.LiveWith))
                            db.LiveWith = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.LiveWith);

                        if (!string.IsNullOrEmpty(settingsData.Pets))
                            db.Pets = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Pets);

                        if (!string.IsNullOrEmpty(settingsData.Friends))
                            db.Friends = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Friends);

                        if (!string.IsNullOrEmpty(settingsData.Children))
                            db.Children = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Children);

                        if (!string.IsNullOrEmpty(settingsData.Character))
                            db.Character = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Character);

                        if (!string.IsNullOrEmpty(settingsData.Body))
                            db.Body = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Body);

                        if (!string.IsNullOrEmpty(settingsData.Ethnicity))
                            db.Ethnicity = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Ethnicity);

                        if (!string.IsNullOrEmpty(settingsData.Education))
                            db.Education = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Education);

                        if (!string.IsNullOrEmpty(settingsData.WorkStatus))
                            db.WorkStatus = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.WorkStatus);

                        if (!string.IsNullOrEmpty(settingsData.Relationship))
                            db.Relationship = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Relationship);

                        if (!string.IsNullOrEmpty(settingsData.Language))
                            db.Language = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(settingsData.Language);

                        if (!string.IsNullOrEmpty(settingsData.CustomFields))
                            db.CustomFields = JsonConvert.DeserializeObject<List<GetOptionsObject.CustomField>>(settingsData.CustomFields);

                        if (!string.IsNullOrEmpty(settingsData.CurrencyArray))
                            db.CurrencyArray = JsonConvert.DeserializeObject<List<string>>(settingsData.CurrencyArray);

                        if (!string.IsNullOrEmpty(settingsData.CurrencySymbolArray))
                            db.CurrencySymbolArray = JsonConvert.DeserializeObject<Dictionary<string, string>>(settingsData.CurrencySymbolArray);

                        if (!string.IsNullOrEmpty(settingsData.CashfreeCurrencyArray))
                            db.CashfreeCurrencyArray = JsonConvert.DeserializeObject<List<string>>(settingsData.CashfreeCurrencyArray);

                        if (!string.IsNullOrEmpty(settingsData.IyzipayCurrencyArray))
                            db.IyzipayCurrencyArray = JsonConvert.DeserializeObject<List<string>>(settingsData.IyzipayCurrencyArray);

                        if (!string.IsNullOrEmpty(settingsData.ReservedUsernamesArray))
                            db.ReservedUsernamesArray = JsonConvert.DeserializeObject<List<string>>(settingsData.ReservedUsernamesArray);

                        AppSettings.OneSignalAppId = db.PushId;

                        ListUtils.SettingsSiteList = db;

                        return db;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetSettings();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
        }

        //remove Settings
        public void remove_Settings()
        {
            try
            {
                var connection = OpenConnection();
                connection.DeleteAll<DataTables.SettingsTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    remove_Settings();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region My Info Data

        //Insert Or Update data MyInfo 
        public void InsertOrUpdate_DataMyInfo(UserInfoObject info)
        {
            try
            {
                using var connection = OpenConnection();
                var resultInfoTb = connection.Table<DataTables.InfoUsersTb>().FirstOrDefault();
                if (resultInfoTb != null)
                {
                    resultInfoTb.Id = info.Id;
                    resultInfoTb.VerifiedFinal = info.VerifiedFinal;
                    resultInfoTb.CountryTxt = info.CountryTxt;
                    resultInfoTb.WebToken = info.WebToken;
                    resultInfoTb.Password = info.Password;
                    resultInfoTb.Age = info.Age;
                    resultInfoTb.ProfileCompletion = info.ProfileCompletion;
                    resultInfoTb.Avater = info.Avater;
                    resultInfoTb.FullName = info.FullName;
                    resultInfoTb.Username = info.Username;
                    resultInfoTb.Email = info.Email;
                    resultInfoTb.FirstName = info.FirstName;
                    resultInfoTb.LastName = info.LastName;
                    resultInfoTb.Address = info.Address;
                    resultInfoTb.Gender = info.Gender;
                    resultInfoTb.GenderTxt = info.GenderTxt;
                    resultInfoTb.Facebook = info.Facebook;
                    resultInfoTb.Google = info.Google;
                    resultInfoTb.Twitter = info.Twitter;
                    resultInfoTb.Linkedin = info.Linkedin;
                    resultInfoTb.Website = info.Website;
                    resultInfoTb.Instagram = info.Instagram;
                    resultInfoTb.WebDeviceId = info.WebDeviceId;
                    resultInfoTb.Language = info.Language;
                    resultInfoTb.LanguageTxt = info.LanguageTxt;
                    resultInfoTb.EmailCode = info.EmailCode;
                    resultInfoTb.Src = info.Src;
                    resultInfoTb.IpAddress = info.IpAddress;
                    resultInfoTb.Type = info.Type;
                    resultInfoTb.PhoneNumber = info.PhoneNumber;
                    resultInfoTb.Timezone = info.Timezone;
                    resultInfoTb.Lat = info.Lat;
                    resultInfoTb.Lng = info.Lng;
                    resultInfoTb.About = info.About;
                    resultInfoTb.Birthday = info.Birthday;
                    resultInfoTb.Country = info.Country;
                    resultInfoTb.Registered = info.Registered;
                    resultInfoTb.Lastseen = info.Lastseen;
                    resultInfoTb.Smscode = info.Smscode;
                    resultInfoTb.ProTime = info.ProTime;
                    resultInfoTb.LastLocationUpdate = info.LastLocationUpdate;
                    resultInfoTb.Balance = info.Balance;
                    resultInfoTb.Verified = info.Verified;
                    resultInfoTb.Status = info.Status;
                    resultInfoTb.Active = info.Active;
                    resultInfoTb.Admin = info.Admin;
                    resultInfoTb.StartUp = info.StartUp;
                    resultInfoTb.IsPro = info.IsPro;
                    resultInfoTb.ProType = info.ProType;
                    resultInfoTb.SocialLogin = info.SocialLogin;
                    resultInfoTb.CreatedAt = info.CreatedAt;
                    resultInfoTb.UpdatedAt = info.UpdatedAt;
                    resultInfoTb.DeletedAt = info.DeletedAt;
                    resultInfoTb.MobileDeviceId = info.MobileDeviceId;
                    resultInfoTb.MobileToken = info.MobileToken;
                    resultInfoTb.Height = info.Height;
                    resultInfoTb.HeightTxt = info.HeightTxt;
                    resultInfoTb.HairColor = info.HairColor;
                    resultInfoTb.HairColorTxt = info.HairColorTxt;
                    resultInfoTb.WebTokenCreatedAt = info.WebTokenCreatedAt;
                    resultInfoTb.MobileTokenCreatedAt = info.MobileTokenCreatedAt;
                    resultInfoTb.MobileDevice = info.MobileDevice;
                    resultInfoTb.Interest = info.Interest;
                    resultInfoTb.Location = info.Location;
                    resultInfoTb.Relationship = info.Relationship;
                    resultInfoTb.RelationshipTxt = info.RelationshipTxt;
                    resultInfoTb.WorkStatus = info.WorkStatus;
                    resultInfoTb.WorkStatusTxt = info.WorkStatusTxt;
                    resultInfoTb.Education = info.Education;
                    resultInfoTb.EducationTxt = info.EducationTxt;
                    resultInfoTb.Ethnicity = info.Ethnicity;
                    resultInfoTb.EthnicityTxt = info.EthnicityTxt;
                    resultInfoTb.Body = info.Body;
                    resultInfoTb.BodyTxt = info.BodyTxt;
                    resultInfoTb.Character = info.Character;
                    resultInfoTb.CharacterTxt = info.CharacterTxt;
                    resultInfoTb.Children = info.Children;
                    resultInfoTb.ChildrenTxt = info.ChildrenTxt;
                    resultInfoTb.Friends = info.Friends;
                    resultInfoTb.FriendsTxt = info.FriendsTxt;
                    resultInfoTb.Pets = info.Pets;
                    resultInfoTb.PetsTxt = info.PetsTxt;
                    resultInfoTb.LiveWith = info.LiveWith;
                    resultInfoTb.LiveWithTxt = info.LiveWithTxt;
                    resultInfoTb.Car = info.Car;
                    resultInfoTb.CarTxt = info.CarTxt;
                    resultInfoTb.Religion = info.Religion;
                    resultInfoTb.ReligionTxt = info.ReligionTxt;
                    resultInfoTb.Smoke = info.Smoke;
                    resultInfoTb.SmokeTxt = info.SmokeTxt;
                    resultInfoTb.Drink = info.Drink;
                    resultInfoTb.DrinkTxt = info.DrinkTxt;
                    resultInfoTb.Travel = info.Travel;
                    resultInfoTb.TravelTxt = info.TravelTxt;
                    resultInfoTb.Music = info.Music;
                    resultInfoTb.Dish = info.Dish;
                    resultInfoTb.Song = info.Song;
                    resultInfoTb.Hobby = info.Hobby;
                    resultInfoTb.City = info.City;
                    resultInfoTb.Sport = info.Sport;
                    resultInfoTb.Book = info.Book;
                    resultInfoTb.Movie = info.Movie;
                    resultInfoTb.Colour = info.Colour;
                    resultInfoTb.Tv = info.Tv;
                    resultInfoTb.PrivacyShowProfileOnGoogle = info.PrivacyShowProfileOnGoogle;
                    resultInfoTb.PrivacyShowProfileRandomUsers = info.PrivacyShowProfileRandomUsers;
                    resultInfoTb.PrivacyShowProfileMatchProfiles = info.PrivacyShowProfileMatchProfiles;
                    resultInfoTb.EmailOnProfileView = info.EmailOnProfileView;
                    resultInfoTb.EmailOnNewMessage = info.EmailOnNewMessage;
                    resultInfoTb.EmailOnProfileLike = info.EmailOnProfileLike;
                    resultInfoTb.EmailOnPurchaseNotifications = info.EmailOnPurchaseNotifications;
                    resultInfoTb.EmailOnSpecialOffers = info.EmailOnSpecialOffers;
                    resultInfoTb.EmailOnAnnouncements = info.EmailOnAnnouncements;
                    resultInfoTb.PhoneVerified = info.PhoneVerified;
                    resultInfoTb.Online = info.Online;
                    resultInfoTb.IsBoosted = info.IsBoosted;
                    resultInfoTb.BoostedTime = info.BoostedTime;
                    resultInfoTb.IsBuyStickers = info.IsBuyStickers;
                    resultInfoTb.UserBuyXvisits = info.UserBuyXvisits;
                    resultInfoTb.XvisitsCreatedAt = info.XvisitsCreatedAt;
                    resultInfoTb.UserBuyXmatches = info.UserBuyXmatches;
                    resultInfoTb.XmatchesCreatedAt = info.XmatchesCreatedAt;
                    resultInfoTb.UserBuyXlikes = info.UserBuyXlikes;
                    resultInfoTb.XlikesCreatedAt = info.XlikesCreatedAt;
                    resultInfoTb.ShowMeTo = info.ShowMeTo;
                    resultInfoTb.EmailOnGetGift = info.EmailOnGetGift;
                    resultInfoTb.EmailOnGotNewMatch = info.EmailOnGotNewMatch;
                    resultInfoTb.EmailOnChatRequest = info.EmailOnChatRequest;
                    resultInfoTb.LastEmailSent = info.LastEmailSent;
                    resultInfoTb.ApprovedAt = info.ApprovedAt;
                    resultInfoTb.Snapshot = info.Snapshot;
                    resultInfoTb.HotCount = info.HotCount;
                    resultInfoTb.SpamWarning = info.SpamWarning;
                    resultInfoTb.ActivationRequestCount = info.ActivationRequestCount;
                    resultInfoTb.LastActivationRequest = info.LastActivationRequest;
                    resultInfoTb.TwoFactor = info.TwoFactor;
                    resultInfoTb.TwoFactorVerified = info.TwoFactorVerified;
                    resultInfoTb.TwoFactorEmailCode = info.TwoFactorEmailCode;
                    resultInfoTb.NewEmail = info.NewEmail;
                    resultInfoTb.NewPhone = info.NewPhone;
                    resultInfoTb.Permission = info.Permission;
                    resultInfoTb.Referrer = info.Referrer;
                    resultInfoTb.AffBalance = info.AffBalance;
                    resultInfoTb.PaypalEmail = info.PaypalEmail;
                    resultInfoTb.ConfirmFollowers = info.ConfirmFollowers;
                    resultInfoTb.RewardDailyCredit = info.RewardDailyCredit;
                    resultInfoTb.LockProVideo = info.LockProVideo;
                    resultInfoTb.LockPrivatePhoto = info.LockPrivatePhoto;
                    resultInfoTb.LastseenTxt = info.LastseenTxt;
                    resultInfoTb.LastseenDate = info.LastseenDate;
                    resultInfoTb.FullPhoneNumber = info.FullPhoneNumber;
                    resultInfoTb.IsBlocked = info.IsBlocked;
                    resultInfoTb.IsFavorite = info.IsFavorite;
                    resultInfoTb.IsFriend = info.IsFriend;
                    resultInfoTb.IsFriendRequest = info.IsFriendRequest;
                    resultInfoTb.IsLiked = info.IsLiked;
                    resultInfoTb.IsOwner = info.IsOwner;
                    resultInfoTb.LikesCount = info.LikesCount;
                    resultInfoTb.VisitsCount = info.VisitsCount;
                    resultInfoTb.AamarpayTranId = info.AamarpayTranId;
                    resultInfoTb.CcPhoneNumber = info.CcPhoneNumber;
                    resultInfoTb.CoinbaseCode = info.CoinbaseCode;
                    resultInfoTb.CoinbaseHash = info.CoinbaseHash;
                    resultInfoTb.CoinpaymentsTxnId = info.CoinpaymentsTxnId;
                    resultInfoTb.ConversationId = info.ConversationId;
                    resultInfoTb.Discord = info.Discord;
                    resultInfoTb.FortumoHash = info.FortumoHash;
                    resultInfoTb.InfoFile = info.InfoFile;
                    resultInfoTb.Mailru = info.Mailru;
                    resultInfoTb.NgeniusRef = info.NgeniusRef;
                    resultInfoTb.Okru = info.Okru;
                    resultInfoTb.PaystackRef = info.PaystackRef;
                    resultInfoTb.ProIcon = info.ProIcon;
                    resultInfoTb.Qq = info.Qq;
                    resultInfoTb.SecurionpayKey = info.SecurionpayKey;
                    resultInfoTb.State = info.State;
                    resultInfoTb.Wechat = info.Wechat;
                    resultInfoTb.YoomoneyHash = info.YoomoneyHash;
                    resultInfoTb.Zip = info.Zip;

                    Console.WriteLine(resultInfoTb.GenderTxt);

                    //resultInfoTb.Avater = JsonConvert.SerializeObject(info.Avater);
                    resultInfoTb.ProfileCompletionMissing = JsonConvert.SerializeObject(info.ProfileCompletionMissing);
                    resultInfoTb.Mediafiles = JsonConvert.SerializeObject(info.Mediafiles);
                    resultInfoTb.Likes = JsonConvert.SerializeObject(info.Likes);
                    resultInfoTb.Blocks = JsonConvert.SerializeObject(info.Blocks);
                    resultInfoTb.Payments = JsonConvert.SerializeObject(info.Payments);
                    resultInfoTb.Reports = JsonConvert.SerializeObject(info.Reports);
                    resultInfoTb.Visits = JsonConvert.SerializeObject(info.Visits);
                    resultInfoTb.AffPayments = JsonConvert.SerializeObject(info.AffPayments);
                    resultInfoTb.Referrals = JsonConvert.SerializeObject(info.Referrals);
                    connection.Update(resultInfoTb);
                }
                else
                {
                    var db = new DataTables.InfoUsersTb
                    {
                        Id = info.Id,
                        VerifiedFinal = info.VerifiedFinal,
                        CountryTxt = info.CountryTxt,
                        WebToken = info.WebToken,
                        Password = info.Password,
                        Age = info.Age,
                        ProfileCompletion = info.ProfileCompletion,
                        Avater = info.Avater,
                        FullName = info.FullName,
                        Username = info.Username,
                        Email = info.Email,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        Address = info.Address,
                        Gender = info.Gender,
                        GenderTxt = info.GenderTxt,
                        Facebook = info.Facebook,
                        Google = info.Google,
                        Twitter = info.Twitter,
                        Linkedin = info.Linkedin,
                        Website = info.Website,
                        Instagram = info.Instagram,
                        WebDeviceId = info.WebDeviceId,
                        Language = info.Language,
                        LanguageTxt = info.LanguageTxt,
                        EmailCode = info.EmailCode,
                        Src = info.Src,
                        IpAddress = info.IpAddress,
                        Type = info.Type,
                        PhoneNumber = info.PhoneNumber,
                        Timezone = info.Timezone,
                        Lat = info.Lat,
                        Lng = info.Lng,
                        About = info.About,
                        Birthday = info.Birthday,
                        Country = info.Country,
                        Registered = info.Registered,
                        Lastseen = info.Lastseen,
                        Smscode = info.Smscode,
                        ProTime = info.ProTime,
                        LastLocationUpdate = info.LastLocationUpdate,
                        Balance = info.Balance,
                        Verified = info.Verified,
                        Status = info.Status,
                        Active = info.Active,
                        Admin = info.Admin,
                        StartUp = info.StartUp,
                        IsPro = info.IsPro,
                        ProType = info.ProType,
                        SocialLogin = info.SocialLogin,
                        CreatedAt = info.CreatedAt,
                        UpdatedAt = info.UpdatedAt,
                        DeletedAt = info.DeletedAt,
                        MobileDeviceId = info.MobileDeviceId,
                        MobileToken = info.MobileToken,
                        Height = info.Height,
                        HeightTxt = info.HeightTxt,
                        HairColor = info.HairColor,
                        HairColorTxt = info.HairColorTxt,
                        WebTokenCreatedAt = info.WebTokenCreatedAt,
                        MobileTokenCreatedAt = info.MobileTokenCreatedAt,
                        MobileDevice = info.MobileDevice,
                        Interest = info.Interest,
                        Location = info.Location,
                        Relationship = info.Relationship,
                        RelationshipTxt = info.RelationshipTxt,
                        WorkStatus = info.WorkStatus,
                        WorkStatusTxt = info.WorkStatusTxt,
                        Education = info.Education,
                        EducationTxt = info.EducationTxt,
                        Ethnicity = info.Ethnicity,
                        EthnicityTxt = info.EthnicityTxt,
                        Body = info.Body,
                        BodyTxt = info.BodyTxt,
                        Character = info.Character,
                        CharacterTxt = info.CharacterTxt,
                        Children = info.Children,
                        ChildrenTxt = info.ChildrenTxt,
                        Friends = info.Friends,
                        FriendsTxt = info.FriendsTxt,
                        Pets = info.Pets,
                        PetsTxt = info.PetsTxt,
                        LiveWith = info.LiveWith,
                        LiveWithTxt = info.LiveWithTxt,
                        Car = info.Car,
                        CarTxt = info.CarTxt,
                        Religion = info.Religion,
                        ReligionTxt = info.ReligionTxt,
                        Smoke = info.Smoke,
                        SmokeTxt = info.SmokeTxt,
                        Drink = info.Drink,
                        DrinkTxt = info.DrinkTxt,
                        Travel = info.Travel,
                        TravelTxt = info.TravelTxt,
                        Music = info.Music,
                        Dish = info.Dish,
                        Song = info.Song,
                        Hobby = info.Hobby,
                        City = info.City,
                        Sport = info.Sport,
                        Book = info.Book,
                        Movie = info.Movie,
                        Colour = info.Colour,
                        Tv = info.Tv,
                        PrivacyShowProfileOnGoogle = info.PrivacyShowProfileOnGoogle,
                        PrivacyShowProfileRandomUsers = info.PrivacyShowProfileRandomUsers,
                        PrivacyShowProfileMatchProfiles = info.PrivacyShowProfileMatchProfiles,
                        EmailOnProfileView = info.EmailOnProfileView,
                        EmailOnNewMessage = info.EmailOnNewMessage,
                        EmailOnProfileLike = info.EmailOnProfileLike,
                        EmailOnPurchaseNotifications = info.EmailOnPurchaseNotifications,
                        EmailOnSpecialOffers = info.EmailOnSpecialOffers,
                        EmailOnAnnouncements = info.EmailOnAnnouncements,
                        PhoneVerified = info.PhoneVerified,
                        Online = info.Online,
                        IsBoosted = info.IsBoosted,
                        BoostedTime = info.BoostedTime,
                        IsBuyStickers = info.IsBuyStickers,
                        UserBuyXvisits = info.UserBuyXvisits,
                        XvisitsCreatedAt = info.XvisitsCreatedAt,
                        UserBuyXmatches = info.UserBuyXmatches,
                        XmatchesCreatedAt = info.XmatchesCreatedAt,
                        UserBuyXlikes = info.UserBuyXlikes,
                        XlikesCreatedAt = info.XlikesCreatedAt,
                        ShowMeTo = info.ShowMeTo,
                        EmailOnGetGift = info.EmailOnGetGift,
                        EmailOnGotNewMatch = info.EmailOnGotNewMatch,
                        EmailOnChatRequest = info.EmailOnChatRequest,
                        LastEmailSent = info.LastEmailSent,
                        ApprovedAt = info.ApprovedAt,
                        Snapshot = info.Snapshot,
                        HotCount = info.HotCount,
                        SpamWarning = info.SpamWarning,
                        ActivationRequestCount = info.ActivationRequestCount,
                        LastActivationRequest = info.LastActivationRequest,
                        TwoFactor = info.TwoFactor,
                        TwoFactorVerified = info.TwoFactorVerified,
                        TwoFactorEmailCode = info.TwoFactorEmailCode,
                        NewEmail = info.NewEmail,
                        NewPhone = info.NewPhone,
                        Permission = info.Permission,
                        Referrer = info.Referrer,
                        AffBalance = info.AffBalance,
                        PaypalEmail = info.PaypalEmail,
                        ConfirmFollowers = info.ConfirmFollowers,
                        RewardDailyCredit = info.RewardDailyCredit,
                        LockProVideo = info.LockProVideo,
                        LockPrivatePhoto = info.LockPrivatePhoto,
                        LastseenTxt = info.LastseenTxt,
                        LastseenDate = info.LastseenDate,
                        FullPhoneNumber = info.FullPhoneNumber,
                        IsBlocked = info.IsBlocked,
                        IsFavorite = info.IsFavorite,
                        IsFriend = info.IsFriend,
                        IsFriendRequest = info.IsFriendRequest,
                        IsLiked = info.IsLiked,
                        IsOwner = info.IsOwner,
                        LikesCount = info.LikesCount,
                        VisitsCount = info.VisitsCount,
                        AamarpayTranId = info.AamarpayTranId,
                        CcPhoneNumber = info.CcPhoneNumber,
                        CoinbaseCode = info.CoinbaseCode,
                        CoinbaseHash = info.CoinbaseHash,
                        CoinpaymentsTxnId = info.CoinpaymentsTxnId,
                        ConversationId = info.ConversationId,
                        Discord = info.Discord,
                        FortumoHash = info.FortumoHash,
                        InfoFile = info.InfoFile,
                        Mailru = info.Mailru,
                        NgeniusRef = info.NgeniusRef,
                        Okru = info.Okru,
                        PaystackRef = info.PaystackRef,
                        ProIcon = info.ProIcon,
                        Qq = info.Qq,
                        SecurionpayKey = info.SecurionpayKey,
                        State = info.State,
                        Wechat = info.Wechat,
                        YoomoneyHash = info.YoomoneyHash,
                        Zip = info.Zip,
                        //db.Avater = JsonConvert.SerializeObject(info.Avater);
                        ProfileCompletionMissing = JsonConvert.SerializeObject(info.ProfileCompletionMissing),
                        Mediafiles = JsonConvert.SerializeObject(info.Mediafiles),
                        Likes = JsonConvert.SerializeObject(info.Likes),
                        Blocks = JsonConvert.SerializeObject(info.Blocks),
                        Payments = JsonConvert.SerializeObject(info.Payments),
                        Reports = JsonConvert.SerializeObject(info.Reports),
                        Visits = JsonConvert.SerializeObject(info.Visits),
                        AffPayments = JsonConvert.SerializeObject(info.AffPayments),
                        Referrals = JsonConvert.SerializeObject(info.Referrals)
                    };

                    connection.Insert(db);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdate_DataMyInfo(info);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get Data My Info
        public UserInfoObject GetDataMyInfo()
        {
            try
            {
                using var connection = OpenConnection();
                DataTables.InfoUsersTb info = connection.Table<DataTables.InfoUsersTb>().FirstOrDefault();
                if (info != null)
                {
                    UserInfoObject infoObject = new UserInfoObject
                    {
                        Id = info.Id,
                        VerifiedFinal = info.VerifiedFinal,
                        CountryTxt = info.CountryTxt,
                        WebToken = info.WebToken,
                        Password = info.Password,
                        Age = info.Age,
                        ProfileCompletion = info.ProfileCompletion,
                        Avater = info.Avater,
                        FullName = info.FullName,
                        Username = info.Username,
                        Email = info.Email,
                        FirstName = info.FirstName,
                        LastName = info.LastName,
                        Address = info.Address,
                        Gender = info.Gender,
                        GenderTxt = info.GenderTxt,
                        Facebook = info.Facebook,
                        Google = info.Google,
                        Twitter = info.Twitter,
                        Linkedin = info.Linkedin,
                        Website = info.Website,
                        Instagram = info.Instagram,
                        WebDeviceId = info.WebDeviceId,
                        Language = info.Language,
                        LanguageTxt = info.LanguageTxt,
                        EmailCode = info.EmailCode,
                        Src = info.Src,
                        IpAddress = info.IpAddress,
                        Type = info.Type,
                        PhoneNumber = info.PhoneNumber,
                        Timezone = info.Timezone,
                        Lat = info.Lat,
                        Lng = info.Lng,
                        About = info.About,
                        Birthday = info.Birthday,
                        Country = info.Country,
                        Registered = info.Registered,
                        Lastseen = info.Lastseen,
                        Smscode = info.Smscode,
                        ProTime = info.ProTime,
                        LastLocationUpdate = info.LastLocationUpdate,
                        Balance = info.Balance,
                        Verified = info.Verified,
                        Status = info.Status,
                        Active = info.Active,
                        Admin = info.Admin,
                        StartUp = info.StartUp,
                        IsPro = info.IsPro,
                        ProType = info.ProType,
                        SocialLogin = info.SocialLogin,
                        CreatedAt = info.CreatedAt,
                        UpdatedAt = info.UpdatedAt,
                        DeletedAt = info.DeletedAt,
                        MobileDeviceId = info.MobileDeviceId,
                        MobileToken = info.MobileToken,
                        Height = info.Height,
                        HeightTxt = info.HeightTxt,
                        HairColor = info.HairColor,
                        HairColorTxt = info.HairColorTxt,
                        WebTokenCreatedAt = info.WebTokenCreatedAt,
                        MobileTokenCreatedAt = info.MobileTokenCreatedAt,
                        MobileDevice = info.MobileDevice,
                        Interest = info.Interest,
                        Location = info.Location,
                        Relationship = info.Relationship,
                        RelationshipTxt = info.RelationshipTxt,
                        WorkStatus = info.WorkStatus,
                        WorkStatusTxt = info.WorkStatusTxt,
                        Education = info.Education,
                        EducationTxt = info.EducationTxt,
                        Ethnicity = info.Ethnicity,
                        EthnicityTxt = info.EthnicityTxt,
                        Body = info.Body,
                        BodyTxt = info.BodyTxt,
                        Character = info.Character,
                        CharacterTxt = info.CharacterTxt,
                        Children = info.Children,
                        ChildrenTxt = info.ChildrenTxt,
                        Friends = info.Friends,
                        FriendsTxt = info.FriendsTxt,
                        Pets = info.Pets,
                        PetsTxt = info.PetsTxt,
                        LiveWith = info.LiveWith,
                        LiveWithTxt = info.LiveWithTxt,
                        Car = info.Car,
                        CarTxt = info.CarTxt,
                        Religion = info.Religion,
                        ReligionTxt = info.ReligionTxt,
                        Smoke = info.Smoke,
                        SmokeTxt = info.SmokeTxt,
                        Drink = info.Drink,
                        DrinkTxt = info.DrinkTxt,
                        Travel = info.Travel,
                        TravelTxt = info.TravelTxt,
                        Music = info.Music,
                        Dish = info.Dish,
                        Song = info.Song,
                        Hobby = info.Hobby,
                        City = info.City,
                        Sport = info.Sport,
                        Book = info.Book,
                        Movie = info.Movie,
                        Colour = info.Colour,
                        Tv = info.Tv,
                        PrivacyShowProfileOnGoogle = info.PrivacyShowProfileOnGoogle,
                        PrivacyShowProfileRandomUsers = info.PrivacyShowProfileRandomUsers,
                        PrivacyShowProfileMatchProfiles = info.PrivacyShowProfileMatchProfiles,
                        EmailOnProfileView = info.EmailOnProfileView,
                        EmailOnNewMessage = info.EmailOnNewMessage,
                        EmailOnProfileLike = info.EmailOnProfileLike,
                        EmailOnPurchaseNotifications = info.EmailOnPurchaseNotifications,
                        EmailOnSpecialOffers = info.EmailOnSpecialOffers,
                        EmailOnAnnouncements = info.EmailOnAnnouncements,
                        PhoneVerified = info.PhoneVerified,
                        Online = info.Online,
                        IsBoosted = info.IsBoosted,
                        BoostedTime = info.BoostedTime,
                        IsBuyStickers = info.IsBuyStickers,
                        UserBuyXvisits = info.UserBuyXvisits,
                        XvisitsCreatedAt = info.XvisitsCreatedAt,
                        UserBuyXmatches = info.UserBuyXmatches,
                        XmatchesCreatedAt = info.XmatchesCreatedAt,
                        UserBuyXlikes = info.UserBuyXlikes,
                        XlikesCreatedAt = info.XlikesCreatedAt,
                        ShowMeTo = info.ShowMeTo,
                        EmailOnGetGift = info.EmailOnGetGift,
                        EmailOnGotNewMatch = info.EmailOnGotNewMatch,
                        EmailOnChatRequest = info.EmailOnChatRequest,
                        LastEmailSent = info.LastEmailSent,
                        ApprovedAt = info.ApprovedAt,
                        Snapshot = info.Snapshot,
                        HotCount = info.HotCount,
                        SpamWarning = info.SpamWarning,
                        ActivationRequestCount = info.ActivationRequestCount,
                        LastActivationRequest = info.LastActivationRequest,
                        TwoFactor = info.TwoFactor,
                        TwoFactorVerified = info.TwoFactorVerified,
                        TwoFactorEmailCode = info.TwoFactorEmailCode,
                        NewEmail = info.NewEmail,
                        NewPhone = info.NewPhone,
                        Permission = info.Permission,
                        Referrer = info.Referrer,
                        AffBalance = info.AffBalance,
                        PaypalEmail = info.PaypalEmail,
                        ConfirmFollowers = info.ConfirmFollowers,
                        RewardDailyCredit = info.RewardDailyCredit,
                        LockProVideo = info.LockProVideo,
                        LockPrivatePhoto = info.LockPrivatePhoto,
                        LastseenTxt = info.LastseenTxt,
                        LastseenDate = info.LastseenDate,
                        FullPhoneNumber = info.FullPhoneNumber,
                        IsBlocked = info.IsBlocked,
                        IsFavorite = info.IsFavorite,
                        IsFriend = info.IsFriend,
                        IsFriendRequest = info.IsFriendRequest,
                        IsLiked = info.IsLiked,
                        IsOwner = info.IsOwner,
                        LikesCount = info.LikesCount,
                        VisitsCount = info.VisitsCount,
                        AamarpayTranId = info.AamarpayTranId,
                        CcPhoneNumber = info.CcPhoneNumber,
                        CoinbaseCode = info.CoinbaseCode,
                        CoinbaseHash = info.CoinbaseHash,
                        CoinpaymentsTxnId = info.CoinpaymentsTxnId,
                        ConversationId = info.ConversationId,
                        Discord = info.Discord,
                        FortumoHash = info.FortumoHash,
                        InfoFile = info.InfoFile,
                        Mailru = info.Mailru,
                        NgeniusRef = info.NgeniusRef,
                        Okru = info.Okru,
                        PaystackRef = info.PaystackRef,
                        ProIcon = info.ProIcon,
                        Qq = info.Qq,
                        SecurionpayKey = info.SecurionpayKey,
                        State = info.State,
                        Wechat = info.Wechat,
                        YoomoneyHash = info.YoomoneyHash,
                        Zip = info.Zip,
                        ProfileCompletionMissing = new List<string>(),
                        Mediafiles = new List<MediaFile>(),
                        Likes = new List<Like>(),
                        Blocks = new List<Block>(),
                        Payments = new List<Payment>(),
                        Reports = new List<Report>(),
                        Visits = new List<Visit>(),
                        AffPayments = new List<AffPayment>(),
                        Referrals = new List<UserInfoObject>(),
                    };

                    infoObject.ProfileCompletionMissing = JsonConvert.DeserializeObject<List<string>>(info.ProfileCompletionMissing);
                    infoObject.Mediafiles = JsonConvert.DeserializeObject<List<MediaFile>>(info.Mediafiles);
                    infoObject.Likes = JsonConvert.DeserializeObject<List<Like>>(info.Likes);
                    infoObject.Blocks = JsonConvert.DeserializeObject<List<Block>>(info.Blocks);
                    infoObject.Payments = JsonConvert.DeserializeObject<List<Payment>>(info.Payments);
                    infoObject.Reports = JsonConvert.DeserializeObject<List<Report>>(info.Reports);
                    infoObject.Visits = JsonConvert.DeserializeObject<List<Visit>>(info.Visits);
                    infoObject.AffPayments = JsonConvert.DeserializeObject<List<AffPayment>>(info.AffPayments);
                    infoObject.Referrals = JsonConvert.DeserializeObject<List<UserInfoObject>>(info.Referrals);
                    //infoObject.Avater = new AvaterUnion()
                    //{
                    //    PurpleUri = JsonConvert.DeserializeObject<string>(myInfo.Avater)
                    //};

                    ListUtils.MyUserInfo.Clear();
                    ListUtils.MyUserInfo.Add(infoObject);

                    return infoObject;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetDataMyInfo();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
        }

        #endregion

        #region Gifts

        //Insert data Gifts
        public void InsertAllGifts(ObservableCollection<DataFile> listData)
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.GiftsTb>().ToList();
                List<DataTables.GiftsTb> list = new List<DataTables.GiftsTb>();
                foreach (var gift in listData)
                {
                    var item = new DataTables.GiftsTb
                    {
                        IdGifts = gift.Id,
                        File = gift.File,
                    };
                    list.Add(item);

                    var update = result.FirstOrDefault(a => a.IdGifts == gift.Id);
                    if (update != null)
                    {
                        update = item;
                        connection.Update(update);
                    }
                }

                if (list.Count > 0)
                {
                    connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.IdGifts).Contains(c.IdGifts)).ToList();
                    if (newItemList.Count > 0)
                    {
                        connection.InsertAll(newItemList);
                    }

                    result = connection.Table<DataTables.GiftsTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.IdGifts).Contains(c.IdGifts)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            connection.Delete(delete);

                    connection.Commit();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertAllGifts(listData);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get List Gifts 
        public ObservableCollection<DataFile> GetGiftsList()
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.GiftsTb>().ToList();
                if (result?.Count > 0)
                {
                    List<DataFile> list = result.Select(gift => new DataFile
                    {
                        Id = gift.IdGifts,
                        File = gift.File,
                    }).ToList();

                    return new ObservableCollection<DataFile>(list);
                }
                else
                {
                    return new ObservableCollection<DataFile>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetGiftsList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<DataFile>();
                }
            }
        }

        #endregion

        #region Stickers

        //Insert data Stickers
        public void InsertAllStickers(ObservableCollection<DataFile> listData)
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.StickersTb>().ToList();
                List<DataTables.StickersTb> list = new List<DataTables.StickersTb>();
                foreach (var stickers in listData)
                {
                    var item = new DataTables.StickersTb
                    {
                        IdStickers = stickers.Id,
                        File = stickers.File,
                    };
                    list.Add(item);

                    var update = result.FirstOrDefault(a => a.IdStickers == stickers.Id);
                    if (update != null)
                    {
                        update = item;
                        connection.Update(update);
                    }
                }

                if (list.Count > 0)
                {
                    connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.IdStickers).Contains(c.IdStickers)).ToList();
                    if (newItemList.Count > 0)
                    {
                        connection.InsertAll(newItemList);
                    }

                    result = connection.Table<DataTables.StickersTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.IdStickers).Contains(c.IdStickers)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            connection.Delete(delete);

                    connection.Commit();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertAllStickers(listData);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get List Stickers 
        public ObservableCollection<DataFile> GetStickersList()
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.StickersTb>().ToList();
                if (result?.Count > 0)
                {
                    List<DataFile> list = result.Select(stickers => new DataFile
                    {
                        Id = stickers.IdStickers,
                        File = stickers.File,
                    }).ToList();

                    return new ObservableCollection<DataFile>(list);
                }
                else
                {
                    return new ObservableCollection<DataFile>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetStickersList();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<DataFile>();
                }
            }
        }

        #endregion

        #region Last Chat

        //Insert data To Last Chat Table
        public void InsertOrReplaceLastChatTable(ObservableCollection<GetConversationListObject.DataConversation> usersContactList)
        {
            try
            {
                using var connection = OpenConnection();
                var result = connection.Table<DataTables.LastChatTb>().ToList();
                List<DataTables.LastChatTb> list = new List<DataTables.LastChatTb>();
                foreach (var user in usersContactList)
                {
                    var item = new DataTables.LastChatTb
                    {
                        Id = user.Id,
                        Owner = user.Owner,
                        Seen = user.Seen,
                        Text = user.Text,
                        Media = user.Media,
                        Sticker = user.Sticker,
                        Time = user.Time,
                        CreatedAt = user.CreatedAt,
                        UserId = user.User.Id.ToString(),
                        NewMessages = user.NewMessages,
                        MessageType = user.MessageType,
                        UserDataJson = JsonConvert.SerializeObject(user.User),
                        Accepted = user.Accepted,
                        ConversationStatus = user.ConversationStatus,
                        FromId = user.FromId?.ToString(),
                        ToId = user.ToId?.ToString(),
                        ConversationCreatedAt = user.ConversationCreatedAt,
                    };

                    list.Add(item);

                    var update = result.FirstOrDefault(a => a.Id == user.Id);
                    if (update != null)
                    {
                        update = item;
                        if (user.User != null)
                            update.UserDataJson = JsonConvert.SerializeObject(user.User);

                        connection.Update(update);
                    }
                }

                if (list.Count > 0)
                {
                    connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (newItemList.Count > 0)
                    {
                        connection.InsertAll(newItemList);
                    }

                    result = connection.Table<DataTables.LastChatTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            connection.Delete(delete);

                    connection.Commit();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrReplaceLastChatTable(usersContactList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To LastChat Table
        public ObservableCollection<GetConversationListObject.DataConversation> GetAllLastChat()
        {
            try
            {
                using var connection = OpenConnection();
                var select = connection.Table<DataTables.LastChatTb>().ToList();
                if (select.Count > 0)
                {
                    List<GetConversationListObject.DataConversation> list = new List<GetConversationListObject.DataConversation>();
                    foreach (var user in select)
                    {
                        var item = new GetConversationListObject.DataConversation
                        {
                            Id = user.Id,
                            Owner = user.Owner,
                            Seen = user.Seen,
                            Text = user.Text,
                            Media = user.Media,
                            Sticker = user.Sticker,
                            Time = user.Time,
                            CreatedAt = user.CreatedAt,
                            NewMessages = user.NewMessages,
                            MessageType = user.MessageType,
                            Accepted = user.Accepted,
                            ConversationStatus = user.ConversationStatus,
                            ConversationCreatedAt = user.ConversationCreatedAt,
                            User = new UserInfoObject(),
                        };

                        if (!string.IsNullOrEmpty(user.ToId)) item.ToId = Convert.ToInt32(user.ToId);
                        if (!string.IsNullOrEmpty(user.FromId)) item.FromId = Convert.ToInt32(user.FromId);

                        if (user.UserDataJson != null)
                            item.User = JsonConvert.DeserializeObject<UserInfoObject>(user.UserDataJson);

                        list.Add(item);
                    }
                    return new ObservableCollection<GetConversationListObject.DataConversation>(list);
                }
                else
                    return new ObservableCollection<GetConversationListObject.DataConversation>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetAllLastChat();
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GetConversationListObject.DataConversation>();
                }
            }
        }

        //Remove data To LastChat Table
        public void DeleteUserLastChat(string userId)
        {
            try
            {
                using var connection = OpenConnection();
                var user = connection.Table<DataTables.LastChatTb>().FirstOrDefault(c => c.UserId == userId);
                if (user != null)
                {
                    connection.Delete(user);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteUserLastChat(userId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Clear All data LastChat
        public void ClearLastChat()
        {
            try
            {
                using var connection = OpenConnection();
                connection.DeleteAll<DataTables.LastChatTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearLastChat();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void InsertOrReplaceMessages(ObservableCollection<GetChatConversationsObject.Messages> messageList)
        {
            try
            {
                using var connection = OpenConnection();
                List<DataTables.MessageTb> listOfDatabaseForInsert = new List<DataTables.MessageTb>();

                // get data from database
                var resultMessage = connection.Table<DataTables.MessageTb>().ToList();
                var listAllMessage = resultMessage.Select(messages => new GetChatConversationsObject.Messages
                {
                    Id = messages.Id,
                    FromName = messages.FromName,
                    FromAvater = messages.FromAvater,
                    ToName = messages.ToName,
                    ToAvater = messages.ToAvater,
                    From = messages.FromId,
                    To = messages.ToId,
                    Text = messages.Text,
                    Media = messages.Media,
                    FromDelete = messages.FromDelete,
                    ToDelete = messages.ToDelete,
                    Sticker = messages.Sticker,
                    CreatedAt = messages.CreatedAt,
                    Seen = messages.Seen,
                    Type = messages.Type,
                    MessageType = messages.MessageType,
                }).ToList();

                foreach (var messages in messageList)
                {
                    DataTables.MessageTb maTb = new DataTables.MessageTb
                    {
                        Id = messages.Id,
                        FromName = messages.FromName,
                        FromAvater = messages.FromAvater,
                        ToName = messages.ToName,
                        ToAvater = messages.ToAvater,
                        FromId = messages.From,
                        ToId = messages.To,
                        Text = messages.Text,
                        Media = messages.Media,
                        FromDelete = messages.FromDelete,
                        ToDelete = messages.ToDelete,
                        Sticker = messages.Sticker,
                        CreatedAt = messages.CreatedAt,
                        Seen = messages.Seen,
                        Type = messages.Type,
                        MessageType = messages.MessageType,
                    };

                    var dataCheck = listAllMessage.FirstOrDefault(a => a.Id == messages.Id);
                    if (dataCheck != null)
                    {
                        var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                        if (checkForUpdate != null)
                        {
                            checkForUpdate.Id = messages.Id;
                            checkForUpdate.FromName = messages.FromName;
                            checkForUpdate.FromAvater = messages.FromAvater;
                            checkForUpdate.ToName = messages.ToName;
                            checkForUpdate.ToAvater = messages.ToAvater;
                            checkForUpdate.FromId = messages.From;
                            checkForUpdate.ToId = messages.To;
                            checkForUpdate.Text = messages.Text;
                            checkForUpdate.Media = messages.Media;
                            checkForUpdate.FromDelete = messages.FromDelete;
                            checkForUpdate.ToDelete = messages.ToDelete;
                            checkForUpdate.Sticker = messages.Sticker;
                            checkForUpdate.CreatedAt = messages.CreatedAt;
                            checkForUpdate.Seen = messages.Seen;
                            checkForUpdate.Type = messages.Type;
                            checkForUpdate.MessageType = messages.MessageType;

                            connection.Update(checkForUpdate);
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }
                    else
                    {
                        listOfDatabaseForInsert.Add(maTb);
                    }
                }

                connection.BeginTransaction();

                //Bring new  
                if (listOfDatabaseForInsert.Count > 0)
                {
                    connection.InsertAll(listOfDatabaseForInsert);
                }

                connection.Commit();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrReplaceMessages(messageList);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Update one Messages Table
        public void InsertOrUpdateToOneMessages(GetChatConversationsObject.Messages message)
        {
            try
            {
                using var connection = OpenConnection();
                var data = connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == message.Id);
                if (data != null)
                {
                    data.Id = message.Id;
                    data.FromName = message.FromName;
                    data.FromAvater = message.FromAvater;
                    data.ToName = message.ToName;
                    data.ToAvater = message.ToAvater;
                    data.FromId = message.From;
                    data.ToId = message.To;
                    data.Text = message.Text;
                    data.Media = message.Media;
                    data.FromDelete = message.FromDelete;
                    data.ToDelete = message.ToDelete;
                    data.Sticker = message.Sticker;
                    data.CreatedAt = message.CreatedAt;
                    data.Seen = message.Seen;
                    data.Type = message.Type;
                    data.MessageType = message.MessageType;
                    connection.Update(data);
                }
                else
                {
                    DataTables.MessageTb mdb = new DataTables.MessageTb
                    {
                        Id = message.Id,
                        FromName = message.FromName,
                        FromAvater = message.FromAvater,
                        ToName = message.ToName,
                        ToAvater = message.ToAvater,
                        FromId = message.From,
                        ToId = message.To,
                        Text = message.Text,
                        Media = message.Media,
                        FromDelete = message.FromDelete,
                        ToDelete = message.ToDelete,
                        Sticker = message.Sticker,
                        CreatedAt = message.CreatedAt,
                        Seen = message.Seen,
                        Type = message.Type,
                        MessageType = message.MessageType,
                    };

                    //Insert  one Messages Table
                    connection.Insert(mdb);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    InsertOrUpdateToOneMessages(message);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Get data To Messages
        public ObservableCollection<GetChatConversationsObject.Messages> GetMessagesList(long fromId, long toId, long beforeMessageId)
        {
            try
            {
                using var connection = OpenConnection();
                var beforeQ = "";
                if (beforeMessageId != 0)
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query = connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);
                List<DataTables.MessageTb> queryList = query.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).OrderBy(q => q.CreatedAt).TakeLast(35).ToList();
                if (queryList.Count > 0)
                {
                    ObservableCollection<GetChatConversationsObject.Messages> list = new ObservableCollection<GetChatConversationsObject.Messages>();
                    foreach (var m in queryList.Select(message => new GetChatConversationsObject.Messages
                    {
                        Id = message.Id,
                        FromName = message.FromName,
                        FromAvater = message.FromAvater,
                        ToName = message.ToName,
                        ToAvater = message.ToAvater,
                        From = message.FromId,
                        To = message.ToId,
                        Text = message.Text,
                        Media = message.Media,
                        FromDelete = message.FromDelete,
                        ToDelete = message.ToDelete,
                        Sticker = message.Sticker,
                        CreatedAt = message.CreatedAt,
                        Seen = message.Seen,
                        Type = message.Type,
                        MessageType = message.MessageType,
                    }))
                    {
                        if (beforeMessageId == 0)
                        {
                            list.Add(m);
                        }
                        else
                        {
                            list.Insert(0, m);
                        }
                    }

                    return list;
                }
                else
                {
                    return new ObservableCollection<GetChatConversationsObject.Messages>();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessagesList(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new ObservableCollection<GetChatConversationsObject.Messages>();
                }
            }
        }

        //Get data To where first Messages >> load more
        public List<DataTables.MessageTb> GetMessageList(long fromId, long toId, long beforeMessageId)
        {
            try
            {
                using var connection = OpenConnection();
                var beforeQ = "";
                if (beforeMessageId != 0)
                {
                    beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                }

                var query = connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);
                List<DataTables.MessageTb> queryList = query
                    .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                    .OrderBy(q => q.CreatedAt).TakeLast(35).ToList();
                return queryList;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    return GetMessageList(fromId, toId, beforeMessageId);
                else
                {
                    Methods.DisplayReportResultTrack(e);
                    return new List<DataTables.MessageTb>();
                }
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(int messageId)
        {
            try
            {
                using var connection = OpenConnection();
                var user = connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id == messageId);
                if (user != null)
                {
                    connection.Delete(user);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    Delete_OneMessageUser(messageId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                using var connection = OpenConnection();
                var query = connection.Query<DataTables.MessageTb>("Delete FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                Console.WriteLine(query);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    DeleteAllMessagesUser(fromId, toId);
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        //Remove All data To Messages Table
        public void ClearAll_Messages()
        {
            try
            {
                using var connection = OpenConnection();
                connection.DeleteAll<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("database is locked"))
                    ClearAll_Messages();
                else
                    Methods.DisplayReportResultTrack(e);
            }
        }

        #endregion

    }
}


