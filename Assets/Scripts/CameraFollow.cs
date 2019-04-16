using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float distance, height;
    
    void Update() { PlayerFollow();}

    void PlayerFollow() { transform.position = new Vector3(player.position.x, player.position.y + height, player.position.z - distance);}
}
