using System;

namespace MathCore.Reflection
{
    public class AnimatedIntProperty<TObject> : AnimatedProperty<TObject, int>
    {
        public AnimatedIntProperty(TObject o, string Name,
                                   int Samples, int Timeout, Func<int, int, int> Translator,
                                   bool Private = false)
            : base(o, Name, Samples, Timeout, Translator, Private)
        {

        }
    }
}