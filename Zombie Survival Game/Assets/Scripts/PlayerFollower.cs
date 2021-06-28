using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform Player;

    private float offset;

    private void Start()
    {
        offset = Player.transform.position.x - transform.position.x;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.position.x - offset, Player.position.y - offset, transform.position.z);
    }

}
