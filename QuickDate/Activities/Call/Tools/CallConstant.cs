using QuickDateClient.Classes.Call;
using System.Collections.Generic;

namespace QuickDate.Activities.Call.Tools
{
    public class CallConstant
    {
        public static string CallType { get; set; }
        public static DataCallObject CallUserObject { get; set; }
        public static List<DataCallObject> CallUserList = new List<DataCallObject>();

        public static TypeCall TypeCall { get; set; }

        public static bool CallActive { get; set; }
        public static bool IsCallActivityVisible { get; set; }
        public static bool IsSpeakerEnabled { get; set; }
    }
}
