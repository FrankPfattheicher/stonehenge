﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using IctBaden.Stonehenge.Creators;
using ServiceStack.Common;
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
                    { ".wav", "audio/x-wav" },
                    
                };

        public object Get(AppFile request)
        {
#if DEBUG
            var duration = new Stopwatch();
            duration.Start();
            Trace.TraceInformation("GET(file) " + Request.AbsoluteUri);

            var result = GetInternal(request);

            duration.Stop();
            Trace.TraceInformation($"GET(file) {duration.ElapsedMilliseconds}ms");

            return result;
        }

        private object GetInternal(AppFile request)
        {
#endif
            if (request.Path1 == "acme-challenge")
            {
                Trace.TraceInformation("Handling ACME request: " + request.FullPath("/.well-known"));
                var acmePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? ".";
                var acmeFile = acmePath + "/.well-known/acme-challenge/" + request.FileName;
                if (File.Exists(acmeFile))
                {
                    var acmeData = File.ReadAllBytes(acmeFile);
                    return new HttpResult(acmeData);
                }

                Trace.TraceError("No ACME data found.");
                return new HttpResult(Request.RawUrl, HttpStatusCode.NotFound);
            }

            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession != null)
            {
                var currentParameters = Request.QueryString.AllKeys
                    .ToDictionary(key => key, key => Request.QueryString.Get(key));
                appSession.Accessed(Request.Cookies, currentParameters, true);
                //appSession.EventsClear(false);
            }
            else
            {
                if (request.IsEmpty)
                {
                    if (Request.RawUrl.Contains("favicon.ico"))
                    {
                        var favicon = ResourceLoader.LoadText("", "", "favicon.ico");
                        return new HttpResult(favicon, "image/x-icon");
                    }
                    if (Request.RawUrl.Contains("robots.txt"))
                    {
                        var robots = ResourceLoader.LoadText("", "", "robots.txt");
                        return new HttpResult(robots, "text/plain");
                    }

                    Debug.WriteLine("AppFileService NOT FOUND:" + Request.RawUrl);
                    return new HttpResult(Request.RawUrl, HttpStatusCode.NotFound);
                }
            }

            HttpResult httpResult;
            var doNotCache = false;

            var path = request.FullPath("");
            Debug.WriteLine("FileService:" + path);

            var fullPath = request.FullPath(RootPath);
            var ext = Path.GetExtension(fullPath) ?? string.Empty;
            var context = string.IsNullOrEmpty(ext) ? string.Empty : request.FileName.Replace(ext, string.Empty);
            
            if((appSession == null) && (string.Compare(request.FileName, "index.html", StringComparison.OrdinalIgnoreCase) == 0))
            {
                if (((AppSessionCache.ReuseSessions & AppSessionCache.ReuseSessionStrategy.Cookie) != 0))
                {
                    if (Request.Cookies.ContainsKey("stonehenge_id"))
                    {
                        sessionId = Request.Cookies["stonehenge_id"].Value;
                        appSession = GetSession(sessionId);
                    }
                    if ((appSession == null) && Request.Cookies.ContainsKey("ss-pid"))
                    {
                        appSession = AppSessionCache.GetSessionByStackId(Request.Cookies["ss-pid"].Value);
                    }
                    
                }
                if ((AppSessionCache.ReuseSessions & AppSessionCache.ReuseSessionStrategy.ClientAddress) != 0)
                {
                    appSession = AppSessionCache.GetSessionByIpAddress(Request.RemoteIp);
                }
                return (appSession != null) ? RedirectToSession(appSession) : RedirectToNewSession();
            }

            if (appSession != null)
            {
                var etag = Request.Headers["If-None-Match"];
                var requestTag = appSession.GetETag(Request.AbsoluteUri);
                if (etag == $"\"{requestTag}\"")
                {
                    Debug.WriteLine("ETag match.");
                    httpResult = new HttpResult("", HttpStatusCode.NotModified);
                    return httpResult;
                }
            }

            var type = "text/html";
            if (ContentType.ContainsKey(ext))
            {
                type = ContentType[ext];
            }

            if (type.StartsWith("image") || type.StartsWith("audio"))
            {
                var data = ResourceLoader.LoadBinary(request.BasePath(RootPath), request.BasePath(""), request.FileName);
                if (data == null)
                {
                    Debug.WriteLine("AppFileService NOT FOUND:" + request.FullPath(""));
                    httpResult = new HttpResult(fullPath, HttpStatusCode.NotFound);
                    return httpResult;
                }
                httpResult = new HttpResult(data, type);
                httpResult.Headers.Add("Cache-Control", "max-age=86400");
                if ((appSession != null) && !appSession.CookieSet)
                {
                    httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                }
                return httpResult;
            }

            string text = null;
            if (!string.IsNullOrEmpty(appSession?.SubDomain) && (request.Path1 == "app"))
            {
                var subrequest = new AppFile
                {
                    Path1 = appSession.SubDomain,
                    Path2 = request.Path2,
                    Path3 = request.Path3,
                    Path4 = request.Path4,
                    FileName = request.FileName
                };
                text = ResourceLoader.LoadText(subrequest.BasePath(RootPath), subrequest.BasePath(""), subrequest.FileName);
            }
            if (string.IsNullOrEmpty(text))
            {
                text = ResourceLoader.LoadText(request.BasePath(RootPath), request.BasePath(""), request.FileName);
            }
            if (!string.IsNullOrEmpty(text))
            {
                if (text.StartsWith("//ViewModel:"))
                {
                    var end = text.IndexOf(@"\n", StringComparison.InvariantCulture);
                    var name = text.Substring(12, end - 12).Trim();

                    appSession?.SetViewModelType(name);
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
                    if ((appSession != null) && !appSession.CookieSet)
                    {
                        httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                    }
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
                        var assembly = Assembly.GetEntryAssembly();
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
                        if (appSession == null)
                        {
                            appSession = AppSessionCache.NewSession();
                        }
                        var vm = appSession.SetViewModelType(vmName);
                        text = ModuleCreator.CreateFromViewModel(vm, context);
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

                doNotCache = true;
            }

            switch (path.Replace(Path.DirectorySeparatorChar, '.'))
            {
                case @"index.html":
                case @"app.index.html":
                    if (appSession != null)
                    {
                        text = UserStyleSheets.InsertUserCssLinks(RootPath, text, appSession.SubDomain);
                        text = UserLibs.InsertUserLibs(RootPath, text);
                    }
                    text = UserIcons.InsertUserIconLinks(RootPath, text);
                    if (!Request.IsLocal)
                    {
                        text = ContentDeliveryNetworkSupport.RersolveHostsHtml(text, Request.IsSecureConnection);
                    }

                    var globalUserJs = ResourceLoader.LoadText("app", "app", "global_user.js");
                    if (globalUserJs != null)
                    {
                        text = text.Replace("<!--UserGlobalJs-->",
                                "<script language=\"javascript\" type=\"text/javascript\" src=\"app/global_user.js\"></script>");
                    }
                    var host = GetResolver() as AppHost;
                    if (host != null)
                    {
                        text = text.Replace("<!--Title-->", host.Title);
                    }
                    break;
                case @"app.shell.js":
                    {
                        var startPage = string.Empty;
                        host = GetResolver() as AppHost;
                        if (host != null)
                        {
                            startPage = host.StartPage;
                        }
                        text = UserPages.InsertUserPages(RootPath, startPage, text).Replace("%STARTPAGE%", startPage);
                    }
                    break;
                case @"app.main.js":
                    {
                        host = GetResolver() as AppHost;
                        if (host != null)
                        {
                            text = text.Replace("%TITLE%", host.Title);
                            if (host.MessageBoxContentHtml)
                            {
                                text = text.Replace("//MessageBox=HTML", "var dialog = require('plugins/dialog');" +
                                                                         "dialog.MessageBox.defaultViewMarkup = dialog.MessageBox.defaultViewMarkup.replace('data-bind=\"text: message\"', 'data-bind=\"html: message\"');");
                            }
                            else
                            {
                                text = text.Replace("//MessageBox=HTML", "");
                            }
                        }
                        if ((appSession != null)
                            && ((appSession.Browser == "IE 7.0") || (appSession.Browser == "IE 8.0")))
                        {
                            text = text.Replace("jquery-2.1.3", "jquery-1.11.2");
                        }
                    }
                    if (!Request.IsLocal)
                    {
                        text = ContentDeliveryNetworkSupport.RersolveHostsJs(text, Request.IsSecureConnection);
                    }
                    break;
            }

            if (string.IsNullOrEmpty(RequestContext.CompressionType))
            {
                httpResult = new HttpResult(text, type);
                if ((appSession != null) && !appSession.CookieSet)
                {
                    httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                }
                return httpResult;
            }

            var compressed = new CompressedResult(text.Compress(RequestContext.CompressionType), RequestContext.CompressionType) { ContentType = type };
            httpResult = new HttpResult(compressed.Contents, type);
            foreach (var header in compressed.Headers)
            {
                httpResult.Headers.Add(header.Key, header.Value);
            }

            var maxAge = 3600;
            if (appSession != null) 
            {
                var requestTag = appSession.GetETag(Request.AbsoluteUri);
                httpResult.Headers.Add("ETag", $"\"{requestTag}\"");
                maxAge = 60;

                if (!appSession.CookieSet)
                {
                    httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                }
            }

            httpResult.Headers.Add("Cache-Control", doNotCache
                ? "no-cache"
                : $"max-age={maxAge}, must-revalidate, proxy-revalidate");
            return httpResult;
        }

        public object Post(AppFile request)
        {
            return Get(request);
        }

        public object Any(AppFile request)
        {
            var message = "AppFileService: No handler for HTTP method: " + Request.HttpMethod;
            Trace.TraceWarning(message);
            return new HttpResult(HttpStatusCode.NotFound, message);
        }

    }
}