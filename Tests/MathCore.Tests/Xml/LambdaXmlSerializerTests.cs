using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Xml
{
    [TestClass]
    public class LambdaXmlSerializerTests
    {
        private class Student
        {
            public string Name { get; set; }
            public string SureName { get; set; }
            public string Patronymic { get; set; }

            public Group Group { get; set; }

            public List<int> Ratings { get; set; } = new List<int>();

            public string Description { get; set; }
        }

        private class Group
        {
            public string Name { get; set; }

            public Student Leader { get; set; }

            public List<Student> Students { get; set; } = new List<Student>();
        }

        [TestMethod]
        public void ObjectGraphSerialization()
        {
            var group = new Group
            {
                Name = "04-216",
                Students = new List<Student>
                {
                    new Student {SureName = "Иванов", Name = "Иван", Patronymic = "Иванович", Ratings = { 3,4,5 }},
                    new Student {SureName = "Петров", Name = "Пётр", Patronymic = "Петрович", Ratings = { 2,3,4 }, Description = "Описание"},
                    new Student {SureName = "Сидоров", Name = "Сидор", Patronymic = "Сидорович", Ratings = { 1,2,3 }},
                }
            };

            group.Leader = group.Students.First();
            group.Students.ForEach(student => student.Group = group);

            var serializer = LambdaXmlSerializer.Create<Group>()
                   .Attribute("Name", g => g.Name)
                   .Attribute("Leader", g => g.Leader.SureName)
                   .Attributes(g => g.Students, (s, i) => $"Student{i + 1}", s => s.SureName)
                       .Elements("Student", g => g.Students, student => student
                           .Attribute("SureName", s => s.SureName)
                           .Attribute("Name", s => s.Name)
                           .Attribute("Patronymic", s => s.Patronymic)
                           .Element("Ratings", s => s.Ratings, rating => rating
                               .Attribute("Min", r => r.Min())
                               .Attribute("Max", r => r.Max())
                               .Attribute("Avg", rr => rr.Average())
                               .Value(r => r.ToSeparatedStr(",")))
                           .Element("Description", s => s.Description, s => !string.IsNullOrEmpty(s)))
                ;

            var xml = serializer.Serialize(group);
        }

    }
}
