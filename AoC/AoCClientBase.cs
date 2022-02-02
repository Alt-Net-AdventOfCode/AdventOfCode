using System;
using System.Threading.Tasks;

namespace AoC
{
    public abstract class AoCClientBase : IDisposable
    {
        public int Day { get; private set; }
        public int Year { get; }
        public void SetCurrentDay(int day)
        {
            Day = day;
        }
        
        public abstract Task<string> RequestPersonalInput();
        public abstract Task<string> PostAnswer(int question, string value);

        protected AoCClientBase(int year)
        {
            Year = year;
        }

        public abstract void Dispose();
    }
}