using Android.App;
using Android.Graphics;
using Android.Widget;
using QuickDate.Helpers.Utils;
using System;

namespace QuickDate.Helpers.Fonts
{
    public static class FontUtils
    {
        //Changes the TextView To IconFrameWork Fonts
        public static void SetTextViewIcon(FontsIconFrameWork type, TextView textViewUi, string iconUnicode)
        {
            try
            {
                switch (type)
                {
                    case FontsIconFrameWork.IonIcons:
                        {
                            var font = Typeface.CreateFromAsset(Application.Context.Resources?.Assets, "ionicons.ttf");
                            textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                            textViewUi.Text = string.IsNullOrEmpty(iconUnicode) switch
                            {
                                false => iconUnicode,
                                _ => textViewUi.Text
                            };
                            //return font;
                            break;
                        }
                    case FontsIconFrameWork.FontAwesomeSolid:
                        {
                            var font = Typeface.CreateFromAsset(Application.Context.Resources?.Assets, "fa-solid-900.ttf");
                            textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                            textViewUi.Text = string.IsNullOrEmpty(iconUnicode) switch
                            {
                                false => iconUnicode,
                                _ => textViewUi.Text
                            };
                            //return font;
                            break;
                        }
                    case FontsIconFrameWork.FontAwesomeRegular:
                        {
                            var font = Typeface.CreateFromAsset(Application.Context.Resources?.Assets, "fa-regular-400.ttf");
                            textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                            textViewUi.Text = string.IsNullOrEmpty(iconUnicode) switch
                            {
                                false => iconUnicode,
                                _ => textViewUi.Text
                            };
                            //return font;
                            break;
                        }
                    case FontsIconFrameWork.FontAwesomeBrands:
                        {
                            var font = Typeface.CreateFromAsset(Application.Context.Resources?.Assets, "fa-brands-400.ttf");
                            textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                            textViewUi.Text = string.IsNullOrEmpty(iconUnicode) switch
                            {
                                false => iconUnicode,
                                _ => textViewUi.Text
                            };
                            //return font;
                            break;
                        }
                    case FontsIconFrameWork.FontAwesomeLight:
                        {
                            var font = Typeface.CreateFromAsset(Application.Context.Resources?.Assets, "fa-light-300.ttf");
                            textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                            textViewUi.Text = string.IsNullOrEmpty(iconUnicode) switch
                            {
                                false => iconUnicode,
                                _ => textViewUi.Text
                            };
                            //return font;
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Set_TextViewIcon Function ERROR " + e);
                Methods.DisplayReportResultTrack(e);
                //return null;
            }
        }
    }
}