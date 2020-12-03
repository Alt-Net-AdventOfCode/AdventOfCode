using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day4
    {
        private static void MainDay4()
        {
            var shift = new Regex("\\[([\\d- :]+).*\\] Guard #(\\d+) begins shift");
            var asleep = new Regex("\\[([\\d- :]+).*\\] falls asleep");
            var wakeUp = new Regex("\\[([\\d- :]+).*\\] wakes up");
            int currentGuard = 0;
            var guards = new Dictionary<int, Guard>();
            var lines = new List<string>();
            for (;;)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                lines.Add(line);
            }

            lines.Sort();
            var transitionTime = DateTime.Now;
            var wake = true;
            foreach (var line in lines)
            {
                var match = shift.Match(line);
                if (match.Success)
                {
                    // end previous shift
                    if (currentGuard > 0)
                    {
                        var endPreviousShift = new DateTime(transitionTime.Year, transitionTime.Month, transitionTime.Day, 1, 0, 0);
                        if (wake)
                        {
                            guards[currentGuard].Round(transitionTime, endPreviousShift);
                        }
                        else
                        {
                            guards[currentGuard].Sleep(transitionTime, endPreviousShift);
                        }
                    }
                    // new shift
                    currentGuard = int.Parse(match.Groups[2].Value);
                    var time = DateTime.Parse(match.Groups[1].Value);
                    if (!guards.ContainsKey(currentGuard))
                    {
                        guards.Add(currentGuard, new Guard());
                    }
                    guards[currentGuard].BeginShift(time);
                    transitionTime = time;
                    wake = true;
                }
                else
                {
                    match = asleep.Match(line);
                    if (match.Success)
                    {
                        var sleepTime = DateTime.Parse(match.Groups[1].Value);
                        guards[currentGuard].Round(transitionTime, sleepTime);
                        transitionTime = sleepTime;
                        wake = false;
                    }
                    else
                    {
                        match = wakeUp.Match(line);
                        var wakeTime = DateTime.Parse(match.Groups[1].Value);
                        guards[currentGuard].Sleep(transitionTime, wakeTime);
                        transitionTime = wakeTime;
                        wake = false;
                    }
                }
            }
            // find longest sleep
            var maxSleep = 0;
            var sleepyGuardId = 0;
            foreach (var guard in guards)
            {
                var sleep = guard.Value.LongestSleep();
                if (sleep > maxSleep)
                {
                    maxSleep = sleep;
                    sleepyGuardId = guard.Key;
                }
            }
            Console.WriteLine($"Sleepy Guard {sleepyGuardId} for {maxSleep} min.");
            Console.WriteLine($"Result {sleepyGuardId*guards[sleepyGuardId].WorstMinute()}");

            var worstMinute = 0;
            var worstGuardId = 0;
            var count = 0;
            for (int i = 0; i < 60; i++)
            {
                foreach (var entry in guards)
                {
                    var temp = entry.Value.CountAsleepAt(i);
                    if (temp > count)
                    {
                        count = temp;
                        worstGuardId = entry.Key;
                        worstMinute = i;
                    }
                }
            }
            Console.WriteLine($"Result 2 {worstGuardId*worstMinute}");
        }

        private class Guard
        {
            private readonly Dictionary<DateTime, bool[]> _rounds = new Dictionary<DateTime, bool[]>();

            public void BeginShift(DateTime begin)
            {
                var night = ComputeRound(begin);
                _rounds.Add(night, new bool[60]);
            }

            private static DateTime ComputeRound(DateTime begin)
            {
                var night = begin.AddHours(2);
                night -= night.TimeOfDay;
                return night;
            }

            public void Round(DateTime begin, DateTime end)
            {
                Tag(begin, end, true);
            }

            public void Sleep(DateTime begin, DateTime end)
            {
                Tag(begin, end, false);
            }

            private void Tag(DateTime begin, DateTime end, bool flag)
            {
                var night = ComputeRound(begin);
                var firstMinute = begin.Minute;
                if (begin.Hour > 12)
                {
                    firstMinute = 0;
                }

                var endMinute = end.Minute;
                if (begin.Hour > 0)
                {
                    endMinute = 60;
                }

                for (int i = firstMinute; i < endMinute; i++)
                {
                    _rounds[night][i] = flag;
                }
            }

            public int CountAsleepAt(int minute)
            {
                var count = 0;
                foreach (var round in _rounds.Values)
                {
                    if (!round[minute])
                    {
                        count++;
                    }
                }

                return count;
            }
            public int WorstMinute()
            {
                var worstMin = 0;
                var maxSleep = 0;
                for (int i = 0; i < 60; i++)
                {
                    var count = CountAsleepAt(i);

                    if (count <= maxSleep) continue;
                    maxSleep = count;
                    worstMin = i;
                }

                return worstMin;
            }

            public int LongestSleep()
            {
                var maxDuration = 0;
                foreach (var round in _rounds)
                {
                    foreach (var status in round.Value)
                    {
                        if (!status)
                        {
                            maxDuration++;
                        }
                    }
                }

                return maxDuration;
            }
        }
    }
}