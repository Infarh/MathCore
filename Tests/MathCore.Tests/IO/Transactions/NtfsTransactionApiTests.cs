using System.Runtime.Versioning;

using MathCore.IO.Transactions;

namespace MathCore.Tests.IO.Transactions;

[TestClass]
public class NtfsTransactionApiTests
{
    [TestMethod]
    public void NtfsTransaction_HasWindowsSupportedOSPlatformAttribute()
    {
        var attributes = typeof(NtfsTransaction).GetCustomAttributes(typeof(SupportedOSPlatformAttribute), false);

        Assert.IsNotEmpty(attributes);
        Assert.AreEqual("windows", ((SupportedOSPlatformAttribute)attributes[0]).PlatformName);
    }

    [TestMethod]
    public void NtfsTransactions_HasWindowsSupportedOSPlatformAttribute()
    {
        var attributes = typeof(NtfsTransactions).GetCustomAttributes(typeof(SupportedOSPlatformAttribute), false);

        Assert.IsNotEmpty(attributes);
        Assert.AreEqual("windows", ((SupportedOSPlatformAttribute)attributes[0]).PlatformName);
    }

    [TestMethod]
    public void NtfsTransactionOptions_StoresConfiguredValues()
    {
        var options = new NtfsTransactionOptions
        {
            Description = "test",
            TimeoutMilliseconds = 1500,
        };

        Assert.AreEqual("test", options.Description);
        Assert.AreEqual(1500, options.TimeoutMilliseconds);
    }
}
