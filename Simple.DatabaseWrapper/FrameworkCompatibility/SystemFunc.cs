#if NET20
namespace System
{
    //public delegate void Action<T>(T arg);
    public delegate TResult Func<in T, out TResult>(T arg);
}
#endif
