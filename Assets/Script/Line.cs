using UnityEngine;
using System.Collections;
using System;

public class Line : MonoBehaviour {
    
    public Transform activationBottomPoint;
    public float scoreSpeedTick;
    public bool negativeScore;

    SpriteRenderer spriteRenderer;

    //[NonSerialized]
    public Color idleColor;
  //  [NonSerialized]
    public Color activeColor;

	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        idleColor = spriteRenderer.color;
        activeColor = new Color(idleColor.r * 1.5f, idleColor.g * 1.5f, idleColor.b * 1.5f);
	}
	
	void Update ()
    {
	}

    public void Toggle(bool isActive)
    {
        if (isActive)
        {
            spriteRenderer.color = activeColor;
        }
        else
        {
            spriteRenderer.color = idleColor;
        }
    }
}
