using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathCore.Annotations;
using MathCore.Monads.WorkFlow;
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
        public void WithValue_ReturnValue()
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
        public void BeginInvoke_Action_Success()
        {
            var action = new TestAction();

            var work_result = Work.BeginInvoke(action).Execute();

            Assert.That.Value(action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void BeginInvoke_Action_Failure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Test exception message"));

            var work_result = Work.BeginInvoke(fail_action).Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(work_result)
               .Is<WorkResult>()
               .Where(Result => Result.Error).Check(Value => Value.IsEqual(fail_action.Exception))
               .Where(Result => Result.Success).Check(value => value.IsFalse())
               .Where(Result => Result.Failure).Check(value => value.IsTrue());
        }

        [TestMethod]
        public void BeginGet_Function_Success()
        {
            const string expected_string = "Hello World!";
            var success_function = TestFunction.Value(expected_string);
            var work_result = Work.BeginGet(success_function.Execute).Execute();

            Assert.That.Value(success_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(Result => Result.Result).Check(Value => Value.IsEqual(expected_string))
               .Where(Result => Result.Error).Check(value => value.IsNull())
               .Where(Result => Result.Success).Check(value => value.IsTrue())
               .Where(Result => Result.Failure).Check(value => value.IsFalse());
        }

        [TestMethod]
        public void Invoke_Action_Success()
        {
            var first_action = TestAction.GetSuccess();
            var second_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(first_action)
               .Invoke(second_action)
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
        public void BeginInvoke_Action_FirstFailure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Test Exception message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(fail_action)
               .Invoke(test_action)
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
        public void BeginInvoke_Action_LastFailure()
        {
            const string exception_message = "Test Exception message";
            var first_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException(exception_message));

            var work_result = Work.BeginInvoke(first_action)
               .Invoke(fail_action)
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
        public void Invoke_Action_AllFailure()
        {
            const string exception_message1 = "Test Exception message 1";
            const string exception_message2 = "Test Exception message 2";
            var first_fail_action = TestAction.GetFail(new ApplicationException(exception_message1));
            var second_fail_action = TestAction.GetFail(new InvalidOperationException(exception_message2));

            var work_result = Work.BeginInvoke(first_fail_action)
               .Invoke(second_fail_action)
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
        public void Invoke_Action_OnBaseWorkResult()
        {
            const string expected_value = "Hello World";
            var first_action = TestFunction.Value(expected_value);

            var test_action_executed = false;
            string actual_value = null;

            void TestAction(string str)
            {
                test_action_executed = true;
                actual_value = str;
            }

            var work_result = Work.BeginGet(first_action.Execute)
               .Invoke(TestAction)
               .Execute();

            Assert.That.Value(test_action_executed).IsTrue();
            Assert.That.Value(actual_value).IsEqual(expected_value);
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }


        [TestMethod]
        public void Invoke_Action_WithException_OnBaseWorkResult()
        {
            const string expected_value = "Hello World";
            var first_action = TestFunction.Value(expected_value);
            var expected_exception = new ApplicationException();

            var test_action_executed = false;
            string actual_value = null;

            void TestAction(string str)
            {
                test_action_executed = true;
                actual_value = str;
                throw expected_exception;
            }

            var work_result = Work.BeginGet(first_action.Execute)
               .Invoke(TestAction)
               .Execute();

            Assert.That.Value(test_action_executed).IsTrue();
            Assert.That.Value(actual_value).IsEqual(expected_value);
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(expected_exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void InvokeIfSuccess_Executed_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .InvokeIfSuccess(test_action)
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
        public void InvokeIfSuccess_NotExecuted_WhenBaseWorkSuccess()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(fail_action)
               .InvokeIfSuccess(test_action)
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
        public void InvokeOnFailure_NotExecuted_WhenBaseWorkSuccess()
        {
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(success_action)
               .InvokeIfFailure(test_action)
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
        public void InvokeOnFailure_Executed_WhenBaseWorkFailure()
        {
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(fail_action)
               .InvokeIfFailure(test_action)
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
        public void InvokeOnFailure_ExceptionHandler_Executed_WhenBaseWorkFailure()
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

            var work_result = Work.BeginInvoke(fail_action)
               .InvokeIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(fail_action.Executed).IsTrue();
            Assert.That.Value(exception_handler_executed).IsTrue();
            Assert.That.Value(handled_exception).IsEqual(expected_exception);
            Assert.That.Value(work_result)
               .As<WorkResult>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(expected_exception))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void Second_InvokeIfSuccess_Execute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .InvokeIfSuccess(success_action)
               .InvokeIfSuccess(test_action)
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

            var work_result = Work.BeginInvoke(begin_action)
               .InvokeIfSuccess(fail_action)
               .InvokeIfSuccess(test_action)
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
        public void Second_InvokeIfSuccess_AfterDo_Execute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(success_action)
               .InvokeIfSuccess(test_action)
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
        public void Second_InvokeIfSuccess_AfterDo_NotExecute_WhenBaseWorkFailure()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .InvokeIfSuccess(test_action)
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
        public void InvokeOnFailure_NotExecute_WhenBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var success_action1 = TestAction.GetSuccess();
            var success_action2 = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(success_action1)
               .InvokeIfSuccess(success_action2)
               .InvokeIfFailure(test_action)
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
        public void InvokeOnFailure_Execute_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_execute_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .InvokeIfSuccess(no_execute_action)
               .InvokeIfFailure(test_action)
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
        public void Invoke_Execute_AfterBaseFailWork()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_execute_action = TestAction.GetSuccess();
            var on_fail_action = TestAction.GetSuccess();
            var test_action = TestAction.GetSuccess();

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .InvokeIfSuccess(no_execute_action)
               .InvokeIfFailure(on_fail_action)
               .Invoke(test_action)
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

            var work_result = Work.BeginGet(test_function.Execute).Execute();

            Assert.That.Value(test_function.Executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(value => value.IsEqual(test_function.ReturnValue))
               .Where(result => result.Error).Check(error => error.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void Get_Function_Success()
        {
            var begin_action = TestAction.GetSuccess();
            var test_function = TestFunction.Value("Hello World");

            var work_result = Work.BeginInvoke(begin_action)
               .Get(test_function.Execute)
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
        public void FunctionWork_GetIfSuccess_NotExecuted_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var test_function = TestFunction.Value("Hello World");

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .GetIfSuccess(test_function.Execute)
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
        public void FunctionWork_GetIfFailure_Executed_WhenBaseWorkFail()
        {
            var begin_action = TestAction.GetSuccess();
            var fail_action = TestAction.GetFail(new ApplicationException("Error message"));
            var no_executed_function = TestFunction.Value("No executed function");
            var on_fail_function = TestFunction.Value("Hello World");

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .GetIfSuccess(no_executed_function.Execute)
               .GetIfFailure(on_fail_function.Execute)
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
        public void FunctionWork_GetIfFailure_NoExecuted_IfBaseWorkSuccess()
        {
            var begin_action = TestAction.GetSuccess();
            var on_fail_function = TestFunction.Value("Hello World");

            var work_result = Work.BeginInvoke(begin_action)
               .GetIfFailure(on_fail_function.Execute)
               .Execute();

            Assert.That.Value(begin_action.Executed).IsTrue();
            Assert.That.Value(on_fail_function.Executed).IsFalse();
            Assert.That.Value(work_result)
               .As<WorkResult<string>>()
               .Where(result => result.Result).Check(value => value.IsNull())
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
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

            var work_result = Work.BeginInvoke(begin_action)
               .Invoke(fail_action)
               .GetIfSuccess(no_executed_function.Execute)
               .GetIfFailure(ExceptionHandler)
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

            var work_result = Work.BeginInvoke(begin_action)
               .GetIfSuccess(value_function.Execute)
               .GetIfFailure(ExceptionHandler)
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

        [TestMethod]
        public void FunctionWork_ValueConversion_ExecuteSuccessfully()
        {
            const string data_value = "123456789";

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .Get(x => x.ToBase(10).Average())
               .Execute();

            Assert.That.Value(work_result)
               .As<WorkResult<int, double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(5))
               .Where(result => result.Parameter).Check(value => value.IsEqual(123456789))
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_ValueConversion_Executed_IfBaseWorkSuccess()
        {
            const string data_value = "123456789";

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .GetIfSuccess(x => x.ToBase(10).Average())
               .Execute();

            Assert.That.Value(work_result)
               .As<WorkResult<int, double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(5))
               .Where(result => result.Parameter).Check(value => value.IsEqual(123456789))
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_ValueConversion_NoExecuted_IfBaseWorkFailure()
        {
            const string data_value = "1234!56789";

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .GetIfSuccess(x => x.ToBase(10).Average())
               .Execute();

            Assert.That.Value(work_result)
               .As<WorkResult<int, double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(default))
               .Where(result => result.Parameter).Check(value => value.IsEqual(default))
               .Where(result => result.Error).Check(exception => exception.Is<FormatException>())
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void FunctionWork_ValueConversion_ExceptionHandler_NoExecuted_IfBaseWorkSuccess()
        {
            const string data_value = "123456789";

            var exception_handler_executed = false;
            Exception handled_exception = null;

            double ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
                return double.NaN;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .GetIfSuccess(x => x.ToBase(10).Average())
               .GetIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(exception_handler_executed).IsFalse();
            Assert.That.Value(handled_exception).IsNull();

            Assert.That.Value(work_result)
               .As<WorkResult<int, double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(5))
               .Where(result => result.Parameter).Check(value => value.IsEqual(123456789))
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_ValueConversion_ExceptionHandler_Executed_IfBaseWorkFailure()
        {
            const string data_value = "1234!56789";

            var exception_handler_executed = false;
            Exception handled_exception = null;
            const double failure_value = double.NaN;

            double ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
                return failure_value;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .GetIfSuccess(x => x.ToBase(10).Average())
               .GetIfFailure(ExceptionHandler)
               .Execute();

            Assert
               .That.Value(exception_handler_executed).IsTrue()
               .And.Value(handled_exception).Is<FormatException>()
               .And.Value(work_result)
               .As<WorkResult<double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(failure_value))
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(state => state.IsTrue())
               .Where(result => result.Failure).Check(state => state.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_ValueConversion_ExceptionHandler_ExecuteWithFail()
        {
            const string data_value = "1234!56789";

            var exception_handler_executed = false;
            Exception handled_exception = null;
            var expected_exception_handler_exception = new ApplicationException("Exception of exception handler");
            double ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
                throw expected_exception_handler_exception;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .GetIfSuccess(x => x.ToBase(10).Average())
               .GetIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(exception_handler_executed).IsTrue();
            Assert.That.Value(handled_exception).Is<FormatException>();
            Assert.That.Value(work_result)
               .As<WorkResult<double>>()
               .Where(result => result.Result).Check(value => value.IsEqual(default))
               .Where(result => result.Error).Check(exception => exception.As<AggregateException>()
                   .Where(ex => ex.InnerExceptions[0]).Check(ex0 => ex0.Is<FormatException>())
                   .Where(ex => ex.InnerExceptions[1]).Check(ex1 => ex1.IsEqual(expected_exception_handler_exception)))
               .Where(result => result.Success).Check(state => state.IsFalse())
               .Where(result => result.Failure).Check(state => state.IsTrue());
        }

        [TestMethod]
        public void FunctionWork_ReturnFailResult_OnException()
        {
            var expected_exception = new ApplicationException("Expected exception");
            var test_function_executed = false;
            int TestFunction()
            {
                test_function_executed = true;
                throw expected_exception;
            }

            var work_result = Work.BeginGet(TestFunction)
               .Execute();

            Assert.That.Value(test_function_executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<int>>()
               .Where(result => result.Error).Check(exception => exception.IsEqual(expected_exception))
               .Where(result => result.Failure).Check(status => status.IsTrue())
               .Where(result => result.Success).Check(status => status.IsFalse());
        }

        [TestMethod]
        public void FunctionWork_ReturnFailResult_AfterFailWork_WithAggregateException()
        {
            var expected_base_work_exception = new ApplicationException("Expected base work exception");
            var expected_function_exception = new ApplicationException("Expected function exception");
            var start_action_executed = false;
            var test_function_executed = false;
            void BaseWorkAction()
            {
                start_action_executed = true;
                throw expected_base_work_exception;
            }
            int TestFunction()
            {
                test_function_executed = true;
                throw expected_function_exception;
            }

            var work_result = Work.BeginInvoke(BaseWorkAction)
               .Get(TestFunction)
               .Execute();

            Assert.That.Value(start_action_executed).IsTrue();
            Assert.That.Value(test_function_executed).IsTrue();
            Assert.That.Value(work_result)
               .As<WorkResult<int>>()
               .Where(result => result.Error).Check(exception => exception.As<AggregateException>()
                   .Where(ex => ex.InnerExceptions[0]).Check(error0 => error0.IsEqual(expected_base_work_exception))
                   .Where(ex => ex.InnerExceptions[1]).Check(error1 => error1.IsEqual(expected_function_exception)))
               .Where(result => result.Failure).Check(status => status.IsTrue())
               .Where(result => result.Success).Check(status => status.IsFalse());
        }

        [TestMethod]
        public void ExceptionHandler_Action_ExecutedCorrect()
        {
            const string data_value = "1234!56789";

            Exception handled_exception = null;
            var exception_handler_executed = false;
            void ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .InvokeIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(handled_exception).Is<FormatException>();
            Assert.That.Value(exception_handler_executed).IsTrue();
            Assert.That.Value(work_result)
               .Where(result => result.Error).Check(exception => exception.Is<FormatException>())
               .Where(result => result.Success).Check(status => status.IsFalse())
               .Where(result => result.Failure).Check(status => status.IsTrue());
        }

        [TestMethod]
        public void ExceptionHandler_NoExecuted_AfterSuccessAction()
        {
            const string data_value = "123456789";

            Exception handled_exception = null;
            var exception_handler_executed = false;
            void ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .InvokeIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(handled_exception).IsNull();
            Assert.That.Value(exception_handler_executed).IsFalse();
            Assert.That.Value(work_result)
               .Where(result => result.Error).Check(exception => exception.IsNull())
               .Where(result => result.Success).Check(status => status.IsTrue())
               .Where(result => result.Failure).Check(status => status.IsFalse());
        }

        [TestMethod]
        public void ExceptionHandler_Action_ExecutedWithException()
        {
            const string data_value = "1234!56789";
            var expected_exception = new ApplicationException("Expected exception");

            Exception handled_exception = null;
            var exception_handler_executed = false;
            void ExceptionHandler(Exception error)
            {
                exception_handler_executed = true;
                handled_exception = error;
                throw expected_exception;
            }

            var work_result = Work.With(data_value)
               .Get(int.Parse)
               .InvokeIfFailure(ExceptionHandler)
               .Execute();

            Assert.That.Value(handled_exception).Is<FormatException>();
            Assert.That.Value(exception_handler_executed).IsTrue();
            Assert.That.Value(work_result)
               .Where(result => result.Error).Check(Exception => Exception.As<AggregateException>()
                   .Where(ex => ex.InnerExceptions[0]).Check(inner_ex0 => inner_ex0.Is<FormatException>())
                   .Where(ex => ex.InnerExceptions[1]).Check(inner_ex1 => inner_ex1.IsEqual(expected_exception)))
               .Where(result => result.Success).Check(status => status.IsFalse())
               .Where(result => result.Failure).Check(status => status.IsTrue());
        }
    }
}
