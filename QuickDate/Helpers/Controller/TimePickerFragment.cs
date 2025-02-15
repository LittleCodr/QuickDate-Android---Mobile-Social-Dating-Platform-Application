using Android.App;
using Android.Icu.Util;
using Android.OS;
using Android.Text.Format;
using Android.Widget;
using System;

namespace QuickDate.Helpers.Controller
{
    public class TimePickerFragment : AndroidX.Fragment.App.DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        public new static readonly string Tag = "MyTimePickerFragment";
        Action<DateTime> TimeSelectedHandler = delegate { };

        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            TimePickerFragment frag = new TimePickerFragment { TimeSelectedHandler = onTimeSelected };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currentTime = DateTime.Now;
            bool is24HourFormat = DateFormat.Is24HourFormat(Activity);
            TimePickerDialog dialog = new TimePickerDialog(Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);

            TimeSelectedHandler(selectedTime);
        }
    }

    public class DatePickerFragment : AndroidX.Fragment.App.DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public new static readonly string Tag = "X:" + nameof(DatePickerFragment)?.ToUpper();
        public static string Type = "";

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> DateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected, string type = "")
        {
            Type = type;
            DatePickerFragment frag = new DatePickerFragment { DateSelectedHandler = onDateSelected };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, currently.Year, currently.Month - 1, currently.Day);

            if (Type == "Birthday")
            {
                // https://www.geeksforgeeks.org/how-to-disable-previous-or-future-dates-in-datepicker-in-android/
                // initialise the calendar
                Calendar calendar = Calendar.Instance;
                dialog.DatePicker.MaxDate = calendar.TimeInMillis;
            }
            else if (Type == "StartDate")
            {
                // https://www.geeksforgeeks.org/how-to-disable-previous-or-future-dates-in-datepicker-in-android/
                // initialise the calendar
                Calendar calendar = Calendar.Instance;
                dialog.DatePicker.MinDate = calendar.TimeInMillis;
            }

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            DateSelectedHandler(selectedDate);
        }
    }
}