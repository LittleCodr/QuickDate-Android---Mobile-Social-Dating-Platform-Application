using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Opengl;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Nio;
using Javax.Microedition.Khronos.Opengles;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Exception = System.Exception;
using IOException = Java.IO.IOException;

namespace QuickDate.Helpers.Utils
{
    public static class BitmapUtil
    {
        /// <summary>
        ///     Save filter bitmap from {@link ImageFilterView}
        /// </summary>
        /// <param name="glSurfaceView">surface view on which is image is drawn</param>
        /// <param name="gl">open gl source to read pixels from {@link GLSurfaceView}</param>
        /// <returns>save bitmap</returns>
        /// <OutOfMemoryError>error when system is out of memory to load and save bitmap</OutOfMemoryError>
        public static Bitmap CreateBitmapFromGlSurface(GLSurfaceView glSurfaceView, IGL10 gl)
        {
            try
            {
                //My Code Work <3  
                var w = glSurfaceView.Width;
                var h = glSurfaceView.Height;

                var ib = IntBuffer.Allocate(w * h);
                IntBuffer ibt = IntBuffer.Allocate(w * h);

                try
                {
                    gl.GlReadPixels(0, 0, w, h, IGL10.GlRgba, IGL10.GlUnsignedByte, ib);

                    //Parallel.For(0, h, i => 
                    //{
                    //    for (var j = 0; j < w; j++)
                    //        ibt.Put((h - i - 1) * w + j, ib.Get(i * w + j));
                    //});

                    for (var i = 0; i < h; i++)
                    {
                        for (var j = 0; j < w; j++)
                            ibt.Put((h - i - 1) * w + j, ib.Get(i * w + j));

                    }

                    var mBitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                    mBitmap.CopyPixelsFromBuffer(ibt);
                    return mBitmap;
                }
                catch (GLException e)
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
                catch (OutOfMemoryError e)
                {
                    Methods.DisplayReportResultTrack(e);
                    return null;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        //############################# NEW Code ########################

        /// <summary>
        ///     Getting bitmap from Assets folder
        /// </summary>
        /// <param name="context"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromAsset(Context context, string strName)
        {
            var assetManager = context.Assets;
            try
            {
                var istr = assetManager.Open(strName, Access.Streaming);
                return BitmapFactory.DecodeStream(istr);
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return null;
            }
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static Bitmap LoadBitmapFromView(View v)
        {
            if (v.MeasuredHeight <= 0)
                v.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            var b = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888, true);
            var c = new Canvas(b);
            c.DrawColor(Color.Transparent);
            v.Layout(0, 0, v.Width, v.Height);
            v.Draw(c);
            return b;
        }


        /// <summary>
        ///     marge tow Images
        ///     var ddd = combineImages(_saveBitmap, LoadBitmapFromView(ParentView));
        ///     ddd.Compress(Bitmap.CompressFormat.Png, 100, stream);
        /// </summary>
        /// <param name="c"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Bitmap CombineImages(Bitmap c, Bitmap s)
        {
            try
            {
                // can add a 3rd parameter 'String loc' if you want to save the new image - left some code to do that at the bottom 
                Bitmap cs;

                int width, height;

                if (c.Width > s.Width)
                {
                    width = c.Width + s.Width;
                    height = c.Height;
                }
                else
                {
                    width = s.Width + s.Width;
                    height = c.Height;
                }

                cs = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

                var comboImage = new Canvas(cs);

                comboImage.DrawBitmap(c, 0f, 0f, null);
                comboImage.DrawBitmap(s, c.Width, 0f, null);

                // this is an extra bit I added, just in-case you want to save the new image somewhere and then return the location 
                var tmpImg = GetTimestamp(DateTime.Now) + ".png";

                var folderDcimMyApp = Methods.Path.FolderDcimImage;
                if (!Directory.Exists(folderDcimMyApp))
                    Directory.CreateDirectory(folderDcimMyApp);

                try
                {
                    var stream = new FileStream(folderDcimMyApp + "/" + tmpImg, FileMode.Create);

                    cs.Compress(Bitmap.CompressFormat.Png, 100, stream);
                }
                catch (FileNotFoundException ex)
                {
                    Log.Debug("combineImages", "problem combining images", ex);
                }

                return cs;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public static Bitmap ConvertToBitmap(Drawable drawable, int widthPixels, int heightPixels)
        {
            try
            {
                var mutableBitmap = Bitmap.CreateBitmap(widthPixels, heightPixels, Bitmap.Config.Argb8888);
                var canvas = new Canvas(mutableBitmap);
                drawable.SetBounds(0, 0, widthPixels, heightPixels);
                drawable.Draw(canvas);

                return mutableBitmap;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public static async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            try
            {
                if (url.Contains("http"))
                {
                    if (Methods.CheckConnectivity())
                    {
                        HttpClient client = new HttpClient();

                        var imageBytes = await client.GetByteArrayAsync(new Uri(url));
                        if (imageBytes is { Length: > 0 })
                            return BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
                else
                {
                    BitmapFactory.Options bmOptions = new BitmapFactory.Options();
                    return BitmapFactory.DecodeFile(url, bmOptions);
                }

                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return null;
            }
        }

        public static Bitmap GetCroppedBitmap(Bitmap bitmap)
        {
            try
            {
                Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(output);

                Color color = Color.ParseColor("#424242");
                Paint paint = new Paint();
                Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

                paint.AntiAlias = true;
                canvas.DrawARGB(0, 0, 0, 0);
                paint.Color = color;
                // canvas.drawRoundRect(rectF, roundPx, roundPx, paint);
                canvas.DrawCircle(bitmap.Width / 2, bitmap.Height / 2, bitmap.Width / 2, paint);

                paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                canvas.DrawBitmap(bitmap, rect, rect, paint);
                //Bitmap _bmp = Bitmap.createScaledBitmap(output, 60, 60, false);
                //return _bmp;
                return output;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e);
                return bitmap;
            }
        }
    }
}