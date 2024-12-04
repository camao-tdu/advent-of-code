using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Day4 : MonoBehaviour
{
    public TextAsset inputFile;
    char[,] grid;
    public void Run()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        CreateGrid();
        int result = 0;
        //XMAS
        var neighboursToFind = "MAS".ToCharArray();
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                if (grid[y, x] == 'X')
                {
                    //y rows, x columns  y=0 x=1
                    //horizontal
                    if (HasNeighboursHorizontalRight(y, x, neighboursToFind)) result++;
                    if (HasNeighboursHorizontaLeft(y, x, neighboursToFind)) result++;
                    //vertical
                    if (HasNeighboursVerticalTop(y, x, neighboursToFind)) result++;
                    if (HasNeighboursVerticaBottom(y, x, neighboursToFind)) result++;
                    //diagonal
                    if (HasNeighboursDiagonalBottomRight(y, x, neighboursToFind)) result++;
                    if (HasNeighboursDiagonalBottomLeft(y, x, neighboursToFind)) result++;
                    if (HasNeighboursDiagonalTopLeft(y, x, neighboursToFind)) result++;
                    if (HasNeighboursDiagonalTopRight(y, x, neighboursToFind)) result++;
                }
            }
        }
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ns", result, stopwatch.Elapsed.TotalMilliseconds * 1000000));
    }
    public void Run2()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        CreateGrid();
        int result = 0;
        var neighboursToFind = "AS".ToCharArray();
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                if (grid[y, x] == 'M')
                {
                    //M.S
                    //.A.
                    //M.S
                    if (y + 2 < grid.GetLength(0) && grid[y + 2, x] == 'M')
                        if (HasNeighboursDiagonalBottomRight(y, x, neighboursToFind)
                            && HasNeighboursDiagonalTopRight(y + 2, x, neighboursToFind)) result++;
                    //S.M
                    //.A.
                    //S.M
                    if (y + 2 < grid.GetLength(0) && grid[y + 2, x] == 'M')
                        if (HasNeighboursDiagonalBottomLeft(y, x, neighboursToFind)
                            && HasNeighboursDiagonalTopLeft(y + 2, x, neighboursToFind)) result++;
                    //M.M
                    //.A.
                    //S.S
                    if (x + 2 < grid.GetLength(1) && grid[y, x + 2] == 'M')
                        if (HasNeighboursDiagonalBottomRight(y, x, neighboursToFind)
                            && HasNeighboursDiagonalBottomLeft(y, x + 2, neighboursToFind)) result++;
                    //S.S
                    //.A.
                    //M.M
                    if (x + 2 < grid.GetLength(1) && grid[y, x + 2] == 'M')
                        if (HasNeighboursDiagonalTopRight(y, x, neighboursToFind)
                            && HasNeighboursDiagonalTopLeft(y, x + 2, neighboursToFind)) result++;
                }
            }
        }
        UnityEngine.Debug.Log(String.Format("result: {0} in: {1} ns", result, stopwatch.Elapsed.TotalMilliseconds * 1000000));
    }

    private void CreateGrid()
    {
        var input = inputFile.text;
        var lines = input.Split("\r\n");
        grid = new char[lines.Length, lines.First().Length];
        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];
            for (int x = 0; x < line.Length; x++)
                grid[y, x] = line[x];
        }
    }

    bool HasNeighboursVerticalTop(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startY - i < 0) return false;
            if (grid[startY - i, startX] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursVerticaBottom(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startY + i >= grid.GetLength(0)) return false;
            if (grid[startY + i, startX] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursDiagonalBottomRight(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX + i >= grid.GetLength(1) || startY + i >= grid.GetLength(0)) return false;

            if (grid[startY + i, startX + i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursDiagonalBottomLeft(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX - i < 0 || startY + i >= grid.GetLength(0)) return false;

            if (grid[startY + i, startX - i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursDiagonalTopRight(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX + i >= grid.GetLength(1) || startY - i < 0) return false;

            if (grid[startY - i, startX + i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursDiagonalTopLeft(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX - i < 0 || startY - i < 0) return false;
            if (grid[startY - i, startX - i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }

    bool HasNeighboursHorizontalRight(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX + i >= grid.GetLength(1)) return false;
            if (grid[startY, startX + i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }
    bool HasNeighboursHorizontaLeft(int startY, int startX, char[] neighboursToFind)
    {
        for (int i = 1; i <= neighboursToFind.Length; i++)
        {
            if (startX - i < 0) return false;
            if (grid[startY, startX - i] != neighboursToFind[i - 1]) return false;
        }
        return true;
    }




}
