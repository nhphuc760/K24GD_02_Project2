using Unity.Cinemachine;
using UnityEngine;

public class CinemachineFollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] CinemachineCamera cinemachineCamera;
    void Start()
    {
        cinemachineCamera??= GetComponent<CinemachineCamera>();
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
