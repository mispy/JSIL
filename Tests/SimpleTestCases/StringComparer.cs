using System;

public static class Program {
    public static void PrintCompared(string a, string b, StringComparer comparer) {
        var result = comparer.Compare(a, b);
        if (result > 0)
            result = 1;
        else if (result < 0)
            result = -1;
        Console.WriteLine(result);
    }

    public static void Main(string[] args) {
        PrintCompared("asd", "asd", StringComparer.Ordinal);
        PrintCompared("asd", "asd", StringComparer.OrdinalIgnoreCase);
        PrintCompared("Asd", "asd", StringComparer.Ordinal);
        PrintCompared("Asd", "asd", StringComparer.OrdinalIgnoreCase);
        PrintCompared("Asd", "asdf", StringComparer.Ordinal);
        PrintCompared("Asd", "asdf", StringComparer.OrdinalIgnoreCase);
        PrintCompared("asd", "asdf", StringComparer.Ordinal);
        PrintCompared("asd", "asdf", StringComparer.OrdinalIgnoreCase);
    }
}