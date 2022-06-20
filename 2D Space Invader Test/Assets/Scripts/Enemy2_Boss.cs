using UnityEngine;

public class Enemy2_Boss : Enemy
{
    protected override void MoveToPosition() {
        Vector3 spawnPosition = transform.position;
        Vector3 targetPosition = targetBossSide;
        transform.position = Vector3.Lerp(spawnPosition, targetPosition, timeToTargetPosition);
    }

    protected override void Movement() {
        Vector3 pos1 = targetBossSide + new Vector3(transform.position.x - targetBossSide.x + 0.01f, 1f, 0);
        Vector3 pos2 = targetBossSide + new Vector3(transform.position.x - targetBossSide.x + 0.01f, -1f, 0);

        transform.position = Vector3.Lerp (pos2, pos1, Mathf.PingPong(Time.time * movementSpeed, 1.0f));
    }
}
