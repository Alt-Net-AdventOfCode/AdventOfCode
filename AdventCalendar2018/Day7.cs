/*
 *  Got stuck on a small glitches (post decrementation instead of pre decrementation + eager work)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    class ActionItem
    {
        private sealed class IdRelationalComparer : IComparer<ActionItem>
        {
            public int Compare(ActionItem x, ActionItem y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return string.Compare(x.ID, y.ID, StringComparison.Ordinal);
            }
        }

        public static IComparer<ActionItem> IdComparer { get; } = new IdRelationalComparer();

        private readonly string ID;
        private int workTime;
        
        private readonly IList<string> dependsOn = new List<string>();

        public ActionItem(string id)
        {
            ID = id;
            workTime = 60 + (id[0] - 'A') + 1;
        }

        public void DependsOn(string other)
        {
            dependsOn.Add(other);
        }

        public bool CanBeDone()
        {
            return dependsOn.Count == 0;
        }

        public bool ResolveDependence(string depends)
        {
            return dependsOn.Remove(depends);
        }

        public bool Work()
        {
            return --workTime == 0;
        }
    }
    public static class Day7
    {
        private static Dictionary<string, ActionItem> _actions;
        private static List<string> _actionItems;

        private static void MainDay7()
        {
            var matcher = new Regex("^Step (\\S) must be finished before step (\\S) can begin.");
            var depends = Input.Split(Environment.NewLine);
            _actions = new Dictionary<string, ActionItem>(depends.Length);
            foreach (var depend in depends)
            {
                var result = matcher.Match(depend);
                if (result.Success)
                {
                    var action = result.Groups[2].Value;
                    var dependence = result.Groups[1].Value;
                    if (!_actions.ContainsKey(action))
                    {
                        _actions.Add(action, new ActionItem(action));
                    }
                    if (!_actions.ContainsKey(dependence))
                    {
                        _actions.Add(dependence, new ActionItem(dependence));
                    }
                    _actions[action].DependsOn(dependence);
                }
            }
/*
            _actionItems = _actions.Keys.ToList();
            _actionItems.Sort();
            
            var resultText = string.Empty;
            while (_actionItems.Count>0)
            {
                var actionItem = PickActionItem();
                resultText += actionItem;    
                foreach (var value in _actions.Values)
                {
                    value.ResolveDependence(actionItem);
                }
            }
            Console.WriteLine($"Result :'{resultText}'");
            */
            // time based computation
            var neededTime = 0;
            _actionItems = _actions.Keys.ToList();
            _actionItems.Sort();
            var nbElves = 5;
            var processing = new string[nbElves];
            while (_actionItems.Count>0 || processing.Any(x => !string.IsNullOrEmpty(x)))
            {
                for (var i = 0; i < processing.Length; i++)
                {
                    if (string.IsNullOrEmpty(processing[i]))
                    {
                        processing[i] = PickActionItem();
                    }
                }
                
                for (var i = 0; i < processing.Length; i++)
                {
                    if (string.IsNullOrEmpty(processing[i]))
                    {
                        Console.Write(". ");
                        continue;
                    }
                    Console.Write(processing[i]+" ");
                    if (_actions[processing[i]].Work())
                    {
                        // we are done with it
                        foreach (var value in _actions.Values)
                        {
                            value.ResolveDependence(processing[i]);
                        }
                        processing[i] = string.Empty;
                    }
                }

                neededTime++;
                Console.WriteLine();
            }
            Console.WriteLine($"TotalTime :'{neededTime}'");
        }

        private static string PickActionItem()
        {
            foreach (var actionItem in _actionItems)
            {
                if (_actions[actionItem].CanBeDone())
                {
                    _actionItems.Remove(actionItem);
                    return actionItem;
                }
            }

            return string.Empty;
        }

        private const string demoInput = 
@"Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.";
        
        private const string Input =
