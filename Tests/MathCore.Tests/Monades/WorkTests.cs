using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Monades;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Monades
{
    [TestClass]
    public class WorkTests
    {
        [TestMethod]
        public void SimpleWork()
        {
            var messages = new List<string>();

            void AddMessage(string msg) => messages.Add(msg ?? throw new ArgumentNullException(nameof(msg)));

            var fail_executed = false;
            var work = Work.Start(() => AddMessage("Hello World!"))
               .IfSuccess(() => AddMessage("Success"))
               .IfSuccess(() => AddMessage(null))
               .IfSuccess(() =>
                {
                    fail_executed = true;
                    AddMessage("null");
                })
               .IfFailure(() => AddMessage("NotNull"))
               .Anyway(() => AddMessage("Anyway"));

            Assert.IsTrue(messages.Contains("Hello World!"));
            Assert.IsTrue(messages.Contains("Success"));
            Assert.IsTrue(messages.Contains("Anyway"));
            Assert.IsFalse(messages.Contains("null"));
            Assert.IsFalse(fail_executed);
            Assert.IsTrue(messages.Contains("NotNull"));
            Assert.That.Value(work.Errors.Single()).Is<ArgumentException>();
            Assert.IsTrue(work.Failure);
            Assert.IsFalse(work.Success);
            Assert.That.Value(work.Errors.Count()).IsEqual(1);
            Assert.That.Value(work.SubWorks.Count()).IsEqual(5);
        }

        [TestMethod]
        public void CreationOfWorkWithResult()
        {
            var work = Work.StartWithResult(() => "Hello World!");
            Assert.That.Value(work.Result).IsEqual("Hello World!");
        }

        [TestMethod]
        public void WorkWithResult()
        {
            var work = Work.StartWithResult(() => "Hello World!")
               //.IfSuccess()
                ;

        }
    }
}
