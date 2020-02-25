using System;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;

namespace Velo.TestsModels
{
    public class BigObject
    {
        public Guid? Id { get; set; }

        public int[] Array { get; set; }

        public Boo Boo { get; set; }

        public bool Bool { get; set; }

        public DateTime? Date { get; set; }

        public Foo Foo { get; set; }

        public float Float { get; set; }

        public int Int { get; set; }

        public double Double { get; set; }

        public string String { get; set; }

        public TimeSpan? TimeSpan { get; set; }
    }
}