using CommandLineArgs;
using System;

[DefaultCommand("Start")]
public class Example2
{
    [Alias("c")]
    public ConsoleColor Color = ConsoleColor.Cyan;

    [PopArg]
    public string Text = "Hello World!";

    public void Start()
    {
        Console.ForegroundColor = Color;
        Console.WriteLine(Text);
        Console.ResetColor();
    }
}
