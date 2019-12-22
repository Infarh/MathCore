using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathCore.Annotations;
using MathCore.Monades.WorkFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Monades.WorkFlow
{
    [TestClass]
    public partial class WorkTests
    {
        private class TestAction
        {
            [NotNull] public static TestAction GetSuccess() => new TestAction();

            [NotNull]
            public static TestFailAction<TException> GetFail<TException>([NotNull] TException Error) where TException : Exception =>
                new TestFailAction<TException>(Error);

            public bool Executed { get; private set; }

            protected virtual void Execute() => Executed = true;

            [NotNull] public static implicit operator Action([NotNull] TestAction action) => action.Execute;
        }

        private class TestFailAction<TException> : TestAction where TException : Exception
        {
            [NotNull] public TException Exception { get; }

            public TestFailAction([NotNull] TException Error) => Exception = Error ?? throw new ArgumentNullException(nameof(Error));

            protected override void Execute()
            {
                base.Execute();
                throw Exception;
            }
        }

        private abstract class TestFunction
        {
            public static ValueFunction<T> Value<T>(T Value, Exception Error = null) => new ValueFunction<T>(Value, Error);

            private readonly Exception _Exception;

            public bool Executed { get; protected set; }

            protected TestFunction(Exception Error) => _Exception = Error;

            protected void ExecuteFunction()
            {
                Executed = true;
                if (_Exception != null) throw _Exception;
            }
        }

        private class ValueFunction<T> : TestFunction
        {
            public T ReturnValue { get; }
            public ValueFunction(T ReturnValue, Exception Error) : base(Error) => this.ReturnValue = ReturnValue;

            public T Execute()
            {
                ExecuteFunction();
                return ReturnValue;
            }
        }

        [TestMethod]
        public void With_ReturnValue()
        {
            var work_result = Work.With("Hello World!").Execute();

            Assert.That.Value(work_result)
               .Is<WorkResult<string>>()
               .Where(Result => Result.Result).Check(value => value.IsEqual(work_result.Result))
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Begin_Action_Success()
        {
            var action = new TestAction();

            var work_result = Work.Begin(action).Execute();

            Assert.That.Value(action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Begin_Action_Failure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Test exception message"));

            var work_result = Work.Begin(fail_action).Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(Value => Value.IsEqual(fail_action.Exception))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Begin_Function_Success()
        {
            const string expected_string = "Hello World!";
            var success_function = TestFunction.Value(expected_string);
            var work_result = Work.Begin(success_function.Execute).Execute();

            Assert.That.Value(success_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(Result => Result.Result).Check(Value => Value.IsEqual(expected_string))
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Do_Action_Success()
        {
            var first_action = TestAction.GetSuccess();
            var second_action = TestAction.GetSuccess();

            var work_result = Work.Begin(first_action)
               .Do(second_action)
               .Execute();

            Assert.That.Value(first_action.Executed).IsTrue();
            Assert.That.Value(second_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Do_Action_FirstFailure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Test Exception message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(fail_action)
               .Do(test_action)
               .Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsEqual(fail_action.Exception))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Do_Action_LastFailure()
        {
            const string exception_message = "Test Exception message";
            var first_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException(exception_message));

            var work_result = Work.Begin(first_action)
               .Do(fail_action)
               .Execute();

            Assert.That.Value(first_action.Executed).IsTrue();
            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.As<ApplicationException>()
                   .Where(exception => exception.Message).IsEqual(exception_message))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void Do_Action_AllFailure()
        {
            const string exception_message1 = "Test Exception message 1";
            const string exception_message2 = "Test Exception message 2";
            var first_fail_action = TestAction.GetFail(new ApplicationException(exception_message1));
            var second_fail_action = TestAction.GetFail(new InvalidOperationException(exception_message2));

            var work_result = Work.Begin(first_fail_action)
               .Do(second_fail_action)
               .Execute();

            Assert.That.Value(first_fail_action.Executed).IsTrue();
            Assert.That.Value(second_fail_action.Executed).IsTrue();
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

        [TestMethod]
        public void IfSuccess_Executed_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(error => error.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void IfSuccess_NotExecuted_WhenBaseWorkSuccess()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(fail_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void IfFailure_NotExecuted_WhenBaseWorkSuccess()
        {
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(success_action)
               .IfFailure(test_action)
               .Execute();

            Assert.That.Value(success_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(error => error.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void IfFailure_Executed_WhenBaseWorkFailure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(fail_action)
               .IfFailure(test_action)
               .Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void IfFailure_ExceptionHandler_Executed_WhenBaseWorkFailure()
        {
            var expected_exception = new ApplicationException("Error message");
            var fail_action = TestAction.GetFail(expected_exception);

            var exception_handler_executed = false;
            Exception handled_exception = null;
            void ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
            }

            var work_result = Work.Begin(fail_action)
               .IfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(exception_handler_executed).IsTrue();
            Assert.That.Value(handled_exception).IsEqual(expected_exception);
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void Second_IfSuccess_Execute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .IfSuccess(success_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(success_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void Second_IfSuccess_NotExecute_WhenBaseWorkFailure()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .IfSuccess(fail_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void Second_IfSuccess_AfterDo_Execute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .Do(success_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(success_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void Second_IfSuccess_AfterDo_NotExecute_WhenBaseWorkFailure()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void IfFail_NotExecute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action1 = TestAction.GetSuccess();
            var success_action2 = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .Do(success_action1)
               .IfSuccess(success_action2)
               .IfFailure(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(success_action1.Executed).IsTrue();
            Assert.That.Value(success_action2.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void IfFail_Execute_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_execute_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(no_execute_action)
               .IfFailure(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(no_execute_action.Executed).IsFalse();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void Do_Execute_AfterBaseFailWork()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_execute_action = TestAction.GetSuccess();
            var on_fail_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(no_execute_action)
               .IfFailure(on_fail_action)
               .Do(test_action)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(no_execute_action.Executed).IsFalse();
            Assert.That.Value(on_fail_action.Executed).IsTrue();
            Assert.That.Value(test_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(fail_action.Exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void Begin_FunctionWork_ReturnValue()
        {
            var test_function = TestFunction.Value("Hello World");

            var work_result = Work.Begin(test_function.Execute).Execute();

            Assert.That.Value(test_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(value => value.IsEqual(test_function.ReturnValue))
               .Where(result => result.Error).Check(error => error.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void Do_Function_Success()
        {
            var begin_action = TestAction.GetSuccess();
            var test_function = TestFunction.Value("Hello World");

            var work_result = Work.Begin(begin_action)
               .Do(test_function.Execute)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(test_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(value => value.IsEqual(test_function.ReturnValue))
               .Where(result => result.Error).Check(error => error.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_IfSuccess_NotExecuted_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_function = TestFunction.Value("Hello World");

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(test_function.Execute)
               .Execute();

            Assert.That
               .Value(begin_action.Executed).IsTrue().And
               .Value(fail_action.Executed).IsTrue().And
               .Value(test_function.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(str => str.IsNull())
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue())
               .Where(result => result.Error).Check(error => error.IsEqual(fail_action.Exception));
        } 

        [TestMethod]
        public void FunctionWork_IfFailure_Executed_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_executed_function = TestFunction.Value("No executed function");
            var on_fail_function = TestFunction.Value("Hello World");

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(no_executed_function.Execute)
               .IfFailure(on_fail_function.Execute)
               .Execute();

            Assert.That
               .Value(begin_action.Executed).IsTrue().And
               .Value(fail_action.Executed).IsTrue().And
               .Value(no_executed_function.Executed).IsFalse().And
               .Value(on_fail_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(str => str.IsEqual(on_fail_function.ReturnValue))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue())
               .Where(result => result.Error).Check(error => error.IsEqual(fail_action.Exception));
        }

        [TestMethod]
        public void FunctionWork_ExceptionHandler_Executed_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_executed_function = TestFunction.Value("No executed function");

            var exception_handler_executed = false;
            string ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                return error.Message;
            }

            var work_result = Work.Begin(begin_action)
               .Do(fail_action)
               .IfSuccess(no_executed_function.Execute)
               .IfFailure(ExceptionHandler)
               .Execute();

            Assert.That
               .Value(begin_action.Executed).IsTrue().And
               .Value(fail_action.Executed).IsTrue().And
               .Value(no_executed_function.Executed).IsFalse().And
               .Value(exception_handler_executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(str => str.IsEqual(fail_action.Exception.Message))
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse())
               .Where(result => result.Error).Check(error => error.IsNull());
        }

        [TestMethod]
        public void FunctionWork_ExceptionHandler_NotExecuted_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var value_function = TestFunction.Value("No executed function");

            var exception_handler_executed = false;
            string ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                return error.Message;
            }

            var work_result = Work.Begin(begin_action)
               .IfSuccess(value_function.Execute)
               .IfFailure(ExceptionHandler)
               .Execute();

            Assert.That
               .Value(begin_action.Executed).IsTrue().And
               .Value(value_function.Executed).IsTrue().And
               .Value(exception_handler_executed).IsFalse();

            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(str => str.IsEqual(value_function.ReturnValue))
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse())
               .Where(result => result.Error).Check(error => error.IsNull());
        }
    }
}
