﻿namespace MathCore.Tests.Extensions.Numerics
{
    [TestClass]
    public class DoubleArrayExtensionsTests
    {
        [TestMethod]
        public void Dispersion_Array_1D()
        {
            //const double expected_average = 5;
            //const double expected_sko = 8;
            //const double dispersion = expected_sko * expected_sko;
            double[] values =
            {
                9.30648634667564, 3.9763292571622, -0.768328748147881, 12.8627951749408, 0.808145593269836,
                18.9954781965425, -8.56051435529085, 17.2188003131097, 24.1294621021259, 3.12024377092156,
                -0.550935417042393, 9.67305044980154, 7.3219192847624, 5.10607945331926, -17.7482514966561,
                8.80047431546089, 3.48563285031988, 10.9138358680488, -10.7952855323334, -0.966363703978207,
                4.12738287813865, 2.57157639304968, 13.8619506038489, -5.57263692973185, 5.43691333007704,
                -8.61451952545933, 12.7738350749563, 9.82642577413115, 17.1646844559898, 14.4397535834104,
                11.5138227617489, -0.58925484539046, 6.37484076925832, 9.78679706895639, 6.02234787315868,
                14.502453282283, 14.0323451234436, -2.63364048998616, 4.99177490492165, 9.60095036014719,
                0.556556828476737, 1.66941447202605, -2.1336767993142, -9.28950910991139, 3.23342348872843,
                5.06360795131452, 2.39226329237963, 8.08771804454255, 9.4431266823574, 15.0639683564602,
                -9.77571264754152, 25.1341997420107, 13.16147840218, 4.38952682402793, 1.40340876059917,
                7.46072421655441, -8.24336142575862, 10.9672701568156, 4.91547006415785, -1.68349565629852,
                1.07796752672171, -4.24907927014106, 0.983602085819325, 14.4717419614239, 3.80226579584103,
                12.3473468883621, 12.1735948236911, 0.757179447294911, -6.1501374821334, -3.867124319719,
                8.48137124284827, -3.70445299326087, 2.56239911381583, -1.68063663543964, -1.8719396806249,
                5.81221716604836, 7.54010493166522, 1.146374162401, 15.2946581262256, -1.23005783795269,
                15.0991284119681, 1.74091166772127, -2.00574235041221, 1.32800728535231, 9.32065592297133,
                4.71964421193056, 8.00713060694699, 10.7103412696686, 9.03276700074391, 8.50858021522337,
                13.4085145834299, -1.59131730298977, 5.25700087897026, -0.981232593057717, -1.18262841845954,
                -0.261757729092094, 0.297570326089552, -14.9905122465409, 6.83205246518404, 2.47458712927715
            };

            var variance = values.Dispersion();

            const double eps = 3.56e-014;
            const double expected_dispersion = 61.692836579796810;
            Assert.That.Value(variance).IsEqual(expected_dispersion, eps);
        }

        [TestMethod]
        public void Dispersion_Array_1D_Simple()
        {
            double[] values = { 3, 7, 10, 5, 4, 8, 2 };

            var variance = values.Dispersion();

            const double eps = 4.45e-015;
            const double expected_dispersion = 7.102040816326531;
            Assert.That.Value(variance).IsEqual(expected_dispersion, eps);
        }
    }
}
