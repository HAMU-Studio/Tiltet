using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private GameObject[] players;
    private int count;
    private float[] distance;
    private GameObject target;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;

        players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        if (players.Length > 0)
        {
            distance = new float[players.Length];
        }

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        SearchPlayer();

        // targetがnullでないことを確認
        if (target != null)
        {
            agent.destination = target.transform.position;
        }
    }

    private void SearchPlayer()
    {
        if (players.Length < 2)
        {
            return;
        }

        foreach (GameObject player in players)
        {
            distance[count] = Vector3.Distance(this.transform.position, player.transform.position);

            if (count == 0)
            {
                count = 1;
            }
            else if (count == 1)
            {
                count = 0;
            }
        }

        if (distance[0] < distance[1])
        {
            target = players[0];
        }
        else if (distance[1] < distance[0])
        {
            target = players[1];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}