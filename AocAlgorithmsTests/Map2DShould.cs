using AoCAlgorithms;
using NFluent;
namespace AocAlgorithmsTests;

public class Map2DShould    
{
    [TestCase(0, 0, 0)]
    [TestCase(5, 0, 5)]
    [TestCase(0, 1, 10)]
    [TestCase(9, 1, 19)]
    public void SupportIndexedAccess(int x, int y, int expected)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap);
        Check.That(map[x, y]).IsEqualTo(expected);
    }

    [TestCase(0, 0, 0)]
    [TestCase(5, 0, 5)]
    [TestCase(0, 1, 10)]
    [TestCase(9, 1, 19)]
    public void SupportTupleIndexedAccess(int x, int y, int expected)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap);
        Check.That(map[(x, y)]).IsEqualTo(expected);
    }

    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(100, 0)]
    [TestCase(0, 100)]
    [TestCase(10, 10)]
    public void ThrowWhenOutOfBounds(int x, int y)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap);
        Check.ThatCode( ()=>
        Check.That(map[x, y]).IsEqualTo(0)).Throws<IndexOutOfRangeException>();
    }
    
    [TestCase(-1, 0)]
    [TestCase(0, -1)]
    [TestCase(100, 0)]
    [TestCase(0, 100)]
    [TestCase(10, 10)]
    public void ReturnsDefaultWhenOutOfBand(int x, int y)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap, OutBoundHandling.DefaultValue, 42);
        Check.That(map[x, y]).IsEqualTo(42);
    }

    [TestCase(-1, 0, 9)]
    [TestCase(0, -1, 10)]
    [TestCase(100, 0, 0)]
    [TestCase(0, 100, 0)]
    [TestCase(11, 11, 11)]
    public void WrapAroundWhenOutOfBonds(int x, int y, int expected)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap, OutBoundHandling.WrapAround);
        Check.That(map[x, y]).IsEqualTo(expected);
    }

    [TestCase(-1, 0, 9)]
    [TestCase(0, -1, 10)]
    [TestCase(100, 0, 0)]
    [TestCase(0, 100, 0)]
    [TestCase(11, 11, 11)]
    public void RepeatWhenOutOfBonds(int x, int y, int expected)
    {
        var buildMap = new List<IEnumerable<int>>
        {
            Enumerable.Range(0, 10),
            Enumerable.Range(10, 10)
        };
        var map = new Map2D<int>(buildMap, OutBoundHandling.Repeat);
        Check.That(map[x, y]).IsEqualTo(expected);
    }
    [Test]
    public void ProvideNearestNeighboursInNormal()
    {
        var dummyList = Enumerable.Range(0, 10);
        var list = new List<IEnumerable<int>>();
        for (var i = 0; i < 10; i++)
        {
            list.Add(dummyList);
        }

        var map = new Map2D<int>(list);
        
        var neighbors = map.NeighborsOf((5, 5)).ToList();
        Check.That(neighbors).ContainsExactly((4, 5), (5, 4), (6, 5), (5, 6));
        
        neighbors = map.NeighborsOf((0, 0)).ToList();
        Check.That(neighbors).ContainsExactly((1, 0), (0, 1));

        neighbors = map.NeighborsOf((1, 9)).ToList();
        Check.That(neighbors).ContainsExactly((0, 9), (1, 8), (2, 9));
    }
    
    [Test]
    public void ProvideNearestNeighboursInWrapAround()
    {
        var dummyList = Enumerable.Range(0, 10);
        var list = new List<IEnumerable<int>>();
        for (var i = 0; i < 10; i++)
        {
            list.Add(dummyList);
        }

        var map = new Map2D<int>(list, OutBoundHandling.WrapAround);
        
        var neighbors = map.NeighborsOf((5, 5)).ToList();
        Check.That(neighbors).ContainsExactly((4, 5), (5, 4), (6, 5), (5, 6));
        
        neighbors = map.NeighborsOf((0, 0)).ToList();
        Check.That(neighbors).ContainsExactly((9, 0), (0, 9), (1, 0), (0, 1));

        neighbors = map.NeighborsOf((1, 9)).ToList();
        Check.That(neighbors).ContainsExactly((0, 9), (1, 8), (2, 9), (1, 0));
    }

    [TestCase(OutBoundHandling.DefaultValue)]
    [TestCase(OutBoundHandling.Repeat)]
    public void ProvideNearestNeighbors(OutBoundHandling mode)
    {
        var dummyList = Enumerable.Range(0, 10);
        var list = new List<IEnumerable<int>>();
        for (var i = 0; i < 10; i++)
        {
            list.Add(dummyList);
        }

        var map = new Map2D<int>(list, mode);
        
        var neighbors = map.NeighborsOf((5, 5)).ToList();
        Check.That(neighbors).ContainsExactly((4, 5), (5, 4), (6, 5), (5, 6));
        
        neighbors = map.NeighborsOf((0, 0)).ToList();
        Check.That(neighbors).ContainsExactly((-1, 0), (0, -1), (1, 0), (0, 1));

        neighbors = map.NeighborsOf((1, 9)).ToList();
        Check.That(neighbors).ContainsExactly((0, 9), (1, 8), (2, 9), (1, 10));
    }
    
}