﻿namespace Quark.Utilities
{
    public interface Identifiable
    {
        string Identifier
        {
            get;
        }
    }

    public interface IMessage
    {
        void Broadcast();
    }

    public interface ITaggable
    {
        string[] Tags
        {
            get;
            set;
        }
    }

    public interface IDeepCopiable
    {
        object DeepCopy();
    }

    public interface IDeepCopiable<out T> : IDeepCopiable
    {
        new T DeepCopy();
    }
}