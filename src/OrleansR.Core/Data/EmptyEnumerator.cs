namespace OrleansR.Core.Data;

using System.Collections;
using System.Collections.Generic;


public class EmptyEnumerator<T> : IEnumerator<T>
{
    private EmptyEnumerator()
    {
    }


    public static readonly EmptyEnumerator<T> Instance = new();
    public T Current => default!;

    object IEnumerator.Current => default!;


    public void Dispose()
    {
    }


    public bool MoveNext() => false;


    public void Reset()
    {
    }
}
