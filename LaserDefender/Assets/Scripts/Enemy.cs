using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] float health = 500f;

    [SerializeField] float shotCounter;

    [SerializeField] float minTimeBetweenShots = 0.2f;

    [SerializeField] float maxTimeBetweenShots = 3f;

    [SerializeField] GameObject enemyLaserPrefab;

    [SerializeField] float enemyLaserSpeed = 0.3f;

    [SerializeField] AudioClip enemyDeathSound;
    [SerializeField] [Range(0, 1)] float enemyDeathSoundVolume = 0.75f;

    [SerializeField] AudioClip enemyShootSound;
    [SerializeField] [Range(0, 1)] float enemyShootSoundVolume = 0.2f;

    [SerializeField] GameObject deathVFX;
    [SerializeField] float explosionDuration = 1f;

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
        //destroy player laser
        dmgDealer.Hit();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        //play enemyDeathSound at the Camera position using the enemyDeathSoundVolume
        AudioSource.PlayClipAtPoint(enemyDeathSound, Camera.main.transform.position, enemyDeathSoundVolume);

        //create an eplosion particle
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);
    }

    void Start()
    {
        //generate a random number
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    private void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        // every frame reduce the amount of time for shot
        shotCounter -= Time.deltaTime;

        if (shotCounter <= 0f)
        {
            EnemyFire();
            //reset ShotCounter
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void EnemyFire()
    {
        //spawn an enemy laser and Enemy Position
        GameObject enemyLaser = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
        //shoot laser downwards: -enemyLaserSpeed
        enemyLaser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -enemyLaserSpeed);

        //play enemyShootSound at the Camera position using the enemyShootSoundVolume
        AudioSource.PlayClipAtPoint(enemyShootSound, Camera.main.transform.position, enemyShootSoundVolume);
    }
}
