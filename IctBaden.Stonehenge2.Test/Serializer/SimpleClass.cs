using System;

namespace IctBaden.Stonehenge2.Test.Serializer
{
    public class SimpleClass
    {
        public int Integer { get; set; }
        public bool Boolean { get; set; }
        public double Floatingpoint { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public string PrivateText;

        public SimpleClass()
        {
            Timestamp = DateTime.Now;
        }
    }
}