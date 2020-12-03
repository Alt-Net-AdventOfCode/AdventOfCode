using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2018
{
    public static class Day9
    {
        private static void MainDay9()
        {
            var nbPlayers = 411;
            var nbRounds = 7205900;


            var scores = new long[nbPlayers];
            var list = new List<int>();
            list.Capacity = nbRounds;
            list.Add(0);
            list.Add(1);
            var currentMarble = 1;
            for (var i = 2; i <= nbRounds; i++)
            {
                if (i % 23 == 0)
                {
                    var currentPlayer = (i-1) % nbPlayers;
                    currentMarble = (currentMarble - 7 + list.Count) % list.Count;
                    var score = list[currentMarble] + i;
                    scores[currentPlayer] += score;
                    list.RemoveAt(currentMarble);
                }
                else
                {
                    currentMarble = (currentMarble + 2) % list.Count;
                    list.Insert(currentMarble, i);
                }
            }

            Console.WriteLine($"Result : {scores.Max()}");
        }

        private const string Input = "411 players; last marble is worth 72059 points";
    }
}