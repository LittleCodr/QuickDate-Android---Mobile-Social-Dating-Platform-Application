using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using SQLite;

namespace QuickDate.SQLite
{
    public class DataTables
    {
        [Table("LoginTb")]
        public class LoginTb
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdLogin { get; set; }

            public string UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AccessToken { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Lang { get; set; }
            public string DeviceId { get; set; }
        }

        [Table("SettingsTb")]
        public class SettingsTb : GetOptionsObject.DataOptions
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdSettings { get; set; }

            public new string Height { get; set; }
            public new string Notification { get; set; }
            public new string Gender { get; set; }
            public new string BlogCategories { get; set; }
            public new string Countries { get; set; }
            public new string HairColor { get; set; }
            public new string Travel { get; set; }
            public new string Drink { get; set; }
            public new string Smoke { get; set; }
            public new string Religion { get; set; }
            public new string Car { get; set; }
            public new string LiveWith { get; set; }
            public new string Pets { get; set; }
            public new string Friends { get; set; }
            public new string Children { get; set; }
            public new string Character { get; set; }
            public new string Body { get; set; }
            public new string Ethnicity { get; set; }
            public new string Education { get; set; }
            public new string WorkStatus { get; set; }
            public new string Relationship { get; set; }
            public new string Language { get; set; }
            public new string CustomFields { get; set; }
            public new string CurrencyArray { get; set; }
            public new string CurrencySymbolArray { get; set; }
            public new string CashfreeCurrencyArray { get; set; }
            public new string IyzipayCurrencyArray { get; set; }
            public new string ReservedUsernamesArray { get; set; }
        }

        [Table("InfoUsersTb")]
        public class InfoUsersTb : UserInfoObject
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdInfoUsers { get; set; }

            //public new string Avater { get; set; }
            public new string ProfileCompletionMissing { get; set; }
            public new string Mediafiles { get; set; }
            public new string Likes { get; set; }
            public new string Blocks { get; set; }
            public new string Payments { get; set; }
            public new string Reports { get; set; }
            public new string Visits { get; set; }
            public new string Referrals { get; set; }
            public new string AffPayments { get; set; }
            public new string UserData { get; set; }
        }

        [Table("FilterOptionsTb")]
        public class FilterOptionsTb
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdFilterOptions { get; set; }
            public long AgeMin { get; set; }
            public long AgeMax { get; set; }
            public string Gender { get; set; }
            public string Location { get; set; }
            public bool IsOnline { get; set; }
            public string Distance { get; set; }
            public string Language { get; set; }
            public string Ethnicity { get; set; }
            public string Religion { get; set; }
            public string RelationShip { get; set; }
            public string Smoke { get; set; }
            public string Drink { get; set; }
            public string Body { get; set; }
            public string FromHeight { get; set; }
            public string ToHeight { get; set; }
            public string Interest { get; set; }
            public string Education { get; set; }
            public string Pets { get; set; }
        }

        [Table("GiftsTb")]
        public class GiftsTb
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdGifts { get; set; }

            public long IdGifts { get; set; }
            public string File { get; set; }
        }

        [Table("StickersTb")]
        public class StickersTb
        {
            [PrimaryKey, AutoIncrement]
            public long AutoIdStickers { get; set; }

            public long IdStickers { get; set; }
            public string File { get; set; }
        }

        [Table("LastChatTb")]
        public class LastChatTb
        {
            [PrimaryKey, AutoIncrement] public long AutoIdLastChat { get; set; }

            public long ConversationStatus { get; set; }
            public string ConversationCreatedAt { get; set; }
            public long Id { get; set; }
            public long Owner { get; set; }
            public string UserDataJson { get; set; }
            public long Seen { get; set; }
            public long Accepted { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public string Sticker { get; set; }
            public string Time { get; set; }
            public string CreatedAt { get; set; }
            public string UserId { get; set; }
            public long NewMessages { get; set; }
            public string MessageType { get; set; }
            public string ToId { get; set; }
            public string FromId { get; set; }
        }

        [Table("MessageTb")]
        public class MessageTb
        {
            [PrimaryKey, AutoIncrement] public long AutoIdMessage { get; set; }

            public long Id { get; set; }
            public string FromName { get; set; }
            public string FromAvater { get; set; }
            public string ToName { get; set; }
            public string ToAvater { get; set; }
            public long FromId { get; set; }
            public long ToId { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public long FromDelete { get; set; }
            public long ToDelete { get; set; }
            public string Sticker { get; set; }
            public string CreatedAt { get; set; }
            public long Seen { get; set; }
            public string Type { get; set; }
            public string MessageType { get; set; }
        }
    }
}