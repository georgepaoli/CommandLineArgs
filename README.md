# CommandLineArgs

This library should provide easiest possible experience for parsing command line args. If you think something is missing or something simpler please create an issue.
Please mind that everything is being prototyped right now so anything may change in the future as well as this readme.md might not be fully up-to-date.

# Download
```
Nothing to download yet. Unfortunatelly you will need to build it yourself :(
```

# Quick Start in examples
Printing Colored Text
```csharp
using CommandLineArgs;
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

    public static int Main(string[] args)
    {
        ConsoleApp.FromCommandLineArgs<Example1>(args).Start();
        return 0;
    }
}
```
Now simply run any of these:
```
QuickStartExamples
QuickStartExamples /color=red
QuickStartExamples /text="Could this be any simpler?"
QuickStartExamples /color=red /text="Could this be any simpler?"
```
Passing args works in many ways - each of them works the same (it is meant to be simple):
```
/color:white
/color=white
/color white
-color:white
-color=white
-color white
```

Now, let's customize our class a little bit:
```csharp
public class Example2
{
    [Alias("c")]
    public ConsoleColor Color = ConsoleColor.Cyan;

    [PopArg] // Pops first free argument from the left
    public string Text = "Hello World!";

    public void Start()
    {
        Console.ForegroundColor = Color;
        Console.WriteLine(Text);
        Console.ResetColor();
    }
}
```

Now in addition to previous example also this works:
```
QuickStartExamples /c white "I am the first free arg"
QuickStartExamples "I am the first free arg" /c:green
```

Also supported:
`[Required]` - makes your arg required (as it wasn't already self descriptive)
Commands - see the test project (I could've gone too far with this)

Currently deserializing types:
- int, string, bool
- any enum type
- nullable types (assuming wrapped type is supported)

Nearest feature:
- arrays
