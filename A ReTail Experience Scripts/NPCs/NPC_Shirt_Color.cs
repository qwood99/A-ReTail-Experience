using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Shirt_Color : MonoBehaviour
{
    void Awake()
    {
        ChangeColor();
    }

    void ChangeColor()
    {
        Color shirt_Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", shirt_Color);
    }

}
