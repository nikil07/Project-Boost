using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRigitBody;
    AudioSource rocketAudioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;


    // Start is called before the first frame update
    void Start()
    {
        rocketRigitBody = GetComponent<Rigidbody>();
        rocketAudioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        processInput();
    }

    private void processInput()
    {
        Thrust();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag) {

            case "Friendly":
                print("You are safe");
                break;
            case "Finish":
                print("You are safe");
                break;
            default:
                print("KABOOM!!");
                break;
        }
    }

    private void Rotate()
    {

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        rocketRigitBody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            print("A");
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("D");
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rocketRigitBody.freezeRotation = false;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float rotationThisFrame = mainThrust * Time.deltaTime;
            rocketRigitBody.AddRelativeForce(Vector3.up * rotationThisFrame);
            if (!rocketAudioSource.isPlaying)
                rocketAudioSource.Play();
        }
        else
        {
            rocketAudioSource.Stop();
        }
    }
}
