public interface ILightSet<T>
{
    public bool Add(T value);
    public bool Remove(T item);
    public void Clear();
    public bool Contains(T val);
    public bool TryGetValue(T equalValue, out T actualValue);
    public int Capacity { get; }
    public int Count { get; }
}

public sealed class LightSet<T> : ILightSet<T>
{
    private const int DefaultInitialCapacity = 10;
    private const double DefaultRecizeRation = 1.2;
    private T[] _values;
    private int _count;
    //private int _capacity;

    public LightSet()
    {
        _values = new T[0];
        //_capacity = _values.Length;
        _count = 0;
    }


    public int Capacity => _values.Length;

    public int Count => _count;

    public bool Add(T value)
    {
        bool result = false;
        if (!HasSlots)
        {
            Resize();
        }

        int index = value.GetHashCode() % Capacity;
        if (_values[index] == default)
        {

        }

        return result;
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T val)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(T equalValue, out T actualValue)
    {
        throw new NotImplementedException();
    }

    private bool HasSlots => Capacity > _count + 1;
    private void Resize()
    {

    }

    private int GetHashCode(T value)
    {
        return value.GetHashCode();
    }
}

class Program
{
    static void Main(string[] args)
    {
        var set = new HashSet<int>();

    }
}