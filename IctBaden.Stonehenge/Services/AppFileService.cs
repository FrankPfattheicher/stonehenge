using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IctBaden.Stonehenge.Creators;
using ServiceStack.Common.Web;

namespace IctBaden.Stonehenge.Services
{
  public class AppFileService : AppService
  {
    private static readonly string RootPath = Path.GetDirectoryName(typeof(AppFileService).Assembly.Location);
    private static readonly Dictionary<string, string> ContentType = new Dictionary<string, string>
                {
                    { ".css", "text/css" },
                    { ".js", "text/javascript" },
                    { ".png", "image/png" },
                    { ".gif", "image/gif" },
                    { ".jpg", "image/jpeg" },
                    { ".jpeg", "image/jpeg" },
                    
                };

    public object Any(AppFile request)
    {
      return "Any";
    }

    public object Get(AppFile request)
    {
      var appSession = GetSession();
      appSession.Accessed();
      EventsClear();

      HttpResult httpResult;

      var path = request.FullPath("");
      Debug.WriteLine("FileService:" + path);

      var fullPath = request.FullPath(RootPath);
      var ext = Path.GetExtension(fullPath) ?? string.Empty;

      var type = "text/html";
      if (ContentType.ContainsKey(ext))
      {
        type = ContentType[ext];
      }

      if (type.StartsWith("image"))
      {
        var data = ResourceLoader.LoadBinary(request.BasePath(RootPath), request.BasePath(""), request.FileName);
        if (data == null)
        {
          Debug.WriteLine("AppFileService NOT FOUND:" + request.FullPath(""));
          httpResult = new HttpResult(fullPath, HttpStatusCode.NotFound);
          httpResult.Headers.Add("Expires", "0");
          return httpResult;
        }
        httpResult = new HttpResult(data, type);
        httpResult.Headers.Add("Expires", "0");
        return httpResult;
      }

      var text = ResourceLoader.LoadText(request.BasePath(RootPath), request.BasePath(""), request.FileName);
      if (!string.IsNullOrEmpty(text))
      {
        if (text.StartsWith("//ViewModel:"))
        {
          var end = text.IndexOf(@"\n", StringComparison.InvariantCulture);
          var name = text.Substring(12, end - 12).Trim();
          
          SetViewModelType(name);
        }
      }
      else
      {
        var vmPath = Path.GetFileNameWithoutExtension(fullPath) + ".html";
        text = ResourceLoader.LoadText(request.BasePath(RootPath), request.BasePath(""), vmPath);

        if ((ext != ".js") || string.IsNullOrEmpty(text))
        {
          Debug.WriteLine("AppFileService NOT FOUND:" + request.FullPath(""));
          httpResult = new HttpResult(fullPath, HttpStatusCode.NotFound);
          httpResult.Headers.Add("Expires", "0");
          return httpResult;
        }

        string vmName;
        if (text.StartsWith(@"<!--ViewModel:"))
        {
          var end = text.IndexOf(@"-->", StringComparison.InvariantCulture);
          vmName = text.Substring(14, end - 14).Trim();
        }
        else
        {
          vmName = Path.GetFileName(Path.ChangeExtension(fullPath, string.Empty));
          if (vmName != null)
          {
            vmName = vmName.Substring(0, 1).ToUpper() + vmName.Substring(1, vmName.Length - 2) + "Vm";
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var vmType = assembly.GetTypes().FirstOrDefault(t => t.Name == vmName);
            if (vmType != null)
            {
              vmName = vmType.FullName;
            }
          }
        }

        if (!string.IsNullOrEmpty(vmName))
        {
          try
          {
            var vm = SetViewModelType(vmName);
            text = ModuleCreator.CreateFromViewModel(vm);
          }
          catch (Exception ex)
          {
            text = ex.Message + Environment.NewLine + ex.StackTrace;
          }

          var userJs = Path.GetFileName(fullPath.Replace(".js", "_user.js"));
          var userjs = ResourceLoader.LoadText(request.BasePath(RootPath), request.BasePath(""), userJs);
          if (userjs != null)
          {
            text += userjs;
          }
        }
      }

      switch (path.Replace(Path.DirectorySeparatorChar, '.'))
      {
        case @"index.html":
        case @"app.index.html":
          text = UserStyleSheets.InsertUserCssLinks(RootPath, text, appSession.SubDomain);
          text = UserIcons.InsertUserIconLinks(RootPath, text);
          if (!Request.IsLocal)
          {
            text = ContentDeliveryNetworkSupport.RersolveHosts(text);
          }
          break;
        case @"app.shell.js":
          {
            var startPage = string.Empty;
            var host = GetResolver() as AppHost;
            if (host != null)
            {
              startPage = host.StartPage;
            }
            text = UserPages.InsertUserPages(RootPath, startPage, text).Replace("%STARTPAGE%", startPage);
          }
          break;
        case @"app.main.js":
          {
            var host = GetResolver() as AppHost;
            if (host != null)
              text = text.Replace("%TITLE%", host.Title);
          }
          break;
      }

      if (string.IsNullOrEmpty(RequestContext.CompressionType)) 
        return new HttpResult(text, type);

      var compressed = new CompressedResult(Encoding.UTF8.GetBytes(text), RequestContext.CompressionType) { ContentType = type };
      httpResult = new HttpResult(compressed.Contents, type);
      httpResult.Headers.Add("CompressionType", RequestContext.CompressionType);
      httpResult.Headers.Add("Expires", "0");
      return httpResult;
    }

    public object Post(AppFile request)
    {
      return Get(request);
    }
  }
}