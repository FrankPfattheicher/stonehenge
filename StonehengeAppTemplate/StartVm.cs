﻿using IctBaden.Stonehenge;

namespace StonehengeApp
{
  public class StartVm : ActiveViewModel
  {
    // all public properties are available for knockout binding
    public string Sample { get; set; }

    public StartVm()
    {
      Sample = "See StartVm.cs";
    }
  }
}