using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
