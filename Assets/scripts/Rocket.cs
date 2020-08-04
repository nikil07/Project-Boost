using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour
{
    Rigidbody rocketRigitBody;
    AudioSource rocketAudioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] AudioClip mainEngineAudio;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip finishedAudio;

    [SerializeField] ParticleSystem engineParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] ParticleSystem finishedParticle;

    private int resetLevel = 0;

    enum State {Alive, Dead, Transcending}
    State state = State.Alive;

    bool isCollisionsEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rocketRigitBody = GetComponent<Rigidbody>();
        rocketAudioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        isCollisionsEnabled = !isCollisionsEnabled;
        processInput();
    }

    private void processInput()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        else {
            engineParticle.Stop();
        }

        respondToDebugKeys();
    }

    private void respondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            //isCollisionsEnabled = !isCollisionsEnabled;
            resetLevel = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !isCollisionsEnabled)
            return;
        handleCollision(collision);
    }

    private void handleCollision(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //print("You are safe");
                break;
            case "Finish":
                state = State.Transcending;
                rocketAudioSource.Stop();
                rocketAudioSource.PlayOneShot(finishedAudio);
                finishedParticle.Play();
                StartCoroutine(loadLevel(1));
                print("Finish");
                break;
            default:
                state = State.Dead;
                rocketAudioSource.Stop();
                rocketAudioSource.PlayOneShot(deathAudio);
                deathParticle.Play();
                Invoke("resetLevels", 2f);
                print("Dead");
                break;
        }
    }

    private void resetLevels() {
        SceneManager.LoadScene(resetLevel);
    }

    IEnumerator loadLevel(int level)
    {
        yield return new WaitForSeconds(2f);

        // Code to execute after the delay
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        print("current scene index , " + currentSceneIndex);
        if(currentSceneIndex <= SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(++currentSceneIndex);
    }

    private void Rotate()
    {

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        rocketRigitBody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            //print("A");
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //print("D");
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
            {
                rocketAudioSource.PlayOneShot(mainEngineAudio);
                engineParticle.Play();
            }
        }
        else
        {
            rocketAudioSource.Stop();
            engineParticle.Stop();
        }
    }
}
