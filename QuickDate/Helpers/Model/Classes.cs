using QuickDateClient.Classes.Global;
using System.Collections.Generic;

namespace QuickDate.Helpers.Model
{
    public class Classes
    {
        public class Languages
        {
            public string LanguagesId { get; set; }
            public string LanguagesName { get; set; }
        }

        public class TrendingClass
        {
            public long Id { get; set; }
            public ItemType Type { get; set; }
            public List<UserInfoObject> ProUserList { get; set; }
            public List<UserInfoObject> HotOrNotList { get; set; }
            public UserInfoObject UsersData { get; set; }
        }

        public class CategoriesClass
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Color1 { get; set; }
            public string Color2 { get; set; }
        }

        public class ItemOptionObject
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public int Icon { get; set; }
            public string Color { get; set; }
        }
    }

    public enum ItemType
    {
        ProUser = 100, HotOrNot = 200, Users = 300, EmptyPage = 400
    }

    public enum PaymentsSystem
    {
        All = 1, JustInAppBillingGoogle = 2
    }


    public enum ShowAds
    {
        AllUsers,
        UnProfessional,
    }

    public enum UpdateGenderSystem
    {
        All,
        JustWhenRegister
    }

    public enum TabTheme
    {
        Dark,
        Light,
    }

    public enum BackgroundTheme
    {
        Color,
        Image,
    }
}