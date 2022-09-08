#if NF20
namespace System
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public delegate TOut Func<TOut>();
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public delegate TOut Func<TInput, TOut>(TInput input);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public delegate TOut Func<T1, T2, TOut>(T1 t1, T2 t2);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <returns></returns>
    public delegate TOut Func<T1, T2, T3, TOut>(T1 t1, T2 t2,T3 t3);
}
#endif