namespace MathCore.Tests
{
    [TestClass]
    public class CommandLineArgsTests
    {
        [TestMethod]
        public void Creation()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            string[] expected_args =
            {
                "file1.bin", "file2.bin",
                "-wps", "c:\\Program files 321\\123",
                "123",
                "--in", "file3.txt", "file4.txt", "file5.txt",
                "--in", "file6.txt",
                "-o", "result.bin",
                "--", "file7.qwe", "file8.asd"
            };

            var parser = new CommandLineArgs(command_line.Split(' '));

            Assert.That.Enumerable(parser.Args)
               .IsEqualTo(expected_args);
        }

        [TestMethod]
        public void KeySelection()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var expected_values_in = new[] { "file3.txt", "file4.txt", "file5.txt", "file6.txt" };
            var expected_values_o = new[] { "result.bin" };
            var expected_values__ = new[] { "file7.qwe", "file8.asd" };

            var parser = new CommandLineArgs(command_line.Split(' '));

            var values_in = parser["in"];
            var values_o = parser["o"];
            var values__ = parser["--"];

            Assert.That.Enumerable(values_in).IsEqualTo(expected_values_in);
            Assert.That.Enumerable(values_o).IsEqualTo(expected_values_o);
            Assert.That.Enumerable(values__).IsEqualTo(expected_values__);
        }

        [TestMethod]
        public void StartArgs()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var expected_start_args = new[] { "file1.bin", "file2.bin" };
            var expected_end_args = new[] { "file7.qwe", "file8.asd" };

            var parser = new CommandLineArgs(command_line.Split(' '));

            var values_start = parser.StartArgs;
            var values_end = parser.EndArgs;

            Assert.That.Enumerable(values_start).IsEqualTo(expected_start_args);
            Assert.That.Enumerable(values_end).IsEqualTo(expected_end_args);
        }

        [TestMethod]
        public void FreeArgs()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var expected_start_args = new[] { "file1.bin", "file2.bin" };
            var expected_end_args = new[] { "file7.qwe", "file8.asd" };

            var parser = new CommandLineArgs(command_line.Split(' '));

            var values_free_args = parser.FreeArgs;

            Assert.That.Enumerable(values_free_args).IsEqualTo(expected_start_args.Concat(expected_end_args));
        }

        [TestMethod]
        public void KeyValues()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            var valeus = parser.KeyValues.ToLookup(v => v.Key, v => v.Value);

            Assert.That.Value(valeus.Count).IsEqual(7);
            Assert.That.Enumerable(valeus[""]).IsEqualTo(new[] { "file1.bin", "file2.bin" });

            Assert.That.Enumerable(valeus["w"]).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Enumerable(valeus["p"]).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Enumerable(valeus["s"]).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });

            Assert.That.Enumerable(valeus["in"]).IsEqualTo(new[] { "file3.txt", "file4.txt", "file5.txt", "file6.txt" });
            Assert.That.Enumerable(valeus["o"]).IsEqualTo(new[] { "result.bin" });

            Assert.That.Enumerable(valeus["--"]).IsEqualTo(new[] { "file7.qwe", "file8.asd" });
        }

        [TestMethod]
        public void ContainsKey()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            Assert.That.Value(parser.ContainsKey("")).IsTrue();
            Assert.That.Value(parser.ContainsKey("123")).IsFalse();
            Assert.That.Value(parser.ContainsKey("w")).IsTrue();
            Assert.That.Value(parser.ContainsKey("p")).IsTrue();
            Assert.That.Value(parser.ContainsKey("s")).IsTrue();
            Assert.That.Value(parser.ContainsKey("in")).IsTrue();
            Assert.That.Value(parser.ContainsKey("o")).IsTrue();
            Assert.That.Value(parser.ContainsKey("--")).IsTrue();
        }

        [TestMethod]
        public void GetKeyValues()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            Assert.That.Enumerable(parser.GetKeyValues("").Single()).IsEqualTo(new[] { "file1.bin", "file2.bin" });

            Assert.That.Enumerable(parser.GetKeyValues("w").Single()).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Enumerable(parser.GetKeyValues("p").Single()).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Enumerable(parser.GetKeyValues("s").Single()).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });

            Assert.That.Value(parser.GetKeyValues("in").Count()).IsEqual(2);
            Assert.That.Enumerable(parser.GetKeyValues("in").First()).IsEqualTo(new[] { "file3.txt", "file4.txt", "file5.txt" });
            Assert.That.Enumerable(parser.GetKeyValues("in").Last()).IsEqualTo(new[] { "file6.txt" });

            Assert.That.Enumerable(parser.GetKeyValues("o").Single()).IsEqualTo(new[] { "result.bin" });

            Assert.That.Enumerable(parser.GetKeyValues("--").Single()).IsEqualTo(new[] { "file7.qwe", "file8.asd" });
        }

        [TestMethod]
        public void KeysCount()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            Assert.That.Value(parser.KeysCount("")).IsEqual(1);

            Assert.That.Value(parser.KeysCount("w")).IsEqual(1);
            Assert.That.Value(parser.KeysCount("p")).IsEqual(1);
            Assert.That.Value(parser.KeysCount("s")).IsEqual(1);

            Assert.That.Value(parser.KeysCount("in")).IsEqual(2);
            Assert.That.Value(parser.KeysCount("o")).IsEqual(1);

            Assert.That.Value(parser.KeysCount("--")).IsEqual(1);
        }

        [TestMethod]
        public void ExecForKey()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            var values_ = new List<string>();
            var values_w = new List<string>();
            var values_p = new List<string>();
            var values_s = new List<string>();
            var values_in = new List<string>();
            var values_o = new List<string>();
            var values__ = new List<string>();

            var values_tst = new List<string>();

            bool Check(string key, List<string> values) => parser.ExecForKey(key, values.Add);

            Assert.That.Value(Check("", values_)).IsTrue().And.Collection(values_).IsEqualTo(new[] { "file1.bin", "file2.bin" });

            Assert.That.Value(Check("w", values_w)).IsTrue().And.Collection(values_w).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Value(Check("p", values_p)).IsTrue().And.Collection(values_p).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });
            Assert.That.Value(Check("s", values_s)).IsTrue().And.Collection(values_s).IsEqualTo(new[] { "c:\\Program files 321\\123", "123" });

            Assert.That.Value(Check("in", values_in)).IsTrue().And.Collection(values_in).IsEqualTo(new[] { "file3.txt", "file4.txt", "file5.txt", "file6.txt" });
            Assert.That.Value(Check("o", values_o)).IsTrue().And.Collection(values_o).IsEqualTo(new[] { "result.bin" });

            Assert.That.Value(Check("--", values__)).IsTrue().And.Collection(values__).IsEqualTo(new[] { "file7.qwe", "file8.asd" });

            Assert.That.Value(Check("tst", values_tst)).IsFalse().And.Collection(values_tst).IsEmpty();
        }

        [TestMethod]
        public void KeysIndexer()
        {
            const string command_line = "file1.bin file2.bin -wps \"c:\\Program files 321\\123\" 123 "
                + "--in file3.txt file4.txt file5.txt --in file6.txt -o result.bin "
                + "-- file7.qwe file8.asd";

            var parser = new CommandLineArgs(command_line.Split(' '));

            Assert.That.Enumerable(parser["in"]).IsEqualTo(new[] { "file3.txt", "file4.txt", "file5.txt", "file6.txt" });
            Assert.That.Enumerable(parser["tst"]).IsEqualTo(Enumerable.Empty<string>());
        }
    }
}
