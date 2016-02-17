namespace IctBaden.Stonehenge2.Resources
{
    public class ViewModelInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int SortIndex { get; set; }
        public bool Visible => SortIndex > 0;

        public ViewModelInfo(string name)
        {
            Name = name;
            SortIndex = 1;
        }
    }
}