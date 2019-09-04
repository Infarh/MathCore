using System;

namespace MathCore
{
    [NotImplemented]
    public class MultyDemensionsArray
    {
        public ArraySegment<double> _Values;

        private readonly int _Dimensions;

        public int Demensions => _Dimensions;

        //public double this[int[] index]
        //{
        //    get
        //    {

        //    }
        //    set
        //    {

        //    }
        //}

        public MultyDemensionsArray(int Dimensions) => _Dimensions = Dimensions;


    }
}