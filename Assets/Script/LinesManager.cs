using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LinesManager : MonoBehaviour {

    public event Action OnLineChanged;

    public Line [] lines;
    int currentLine = 0;
    public Line CurrentLine
    {
        get
        {
            if (currentLine >= 0)
                return lines[currentLine];
            else
                return null;
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        UpdateLines();
	}

    void UpdateLines()
    {
        for (int i = lines.Length-1; i >= 0; i--)
        {
            if (GameManager.active.player.transform.position.y >= lines[i].activationBottomPoint.position.y)
            {//if player is above, we can activate this line
                if (currentLine != i)
                {//if the found line is not the current one, it means that the player changed line!

                    lines[i].Toggle(true);

                    if (currentLine >= 0)
                        lines[currentLine].Toggle(false);

                    currentLine = i;

                    if (OnLineChanged != null)
                        OnLineChanged();
                }
                break;
            }
        }
    }
}
