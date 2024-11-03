namespace OrleansR.Core.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using Orleans;


[GenerateSerializer]
public class EmptyList<T> : IReadOnlyList<T>
{
    private EmptyList()
    {
    }


    public static readonly EmptyList<T> Instance = new();
    public int Count => 0;

    public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index));

    public IEnumerator<T> GetEnumerator() => EmptyEnumerator<T>.Instance;


    IEnumerator IEnumerable.GetEnumerator() => EmptyEnumerator<T>.Instance;
}
