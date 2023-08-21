using MathCore.Extensions;

namespace MathCore.Tests.Extensions;

[TestClass]
public class ObjectReflectionExtensionsTests
{
    private class TestPropertyObject
    {
        public string StringProperty { get; set; }

        private string _StringReadOnlyPropertyValue = "ReadOnlyPropertyValue";
        public string StringReadOnlyProperty => _StringReadOnlyPropertyValue;
        public void SetStringReadOnlyPropertyValue(string value) => _StringReadOnlyPropertyValue = value;

        private string _StringWriteOnlyPropertyValue;
        public string StringWriteOnlyProperty { set => _StringWriteOnlyPropertyValue = value; }
        public string GetStringWriteOnlyPropertyValue() => _StringWriteOnlyPropertyValue;

        private string PrivateStringProperty { get; set; }
        private string PrivateStringReadOnlyProperty { get; } = "Value";

        private string _PrivateStringWriteOnlyPropertyValue;
        private string PrivateStringWriteOnlyProperty { set => _PrivateStringWriteOnlyPropertyValue = value; }

        private string GetPrivateStringWriteOnlyPropertyValue() => _PrivateStringWriteOnlyPropertyValue;


        public int IntProperty { get; set; }
        public object ObjectProperty { get; set; }
    }

    [TestMethod]
    public void GetExistPublicPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject
        {
            StringProperty = expected_value,
        };

        var actual_value = obj.GetPropertyValue(nameof(TestPropertyObject.StringProperty));

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryGetExistPublicPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject
        {
            StringProperty = expected_value,
        };

        var get_result = obj.TryGetPropertyValue(nameof(TestPropertyObject.StringProperty), out var actual_value);

        get_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetExistStringPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject
        {
            StringProperty = expected_value,
        };

        var actual_value = obj.GetPropertyValue<TestPropertyObject, string>(
            nameof(TestPropertyObject.StringProperty));

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryGetExistStringPublicPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject
        {
            StringProperty = expected_value,
        };

        var get_result = obj.TryGetPropertyValue<TestPropertyObject, string>(
            nameof(TestPropertyObject.StringProperty),
            out var actual_value);

        get_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetExistIntStringPropertyValue()
    {
        const int expected_value = 123;

        var obj = new TestPropertyObject
        {
            IntProperty = expected_value,
        };

        var actual_value = obj.GetPropertyValue<TestPropertyObject, int>(nameof(TestPropertyObject.IntProperty));

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryGetExistIntPublicPropertyValue()
    {
        const int expected_value = 123;

        var obj = new TestPropertyObject
        {
            IntProperty = expected_value,
        };

        var get_result = obj.TryGetPropertyValue(nameof(TestPropertyObject.IntProperty), out int actual_value);

        get_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetExistReadonlyPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject();
        obj.SetStringReadOnlyPropertyValue(expected_value);

        var actual_value = obj.GetPropertyValue(
            nameof(TestPropertyObject.StringReadOnlyProperty));

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryGetExistReadonlyPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject();
        obj.SetStringReadOnlyPropertyValue(expected_value);

        var get_value_result = obj.TryGetPropertyValue(
            nameof(TestPropertyObject.StringReadOnlyProperty), out var actual_value);

        get_value_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetExistStringReadonlyPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject();
        obj.SetStringReadOnlyPropertyValue(expected_value);

        var actual_value = obj.GetPropertyValue<TestPropertyObject, string>(
            nameof(TestPropertyObject.StringReadOnlyProperty));

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TryGetExistStringReadonlyPropertyValue()
    {
        const string expected_value = "123";

        var obj = new TestPropertyObject();
        obj.SetStringReadOnlyPropertyValue(expected_value);

        var get_value_result = obj.TryGetPropertyValue<TestPropertyObject, string>(
            nameof(TestPropertyObject.StringReadOnlyProperty),
            out var actual_value);

        get_value_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetValueFromWriteOnlyPropertyThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => obj.GetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty))
        );

