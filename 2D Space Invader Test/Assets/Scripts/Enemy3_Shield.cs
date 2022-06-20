using UnityEngine;

public class Enemy3_Shield : Enemy
{
    [field: SerializeField] public GameObject shield2 { get; set; }
    private float myTime = 0;

    protected override void Movement() {
        Vector3 pos1 = target + new Vector3(transform.position.x - target.x - 0.01f, 1f, 0);
        Vector3 pos2 = target + new Vector3(transform.position.x - target.x - 0.01f, -1f, 0);

        myTime += Time.deltaTime;

        transform.position = Vector3.Lerp (pos2, pos1, Mathf.PingPong(myTime * movementSpeed, 1.0f));
    }

    public override void RemoveShield() {
        shield.SetActive(false);
        shield2.SetActive(true);
    }
}
