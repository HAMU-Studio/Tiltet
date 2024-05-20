using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.3f;

    private float[] distance;
    private GameObject[] players;
    private GameObject[] enemys;
    private GameObject target;
    private Vector3 enemyPosition;
    private Vector3 targetPosition;
    private Rigidbody Enemyrb;

    // Start is called before the first frame update
    void Start()
    {
        //player�̃^�O�����Ă���I�u�W�F�N�g����
        players = GameObject.FindGameObjectsWithTag("Player");

        // players�z��̒����Ɋ�Â���distance�z���������
        if (players.Length > 0)
        {
            distance = new float[players.Length];
        }

        Enemyrb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SearchPlayer();

        // target��null�łȂ����Ƃ��m�F
        if (target != null)
        {
            //�i�s����
            Vector3 Direction = (target.transform.position - transform.position).normalized;

            Enemyrb.AddForce(Direction * moveSpeed);
        }
    }

    private void SearchPlayer()
    {
        //player��1�l�����Ȃ���
        if (players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            return;
        }
        //player��1�l�̎�
        if (players.Length == 1)
        {
            target = players[0];
            return;
        }

        //�����𒲍�
        int count = 0;
        foreach (GameObject player in players)
        {
            distance[count] = Vector3.Distance(this.transform.position, player.transform.position);

            if (count == 0)
            {
                count = 1;
            }
           /* else if (count == 1)
            {
                count = 0;
            }*/
        }

        //�ǂ�����player�̂ق����߂���
        if (distance[0] < distance[1])
        {
            target = players[0];
            targetPosition = target.transform.position;
        }
        else if (distance[1] < distance[0])
        {
            target = players[1];
            targetPosition = target.transform.position;
        }
    }

    //DestroyArea�ɐG�ꂽ�������
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}