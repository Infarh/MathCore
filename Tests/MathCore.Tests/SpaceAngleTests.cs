using MathCore.Vectors;

// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace MathCore.Tests;

[TestClass]
public class SpaceAngleTests : UnitTest
{
    public TestContext TestContext { get; set; }

    #region Additional test attributes

    //[ClassInitialize] public static void MyClassInitialize(TestContext testContext) { }
    //[ClassCleanup] public static void MyClassCleanup() { }
    //[TestInitialize] public void MyTestInitialize() { }
    //[TestCleanup] public void MyTestCleanup() { }

    #endregion

    //private SpaceAngle RandomAngle =>
    //    new SpaceAngle
    //    (
    //        Theta: GetRNDDouble(Min: -Consts.Geometry.Pi / 2, Max: Consts.Geometry.Pi / 2),
    //        Phi: GetRNDDouble(Min: 0, Max: 2 * Consts.Geometry.Pi)
    //    );

    /// <summary></summary>
    [TestMethod, Priority(0), Description("")]
    public void Create_Test()
    {
        var angle = new SpaceAngle();
        Assert.AreEqual(0d, angle.Theta);
        Assert.AreEqual(0d, angle.Phi);
    }

    /// <summary></summary>
    [TestMethod, Priority(1), Description("")]
    public void ToVector_Test()
    {
        var a = new SpaceAngle(0, 0, AngleType.Deg);
        var v = a.DirectionalVector;
        Assert.AreEqual(new(0, 0, 1), v);
    }

    /// <summary></summary>
    [TestMethod, Priority(1), Description("")]
    public void Ort_Test()
    {
        static void AreEqual(Vector3D v, SpaceAngle a)
        {
            Assert.AreEqual(v, a.DirectionalVector);
            Assert.AreEqual(v.Angle, a);
        }

        AreEqual(Vector3D.i, SpaceAngle.i);
        AreEqual(Vector3D.j, SpaceAngle.j);
        AreEqual(Vector3D.k, SpaceAngle.k);
    }

    /// <summary></summary>
    [TestMethod, Priority(1), Description("")]
    public void SimpleRotate_Test()
    {
        var a = new SpaceAngle(90, 0, AngleType.Deg);

        Assert.AreEqual(SpaceAngle.i, a);
        Assert.AreEqual(SpaceAngle.i, a.RotatePhiTheta(0, 0));
        Assert.AreEqual(SpaceAngle.i, a.RotatePhiTheta(0, Consts.pi2));
        Assert.AreEqual(SpaceAngle.i, a.RotatePhiTheta(0, -Consts.pi2));

        Assert.AreEqual(SpaceAngle.i_negative, a.RotatePhiTheta(0, Consts.pi));
        Assert.AreEqual(SpaceAngle.i_negative, a.RotatePhiTheta(0, -Consts.pi));


        Assert.AreEqual(SpaceAngle.j, a.RotatePhiTheta(0, Consts.pi05));
        Assert.AreEqual(SpaceAngle.j, a.RotatePhiTheta(0, -Consts.pi3_2));


        Assert.AreEqual(SpaceAngle.j_negative, a.RotatePhiTheta(0, Consts.pi3_2));
        Assert.AreEqual(SpaceAngle.j_negative, a.RotatePhiTheta(0, -Consts.pi05));

        Assert.AreEqual(SpaceAngle.k, a.RotatePhiTheta(-Consts.pi05, 0));
        Assert.AreEqual(SpaceAngle.k_negative, a.RotatePhiTheta(Consts.pi05, 0));
        Assert.AreEqual(SpaceAngle.i_negative, a.RotatePhiTheta(Consts.pi, 0));
        Assert.AreEqual(SpaceAngle.i_negative, a.RotatePhiTheta(-Consts.pi, 0));
    }

    /// <summary></summary>
    [TestMethod, Priority(2), Description("")]
    public void SimpleRotate_Theta_Test()
    {
        static double Func(double a) => Math.Cos(a);
        static double Function(SpaceAngle a) => Func(a.ThetaRad);
        const double delta = 3d;

        static double F1(SpaceAngle a) => Function(a.RotatePhiTheta(delta, 0, AngleType.Deg));
        static double F2(double a) => Func(a + delta * Consts.Geometry.ToRad);

        for (var a = new SpaceAngle(0, AngleType.Deg); a.ThetaDeg <= 360; a += new SpaceAngle(1, 0, AngleType.Deg))
        {
            var fv = F2(a.ThetaRad);
            var Fv = F1(a);
            Assert.AreEqual(fv, Fv, 1e-13);
        }
    }

    /// <summary></summary>
    [TestMethod, Priority(2), Description("")]
    public void SimpleRotate_ThetaToPhi_Test()
    {
        static double Func(double a) => Math.Cos(a);
        static double Func1(SpaceAngle a) => Func(a.ThetaRad);
        static double Func2(SpaceAngle a) => Func1(a.RotatePhiTheta(90, 0, AngleType.Deg));

        for (var a = new SpaceAngle(0, AngleType.Deg); a.PhiDeg <= 360; a += new SpaceAngle(0, 15, AngleType.Deg))
        {
            var fv = Func(a.PhiRad);
            var Fv = Func2(a);
            //Assert.AreEqual(fv, Fv, 1e-13);
        }
    }
}