using Android.App;
using Android.Widget;
using Android.OS;
using Java.IO;
//下面一行不注释表示使用自己绑定的OkHttp3，3.11.0版本，但没有ExecuteAsync()异步方法。自行实现在子线程中执行Execute()！例如：new Thread...start()
//using Okhttp3;
//下面一行不注释表示使用的NuGet下载的OkHttp3，3.8.1版本，代码中使用此版本。还必须引用Square.OkIO
using Square.OkHttp3;
using Square.OkIO;
using Java.Util.Concurrent;
using Java.Lang;
using System.IO;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Android.Net;

namespace OkHttp
{
    [Activity(Label = "OkHttp3", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private OkHttpClient client;
        string url = "https://www.google.com.hk/";
        string url2 = "https://englishpicture.oss-cn-shanghai.aliyuncs.com/fruit/apple.jpg";
        string pictureFolder;
        Java.IO.File pictureFile;
        string filename;
        int PERMISSION_WriteExternalStorage = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var button1 = FindViewById(Resource.Id.button1) as Button;
            var button2 = FindViewById(Resource.Id.button2) as Button;
            var textview1 = FindViewById(Resource.Id.textView1) as TextView;
            var image1 = FindViewById(Resource.Id.imageView1) as ImageView;
            pictureFolder = GetCardPath() + "/okhttp3";
            pictureFile= new Java.IO.File(pictureFolder);
            if (!pictureFile.Exists())
            {
                pictureFile.Mkdirs();
            }
            filename= System.IO.Path.Combine(pictureFolder, "apple.jpg");

            button1.Click += async delegate
            {
                //Android3.0 以后已经不允许在主线程访问网络，报错android.os.NetworkOnMainThreadException
                try
                {
                    string x = await Get(url);
                    textview1.Text = x;
                }
                catch(Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }

            };

            button2.Click += async delegate
            {
                //下载并保存图片到okhttp3文件夹，显示图片。实际项目中已下载不需要重复下载，这里忽略。
                byte[] bytes =  await GetPicture(url2);
                if (bytes != null)
                {
                    FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                    await fs.WriteAsync(bytes, 0, bytes.Length);

                    //Console.WriteLine("localPath:" + localpath3);
                    fs.Close();
                    Uri uri = Uri.FromFile(new Java.IO.File(filename));
                    //Uri uri = Uri.Parse(filename);
                    image1.SetImageURI(uri);
                }
            };
            //动态申请存储权限
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                bool isGrant = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == Permission.Granted;
                if (isGrant == false)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, PERMISSION_WriteExternalStorage);
                }
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == PERMISSION_WriteExternalStorage)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    if (Build.VERSION.SdkInt != Android.OS.BuildVersionCodes.NMr1)
                        Toast.MakeText(this, "存储权限已申请", ToastLength.Short).Show();
                }
                else
                {
                    if (Build.VERSION.SdkInt != Android.OS.BuildVersionCodes.NMr1)
                        Toast.MakeText(this, "申请存储权限被拒，无法存储铃声", ToastLength.Short).Show();
                }
            }

        }
        //Android3.0 以后已经不允许在主线程访问网络，报错android.os.NetworkOnMainThreadException
        //获取网页内容
        public async System.Threading.Tasks.Task<string> Get(string url)
        {
            //连接超时5秒
            //写入数据超时5秒
            //读取数据超时5秒
            client = new OkHttpClient.Builder()
            .ConnectTimeout(5, TimeUnit.Seconds)
            .WriteTimeout(5, TimeUnit.Seconds)
            .ReadTimeout(5, TimeUnit.Seconds)
            .Build();
            Request request = new Request.Builder().Url(url).Build();
            Response response = await client.NewCall(request).ExecuteAsync();
            if (response.Code() == 200)
            {
                //Toast.MakeText(this, "response.Code() == 200", ToastLength.Short).Show();
                var content = response.Body().String();

                return content;
            }
            else
                return "null";
 
        }

        //下载图片
        public async System.Threading.Tasks.Task<byte[]> GetPicture(string url)
        {
            client = new OkHttpClient.Builder()
            .ConnectTimeout(5, TimeUnit.Seconds)
            .WriteTimeout(5, TimeUnit.Seconds)
            .ReadTimeout(5, TimeUnit.Seconds)
            .Build();
            Request request = new Request.Builder().Url(url).Build();
            Response response = await client.NewCall(request).ExecuteAsync();
            if (response.Code() == 200)
            {
                var bytes = await response.Body().BytesAsync();

                return bytes;
            }
            else
                return null;

        }

        private string GetCardPath()
        {
            Java.IO.File sdDir = Android.OS.Environment.ExternalStorageDirectory;//获取手机根目录             
            return sdDir.ToString();

        }
    }
}

