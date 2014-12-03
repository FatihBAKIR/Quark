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
                if (Regex.IsMatch(Event, Regex.Escape(pattern).Replace( @"\*", ".*" ).Replace( @"\?", "." )))
                    matches.Add(this[pattern]);
            UnityEngine.Debug.Log(matches.Count);
            return matches.ToArray();
        }
    }
}