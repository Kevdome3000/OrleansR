namespace OrgnalR.Core.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orleans;


[GenerateSerializer]
public class EmptySet<T> : ISet<T>
{
    private EmptySet()
    {
    }


    public static readonly EmptySet<T> Instance = new();
    public int Count => 0;

    public bool IsReadOnly => true;

    public bool Add(T item) => throw new NotSupportedException();


    public void Clear()
    {
        throw new NotSupportedException();
    }


    public bool Contains(T item) => false;


    public void CopyTo(T[] array, int arrayIndex)
    {
    }


    public void ExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException();
    }


    public IEnumerator<T> GetEnumerator() => EmptyEnumerator<T>.Instance;


    public void IntersectWith(IEnumerable<T> other)
    {
        throw new NotSupportedException();
    }


    public bool IsProperSubsetOf(IEnumerable<T> other) => true;

    public bool IsProperSupersetOf(IEnumerable<T> other) => !other.Any();

    public bool IsSubsetOf(IEnumerable<T> other) => true;

    public bool IsSupersetOf(IEnumerable<T> other) => !other.Any();

    public bool Overlaps(IEnumerable<T> other) => false;

    public bool Remove(T item) => throw new NotSupportedException();

    public bool SetEquals(IEnumerable<T> other) => !other.Any();


    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new NotSupportedException();
    }


    public void UnionWith(IEnumerable<T> other)
    {
        throw new NotSupportedException();
    }


    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException();
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
