using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Quark
{
    public class EventDictionary : Dictionary<string, Delegate>
    {
        public Delegate[] GetHandlers(string Event)
        {
            List<Delegate> matches = new List<Delegate>();
            foreach (string pattern in Keys)
                if (Regex.IsMatch(Event, pattern))
                    matches.Add(this[pattern]);
            return matches.ToArray();
        }
    }
}