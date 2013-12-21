using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using ServiceStack.ServiceInterface;

namespace IctBaden.Stonehenge.Services
{
  public class AppService : Service
  {
    public const string PropertyNameMessageBox = "_stonehenge_MessageBox_";
    
    public AppSession GetSession()
    {
      AppSession session = null;
      lock (Session)
      { 
        try
        {
          session = Session.Get<object>("~session") as AppSession;
        }
        catch (Exception ex)
        {
          Trace.TraceError(ex.Message);
        }
        if (session != null) 
          return session;

        session = new AppSession(Request.QueryString.Get("hostdomain"), Request.AbsoluteUri, Request.RemoteIp, Request.UserAgent, Session);
        Session.Set("~session", session);

        var host = GetResolver() as AppHost;
        if (host == null) 
          return session;

        host.OnSessionCreated(session);
        if (!host.HasSessionTimeout) 
          return session;

        session.SetTerminator(this);
        session.SetTimeout(host.SessionTimeout);
        session.TimedOut += () =>
        {
          Cache.FlushAll();
          //Session.Set("~session", (object)null);
          host.OnSessionTerminated(session);
        };
      }
      return session;
    }

    public List<string> Events
    {
      get
      {
        List<string> events = null;
        lock (Session)
        { 
          try
          {
            events = Session.Get<object>("~ev") as List<string>;
          }
          catch (Exception ex)
          {
            Trace.TraceError(ex.Message);
          }
          if (events == null)
          {
            events = new List<string>();
            Session.Set("~ev", events);
          }
        }
        return events;
      }
    }
    public AutoResetEvent EventRelease
    {
      get
      {
        AutoResetEvent eventRelease = null;
        lock (Session)
        {
          try
          {
            eventRelease = Session.Get<object>("~er") as AutoResetEvent;
          }
          catch (Exception ex)
          {
            Trace.TraceError(ex.Message);
          }
          if (eventRelease == null)
          {
            eventRelease = new AutoResetEvent(false);
            Session.Set("~er", eventRelease);
          }
        }
        return eventRelease;
      }
    }

    public void EventsClear()
    {
      lock (Events)
      {
        var msgBox = Events.FirstOrDefault(e => e == PropertyNameMessageBox);
        Events.Clear();
        EventAdd(msgBox ?? string.Empty);
      }
    }

    public void EventAdd(string name)
    {
      lock (Events)
      {
        Events.Add(name);
        EventRelease.Set();
      }
    }

    public object ViewModel
    {
      get { return Session.Get<object>("~vm"); }
      set
      {
        if (Request == null)
          return;
        try
        {
          Session.Set("~vm", value);
        }
        // ReSharper disable EmptyGeneralCatchClause
        catch (Exception)
        {
        }
        // ReSharper restore EmptyGeneralCatchClause


        var npc = value as INotifyPropertyChanged;
        if (npc != null)
        {
          npc.PropertyChanged += (_, args) =>
            {
              lock (Events)
              {
                EventAdd(args.PropertyName);
              }
            };
        }
      }
    }

    public object SetViewModelType(string typeName)
    {
      var vm = ViewModel;
      if (ViewModel != null)
      {
        if((ViewModel.GetType().FullName == typeName))
          return vm;

        var avm = vm as ActiveViewModel;
        if (avm != null)
        {
          EventsClear();
        }
        var disposable = vm as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }

      
      var asm = Assembly.GetEntryAssembly();
      var vmtype = asm.GetTypes().FirstOrDefault(type => type.FullName.EndsWith(typeName));
      if (vmtype == null)
      {
        ViewModel = null;
        Debug.WriteLine("Could not create ViewModel:" + typeName);
        return null;
      }

      try
      {
        if (typeof(ActiveViewModel).IsAssignableFrom(vmtype))
        {
          var sessionCtor = vmtype.GetConstructors().FirstOrDefault(ctor => ctor.GetParameters().Length == 1);
          vm = (sessionCtor != null) ? Activator.CreateInstance(vmtype, new object[] { GetSession() }) : Activator.CreateInstance(vmtype);
        }
        else
        {
          vm = Activator.CreateInstance(vmtype);
        }
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.Message);
        vm = null;
      }

      ViewModel = vm;
      return vm;
    }

  }
}