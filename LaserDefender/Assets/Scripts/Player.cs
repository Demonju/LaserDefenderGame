using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    //makes the variable editable from Unity editor
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 15f;
    [SerializeField] float laserFiringTime = 0.2f;
    [SerializeField] float health = 200f;

    Coroutine firingCoroutine;

    float xMin, xMax, yMin, yMax;

    float padding = 0.5f;

    bool coroutineStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        //StartCoroutine(PrintAndWait());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private IEnumerator FireContinuously()
    {
        while(true) //while coroutine is running
        {
            //create an instant of laserPrefab at the position of the Player ship
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            //add a velocity to the alser in the y-axis
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            //wait x amount of seconds
            yield return new WaitForSeconds(laserFiringTime);
        }
    }

    ////coroutine example
    //private IEnumerator PrintAndWait()
    //{
    //    print("Bonswa");
    //    //wait 10 seconds
    //    yield return new WaitForSeconds(10);
    //    print("Ara ostra ghadek ma mietx :O");
    //    yield return new WaitForSeconds(10);
    //    print("O M G");
    //}

    //sets up the boundaries according to the camera
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        //xMin = 0 according to Camera view
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding; 
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).y - padding;
    }

    //if coroutine is started, do not start another 1

    private void Fire()
    {
        if (!coroutineStarted) //if coroutineStarted == false
        {
            //if fire button is pressed, start coroutine to fire
            if (Input.GetButtonDown("Fire1"))
            {
                firingCoroutine = StartCoroutine(FireContinuously());
                coroutineStarted = true;
            }
        }
        
        //if fire button is released, Stop Coroutine
        if(Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
            coroutineStarted = false;
        }
    }

    //moves the Player ship
    private void Move()
    {
        //var changes its variable type
        //depending on what i save it in
        //deltaX will have the difference in the x-axis which the player moves
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        //newXPos = current x-position     + difference in x
        var newXPos = transform.position.x + deltaX;
        //clamp the ship between xMin and xMax
        newXPos = Mathf.Clamp(newXPos, xMin, xMax);

        //the above in y axis:
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;
        var newYPos = transform.position.y + deltaY;
        //clamp the ship between yMin and yMax
        newYPos = Mathf.Clamp(newYPos, yMin, yMax);

        //move the Player ship to the newXPos
        this.transform.position = new Vector2(newXPos, newYPos);
    }

    //reduces health whenever the enemy collides with a gameObject
    //which has a DamageDealer component
    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        //access the DamageDealer class from "otherObject" which hits enemy
        //and reduces health accordingly
        DamageDealer dmgDealer = otherObject.gameObject.GetComponent<DamageDealer>();

        ProcessHit(dmgDealer);
    }

    //Whenever ProcessHit() is called, send us the DamageDealer details
    private void ProcessHit(DamageDealer dmgDealer)
    {
        health -= dmgDealer.GetDamage();
        //destroy enemy laser
        dmgDealer.Hit();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
