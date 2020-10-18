using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable 0414,0649

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxHurt;
    [SerializeField] public int value = 1;
    [SerializeField] public int health = 1;



public void Damage(int hits)
    {
        if (health > 0)
        {
            health -= hits;
            if (health <= 0)
                Die();
            else
                GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxHurt);
        }
    }


    public void Die()
    {
        GameManager.instance.AddScore(value);
        GetComponent<NavMeshAgent>().enabled = false;
        float rotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(90, rotation, 0);
        transform.Translate(0, -1f, 0, Space.World);
        GameManager.instance.GetComponent<AudioSource>().PlayOneShot(sfxDeath);
    }
}