        exception.Data["obj"].AssertEquals(typeof(TestPropertyObject));
        exception.Data["PropertyName"].AssertEquals(nameof(TestPropertyObject.StringWriteOnlyProperty));
    }

    [TestMethod]
    public void TryGetValueFromWriteOnlyPropertyReturnsFalse()
    {
        var obj = new TestPropertyObject();

        var get_result = obj.TryGetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty), out var actual_value);

        get_result.AssertFalse();
        actual_value.AssertEquals(default);
    }

    [TestMethod]
    public void GetValueFromWriteOnlyStringPropertyThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => obj.GetPropertyValue<TestPropertyObject, string>(
                nameof(TestPropertyObject.StringWriteOnlyProperty))
        );

        exception.Data["obj"].AssertEquals(typeof(TestPropertyObject));
        exception.Data["T"].AssertEquals(typeof(TestPropertyObject));
        exception.Data["TValue"].AssertEquals(typeof(string));
        exception.Data["PropertyName"].AssertEquals(nameof(TestPropertyObject.StringWriteOnlyProperty));
    }

    [TestMethod]
    public void TryGetValueFromWriteOnlyStringPropertyReturnsFalse()
    {
        var obj = new TestPropertyObject();

        var get_result = obj.TryGetPropertyValue<TestPropertyObject, string>(
            nameof(TestPropertyObject.StringWriteOnlyProperty),
            out var actual_value);

        get_result.AssertFalse();
        actual_value.AssertEquals(default);
    }

    [TestMethod]
    public void SetExistPublicPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "000";

        var obj = new TestPropertyObject
        {
            StringProperty = initial_value,
        };

        object o = obj;

        o.SetPropertyValue(nameof(TestPropertyObject.StringProperty), (object)expected_value);

        obj.StringProperty.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TrySetExistPublicPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "000";

        var obj = new TestPropertyObject
        {
            StringProperty = initial_value,
        };

        object o = obj;
        var set_value_result = o.TrySetPropertyValue(nameof(TestPropertyObject.StringProperty), (object)expected_value);

        set_value_result.AssertTrue();
        obj.StringProperty.AssertEquals(expected_value);
    }

    [TestMethod]
    public void SetExistPublicStringPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "000";

        var obj = new TestPropertyObject
        {
            StringProperty = initial_value,
        };

        obj.SetPropertyValue(nameof(TestPropertyObject.StringProperty), expected_value);

        obj.StringProperty.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TrySetExistPublicStringPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "000";

        var obj = new TestPropertyObject
        {
            StringProperty = initial_value,
        };

        var set_value_result = obj.TrySetPropertyValue(nameof(TestPropertyObject.StringProperty), expected_value);

        set_value_result.AssertTrue();
        obj.StringProperty.AssertEquals(expected_value);
    }

    [TestMethod]
    public void SetReadonlyPublicPropertyValueThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        object o = obj;

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => o.SetPropertyValue(nameof(TestPropertyObject.StringReadOnlyProperty), (object)"123")
        );

        exception.AssertThatValue()
            .Where(e => e.Data["obj"], v => v.IsEqual(typeof(TestPropertyObject)))
            .Where(e => e.Data["PropertyName"], v => v.IsEqual(nameof(TestPropertyObject.StringReadOnlyProperty)))
        ;
    }

    [TestMethod]
    public void TrySetReadonlyPublicPropertyValueThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        object o = obj;

        var set_value_result = o.TrySetPropertyValue(nameof(TestPropertyObject.StringReadOnlyProperty), (object)"123");

        set_value_result.AssertFalse();
    }


    [TestMethod]
    public void SetReadonlyPublicStringPropertyValueThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => obj.SetPropertyValue(nameof(TestPropertyObject.StringReadOnlyProperty), "123")
        );

        exception.AssertThatValue()
            .Where(e => e.Data["obj"], v => v.IsEqual(typeof(TestPropertyObject)))
            .Where(e => e.Data["T"], v => v.IsEqual(typeof(TestPropertyObject)))
            .Where(e => e.Data["TValue"], v => v.IsEqual(typeof(string)))
            .Where(e => e.Data["PropertyName"], v => v.IsEqual(nameof(TestPropertyObject.StringReadOnlyProperty)))
        ;
    }

    [TestMethod]
    public void TrySetReadonlyPublicStringPropertyValueThrownInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var set_value_result = obj.TrySetPropertyValue(nameof(TestPropertyObject.StringReadOnlyProperty), "123");

        set_value_result.AssertFalse();
    }

    [TestMethod]
    public void SetWriteOnlyPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "---";

        var obj = new TestPropertyObject
        {
            StringWriteOnlyProperty = initial_value,
        };

        object o = obj;

        o.SetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty), (object)expected_value);

        var actual_value = obj.GetStringWriteOnlyPropertyValue();

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TrySetWriteOnlyPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "---";

        var obj = new TestPropertyObject
        {
            StringWriteOnlyProperty = initial_value,
        };

        object o = obj;

        var set_value_result = o.TrySetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty), (object)expected_value);

        var actual_value = obj.GetStringWriteOnlyPropertyValue();

        set_value_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void SetWriteOnlyStringPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "---";

        var obj = new TestPropertyObject
        {
            StringWriteOnlyProperty = initial_value,
        };

        obj.SetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty), expected_value);

        var actual_value = obj.GetStringWriteOnlyPropertyValue();

        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void TrySetWriteOnlyStringPropertyValue()
    {
        const string expected_value = "123";
        const string initial_value = "---";

        var obj = new TestPropertyObject
        {
            StringWriteOnlyProperty = initial_value,
        };

        var set_value_result = obj.TrySetPropertyValue(nameof(TestPropertyObject.StringWriteOnlyProperty), expected_value);

        var actual_value = obj.GetStringWriteOnlyPropertyValue();

        set_value_result.AssertTrue();
        actual_value.AssertEquals(expected_value);
    }

    [TestMethod]
    public void GetPrivatePropertyThrowInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => obj.GetPropertyValue("PrivateStringProperty")
        );

        exception.Data["PropertyName"].AssertEquals("PrivateStringProperty");
    }

    [TestMethod]
    public void TryGetPrivatePropertyReturnFalse()
    {
        var obj = new TestPropertyObject();

        var get_value_result = obj.TryGetPropertyValue("PrivateStringProperty", out _);

        get_value_result.AssertFalse();
    }

    [TestMethod]
    public void GetPrivateStringPropertyThrowInvalidOperationException()
    {
        var obj = new TestPropertyObject();

        var exception = Assert.ThrowsException<InvalidOperationException>(
            () => obj.GetPropertyValue<TestPropertyObject, string>("PrivateStringProperty")
        );

        exception.Data["PropertyName"].AssertEquals("PrivateStringProperty");
    }

    [TestMethod]
    public void TryGetPrivateStringPropertyReturnFalse()
    {
        var obj = new TestPropertyObject();

        var get_value_result = obj.TryGetPropertyValue<TestPropertyObject, string>("PrivateStringProperty", out _);

        get_value_result.AssertFalse();
    }
}
