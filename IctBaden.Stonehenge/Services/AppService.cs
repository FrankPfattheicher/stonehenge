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
        session = new AppSession(Request.AbsoluteUri, Session);
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
        var events = Session.Get<object>("~ev") as List<string>;
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
        if (value == null) 
          return;

        Session.Set("~vm", value);

        var npc = value as INotifyPropertyChanged;
        if (npc != null)
        {
          npc.PropertyChanged += OnPropertyChanged;
        }
      }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      lock (Events)
      {
        Events.Add(propertyChangedEventArgs.PropertyName);
      }
    }

    public object SetViewModelType(string typeName)
    {
      var vm = ViewModel;
      if ((ViewModel != null) && (ViewModel.GetType().FullName == typeName)) 
        return vm;

      var asm = Assembly.GetEntryAssembly();
      var vmtype = asm.GetType(typeName);
      if (vmtype == null)
      {
        Debug.WriteLine("Could not create ViewModel:" + typeName);
        return null;
      }

      if (typeof (ActiveViewModel).IsAssignableFrom(vmtype))
      {
        var appSession = GetSession();
        vm = Activator.CreateInstance(vmtype, new object[] { appSession });
      }
      else
      {
        vm = Activator.CreateInstance(vmtype);
      }

      ViewModel = vm;
      return vm;
    }

  }
}