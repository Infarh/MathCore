using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using MathCore.CSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.CSV
{
    [TestClass]
    public class CSVWriterTests : CSVTests
    {
        [TestMethod]
        public void WriteTest()
        {
            var result_builder = new StringBuilder(350);
            using (var writer = new StringWriter(result_builder))
            {
                var students = GetStudents().Take(5);

                Expression<Func<Student, object>> cs = student => student.Name;

                students.AsCSV()
                   .Separator(';')
                   .AddDefaultHeaders()
                   .AddColumn("Name-Length", s => s.Name.Length)
                   .WriteTo(writer);
            }
            var result = result_builder.ToString();
        }
    }
}
