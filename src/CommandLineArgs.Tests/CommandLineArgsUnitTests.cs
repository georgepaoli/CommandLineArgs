using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    [DefaultCommand(".*")]
    public class CommandLineArgsUnitTests
    {
        // TODO: Doesn't look too bad if you use foreach although might be slightly confusing if used without
        // TODO: Does this design make sense?
        // TODO: This test looks pretty bad although it does cover most of the stuff
        public void MainScenario()
        {
            CommandLineArgs args = new CommandLineArgs();
            args.AddArgs(new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i" });

            // parse args
            foreach (var arg in args)
            {
                switch (arg.OriginalValue)
                {
                    case "a":
                    case "c":
                    case "d":
                    case "h":
                    case "i":
                        args.ProcessCurrentArgLater();
                        break;
                }
            }

            Assert.False(args.Empty);

            // wave 2
            int n = 0;
            foreach (var arg in args)
            {
                switch (arg.OriginalValue)
                {
                    case "a":
                    case "i":
                        n++;
                        break;
                    case "c":
                    case "d":
                    case "h":
                        args.ProcessCurrentArgLater();
                        n++;
                        break;
                    default:
                        Assert.True(false);
                        break;
                }
            }

            Assert.Equal(5, n);
            Assert.False(args.Empty);

            foreach (var arg in args)
            {
                switch (arg.OriginalValue)
                {
                    case "c":
                    case "d":
                    case "h":
                        args.ProcessCurrentArgLater();
                        break;
                }
            }

            Assert.False(args.Empty);

            foreach (var arg in args)
            {
                args.ProcessCurrentArgLater();

                switch (arg.OriginalValue)
                {
                    case "c":
                        Assert.Equal("d", args.PeekNext()?.OriginalValue);
                        break;
                    case "d":
                        Assert.Equal("h", args.PeekNext()?.OriginalValue);
                        break;
                    case "h":
                        Assert.Equal(null, args.PeekNext());
                        break;
                }
            }

            Assert.False(args.Empty);

            foreach (var arg in args)
            {
                args.ProcessCurrentArgLater();
                switch (arg.OriginalValue)
                {
                    case "c":
                    case "d":
                    case "h":
                        if (args.Skip())
                        {
                            args.ProcessCurrentArgLater();
                        }
                        break;
                }
            }

            Assert.False(args.Empty);

            args.AddArgs(new string[] { "j", "k" });

            bool firstIteration = true;
            foreach (var arg in args)
            {
                Assert.True(firstIteration);
                args.ProcessCurrentArgLater();
                args.ForceNextWave();
                firstIteration = false;
            }

            Assert.False(args.Empty);

            // wave 3
            Assert.True((from arg in args select arg.OriginalValue).SequenceEqual(new string[] { "c", "d", "h", "j", "k" }));
            Assert.True(args.Empty);

            // finished
            Assert.True((from arg in args select arg.OriginalValue).SequenceEqual(new string[0]));
            Assert.True(args.Empty);
        }
    }
}
