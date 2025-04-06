using System.Collections.Generic;
using System;

public static class BinarySearch
{
    public static int FindPosition(int[] array, int number)
    {
        var high = array.Length - 1;
        var less = 0;

        while(true)
        {
            var middle = (less + high) / 2;
            if(array[middle] == number)
            {
                return middle;
            } else if (array[middle] > number)
            {
                high = middle - 1;
            } else if (array[middle] < number)
            {
                less = middle + 1;
            }
        }
    }

    public static int? MissingNumber(int[] nums)
    {
        for(var i = 0; i < nums.Length; i++)
        {
            if (nums.Contains(i + 1) == false)
                return i + 1;
        }
        return null;
    }

    public class Solution : GuessGame
    {
        public int GuessNumber(int n)
        {
            var high = n;
            var low = 0;

            while(true)
            {
                int middle = Math.Abs((low + high) / 2);
                var result = guess(middle);
                if(result == 0)
                {
                    return middle;
                } else if(result == 1)
                {
                    high = middle  1;
                } else if(result == -1)
                {
                    low = middle - 1
                }
            }
        }
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        var array = new int[3] {4, 2, 1};

        Console.WriteLine(BinarySearch.MissingNumber(array));
        Console.ReadKey();
    }
}