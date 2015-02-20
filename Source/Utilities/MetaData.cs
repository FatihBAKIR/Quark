using System;

namespace Quark.Utilities
{
    public class MetaData
    {
        public MetaData(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}

