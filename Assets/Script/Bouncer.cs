using UnityEngine;
using System.Collections;

public class Bouncer : MonoBehaviour
{//Handles bouncing everywhere when dying

    public MonoBehaviour currentScript;
    public bool isHead = false;
    public new Rigidbody2D rigidbody2D;
    public new BoxCollider2D collider2D;

    void Start ()
    {
        ResetBounce();
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Bounce()
    {
        currentScript.enabled = false;
        rigidbody2D.isKinematic = false;
        collider2D.enabled = true;
        if(isHead)
            rigidbody2D.constraints = RigidbodyConstraints2D.None;

        rigidbody2D.AddForce(new Vector2(3, 3));
        rigidbody2D.AddTorque(40);
    }
    public void ResetBounce()
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0;
        currentScript.enabled = true;
        if (!isHead)
        {
            rigidbody2D.isKinematic = true;
            collider2D.enabled = false;
        }
        else
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