@"Step G must be finished before step W can begin.
Step X must be finished before step S can begin.
Step F must be finished before step V can begin.
Step C must be finished before step Y can begin.
Step M must be finished before step J can begin.
Step K must be finished before step Z can begin.
Step U must be finished before step W can begin.
Step I must be finished before step H can begin.
Step W must be finished before step B can begin.
Step A must be finished before step Y can begin.
Step Y must be finished before step D can begin.
Step S must be finished before step Q can begin.
Step N must be finished before step V can begin.
Step H must be finished before step D can begin.
Step D must be finished before step Q can begin.
Step L must be finished before step E can begin.
Step Q must be finished before step E can begin.
Step T must be finished before step R can begin.
Step J must be finished before step P can begin.
Step R must be finished before step E can begin.
Step E must be finished before step V can begin.
Step O must be finished before step P can begin.
Step P must be finished before step B can begin.
Step Z must be finished before step V can begin.
Step B must be finished before step V can begin.
Step Y must be finished before step B can begin.
Step C must be finished before step B can begin.
Step Q must be finished before step T can begin.
Step W must be finished before step P can begin.
Step X must be finished before step Z can begin.
Step L must be finished before step T can begin.
Step G must be finished before step Y can begin.
Step Y must be finished before step R can begin.
Step E must be finished before step B can begin.
Step X must be finished before step E can begin.
Step Y must be finished before step V can begin.
Step H must be finished before step L can begin.
Step L must be finished before step J can begin.
Step S must be finished before step T can begin.
Step F must be finished before step T can begin.
Step Y must be finished before step J can begin.
Step A must be finished before step H can begin.
Step P must be finished before step Z can begin.
Step R must be finished before step O can begin.
Step X must be finished before step F can begin.
Step I must be finished before step O can begin.
Step Y must be finished before step Q can begin.
Step S must be finished before step D can begin.
Step Q must be finished before step B can begin.
Step C must be finished before step D can begin.
Step Y must be finished before step N can begin.
Step O must be finished before step Z can begin.
Step G must be finished before step D can begin.
Step A must be finished before step O can begin.
Step U must be finished before step N can begin.
Step Y must be finished before step P can begin.
Step E must be finished before step O can begin.
Step I must be finished before step Q can begin.
Step W must be finished before step O can begin.
Step D must be finished before step B can begin.
Step Z must be finished before step B can begin.
Step L must be finished before step B can begin.
Step P must be finished before step V can begin.
Step C must be finished before step E can begin.
Step S must be finished before step O can begin.
Step U must be finished before step T can begin.
Step U must be finished before step O can begin.
Step Y must be finished before step L can begin.
Step N must be finished before step L can begin.
Step Q must be finished before step Z can begin.
Step U must be finished before step L can begin.
Step U must be finished before step D can begin.
Step J must be finished before step O can begin.
Step L must be finished before step R can begin.
Step S must be finished before step P can begin.
Step H must be finished before step R can begin.
Step F must be finished before step I can begin.
Step D must be finished before step T can begin.
Step C must be finished before step M can begin.
Step W must be finished before step D can begin.
Step R must be finished before step V can begin.
Step U must be finished before step S can begin.
Step K must be finished before step R can begin.
Step D must be finished before step V can begin.
Step D must be finished before step R can begin.
Step I must be finished before step E can begin.
Step L must be finished before step O can begin.
Step T must be finished before step Z can begin.
Step A must be finished before step E can begin.
Step D must be finished before step Z can begin.
Step H must be finished before step V can begin.
Step A must be finished before step L can begin.
Step W must be finished before step R can begin.
Step F must be finished before step A can begin.
Step Y must be finished before step Z can begin.
Step I must be finished before step P can begin.
Step F must be finished before step J can begin.
Step H must be finished before step B can begin.
Step G must be finished before step Z can begin.
Step C must be finished before step K can begin.
Step D must be finished before step E can begin.";
    }
}