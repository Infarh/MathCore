using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
                           .Element("Ratings", s => s.Ratings, s=>s.Average() > 2, rating => rating
                               .Attribute("Min", r => r.Min())
                               .Attribute("Max", r => r.Max())
                               .Attribute("Avg", rr => rr.Average())
                               .Value(r => r.ToSeparatedStr(",")))
                           .Element("Description", s => s.Description, s => !string.IsNullOrEmpty(s)))
                ;

            var xml = serializer.Serialize(group);

            Assert.That.Value(xml.Name).IsEqual(typeof(Group).Name);
            Assert.That.Value((string)xml.Attribute("Name")).IsEqual("04-216");
            Assert.That.Value(xml.XPathString("@Leader")).IsEqual(group.Students.First().SureName);

            CollectionAssert.That
               .Collection(xml.Attributes().Where(a => a.Name.LocalName.StartsWith("Student")).Select(a => a.Value).ToArray())
               .IsEqualTo(group.Students.Select(s => s.SureName).ToArray());

            for (var i = 0; i < group.Students.Count; i++)
            {
                Assert.That.Value(xml.XPathString($"Student[{i + 1}]/@SureName")).IsEqual(group.Students[i].SureName);
                Assert.That.Value(xml.XPathString($"Student[{i + 1}]/@Name")).IsEqual(group.Students[i].Name);
                Assert.That.Value(xml.XPathString($"Student[{i + 1}]/@Patronymic")).IsEqual(group.Students[i].Patronymic);
            }

            Assert.That.Value(xml.XPath("Student[1]/Ratings")).IsNotNull();
            Assert.That.Value(xml.XPathInt32("Student[1]/Ratings/@Min")).IsEqual(3);
            Assert.That.Value(xml.XPathInt32("Student[1]/Ratings/@Max")).IsEqual(5);

            Assert.That.Value(xml.XPath("Student[2]/Ratings")).IsNotNull();
            Assert.That.Value(xml.XPathInt32("Student[2]/Ratings/@Min")).IsEqual(2);
            Assert.That.Value(xml.XPathInt32("Student[2]/Ratings/@Max")).IsEqual(4);

            Assert.That.Value(xml.XPath("Student[3]/Ratings")).IsNull();
            Assert.That.Value(xml.XPathInt32("Student[3]/Ratings/@Min")).IsEqual(null);
            Assert.That.Value(xml.XPathInt32("Student[3]/Ratings/@Max")).IsEqual(null);

            Assert.That.Value(xml.XPathDouble("Student[1]/Ratings/@Avg")).IsEqual(group.Students[0].Ratings.Average());
            Assert.That.Value(xml.XPathDouble("Student[2]/Ratings/@Avg")).IsEqual(group.Students[1].Ratings.Average());
            Assert.That.Value(xml.XPathDouble("Student[3]/Ratings/@Avg")).IsEqual(null);

            Assert.That.Value(xml.XPath("Student[1]/Ratings"))
               .As<XElement>()
               .Where(e => e.Value)
               .IsEqual(group.Students[0].Ratings.ToSeparatedStr(","));
            Assert.That.Value(xml.XPath("Student[2]/Ratings"))
               .As<XElement>()
               .Where(e => e.Value)
               .IsEqual(group.Students[1].Ratings.ToSeparatedStr(","));
        }

    }
}
