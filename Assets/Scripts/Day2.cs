using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework.Interfaces;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Day2 : MonoBehaviour
{
    public TextAsset inputFile;

    public void Run()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;
        List<List<int>> reportsList = new();
        Regex regx = new(@"\d+");
        using (StringReader reader = new(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = regx.Matches(line).Cast<Match>().Select(match => (int.Parse(match.Value)));
                reportsList.Add(values.ToList());
            }
        }

        NativeArray<JobHandle> jobHandles = new(reportsList.Count(), Allocator.Persistent);
        List<NativeArray<bool>> outputs = new();
        List<NativeArray<int>> nativeLevelArrays = new();

        for (int i = 0; i < reportsList.Count; i++)
        {
            List<int> report = reportsList[i];
            var nativeArray = new NativeArray<int>(report.Count, Allocator.Persistent);
            for (int j = 0; j < report.Count(); ++j)
            {
                nativeArray[j] = report[j];
            }
            nativeLevelArrays.Add(nativeArray);
            var output = new NativeArray<bool>(1, Allocator.Persistent);
            output[0] = true;
            outputs.Add(output);
            var job = new FirstJob
            {
                levels = nativeLevelArrays[i],
                output = outputs[i],
            };
            JobHandle scheduleJobHandle = job.Schedule();
            jobHandles[i] = scheduleJobHandle;
        }

        JobHandle.CompleteAll(jobHandles);
        var result = outputs.Where(res => res[0] == true).Count();
        stopwatch.Stop();

        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ms", result, stopwatch.ElapsedMilliseconds));

        jobHandles.Dispose();
        foreach (var outputArray in outputs)
        {
            outputArray.Dispose();
        }
        foreach (var nativeLevelArray in nativeLevelArrays)
        {
            nativeLevelArray.Dispose();
        }
    }

    public void Run2()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;
        List<List<int>> reportsList = new();
        Regex regx = new(@"\d+");
        using (StringReader reader = new(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = regx.Matches(line).Cast<Match>().Select(match => (int.Parse(match.Value)));
                reportsList.Add(values.ToList());
            }
        }

        NativeArray<JobHandle> jobHandles = new(reportsList.Count(), Allocator.Persistent);
        List<NativeArray<bool>> outputs = new();
        List<NativeArray<int>> nativeLevelArrays = new();

        for (int i = 0; i < reportsList.Count; i++)
        {
            List<int> report = reportsList[i];
            var nativeArray = new NativeArray<int>(report.Count, Allocator.Persistent);
            for (int j = 0; j < report.Count(); ++j)
            {
                nativeArray[j] = report[j];
            }
            nativeLevelArrays.Add(nativeArray);
            var output = new NativeArray<bool>(1, Allocator.Persistent);
            output[0] = true;
            outputs.Add(output);
            var job = new SecondJob
            {
                levels = nativeLevelArrays[i],
                output = outputs[i],
            };
            JobHandle scheduleJobHandle = job.Schedule();
            scheduleJobHandle.Complete();
            jobHandles[i] = scheduleJobHandle;
        }

        JobHandle.CompleteAll(jobHandles);
        var result = outputs.Where(res => res[0] == true).Count();
        stopwatch.Stop();

        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ms", result, stopwatch.ElapsedMilliseconds));

        jobHandles.Dispose();
        foreach (var outputArray in outputs)
        {
            outputArray.Dispose();
        }
        foreach (var nativeLevelArray in nativeLevelArrays)
        {
            nativeLevelArray.Dispose();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct SecondJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> levels;
        [WriteOnly]
        public NativeArray<bool> output;

        public void Execute()
        {
            for (int i = -1; i < levels.Count(); i++)
            {
                if (AreLevelsSave(i)) return;
            }
            output[0] = false;
        }

        private bool AreLevelsSave(int levelToRemove)
        {
            if (levels.Count() == 1 || (levelToRemove >= 0 && levels.Count() == 2)) return true;
            bool decreasing = levels[levelToRemove == 0 ? 1 : 0] > levels[(levelToRemove == 0 || levelToRemove == 1) ? 2 : 1];
            for (int i = 0; i < levels.Count() - 1; ++i)
            {
                int firstIndex = i == levelToRemove ? i + 1 : i;
                int sexondIndex = i == levelToRemove ? i + 2 : i + 1 == levelToRemove ? i + 2 : i + 1;
                if (sexondIndex >= levels.Count()) return true;
                if (decreasing)
                {
                    var dif = levels[firstIndex] - levels[sexondIndex];
                    if (dif <= 0 || dif >= 4)
                    {
                        return false;
                    }
                }
                else
                {
                    var dif = levels[sexondIndex] - levels[firstIndex];
                    if (dif <= 0 || dif >= 4)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }


    [BurstCompile(CompileSynchronously = true)]
    private struct FirstJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> levels;
        [WriteOnly]
        public NativeArray<bool> output;

        public void Execute()
        {
            if (levels.Count() == 1) return;
            bool decreasing = levels[0] > levels[1];
            for (int i = 0; i < levels.Count() - 1; ++i)
            {
                if (decreasing)
                {
                    var dif = levels[i] - levels[i + 1];
                    if (dif <= 0 || dif >= 4)
                    {
                        output[0] = false;
                        return;
                    }
                }
                else
                {
                    var dif = levels[i + 1] - levels[i];
                    if (dif <= 0 || dif >= 4)
                    {
                        output[0] = false;
                        return;
                    }
                }
            }
        }
    }
}
