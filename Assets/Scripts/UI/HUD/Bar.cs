using System;
using UnityEngine;

public class Bar : MonoBehaviour
{
    private SwitchableSprite[] hps;

    private void Awake()
    {
        hps = GetComponentsInChildren<SwitchableSprite>();
    }

    public void Display(int heartsToDisplay)
    {
        for (int i = 0; i < heartsToDisplay; i++)
        {
            hps[i].Switch(1);
        }
        for (int i = heartsToDisplay; i < hps.Length; i++)
        {
            hps[i].Switch(0);
        }
    }
}