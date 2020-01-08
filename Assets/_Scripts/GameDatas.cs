﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameDatas", menuName = "GameDatas")]
public class GameDatas : ScriptableObject
{
    public float playerSpeed;
    public bool previsualisation;
    public int spheresMinRadius;
    public int spheresMaxRadius;
    public int distanceInit;
    public int MaxRotationZ;
    public GameObject[] objects;




    [Header ("Light Settings")]
    [Tooltip ("Brightness max of the light")] public float brightnessMax;
    [Tooltip("Time to arrive to max brightness")] public float progressifLighting;
    [Tooltip("Duration of the light (10 = 1s)")] public float durationOfLight;
    [Tooltip("Range Of Action Of the object light")] public float rangeOfAction;
}
