namespace Quark.Spells
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

    public class CancelError : CastError
    {
        public CancelError()
            : base("Cancelled casting")
        { }
    }

    public class RangeError : CastError
    {
        public RangeError()
            : base("Target out of range")
        { }
    }
}
