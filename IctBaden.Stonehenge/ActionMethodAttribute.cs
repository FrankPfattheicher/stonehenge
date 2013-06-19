using System;

namespace IctBaden.Stonehenge
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class ActionMethodAttribute : Attribute
  {
    public string Name { get; set; }
    /// <summary>
    /// see KeyGestureConverter
    /// </summary>
    public string Shortcut { get; set; }
  }
}