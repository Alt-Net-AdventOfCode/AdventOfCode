// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay7 : SolverWithLineParser
{
    private readonly Directory _root = new Directory("", null);
    private Directory? _current;
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 7;
        dayAutomaton.RegisterTestDataAndResult(@"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
", 95437, 1);
        dayAutomaton.RegisterTestDataAndResult(@"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
", 24933642, 2);
    }

    private static int FindSmallDir(Directory directory, int threshold) => 
        directory.SubDirectories.Sum(sub => FindSmallDir(sub, threshold)) + (directory.Size <= threshold ? directory.Size : 0);

    public override object GetAnswer1() => FindSmallDir(_root, 100000);

    private static int FindSmallestMatch(Directory directory, int currentMin, int neededSpace)
    {
        var thisSize = directory.Size;
        if (thisSize < neededSpace)
        {
            // we cannot free enough space
            return int.MaxValue;
        }

        if (thisSize >= neededSpace)
        {
            currentMin = Math.Min(currentMin, thisSize);
        }
        foreach (var sub in directory.SubDirectories)
        {
            var smallest = FindSmallestMatch(sub, currentMin, neededSpace);
            currentMin = Math.Min(smallest, currentMin);
        }

        return currentMin;
    }
    
    public override object GetAnswer2() => FindSmallestMatch(_root, int.MaxValue, 30000000 - (70000000 - _root.Size));

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _current ??= _root;
        if (line.StartsWith("$ cd"))
        {
            var subDirectory = line[4..].Trim();
            _current = subDirectory == "/" ? _root : _current.FindSubDirectory(subDirectory);
        }
        else if (!line.StartsWith("$ ls"))
        {
            if (line.StartsWith("dir"))
            {
                var sub = new Directory(line[4..], _current);
                _current.AddSubDirectory(sub);
            }
            else
            {
                var attributes = line.Split(' ');
                var file = new File(attributes[1], int.Parse(attributes[0]));
                _current.AddChild(file);
            }
        }
    }

    private abstract class Node
    {
        public abstract int Size { get; }
        public abstract string Name { get; }
    }

    private class Directory : Node
    {
        private readonly List<File> _children = new();
        private readonly List<Directory> _subDirectories = new();
        private readonly Directory? _parent;

        public IList<Directory> SubDirectories => _subDirectories;

        public override int Size => _children.Sum(c => c.Size) + _subDirectories.Sum(c =>c.Size);
        public override string Name { get; }

        public Directory(string name, Directory? parent)
        {
            Name = name;
            _parent = parent;
        }

        public void AddSubDirectory(Directory directory)
        {
            _subDirectories.Add(directory);
        }
        
        public void AddChild(File child)
        {
            _children.Add(child);
        }

        public Directory FindSubDirectory(string name)
        {
            if (name == "..")
            {
                return _parent ?? this;
            }
            
            foreach (var child in _subDirectories.Where(child => child.Name == name))
            {
                return child;
            }

            var sub = new Directory(name, this);
            _subDirectories.Add(sub);
            return sub;
        }
    }
    
    private class File : Node
    {
        public override int Size { get; }
        public override string Name { get; }

        public File(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}