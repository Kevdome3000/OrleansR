namespace OrleansR.Core.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using Orleans;


[GenerateSerializer]
public class SingletonList<T> : IReadOnlyList<T>
{
    [Id(0)]
    private readonly T value;

    public SingletonList(T value) => this.value = value;

    public T this[int index] =>
        index == 0
            ? value
            : throw new ArgumentOutOfRangeException(
                "List contains 1 element, provided " + index
            );

    public int Count => 1;

    public IEnumerator<T> GetEnumerator() => new SingletonEnumerator<T>(value);


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
