using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using ServiceStack.ServiceInterface;

namespace IctBaden.Stonehenge.Services
{
  public class AppService : Service
  {
    public AppSession GetSession()
    {
      var session = Session.Get<object>("~session") as AppSession;
      if (session == null)
      {
        session = new AppSession(Request.AbsoluteUri, Request.UserAgent, Session);
        Session.Set("~session", session);

        var host = GetResolver() as AppHost;
        if (host != null)
        {
          host.OnNewSession(session);
        }
      }
      return session;
    }

    public List<string> Events
    {
      get
      {
        List<string> events = null;
        try
        {
          events = Session.Get<object>("~ev") as List<string>;
        }
        catch (Exception)
        {
          throw;
        }
        if (events == null)
        {
          events = new List<string>();
          Session.Set("~ev", events);
        }
        return events;
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
                Events.Add(args.PropertyName);
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
          Events.Add(string.Empty);
        }
        var disposable = vm as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }

      
      var asm = Assembly.GetEntryAssembly();
      var vmtype = asm.GetType(typeName);
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
          var appSession = GetSession();
          vm = Activator.CreateInstance(vmtype, new object[] { appSession });
        }
        else
        {
          vm = Activator.CreateInstance(vmtype);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        vm = null;
      }

      ViewModel = vm;
      return vm;
    }

  }
}