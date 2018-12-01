using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    Rigidbody2D rigidBody2D;

    bool buttonPressed;
    float doublejumpCount;

    public Transform [] lineJoints;
   // public LineRenderer lineRenderer;
	public ParticleSystem particles;
    public float doubleJumpTime = 0.2f;
    public float jumpForce = 250;
    public float firstJumpForce = 350;
    public Vector2 jumpInstantVelocity = new Vector2(0, 1);

    //Rotation management
    float maxRotationAngle = 90;
    float maxVelocityRotation = 20;
    float rotationSpeed = 90;//rotation lerp speed

    public Bouncer headBouncer;
    public Bouncer bodyBouncer;
    public Bouncer tailBouncer;

    void Start ()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        if (GameManager.active.State == GameManager.GameState.RUNNING)
        {
            //debug
            //if (Input.GetKeyDown(KeyCode.Space))
            //    Die();
            
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
            {
                buttonPressed = true;
                doublejumpCount = 0;
                JumpFromZero();
            }
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
            {
                buttonPressed = false;
            }

            if (buttonPressed)
            {//double jump managing
                doublejumpCount += Time.deltaTime;

                if (doublejumpCount >= doubleJumpTime)
                {
                    Jump();
                    doublejumpCount = 0;
                }
            }

            UpdateRotation();
            //UpdateLineRenderer(); // Currently unused
        }
        else if (GameManager.active.State == GameManager.GameState.STARTING && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)))
        {
            GameManager.active.State = GameManager.GameState.RUNNING;
            FirstJump();
        }
    }
	void JumpFromZero()
	{
		rigidBody2D.velocity = jumpInstantVelocity;
		Jump();
    }
    void Jump()
    {
        rigidBody2D.AddForce(new Vector2(0, jumpForce));
		AudioManager.activeManager.PlayClipFromLibrary (3, transform.position, false);

        particles.Emit (5);
    }
    public void FirstJump()
    {
		rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.AddForce(new Vector2(0, firstJumpForce));
		AudioManager.activeManager.PlayClipFromLibrary (4, transform.position, false);
        //particles.Play();
    }
    void UpdateRotation()
    {//Update rotation
        float targetRotation = (rigidBody2D.velocity.y / maxVelocityRotation) * maxRotationAngle;
        if (targetRotation > maxRotationAngle)
            targetRotation = maxRotationAngle;
        else if (targetRotation < -maxRotationAngle)
            targetRotation = -maxRotationAngle;

        targetRotation = Mathf.Lerp(transform.rotation.y, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, targetRotation);
    }
    //void UpdateLineRenderer()
    //{
    //    for (int i = 0; i < lineJoints.Length; i++)
    //    {
    //        lineRenderer.SetPosition(i, lineJoints[i].position);
    //    }
    //}
    public void Die()
    {
		AudioManager.activeManager.PlayClipFromLibrary (2, transform.position, false);
        GameManager.active.State = GameManager.GameState.GAME_OVER;

        headBouncer.Bounce();
        bodyBouncer.Bounce();
        tailBouncer.Bounce();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.active.State == GameManager.GameState.RUNNING || GameManager.active.State == GameManager.GameState.STARTING)
        {
            if (collision.collider.tag == "Obstacle")
                Die();
            else
                AudioManager.activeManager.PlayClipFromLibrary(0, transform.position, false);
        }
    }

    public void Restart()
    {
        particles.Play();
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
        headBouncer.ResetBounce();
        bodyBouncer.ResetBounce();
        tailBouncer.ResetBounce();
    }
}
