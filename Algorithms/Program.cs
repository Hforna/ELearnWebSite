using System.Collections.Generic;
using System;
using System.Numerics;
using System.Runtime.ExceptionServices;

public static class BinarySearch
{
    //public static int FindPosition(int[] array, int number)
    //{
    //    var high = array.Length - 1;
    //    var less = 0;
    //
    //    while(true)
    //    {
    //        var middle = (less + high) / 2;
    //        if(array[middle] == number)
    //        {
    //            return middle;
    //        } else if (array[middle] > number)
    //        {
    //            high = middle - 1;
    //        } else if (array[middle] < number)
    //        {
    //            less = middle + 1;
    //        }
    //    }
    //}
    //
    //public static int? MissingNumber(int[] nums)
    //{
    //    for(var i = 0; i < nums.Length; i++)
    //    {
    //        if (nums.Contains(i + 1) == false)
    //            return i + 1;
    //    }
    //    return null;
    //}
    //
    //public static int[] Intersection(int[] nums1, int[] nums2)
    //{
    //    var intersection = new List<int>();
    //
    //    for (var i = 0; i < nums1.Length; i++)
    //    {
    //        if (nums2.Contains(nums1[i]) && intersection.Contains(nums1[i] == false)
    //            intersection[i] = nums1[i];
    //    }
    //    return intersection.ToArray();
    //}
    //
    //public int GuessNumber(int n, int res)
    //{
    //    var high = n;
    //    var low = 1;
    //
    //    while(true)
    //    {
    //        int middle = Math.Abs((low + high) / 2);
    //        if(result == 0)
    //        {
    //            return middle;
    //        } else if(result == 1)
    //        {
    //            high = middle  1;
    //        } else if(result == -1)
    //        {
    //            low = middle - 1;
    //        }
    //    }
    //}

    //public static int CountNegatives(int[][] grid)
    //{
    //    var count = 0;
    //    for (var i = 0; i < grid.Length; i++)
    //    {
    //        for(var x = 0; x < grid[i].Length; x++)
    //        {
    //            if (grid[i][x] < 0)
    //                count += 1;
    //        }
    //    }
    //    var list = new List<string>();
    //    list.AddRange(list[]);
    //    return count;
    //}

    public static string MergeAlternately(string word1, string word2)
    {
        var list = new List<char>();

        for (var i = 0; i < word1.Length; i++)
        {
            list.Add(word1[i]);
            if (i <= word2.Length - 1)
            {
                list.Add(word2[i]);
            }
        }
        if(word1.Length < word2.Length)
            list.AddRange(word2[(word1.Length)..word2.Length]);

        return string.Join("", list);
    }

    public static double SumArray(int[] array)
    {
        if (array.Length == 0)
        {
            return 0;
        }
        return array[0] + SumArray(array[1..]);
    }

    public static int[] QuickSort(int[] array)
    {
        if (array.Length < 2)
            return array;

        var high = new List<int>();
        var less = new List<int>();
        var p = array[0];
        for(var i = 1; i < array.Length; i++)
        {
            if (array[i] < p)
            {
                less.Add(array[i]);
            } else if (array[i] > p)
            {
                less.Add(array[i]);
            }
        }
        return QuickSort(less.ToArray()) + [p] + QuickSort(high.ToArray());
    }

    //public static bool IsValidScheduleList(Dictionary<string, string[]> schedule, string[] scheduleWish)
    //{
    //    //var queue = Queue
    //    while(schedule)
    //    {
    //        var step = schedule
    //    }
    //}

}


public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(BinarySearch.QuickSort(new int[6] { 2, 6, 3, 4, 5, 1 }));
        //Console.WriteLine(BinarySearch.MergeAlternately("abc", "fghid"));
        Console.WriteLine(BinarySearch.SumArray(new int[6] { 1, 4, 7, 9, 5, 7 }));
        //Console.WriteLine(BinarySearch.CountNegatives(array));
        //Console.WriteLine(BinarySearch.MissingNumber(array));
        Console.ReadKey();
    }
}