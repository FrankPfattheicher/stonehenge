using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample.ViewModels
{
  class IsDirtyVm : ActiveViewModel
  {
    public string Name { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }

    public IsDirtyVm()
    {
      Name = "Nobody";
      Address = "addr";
    }

    [ActionMethod]
    public void Save()
    {
      
    }
  }
}
