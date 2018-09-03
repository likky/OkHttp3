# OkHttp3
OkHttp3 for Xamarin.Android

推荐直接在Nuget下载下面两个包即可使用OkHttp3。
  <package id="Square.OkHttp3" version="3.8.1" targetFramework="monoandroid81" />
  <package id="Square.OkIO" version="1.13.0" targetFramework="monoandroid81" />

//下面一行不注释表示使用的NuGet下载的OkHttp3，3.8.1版本，推荐使用此版本。还必须引用Square.OkIO
using Square.OkHttp3;

//下面一行不注释表示使用自己绑定的OkHttp3，3.11.0版本，但没有ExecuteAsync()异步方法。
自行实现在子线程中执行Execute()！例如：new Thread...start()
//using Okhttp3;
