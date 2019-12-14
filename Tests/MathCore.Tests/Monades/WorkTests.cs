using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Monades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MathCore.Tests.Monades
{
    [TestClass]
    public partial class WorkTests
    {
        private static (List<string> Messages, Action<string> AddMethod) CreateMessagesStorage()
        {
            var messages = new List<string>();
            void AddMessage(string msg) => messages.Add(msg ?? throw new ArgumentNullException(nameof(msg)));
            return (messages, AddMessage);
        }

        [TestMethod]
        public void SingleActionSuccessWork()
        {
            var (messages, add_message) = CreateMessagesStorage();

            const string first_message = "Hello World!";
            var work = Work.Begin(() => add_message(first_message));

            CollectionAssert.That.Collection(messages).NotContains(first_message);

            Assert.IsFalse(work.Executed);
            work.Execute();
            Assert.IsTrue(work.Executed);

            CollectionAssert.That.Collection(messages).Contains(first_message);
            Assert.IsTrue(work.Success);
            Assert.IsFalse(work.Failure);
        }

        [TestMethod]
        public void SingleFunctionSuccessWork()
        {
            const string first_message = "Hello World!";
            var work = Work.Begin(() => first_message);

            Assert.IsFalse(work.Executed);
            Assert.That.Value(work.Result).IsEqual(first_message);
            Assert.IsTrue(work.Executed);
        }

        [TestMethod]
        public void SingleActionFailWork()
        {
            var (messages, add_message) = CreateMessagesStorage();

            const string first_message = null;
            var work = Work.Begin(() => add_message(first_message));

            CollectionAssert.That.Collection(messages).NotContains(first_message);

            Assert.IsFalse(work.Executed);
            work.Execute();
            Assert.IsTrue(work.Executed);

            CollectionAssert.That.Collection(messages).NotContains(first_message);
            Assert.IsFalse(work.Success);
            Assert.IsTrue(work.Failure);
            Assert.That.Value(work.CurrentError)
               .As<ArgumentNullException>()
               .Where(error => error.ParamName)
               .IsEqual("msg");
        }

        [TestMethod]
        public void SequenceActionWorks()
        {
            var (messages, add_message) = CreateMessagesStorage();
            const string first_message = "First message";
            const string anyway_message1 = "Anyway message1";
            const string anyway_message2 = "Anyway message2";

            var work = Work.Begin(() => add_message(first_message))
                   .Do(() => add_message(anyway_message1))
                   .Do(() => add_message(anyway_message2));

            work.Execute();

            CollectionAssert.That.Collection(messages).Contains(first_message);
            CollectionAssert.That.Collection(messages).Contains(anyway_message1);
            CollectionAssert.That.Collection(messages).Contains(anyway_message2);

            Assert.That.Value(work.SubWorks.Count()).IsEqual(2);
            Assert.IsTrue(work.SubWorks.All(w => w.Executed));
            Assert.IsTrue(work.SubWorks.All(w => w.Success));
        }

        [TestMethod]
        public void SequenceActionSuccessAndFailureWorks()
        {
            var (messages, add_message) = CreateMessagesStorage();
            const string message1 = "Message1";
            const string message2 = "Message2";
            const string not_exist_message1 = "<not-exist>";
            const string not_exist_message2 = "<null>";
            const string anyway_message = "Anyway";

            var not_executed_1 = true;
            var not_executed_2 = true;
            var work = Work
               .Begin(() => add_message(message1))
               .IfSuccess(() => add_message(message2))
               .IfFailure(() => { add_message(not_exist_message1); not_executed_1 = false; })
               .IfSuccess(() => add_message(null))
               .IfSuccess(() => { add_message(not_exist_message2); not_executed_2 = false; })
               .Do(() => add_message(anyway_message));

            Assert.IsFalse(work.Executed);
            work.Execute();
            Assert.IsTrue(work.Executed);
            Assert.IsTrue(not_executed_1);
            Assert.IsTrue(not_executed_2);

            Assert.IsFalse(work.Success);
            Assert.IsTrue(work.Failure);
            Assert.That.Value(work.CurrentError).IsNull();
            Assert.That.Value(work.Error)
               .As<ArgumentNullException>().Where(error => error.ParamName).IsEqual("msg");

            CollectionAssert.That.Collection(messages).IsEqualTo(new [] { message1, message2, anyway_message });
        }

        [TestMethod]
        public void SequenceSuccessFunctionWorks()
        {
            var messages = new List<string>();
            var values = new List<int>();

            const string message = "Message";
            var work = Work.Begin(() => message)
                   .Do(messages.Add)
                   .IfSuccess(str => str.Length)
                   .IfFailure(e => e.Message.Length)
                   .Do(values.Add)
                ;

            work.Execute();
            CollectionAssert.That.Collection(messages).Contains(message);
            CollectionAssert.That.Collection(values).NotContains(message.Length);
        }

        [TestMethod]
        public void SequenceFailureFunctionWorks()
        {
            var messages = new List<string>();

            const string message = "Message";
            var work = Work.Begin(() => message)
                   .Do(msg => throw new InvalidOperationException(msg))
                   .IfSuccess(() => messages.Add("Not success"))
                   .IfFailure(e => messages.Add(e.Message))
                ;

            work.Execute();
            CollectionAssert.That.Collection(messages).Contains(message);
            CollectionAssert.That.Collection(messages).NotContains("Not success");
        }
    }
}
