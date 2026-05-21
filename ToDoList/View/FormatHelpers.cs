using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.VisualBasic;

public static class Format
{
    public static void SetConsoleColor(ConsoleColor foreground, ConsoleColor background)
    {
        Console.BackgroundColor = background;
        Console.ForegroundColor = foreground;
    }

    public static void TrueClear()
    {
        Console.Clear();
        Console.Write("\x1b[3J");
        Console.SetCursorPosition(0, 0);
    }

    public static void CentreCursor()
    {
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
    }

    public static void WriteCentered(string text)
    {
        Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
        Console.Write(text);
    }

    public static void WriteList(IEnumerable<string> listItems, int top, int left = -1)
    {
        int longestStringLength = 0;

        for (int i = 0; i < listItems.Count(); i++)
        {
            longestStringLength = Math.Max(listItems.ElementAt(i).Length, longestStringLength );
        }
        left = left == -1 ? (Console.WindowWidth - longestStringLength) / 2 : left;
        Console.SetCursorPosition(left, top);

        for (int i = 0; i < listItems.Count(); i++)
        {
            string item = listItems.ElementAt(i);
            Console.Write($"{i + 1}. {item}");
            top = top + 1;
            Console.SetCursorPosition(left, top);
        }
    }

    public static void WriteTitle(string title)
    {
        Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, 1);
        Console.Write(title);
    }

    public static string Prompt(string message, bool mask = false)
    {
        WriteCentered(message);
        string input = "";

        if (!mask)
        {
            input = Console.ReadLine() ?? "";
        }
        else
        {
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
        }
        return input;
    }

    public static string PromptReadKey(string message)
    {
        WriteCentered(message);
        string input;

        var key = Console.ReadKey(intercept: true);
        input = key.KeyChar.ToString();

        return input;
    }

    public static void Pad(int lines = 1)
    {
        for (int i = 0; i < lines; i++)
            Console.WriteLine();
    }
}