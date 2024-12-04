using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Day1 : MonoBehaviour
{
    public TextAsset inputFile;

    public void Run()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;
        List<int> column1List = new();
        List<int> column2List = new();
        Regex regx = new(@"\d+");
        using (StringReader reader = new(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = regx.Matches(line).ToArray();
                column1List.Add(int.Parse(values[0].Value));
                column2List.Add(int.Parse(values[1].Value));
            }
        }

        var column1 = new NativeArray<int>(column1List.Count, Allocator.Persistent);
        var column2 = new NativeArray<int>(column1List.Count, Allocator.Persistent);
        var output = new NativeArray<int>(1, Allocator.Persistent);

        for (int i = 0; i < column1List.Count; ++i)
        {
            column1[i] = column1List[i];
            column2[i] = column2List[i];
        }

        var job = new FirstJob
        {
            column1 = column1,
            column2 = column2,
            output = output
        };
        job.Schedule().Complete();
        stopwatch.Stop();
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ms", output[0], stopwatch.ElapsedMilliseconds));
        output.Dispose();
        column1.Dispose();
        column2.Dispose();
    }

    public void Run2()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;
        List<int> column1List = new();
        List<int> column2List = new();
        Regex regx = new(@"\d+");
        using (StringReader reader = new(input))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = regx.Matches(line).ToArray();
                column1List.Add(int.Parse(values[0].Value));
                column2List.Add(int.Parse(values[1].Value));
            }
        }

        var column1 = new NativeArray<int>(column1List.Count, Allocator.Persistent);
        var column2 = new NativeArray<int>(column1List.Count, Allocator.Persistent);
        var output = new NativeArray<int>(1, Allocator.Persistent);

        for (int i = 0; i < column1List.Count; ++i)
        {
            column1[i] = column1List[i];
            column2[i] = column2List[i];
        }

        var job = new SecondJob
        {
            column1 = column1,
            column2 = column2,
            output = output
        };
        job.Schedule().Complete();
        stopwatch.Stop();
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ns", output[0], stopwatch.Elapsed.TotalMilliseconds * 1000000));
        output.Dispose();
        column1.Dispose();
        column2.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct SecondJob : IJob
    {
        public NativeArray<int> column1;
        public NativeArray<int> column2;
        public NativeArray<int> output;

        public void Execute()
        {
            column1.Sort();
            column2.Sort();
            for (int i = 0; i < column1.Length; i++)
            {
                var numberToCheck = column1[i];
                output[0] += column1[i] * column2.Where(x => x == numberToCheck).Count();
            }

        }
    }



    [BurstCompile(CompileSynchronously = true)]
    private struct FirstJob : IJob
    {
        public NativeArray<int> column1;
        public NativeArray<int> column2;
        public NativeArray<int> output;

        public void Execute()
        {
            column1.Sort();
            column2.Sort();
            for (int i = 0; i < column1.Length; i++)
            {
                output[0] += Math.Abs(column1[i] - column2[i]);
            }

        }
    }
}
