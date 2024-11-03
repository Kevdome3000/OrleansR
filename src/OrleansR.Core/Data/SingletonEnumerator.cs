namespace OrleansR.Core.Data;

using System.Collections;
using System.Collections.Generic;


public class SingletonEnumerator<T> : IEnumerator<T>
{
    private bool done;
    public T Current { get; }

    public SingletonEnumerator(T value) => Current = value;

    object IEnumerator.Current => Current!;


    public void Dispose()
    {
    }


    public bool MoveNext() => !done && (done = true);


    public void Reset()
    {
        done = false;
    }
}
