using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sandbox
{
    [DefaultCommand("Start")]
    public class Test
    {
        // this is actually ignored
        // TODO: Should app have an option ConsoleApp<Program>.PrintUsage()?
        //       I'd rather add all missing cases in the library and force people not to have to use that...
        // TODO: Standard description for only Alias("h") is super verbose.
        //      Code Should have only and only Alias("h") and no [NoDefaultAlias] especially
        [Alias("-h|--help|--?")]
        [NoDefaultAlias]
        [Description("Prints this usage text")]
        public bool Help;

        [NoDefaultAlias]
        // TODO: Messages should be supressed by default for required args when using with commands
        [Required]
        [PopArg]
        [LastProcessedNamedArg]
        [Description("Command to execute")]
        public string Command;

        [NoDefaultAlias]
        [PopRemainingArgs]
        [Description("Arguments to pass to the app")]
        public List<string> Arguments;

        // BUGBUGBUG: This is actually a repro
        //            Repro: Call this program with any args
        //            Expected: works
        //            Actual: Broken
        //            Quick investigation: when command doesn't exist the arg is still being consumed
        // TODO: [HideFromUsage]
        // TODO: [HideAllCommandsFromUsage] [HideAllParametersFromUsage]
        // TODO: [Add fake command for usage (or make [Alias] work with commands...)]
        public void Start()
        {
            if (Command == null)
            {
                // TODO: Not sure what would be the best way to represent commands which are visible only by DefaultCommand
                //       [PrivateCommand]?
                //       or maybe by just making the method internal :-]
                throw new NotSupportedException("TODO: fix it");
            }

            Process p = Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = Command,
                // TODO: escape existing quotes. how do you actually do that the portable way? https://github.com/dotnet/corefx/issues/4720
                Arguments = Arguments != null ? $"\"{string.Join("\" \"", Arguments)}\"" : null
            });
        }
    }
}
