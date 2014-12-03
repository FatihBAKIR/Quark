using System;
using UnityEngine;

namespace Quark
{
    public class Events
    {
        public static string Make(string Event, params Identifiable[] objs)
        {
            string e = "";
            foreach (Identifiable i in objs)
            {
                e += i.Identifier() + ".";
            }
            e += Event;
            return e;
        }

        public static void StatChange(Character of, Stat stat, double Difference)
        {
            Messenger<Character, double>.Broadcast(stat.Tag + ".Changed", of, Difference);
            Messenger<double>.Broadcast(StatChangeEvent(of, stat.Tag), Difference);
        }

        public static string StatChangeEvent(Character of, string stat)
        {
            return of.GetHashCode() + "." + stat + ".Changed";
        }
    }

    public class Event<T>
    {
        string name;

        public Event(string Name)
        {
            this.name = Name;
        }

        public string Generic()
        {
            return this.name;
        }

        public string Of(Character of)
        {
            return of.GetHashCode() + "." + this.Generic();
        }

        public void Listen(Callback handler)
        {
            //Messenger.AddListener(this.Of(), handler);
        }

        public void Invoke(T data, Character of = null)
        {
            Messenger<T>.Broadcast(this.Generic(), data);
            if (of != null)
                Messenger<Character, T>.Broadcast(this.Generic(), of, data);
        }
    }
}