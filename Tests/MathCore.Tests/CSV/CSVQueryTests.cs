using System;
using System.IO;
using System.Linq;
using MathCore.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.CSV
{
    [TestClass]
    public class CSVQueryTests
    {
        private class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SurName { get; set; }
            public DateTime Birthday { get; set; }
            public double Rating { get; set; }
            public int GroupId { get; set; }
        }

        private const string __DataFileName = "CSVQueryTests_Students.csv";
        private const int __DataRowsCount = 10000;
        private const int __BeforeLinesCount = 3;
        private const int __AfterLinesCount = 3;
        private const string __LineDelimiter = "------------------------------------------";
        private const char __ValuesSeparator = ';';

        [ClassInitialize]
        public static void ClassInitialize(TestContext Context)
        {
            var now = DateTime.Now;
            var rnd = new Random((int)now.TimeOfDay.Ticks);
            var students = Enumerable.Range(1, __DataRowsCount)
               .Select(i => new Student
                {
                    Id = i,
                    Name = $"Name-{i}",
                    SurName = $"SurName-{i}",
                    Birthday = now.Subtract(TimeSpan.FromDays(365 * 10 * i / __DataRowsCount + 16)),
                    Rating = rnd.NextDouble() * 100,
                    GroupId = rnd.Next(1, 21),
                });
            using var writer = File.CreateText(__DataFileName);

            for(var i = 0; i < __BeforeLinesCount; i++)
                writer.WriteLine(__LineDelimiter);

            writer.WriteLine(string.Join(__ValuesSeparator, "Id", "Name", "SurName", "Birthday", "Rating", "GroupId"));

            for (var i = 0; i < __BeforeLinesCount; i++)
                writer.WriteLine(__LineDelimiter);

            foreach (var student in students)
                writer.WriteLine(string.Join(__ValuesSeparator,
                    student.Id,
                    student.Name,
                    student.SurName,
                    student.Birthday,
                    student.Rating,
                    student.GroupId));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if(File.Exists(__DataFileName))
                File.Delete(__DataFileName);
        }

        [TestMethod]
        public void ReadDataTest()
        {
            var file = new FileInfo(__DataFileName);
            var query = file.OpenCSV()
               .ValuesSeparator(__ValuesSeparator)
               .Skip(__BeforeLinesCount)
               .ReadHeader()
               .SkipAfterHeader(__AfterLinesCount);

            var students = query.Select(line => new Student
            {
                Id = line.ValueAs<int>("Id"),
                Name = line["Name"],
                SurName = line["SurName"],
                Birthday = line.ValueAs<DateTime>("Birthday"),
                Rating = line.ValueAs<double>("Rating"),
                GroupId = line.ValueAs<int>("GroupId")
            });

            var students_array = students.Take(5).ToArray();

            Assert.That.Collection(students_array).All(student => student.IsNull());
        }
    }
}
