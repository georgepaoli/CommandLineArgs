using System;

public class Example1
{
    public ConsoleColor Color = ConsoleColor.Cyan;
    public string Text = "Hello World!";

    public void Start()
    {
        Console.ForegroundColor = Color;
        Console.WriteLine(Text);
        Console.ResetColor();
    }
}
