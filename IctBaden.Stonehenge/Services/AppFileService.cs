﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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
      GetSession();

      var path = request.FullPath("");
      Debug.WriteLine("FileService:" + path);

      var fullPath = request.FullPath(RootPath);
      var ext = Path.GetExtension(fullPath) ?? string.Empty;

      var type = "text/html";
      if (ContentType.ContainsKey(ext))
      {
        type = ContentType[ext];
      }

      if (type.StartsWith("image") && File.Exists(fullPath))
      {
        var data = File.ReadAllBytes(fullPath);
        return new HttpResult(data, type);
      }

      string text;
      if (File.Exists(fullPath))
      {
        text = File.ReadAllText(fullPath);

        if (text.StartsWith("//ViewModel:"))
        {
          var end = text.IndexOf(@"\n");
          var name = text.Substring(12, end - 12).Trim();
          
          SetViewModelType(name);
        }
      }
      else
      {
        var vmPath = Path.ChangeExtension(fullPath, ".html");
        if ((ext != ".js") || !File.Exists(vmPath))
        {
          Debug.WriteLine("AppFileService NOT FOUND:" + request.FullPath(""));
          return new HttpResult(fullPath, HttpStatusCode.NotFound);
        }

        text = File.ReadAllText(vmPath);
        if (text.StartsWith(@"<!--ViewModel:"))
        {
          var end = text.IndexOf(@"-->");
          var vmName = text.Substring(14, end - 14).Trim();
          var vm = SetViewModelType(vmName);

          text = ModuleCreator.CreateFromViewModel(text, vm);

          var userJsPath = fullPath.Replace(".js", "_user.js");
          if (File.Exists(userJsPath))
          {
            text += File.ReadAllText(userJsPath);
          }
        }
      }

      switch (path)
      {
        case @"App\index.html":
          text = UserStyleSheets.InsertUserCssLinks(RootPath, text, GetSession().SubDomain);
          break;
        case @"App\shell.js":
          {
            var host = GetResolver() as AppHost;
            if (host != null)
              text = text.Replace("%STARTPAGE%", host.StartPage);
          }
          break;
        case @"App\main.js":
          {
            var host = GetResolver() as AppHost;
            if (host != null)
              text = text.Replace("%TITLE%", host.Title);
            text = UserPages.InsertUserPages(RootPath, text);
          }
          break;
      }

      return new HttpResult(text, type);
    }

    public object Post(AppFile request)
    {
      return Get(request);
    }
  }
}