﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  public class FormVm : ActiveViewModel
  {
    private static int nid = 1;
    private string name;
    private Timer timer;
    
    public string Id { get; set; }
    public string Clock { get; private set; }
    public string Prompt { get { return "What is your Name?"; } }
    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        Debug.WriteLine("[{0}] Name={1}", Id, name);
      }
    }

    public bool CanSayHello
    { get { return !string.IsNullOrEmpty(Name); } }


    public List<string> OptionValues { get; set; }

    public FormVm(AppSession session)
      : base(session)
    {
      nid++;
      Id = "ViewModel #" + nid;
      Name = "Frank";
      OptionValues = new List<string>{"One", "Two", "Tree", "Four"};

      ClockTick(this);
      timer = new Timer(ClockTick, this, 1000, 1000);
    }

    private void ClockTick(object state)
    {
      this["Clock"] = DateTime.Now.ToLongTimeString();
    }
  }
}
