using System;
using System.Diagnostics;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  public class FormVm : ActivePresenter
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

    public FormVm()
    {
      nid++;
      Id = "ViewModel #" + nid;
      Name = "Frank";
			ClockTick(this);
			timer = new Timer(ClockTick, this, 1000, 1000);
    }

	  private void ClockTick(object state)
	  {
			this["Clock"] = DateTime.Now.ToLongTimeString();
	  }
  }
}
