using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Only temporary to test
/// </summary>
public class ConditionsChecker : MonoBehaviour {
    public ConditionDescription[] conditionals;


    void Start() {

        for (int i = 0; i < conditionals.Length; i++)
            Debug.Log("Conditional check[" + i + "]: " + conditionals[i].GetValue());

    }
}




