namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using IctBaden.Stonehenge;

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
