namespace IctBaden.Stonehenge2.ViewModel
{
    public class ActiveModel
    {
        public readonly string Prefix;
        public readonly string TypeName;
        public readonly bool ReadOnly;
        public object Model;

        public ActiveModel(string prefix, object model, bool readOnly)
        {
            Prefix = prefix;
            Model = model;
            ReadOnly = readOnly;
            TypeName = model.GetType().Name;
        }
    }
}
