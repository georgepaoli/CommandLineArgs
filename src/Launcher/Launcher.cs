﻿using CommandLineArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher
{
    [DefaultCommand("Start")]
    public class Test
    {
        // this is actually ignored
        // TODO: Should app have an option ConsoleApp<Program>.PrintUsage()?
        //       I'd rather add all missing cases in the library and force people not to have to use that...
        // TODO: Standard description for only Alias("h") is super verbose.
        //      Code Should have only and only Alias("h") and no [NoDefaultAlias] especially
        [Alias("-h|--?")]
        [Description("Prints this usage text")]
        public bool Help;

        [NoDefaultAlias]
        // TODO: Messages should be supressed by default for required args when using with commands
        [Required]
        [Verb]
        [LastProcessedNamedArg]
        [Description("Command to execute")]
        public string Command;

        [NoDefaultAlias]
        [PopRemainingArgs]
        [Description("Arguments to pass to the app")]
        public List<string> Arguments;

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
                //       perhaps maybe this should not be possible - special behavior is often annoying for users and not intuitive
                throw new NotSupportedException("If you see this error create an issue.");
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
    public class Launcher
    {
    }
}
