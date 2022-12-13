namespace AoCAlgorithms;

/// <summary>
/// Represents a 2D map with arbitrary storage type
/// </summary>
/// <typeparam name="T"></typeparam>
public enum OutBoundHandling
{
    Throw,
    DefaultValue,
    WrapAround
}

public class Map2D<T>
{
    private readonly T[,] _map;
    private OutBoundHandling _mode;
    private T _default;

    public Map2D(IList<IEnumerable<T>> map, OutBoundHandling mode = OutBoundHandling.Throw)
    {
        _map = new T[map.Count,map[0].Count()];
        _mode = mode;
    }
    
    public T this[int x, int y] => GetLocation(x, y);

    public T this[(int x, int y) coordinates] => GetLocation(coordinates.y, coordinates.x);
    
    private T GetLocation(int x, int y)
    {
        var xOutOfBound = x < _map.GetLowerBound(1) || x > _map.GetUpperBound(1);
        var yOutOfBound = y < _map.GetLowerBound(0) || y > _map.GetUpperBound(0);
        if (xOutOfBound || yOutOfBound)
        {
            switch (_mode)
            {
                case OutBoundHandling.Throw:
                    throw new IndexOutOfRangeException((xOutOfBound ? $"x {x} is out of bound ": "") + (yOutOfBound ? $"y {y} is out of bound." : ""));
                case OutBoundHandling.DefaultValue:
                    return _default;
                case OutBoundHandling.WrapAround:
                {
                    while (x<_map.GetLowerBound(1))
                    {
                        x += _map.GetLowerBound(1);
                    }

                    break;
                }
            }
        }
        return _map[y, x];
    }

}
