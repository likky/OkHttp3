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

namespace OkHttp
{
    [Activity(Label = "OkHttp3", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private OkHttpClient client;
        string url = "https://www.google.com.hk/";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var button1 = FindViewById(Resource.Id.button1) as Button;
            var textview1 = FindViewById(Resource.Id.textView1) as TextView;

            button1.Click += async delegate
            {
                //Android3.0 以后已经不允许在主线程访问网络，报错android.os.NetworkOnMainThreadException
                try
                {
                    string x = await Get(url);
                    textview1.Text = x;
                }
                catch(IOException e)
                {
                    e.PrintStackTrace();
                }


            };


        }

        //Android3.0 以后已经不允许在主线程访问网络，报错android.os.NetworkOnMainThreadException
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
                Toast.MakeText(this, "response.Code() == 200", ToastLength.Short).Show();
                var content = response.Body().String();

                return content;
            }
            else
                return "null";
 
        }
    }
}

