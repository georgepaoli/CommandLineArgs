using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs
{
    // TODO: Make it generic?
    public interface IBinder<FromType, ToType>
    {
        void Bind(FromType source, ToType target);
    }
}
