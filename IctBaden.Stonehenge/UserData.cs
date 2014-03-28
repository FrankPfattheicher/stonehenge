using System.Text;

namespace IctBaden.Stonehenge
{
  public class UserData
  {
    public byte[] Bytes { get; set; }
    public string ContentType { get; set; }

    public UserData(string contentType, byte[] data)
    {
      Bytes = data;
      ContentType = contentType;
    }
    public UserData(string text)
    {
      Bytes = Encoding.UTF8.GetBytes(text);
      ContentType = "text/plain; charset=utf-8";
    }
  }
}