using System.Collections.Generic;
using NUnit.Framework;

namespace IctBaden.Stonehenge2.Test.Serializer
{
    public class NestedClass
    {
        public string Name { get; set; }
        public List<NestedClass2> Nested { get; set; }
    }

    public class NestedClass2
    {
        public SimpleClass[] NestedSimple { get; set; }
    }
}