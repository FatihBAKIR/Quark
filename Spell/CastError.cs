using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quark.Spell
{
    public class CastError
    {
        public string Message { get; private set; }

        public CastError(string message)
        {
            Message = message;
        }
    }

    public class BusyError : CastError
    {
        public BusyError()
            : base("Caster Busy")
        { }
    }

    public class TargetingError : CastError
    {
        public TargetingError()
            : base("Only one spell can be targeted at once")
        { }
    }

    public class CancelError : CastError
    {
        public CancelError()
            : base("Cancelled casting")
        { }
    }
}
