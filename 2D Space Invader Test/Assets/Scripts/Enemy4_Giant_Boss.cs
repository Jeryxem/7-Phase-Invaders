using UnityEngine;

public class Enemy4_Giant_Boss : Enemy
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 9) { // PowerUpEquipment Layer Number
            Death();
        }
        switch(other.gameObject.tag) {
            case "SuicideTrigger-Left":
                animator.Play("ChasePlayerGiant");
                break;
            case "SuicideTrigger-Right":
                animator.Play("ChaseBossGiant");
                break;
            case "Player":
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerController>().TakeDamage();
                break;
            case "Boss":
                Destroy(gameObject);
                other.gameObject.GetComponent<Boss>().TakeDamage();
                break;
            case "Projectile":
            case "Projectile-Boss":
                animator.Play("HurtBoss");
                break;
            default:
                break;
        }
    }
    
    protected override void MoveToPosition() {
        Vector3 spawnPosition = transform.position;
        Vector3 targetPosition = targetBossSide;
        transform.position = Vector3.Lerp(spawnPosition, targetPosition, timeToTargetPosition);
    }
    
    protected override void Movement() {
        transform.position += transform.right * Time.deltaTime * movementSpeed;
    }
}
