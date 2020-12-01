using System;
using System.Collections.Generic;
using System.Reflection;

class Test
{
    private static readonly List<string> myStrings = new List<string> { "Foo" };
    public static string Hello1 => "hello";
    public static string Hello2 => myStrings[0];
 
    public static void Boot()
    {
        var fi = typeof(Test).GetField("myStrings", BindingFlags.NonPublic | BindingFlags.Static);
        fi.SetValue("not used object", new List<string> { "Bar" });
    }
}
 
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(Test.Hello1 /* Test.Hello2 */);
        Test.Boot();
        Test.Boot();
        Console.WriteLine(Test.Hello2 /* Test.Hello2 */);
    }
}