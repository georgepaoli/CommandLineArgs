using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    // DO NOT REFACTOR THIS ANYMORE!!! STAYS AS IS UNLESS ABSOLUTELY NECESSARY
    public class CommandLineArg
    {
        public int Position;
        public string OriginalValue;

        public bool IsUsed;
        public string Prefix;
        public string Name;
        public string Operator;
        public string Value;

        public CommandLineArg(int position, string originalValue)
        {
            Position = position;
            OriginalValue = originalValue;

            IsUsed = false;

            // "Is this Arg or Value? Arg starts with '/' or '-'. If this is Arg then we are finished here."();
            if (originalValue[0] != '/' && originalValue[0] != '-')
            {
                Value = originalValue;
                return;
            }

            int p = originalValue.IndexOfAny(new char[] { ':', '=' });
            if (p == -1)
            {
                Prefix = originalValue[0].ToString();
                Name = originalValue.Substring(1);
            }
            else
            {
                int startPos = 1;
                if (originalValue.Length >= 2 && originalValue[1] == '-')
                {
                    startPos++;
                }

                Prefix = originalValue.Substring(0, startPos);
                Name = originalValue.Substring(startPos, p - 1);
                Operator = originalValue.Substring(p, 1);
                Value = originalValue.Substring(p + 1);
            }
        }
    }
}
