using UnityEngine;

public class Enemy2 : Enemy
{
    protected override void Movement() {
        Vector3 pos1 = target + new Vector3(transform.position.x - target.x - 0.01f, 1f, 0);
        Vector3 pos2 = target + new Vector3(transform.position.x - target.x - 0.01f, -1f, 0);

        transform.position = Vector3.Lerp (pos2, pos1, Mathf.PingPong(Time.time * movementSpeed, 1.0f));
    }
}
