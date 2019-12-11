using System.Collections.Generic;
using System.Diagnostics;

namespace Velo.TestsModels.Boos
{
    [DebuggerDisplay("Id = {" + nameof(Id) + "}")]
    public class Boo
    {
        public int Id { get; set; }

        public bool Bool { get; set; }

        public float Float { get; set; }

        public int Int { get; set; }

        public int? IntNullable { get; set; }

        public double Double { get; set; }

        public List<int> Values { get; set; }

        public ModelType Type { get; set; }
    }
}