﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;        // Prefab of explosion effect.
	Network nobj;
	Socket clientSocket;
	byte[] sendBuff = new byte[1024];
	void Start () 
	{
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
		nobj = GameObject.FindGameObjectWithTag("networkobject").GetComponent<Network>();
		clientSocket = nobj.GetClientSock();
	}


	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		// If it hits an enemy...
		if(col.tag == "Enemy")
		{
			// ... find the Enemy script and call the Hurt function.
			col.gameObject.GetComponent<Enemy>().Hurt();

			// Call the explosion instantiation.
			OnExplode();

			// Destroy the rocket.
			Destroy (gameObject);
		}
		// Otherwise if it hits a bomb crate...
		else if(col.tag == "BombPickup")
		{
			// ... find the Bomb script and call the Explode function.
			col.gameObject.GetComponent<Bomb>().Explode();

			// Destroy the bomb crate.
			Destroy (col.transform.root.gameObject);

			// Destroy the rocket.
			Destroy (gameObject);
		}
		// Otherwise if the player manages to shoot himself...
		else if(col.gameObject.tag == "Player")
		{
			if (col.gameObject.GetComponent<PlayerHealth>().health > 0)
			{
				// Instantiate the explosion and destroy the rocket.
				col.gameObject.GetComponent<PlayerHealth>().health -= 10;
				col.gameObject.GetComponent<PlayerHealth>().UpdateHealthBar();
            }
            else
            {
				//sendBuff = System.Text.Encoding.Default.GetBytes("gameover " + nobj.userName);
				//clientSocket.Send(sendBuff);
				col.gameObject.GetComponent<Animator>().SetTrigger("Die");
				
			}
			OnExplode();
			Destroy (gameObject);
		}
		else if(col.gameObject.tag == "Player2")
        {
			if(col.gameObject.GetComponent<PlayerHealth2>().health <= 0)
            {
				col.gameObject.GetComponent<Animator>().SetTrigger("Die");
				sendBuff = System.Text.Encoding.Default.GetBytes("gameover " + nobj.userName);
				clientSocket.Send(sendBuff);
				
			}
			OnExplode();
			Destroy(gameObject);
        }
		else
		{
			// Instantiate the explosion and destroy the rocket.
			OnExplode();
			Destroy(gameObject);
		}
	}
}
