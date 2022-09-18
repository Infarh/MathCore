using MathCore.Annotations;

namespace MathCore.Tests.CSV
{
    public abstract class CSVTestsBase
    {
        protected static readonly string[] Headers = { "Id", "Name", "SurName", "Birthday", "Rating", "GroupId" };
        protected const string LineDelimiter = "------------------------------------------";
        protected const char ValuesSeparator = ';';
        private const int __DataRowsCount = 10000;

        public class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SurName { get; set; }
            public DateTime Birthday { get; set; }
            public double Rating { get; set; }
            public int GroupId { get; set; }
        }

        [NotNull]
        protected static IEnumerable<Student> GetStudents(Random rnd = null)
        {
            var now = DateTime.Now;
            rnd ??= new Random((int)now.Ticks);
            return Enumerable.Range(1, __DataRowsCount)
               .Select(i => new Student
               {
                   Id = i,
                   Name = $"Name-{i}",
                   SurName = $"SurName-{i}",
                   Birthday = now.Subtract(TimeSpan.FromDays(365 * 10 * i / __DataRowsCount + 16)),
                   Rating = rnd.NextDouble() * 100,
                   GroupId = rnd.Next(1, 21),
               });
        }
    }
}