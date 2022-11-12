using System.IO;
using System.Text.RegularExpressions;

using MathCore.Annotations;
using MathCore.CSV;

namespace MathCore.Tests.CSV;

[TestClass]
public class CsvQueryTests : CSVTestsBase
{
    private const string __DataFileName = "CSVQueryTests_Students.csv";
    private const int __BeforeLinesCount = 3;
    private const int __AfterLinesCount = 3;

    [ClassInitialize]
    public static void ClassInitialize(TestContext Context)
    {
        var       seed     = (int)DateTime.Now.Ticks;
        var       students = GetStudents(new Random(seed));
        using var writer   = File.CreateText(__DataFileName);

        for(var i = 0; i < __BeforeLinesCount; i++)
            writer.WriteLine(LineDelimiter);

        writer.WriteLine(string.Join(ValuesSeparator, Headers));

        for (var i = 0; i < __BeforeLinesCount; i++)
            writer.WriteLine(LineDelimiter);

        foreach (var student in students)
            writer.WriteLine(string.Join(ValuesSeparator,
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

    [NotNull] private static FileInfo DataFile => new(__DataFileName);

    [TestMethod]
    public void ReadDataTest()
    {
        var query = DataFile.OpenCSV()
           .ValuesSeparator(ValuesSeparator)
           .SkipRowsBeforeHeader(__BeforeLinesCount)
           .WithHeader()
           .SkipRowsAfterHeader(__AfterLinesCount);

        var students = query.Select(line => new Student
        {
            Id       = line.ValueAs<int>("Id"),
            Name     = line["Name"],
            SurName  = line["SurName"],
            Birthday = line.ValueAs<DateTime>("Birthday"),
            Rating   = line.ValueAs<double>("Rating"),
            GroupId  = line.ValueAs<int>("GroupId")
        });

        var students_array = students.Take(5).ToArray();

        Assert.That.Collection(students_array)
           .AllItems((student, i) => student
               .Where(s => s.Id).Check(id => id.IsEqual(i + 1))
               .Where(s => s.Name).Check(Name => Name.IsEqual($"Name-{i + 1}"))
               .Where(s => s.SurName).Check(SurName => SurName.IsEqual($"SurName-{i + 1}"))
            );
    }

    [TestMethod]
    public void GetHeaderTest()
    {
        var header = DataFile
           .OpenCSV(';')
           .SkipRowsBeforeHeader(__BeforeLinesCount)
           .WithHeader()
           .GetHeader();

        var header_columns = Headers
           .Select((Name, Index) => (Name, Index))
           .ToDictionary(
                HeaderColumn => HeaderColumn.Name, 
                HeaderColumn => HeaderColumn.Index);

        var expected_headers = new SortedList<string, int>(header_columns);

        Assert.That.Collection(header.Keys).IsEqualTo(expected_headers.Keys);
        Assert.That.Collection(header).IsEqualTo(expected_headers);
    }

    [TestMethod]
    public void RegExValues()
    {
        const string test_data = @"6734,""03OI"",""heliport"",""Cleveland Clinic, Marymount Hospital Heliport"",41.420312,-81.599552,890,""NA"",""US"",""US-OH"",""Garfield Heights"",""no"",""03OI"",,""03OI"",,,";

        var regex = new Regex(@"(?<=(?:,|\n|^))(""(?:(?:"""")*[^""]*)*""|[^"",\n]*|(?:\n|$))");

        var values = regex.Matches(test_data).Select(m => m.Value is { Length: > 2 } v && v[0] == '"' && v[^1] == '"' ? v[1..^1] : m.Value).ToArray();
    }
}