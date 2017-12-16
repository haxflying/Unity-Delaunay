using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tick : MonoBehaviour {

    public static Tick instance;

    public delegate void EventHandle();
    public EventHandle OnUpdate, OnDraw;


    private void Awake()
    {
        instance = this;       
    }

    // Update is called once per frame
    void Update () {
        if (OnUpdate != null)
            OnUpdate();                 
	}

    private void OnRenderObject()
    {
        if (OnDraw != null)
            OnDraw();
    }
}
