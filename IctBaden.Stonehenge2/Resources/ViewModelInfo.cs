using System.Collections.Generic;

namespace IctBaden.Stonehenge2.Resources
{
    public class ViewModelInfo
    {
        // CustomComponent
        public string ElementName { get; set; }
        public List<string> Bindings { get; set; }

        // ViewModel
        public string VmName { get; set; }
        public string Title { get; set; }
        public int SortIndex { get; set; }
        public bool Visible => SortIndex > 0;

        public ViewModelInfo(string name)
        {
            VmName = name;
            SortIndex = 1;
        }


    }
}