using Android.Content;
using Android.Widget;
using System.Collections.Generic;
using Exception = Java.Lang.Exception;

namespace QuickDate.Helpers.Utils
{
    public interface IDialogListCallBack
    {
        public void OnSelection(IDialogInterface dialog, int position, string itemString);
    }

    public interface IDialogInputCallBack
    {
        public void OnInput(IDialogInterface dialog, string input);
    }

    public class MaterialDialogUtils : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        private readonly string Type;
        private readonly List<string> ArrayAdapter;
        private readonly EditText EditText;
        private readonly IDialogListCallBack ListCallBack;
        private readonly IDialogInputCallBack InputCallBack;

        public MaterialDialogUtils()
        {

        }

        public MaterialDialogUtils(List<string> arrayAdapter, IDialogListCallBack callBack)
        {
            Type = "List";
            ArrayAdapter = arrayAdapter;
            ListCallBack = callBack;
        }

        public MaterialDialogUtils(EditText editText, IDialogInputCallBack callBack)
        {
            Type = "Input";
            EditText = editText;
            InputCallBack = callBack;
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            try
            {
                if (Type == "List" && ArrayAdapter?.Count > 0)
                {
                    var text = ArrayAdapter[which] ?? "";
                    ListCallBack?.OnSelection(dialog, which, text);
                }
                if (Type == "Input" && EditText != null)
                {
                    var text = EditText.Text ?? "";
                    InputCallBack?.OnInput(dialog, text);
                }
                else
                {
                    dialog?.Cancel();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
            }
        }

    }
}
