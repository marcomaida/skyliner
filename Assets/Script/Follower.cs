using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour
{
    public Transform targetObj;
    public Transform nextJoint;
    float lerpSpeed = 10;
    float nextJointDistance = 0.3f;
	void Start ()
    {
	
	}

    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, targetObj.position, lerpSpeed * Time.deltaTime);

        Vector3 diff = GameManager.active.player.transform.position - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        if (nextJoint != null)
            nextJoint.transform.position = Vector2.Lerp(nextJoint.transform.position, transform.position - new Vector3(nextJointDistance, 0, 0), lerpSpeed * 3 * Time.deltaTime);
    }
}
