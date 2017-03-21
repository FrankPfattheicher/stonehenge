namespace IctBaden.Stonehenge
{
    public class CheckedItem
    {
        public string Title { get; set; }
        public bool Checked { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{Title}={Checked}";
        }
    }
}
