using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathCore.Monades.WorkFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Monades.WorkFlow
{
    [TestClass]
    public partial class WorkTests
    {
        [TestMethod]
        public void Work_With_ReturnValue()
        {
            const string expected_string = "Hello World!";

            var work_result = Work.With(expected_string).Execute();

            Assert.That.Value(work_result)
               .Is<WorkResult<string>>()
               .Where(Result => Result.Result).Check(value => value.IsEqual(expected_string))
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Work_Begin_Action_Success()
        {
            var executed = false;
            void WorkMethod() => executed = true;

            var work_result = Work.Begin(WorkMethod).Execute();

            Assert.That.Value(executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Work_Begin_Action_Failure()
        {
            var executed = false;
            const string exception_message = "Test exception message";
            void WorkMethod()
            {
                executed = true;
                throw new ApplicationException(exception_message);
            }

            var work_result = Work.Begin(WorkMethod).Execute();

            Assert.That.Value(executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(Value => Value.As<ApplicationException>()
                   .Where(exception => exception.Message).IsEqual(exception_message))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Work_Begin_Function_Success()
        {
            var executed = false;
            const string expected_string = "Hello World!";
            string WorkFunction()
            {
                executed = true;
                return expected_string;
            }

            var work_result = Work.Begin(WorkFunction).Execute();

            Assert.That.Value(executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(Result => Result.Result).Check(Value => Value.IsEqual(expected_string))
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Work_Do_Action_Success()
        {
            var first_executed = false;
            var second_executed = false;

            void FirstWorkAction() => first_executed = true;
            void SecondWorkAction() => second_executed = true;

            var work_result = Work.Begin(FirstWorkAction)
               .Do(SecondWorkAction)
               .Execute();

            Assert.That.Value(first_executed).IsTrue();
            Assert.That.Value(second_executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Work_Do_Action_FirstFailure()
        {
            var first_executed = false;
            var second_executed = false;
            const string exception_message = "Test Exception message";

            void FirstWorkAction()
            {
                first_executed = true;
                throw new ApplicationException(exception_message);
            }
            void SecondWorkAction() => second_executed = true;

            var work_result = Work.Begin(FirstWorkAction)
               .Do(SecondWorkAction)
               .Execute();

            Assert.That.Value(first_executed).IsTrue();
            Assert.That.Value(second_executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.As<ApplicationException>()
                   .Where(exception => exception.Message).IsEqual(exception_message))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Work_Do_Action_LastFailure()
        {
            var first_executed = false;
            var second_executed = false;
            const string exception_message = "Test Exception message";

            void FirstWorkAction() => first_executed = true;
            void SecondWorkAction()
            {
                second_executed = true;
                throw new ApplicationException(exception_message);
            }

            var work_result = Work.Begin(FirstWorkAction)
               .Do(SecondWorkAction)
               .Execute();

            Assert.That.Value(first_executed).IsTrue();
            Assert.That.Value(second_executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.As<ApplicationException>()
                   .Where(exception => exception.Message).IsEqual(exception_message))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Work_Do_Action_AllFailure()
        {
            var first_executed = false;
            var second_executed = false;
            const string exception_message1 = "Test Exception message 1";
            const string exception_message2 = "Test Exception message 2";

            void FirstWorkAction()
            {
                first_executed = true;
                throw new ApplicationException(exception_message1);
            }

            void SecondWorkAction()
            {
                second_executed = true;
                throw new InvalidOperationException(exception_message2);
            }

            var work_result = Work.Begin(FirstWorkAction)
               .Do(SecondWorkAction)
               .Execute();

            Assert.That.Value(first_executed).IsTrue();
            Assert.That.Value(second_executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(Value => Value.As<AggregateException>()
                   .Where(exception => exception.InnerExceptions).Check(value =>
                    {
                        value
                           .Where(ee => ee.Count).Check(ErrorsCount => ErrorsCount.IsEqual(2))
                           .Where(ee => ee[0]).Check(e => e.As<InvalidOperationException>().Where(err => err.Message).IsEqual(exception_message2))
                           .Where(ee => ee[1]).Check(e => e.As<ApplicationException>().Where(err => err.Message).IsEqual(exception_message1));
                    }))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }
    }
}
