using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    //�X�|�[���͈̓I�u�W�F�N�g
    [SerializeField] private GameObject minimumValue;
    [SerializeField] private GameObject muximumValueX;
    [SerializeField] private GameObject muximumValueZ;

    //�G���X�|�[������C���^�[�o��
    [SerializeField] private float spawnInterval = 3.0f;

    //��x�ɃX�|�[�����鐔
    [SerializeField] private int spawnNum = 1;

    private float spawnTime;

    //�X�|�[���͈̔�
    private float lowestPositionX;
    private float lowestPositionZ;
    private float highestPositionX;
    private float highestPositionZ;

    //�G�̐����m
    private GameObject[] enemies;
    //private int enemyNum;
    private bool ableSpawn;

    //�G�̃X�|�[���ꏊ
    Vector3 enemyPos = new Vector3();

    //�X�|�[���͈̓I�u�W�F�N�g�p
    Vector3 miniPos = new Vector3();
    Vector3 maxPosX = new Vector3();
    Vector3 maxPosZ = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        //�X�|�[���͈̓I�u�W�F�N�g�̃|�W�V�����擾
        miniPos = minimumValue.transform.position;
        maxPosX = muximumValueX.transform.position;
        maxPosZ = muximumValueZ.transform.position;

        //�X�|�[���͈͑��
        lowestPositionX = miniPos.x;
        lowestPositionZ = miniPos.z;
        highestPositionX = maxPosX.x;
        highestPositionZ = maxPosZ.z;

        //�����̎擾
        enemyPos.y = miniPos.y;

        // �Q�[�����n�܂����Ɠ����ɃX�|�[���i�Ȃ��Ă������j
        spawnTime = spawnInterval;

        ableSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;

        if (spawnTime > spawnInterval)
        {
            CheakEnemy();

            if (ableSpawn)
            {
                for (int i = 0; spawnNum > i; i++)
                {
                    EnemySpawn();
                }
            }

            spawnTime = 0;
        }
    }

    private void EnemySpawn()
    {
        GameObject newEnemy = Instantiate(enemy);

        enemyPos.x = Random.Range(lowestPositionX, highestPositionX);
        enemyPos.z = Random.Range(lowestPositionZ, highestPositionZ);
        //enemyPos = new Vector3(ramdomPositionX, 0, ramdomPositionZ);

        newEnemy.transform.position = enemyPos;
    }

    private void CheakEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //enemyNum = enemies.Length;

        if (2 < enemies.Length)
        {
            ableSpawn = false;
        }
        else
        {
            ableSpawn = true;
        }

    }

}