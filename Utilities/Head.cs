using System;
using UnityEngine;
public class Head : MonoBehaviour
{
	void Start()
	{
		if (Head.head != null)
		{
			DestroyImmediate(this);
                Quark.Logger.Warn("Multiple Head Objects Are Forbidden");
			return;
		}
		Head.head = this;
        Quark.Logger.Info("Head Started");
	}

	void Update()
	{
		Messenger.Broadcast("Update");
	}
	
	public static Head head = null;
}