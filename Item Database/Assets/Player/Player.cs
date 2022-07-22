using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private ForceMode mode;

    private Rigidbody body;

    private Vector3 direction;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Item>() != null)
            Destroy(other.gameObject);
    }

    private void Update()
    {
        direction.x = Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        body.AddForce(direction * speed, ForceMode.Force);
    }
}
