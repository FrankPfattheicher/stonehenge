using System;
using System.Diagnostics;

namespace IctBaden.StonehengeSample
{
  public class FormVm
  {
    private static int nid = 1;
    private string name;
    public string Id { get; set; }
    public string Clock { get { return DateTime.Now.ToLongTimeString(); } }
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

    public FormVm()
    {
      nid++;
      Id = "ViewModel #" + nid;
      Name = "Frank";
    }
  }
}
