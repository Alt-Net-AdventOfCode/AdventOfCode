// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
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
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2024;

public class DupdobDay09 : SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 9;
        dayAutomatonBase.RegisterTestDataAndResult("2333133121414131402", 1928, 1);
        dayAutomatonBase.RegisterTestResult(2858, 2);
    }

    public override object GetAnswer1()
    {
        // we need to fill the free space with files at the end
        var filePosToMove = _disk.Count - 1;
        var remainingFileSize = _disk[filePosToMove].size;
        var result = 0L;
        var currentPos = 0L; 
        for (var currentId =0; ; currentId++)
        {
            var blockSize = _disk[currentId].size;
            if (currentId==filePosToMove)
            {
                blockSize = remainingFileSize;
                result += ComputeBlockChecksum(currentId, blockSize);
                break;
            }
            // we compute the checksum of the current non-split fle
            result += ComputeBlockChecksum(currentId, blockSize);
            // now fill the empty space
            blockSize = _disk[currentId].free;
            while (blockSize>0)
            {
                var nexBlock = Math.Min(blockSize, remainingFileSize);
                result+= ComputeBlockChecksum(filePosToMove, nexBlock);
                remainingFileSize -= nexBlock;
                blockSize-=nexBlock;
                
                if (remainingFileSize == 0)
                {
                    filePosToMove--;
                    remainingFileSize = _disk[filePosToMove].size;
                }

                if (filePosToMove == currentId)
                {
                    // we ran out of file to move
                    break;
                }
            }

            if (filePosToMove == currentId)
            {
                break;
            }

        }

        return result;

        long ComputeBlockChecksum(int id, int blockSize)
        {
            var localChecksum = id*((2*currentPos+blockSize-1)*blockSize/2);
            currentPos += blockSize;
            return localChecksum;
        }
    }

    public override object GetAnswer2()
    {
        // we will defragment the files
        var newDisk = new List<(int id, int size, int free)>(_disk);
        var firstSpace = -1;
        for (var currentId = newDisk.Count-1; currentId > 0; currentId--)
        {
            var (id,size, free) = newDisk[currentId];
            for (var i = firstSpace+1; i < currentId; i++)
            {
                if (newDisk[i].free == 0 && i == firstSpace + 1)
                {
                    firstSpace++; 
                    continue;
                }
                if (newDisk[i].free < newDisk[currentId].size) continue;
                // we can move it
                var newFree = newDisk[i].free - size;
                newDisk[i]= (newDisk[i].id, newDisk[i].size,  0 );
                newDisk.Insert(i+1, (id, size, newFree));
                newDisk[currentId] = (newDisk[currentId].id, newDisk[currentId].size, newDisk[currentId].free+free+size);
                currentId++;
                newDisk.RemoveAt(currentId);
                break;
            }
        }
        
        var result = 0L;
        // we must compute the score
        var currentPos= 0;
        foreach (var entry in newDisk)
        {
            result+= entry.id*((2*currentPos+entry.size-1)*(long)entry.size/2);
            currentPos += entry.size + entry.free;
        }

        return result;
    }

    private readonly List<(int id, int size, int free)> _disk = new();
    
    protected override void Parse(string data)
    {
        for (var i = 0; i < data.Length; i += 2)
        {
            var freeSpace = i < data.Length - 1 ? (data[i + 1] - '0') : 0;
            _disk.Add((i/2, data[i]-'0', freeSpace));
        }
    }
}