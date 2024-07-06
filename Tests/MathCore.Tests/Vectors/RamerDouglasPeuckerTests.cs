using MathCore.Vectors;

namespace MathCore.Tests.Vectors;

[TestClass]
public class RamerDouglasPeuckerTests
{
    [TestMethod]
    public void SmoothTest() // https://habr.com/ru/articles/448618/
    {
        Vector2D[] points = [(1, 5), (2, 3), (5, 1), (6, 4), (9, 6), (11, 4), (13, 3), (14, 2), (18, 5)];

        //  ^
        // 7|                         (9,6)                           
        // 6| (1,5)                     *                       (18,5)
        // 5|   *            (6,4)                                 *  
        // 4|    (2,3)         +              +  (13,3)               
        // 3|      +                       (11,4)   +                 
        // 2|                                          *              
        // 1|               *                       (14,2)            
        // 0|             (5,1)                                       
        //  +---------------------------------------------------------->
        //  0   1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19

        Vector2D[] expected_0_5 = [(1,5),   (2,3),   (5,1),   (6,4),   (9,6), /*(11,4),*/ /*(13,3),*/ (14,2), (18,5)];
        Vector2D[] expected_1_0 = [(1,5), /*(2,3),*/ (5,1),   (6,4),   (9,6), /*(11,4),*/ /*(13,3),*/ (14,2), (18,5)];
        Vector2D[] expected_1_5 = [(1,5), /*(2,3),*/ (5,1), /*(6,4),*/ (9,6), /*(11,4),*/ /*(13,3),*/ (14,2), (18,5)];

        const double eps_0_5 = 0.5;
        const double eps_1_0 = 1.0;
        const double eps_1_5 = 1.5;
        var smooth_path_0_5 = RamerDouglasPeucker.Smooth(points, eps_0_5);
        var smooth_path_1_0 = RamerDouglasPeucker.Smooth(points, eps_1_0);
        var smooth_path_1_5 = RamerDouglasPeucker.Smooth(points, eps_1_5);

        //var smooth_path_0_5_str = smooth_path_0_5.ToArrayFormattedString();
        //var smooth_path_1_0_str = smooth_path_1_0.ToArrayFormattedString();
        //var smooth_path_1_5_str = smooth_path_1_5.ToArrayFormattedString();

        smooth_path_0_5.AssertEquals(expected_0_5);
        smooth_path_1_0.AssertEquals(expected_1_0);
        smooth_path_1_5.AssertEquals(expected_1_5);
    }
}
