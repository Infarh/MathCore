using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Extensions
{
    [TestClass]
    public class IQueryableExtensionsTests
    {
        class Student
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public int GroupId { get; set; }

            public override string ToString() => $"{Name}[{Id}]({GroupId})";
        }

        class Group
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => $"{Name}[{Id}]";
        }

        class StudentGroup
        {
            public Student Student { get; set; }
            public Group Group { get; set; }
            public override string ToString() => $"{Student} - {Group}";
        }

        [TestMethod]
        public void LeftOuterJoinTest()
        {
            const int groups_count = 10;
            const int students_count = 100;
            var groups = Enumerable
               .Range(1, groups_count)
               .Select(i => new Group { Id = i, Name = $"Group {i}" })
               .AsQueryable();
            var students = Enumerable
               .Range(1, students_count)
               .Select(i => new Student { Id = i, Name = $"Student {i}", GroupId = (i * 3 + 7) % groups_count + 1 })
               .AsQueryable();

            var student_groups_query = groups.LeftOuterJoin(
                students,
                g => g.Id,
                s => s.GroupId,
                (g, s) => new StudentGroup { Group = g, Student = s }
                );

            var student_groups = student_groups_query.OrderBy(s => s.Student.Id).ToArray();
        }
    }
}
