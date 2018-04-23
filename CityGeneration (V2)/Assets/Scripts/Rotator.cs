using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float speed;


    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
	}
}
