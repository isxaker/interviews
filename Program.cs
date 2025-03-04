#nullable disable

public interface ILightSet<T>
{
    public bool Add(T value);
    public bool Remove(T value);
    public void Clear();
    public bool Contains(T value);
    public bool TryGetValue(T equalValue, out T actualValue);
    public int Count { get; }
}

public sealed class LightSet<T> : ILightSet<T>
{
    private const int DefaultInitialCapacity = 8;
    private const double DefaultMaxOccupancyRatio = 2;
    private const double DefaultMinOccupancyRatio = 0.125;

    private int _occupiedItemsCounter;

    private WrappedValue<T>[] _values;

    public LightSet()
    {
        _values = new WrappedValue<T>[DefaultInitialCapacity];
        _occupiedItemsCounter = 0;
    }


    public int Count { get; private set; }
    private int Capacity => _values.Length;
    private bool NeedMoreSlots => Capacity < ((_occupiedItemsCounter + 1) * DefaultMaxOccupancyRatio);
    private bool NeedLessSlots => Capacity > (Count / DefaultMinOccupancyRatio);

    public bool Add(T value)
    {
        if (Contains(value))
        {
            return false;
        }

        if (NeedMoreSlots)
        {
            Resize(DefaultMaxOccupancyRatio);
        }

        var hashCode = GetHashCode(value);
        int index = hashCode;
        while (_values[index] != null && _values[index].State != ValueState.Deleted)
        {
            index = (index + 1) % Capacity;
        }
        if (_values[index] == null)
        {
            _occupiedItemsCounter++;
        }
        _values[index] = new WrappedValue<T> { Value = value, State = ValueState.Stetted };
        Count++;

        return true;
    }

    public void Clear()
    {
        _values = new WrappedValue<T>[DefaultInitialCapacity];
        Count = 0;
        _occupiedItemsCounter = 0;
    }

    public bool Contains(T value)
    {
        return TryGetValue(value, out _);
    }

    public bool Remove(T value)
    {
        if (!Contains(value))
        {
            return false;
        }

        var hashCode = GetHashCode(value);
        var index = hashCode;
        var currentValue = _values[index];
        while (!currentValue.Value.Equals(value) && currentValue.State != ValueState.Deleted)
        {
            index = (index + 1) % Capacity;
            currentValue = _values[index];
        }
        currentValue.State = ValueState.Deleted;
        Count--;

        if (NeedLessSlots)
        {
            Resize(DefaultMinOccupancyRatio);
        }

        return true;
    }

    public bool TryGetValue(T equalValue, out T actualValue)
    {
        actualValue = default;

        var hashCode = GetHashCode(equalValue);
        var index = hashCode;
        var currentValue = _values[index];
        while (currentValue != null)
        {
            if (currentValue.Value.Equals(equalValue) && currentValue.State != ValueState.Deleted)
            {
                actualValue = currentValue.Value;
                return true;
            }
            index = (index + 1) % _values.Length;
            currentValue = _values[index];
        }

        return false;
    }

    private void Resize(double resizeRatio)
    {
        int newSize = Convert.ToInt32(Capacity * resizeRatio);
        newSize = Math.Max(newSize, DefaultInitialCapacity);
        if (Capacity == newSize)
        {
            return;
        }

        var oldValues = _values;
        _values = new WrappedValue<T>[newSize];
        for (int index = 0; index < oldValues.Length; index++)
        {
            var oldValue = oldValues[index];
            if (oldValue != null && oldValue.State != ValueState.Deleted)
            {
                var hashCode = GetHashCode(oldValue.Value);
                var newIndex = hashCode;
                var currentValue = _values[hashCode];
                while (currentValue != null)
                {
                    newIndex = (newIndex + 1) % Capacity;
                    currentValue = _values[index];
                }

                _values[newIndex] = oldValue;
            }
        }
        _occupiedItemsCounter = Count;
    }

    private int GetHashCode(T value)
    {
        return value.GetHashCode() % Capacity;
    }

    private sealed class WrappedValue<TValue>
    {
        public TValue Value { get; set; }
        public ValueState State { get; set; }
    }

    private enum ValueState
    {
        Stetted,
        Deleted
    }
}

class Program
{
    static void Main(string[] args)
    {
        var set = new LightSet<int>();

        bool result, success = false;
        int value = -1;
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine($"Value: {i}...");
            result = set.Add(i);
            Console.WriteLine($"Added: {result}");
            Console.WriteLine($"Count: {set.Count}");
            success = set.TryGetValue(i, out value);
            Console.WriteLine($"TryGet: Success: {success} + value:{value}");
            result = set.Add(i);
            Console.WriteLine($"Added: {result}");
            Console.WriteLine($"Count: {set.Count}");
            result = set.Remove(i);
            Console.WriteLine($"Removed: {result}");
            Console.WriteLine($"Count: {set.Count}");
            result = set.Remove(i);
            Console.WriteLine($"Removed: {result}");
            Console.WriteLine($"Count: {set.Count}");
            success = set.TryGetValue(i, out value);
            Console.WriteLine($"TryGet: Success: {success} + value:{value}");
            Console.WriteLine();
        }

        Console.WriteLine(set.Count);
        set.Clear();
        Console.WriteLine(set.Count);
    }
}
