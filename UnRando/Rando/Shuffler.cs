using System.Collections.Generic;

namespace UnRando.Rando;

internal class Shuffler<T>
{
    private List<T> values = [];
    private int index = -1;

    public void Add(T item) => values.Add(item);

    public T Take(System.Random r)
    {
        if (index == -1)
        {
            for (int i = 0; i < values.Count - 1; i++)
            {
                var j = r.Next(i, values.Count);
                (values[i], values[j]) = (values[j], values[i]);
            }
            index = 0;
        }

        T item = values[index];
        if (++index == values.Count) index = -1;
        return item;
    }
}
