namespace MathCore.Monades.WorkFlow
{
    public class ConstValueWork<T> : Work<T>
    {
        private readonly T _Value;

        internal ConstValueWork(T Value, Work BaseWork = null) : base(BaseWork) => _Value = Value;

        protected override IWorkResult Execute(IWorkResult BaseResult) => new WorkResult<T>(_Value);
    }
}