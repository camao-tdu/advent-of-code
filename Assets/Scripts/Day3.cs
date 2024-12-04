using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Day3 : MonoBehaviour
{
    public TextAsset inputFile;

    public void Run()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;



        Regex regx = new(@"mul\(\d{1,3},\d{1,3}\)");

        var values = regx.Matches(input).Cast<Match>().Select(match => match.Value);

        int result = 0;
        foreach (var mul in values)
        {
            var operators = mul.Replace("mul(", "").Replace(")", "").Split(",").Select(p => int.Parse(p)).ToArray();
            result += operators[0] * operators[1];
        }


        stopwatch.Stop();
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ns", result, stopwatch.Elapsed.TotalMilliseconds * 1000000));

    }

    public void Run2()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var input = inputFile.text;

        Regex regx = new(@"mul\(\d{1,3},\d{1,3}\)");
        Regex regxDos = new(@"do\(\)");
        Regex regxDonts = new(@"don't\(\)");


        var matches = regx.Matches(input).Cast<Match>();
        var doIndices = regxDos.Matches(input).Cast<Match>().Select(match => match.Index).ToList();
        var dontIndices = regxDonts.Matches(input).Cast<Match>().Select(match => match.Index).ToList();
        doIndices.Sort();
        dontIndices.Sort();
        List<String> enabledMuls = new();
        foreach (var match in matches)
        {
            var index = match.Index;
            var doBeforeIndex = Int32.MaxValue;
            for (int i = 0; i < doIndices.Count(); i++)
            {
                if (doIndices[i] < index) doBeforeIndex = doIndices[i];
                else break;
            }
            var dontBeforeIndex = Int32.MaxValue;
            for (int i = 0; i < dontIndices.Count(); i++)
            {
                if (dontIndices[i] < index) dontBeforeIndex = dontIndices[i];
                else break;
            }
            if (index < dontBeforeIndex)
                enabledMuls.Add(match.Value);
            else if (index > dontBeforeIndex)
            {
                if (doBeforeIndex < index && doBeforeIndex > dontBeforeIndex)
                    enabledMuls.Add(match.Value);
            }
        }

        int result = 0;
        foreach (var mul in enabledMuls)
        {
            var operators = mul.Replace("mul(", "").Replace(")", "").Split(",").Select(p => int.Parse(p)).ToArray();
            result += operators[0] * operators[1];
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ms", result, stopwatch.ElapsedMilliseconds));

    }


}
