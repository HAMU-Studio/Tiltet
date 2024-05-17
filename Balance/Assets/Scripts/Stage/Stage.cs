using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    //�Œ肵����Y���̉�]
    public float FixedYRotation = 0f;

    // AddForce�Ŏg�p����͂̑傫��
    public Vector3 ForceDirection = new Vector3(0, 0, 10);
    public ForceMode forceMode = ForceMode.Force;

    private Rigidbody rb;

    void Start()
    {
        // Rigidbody�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();

        // null�`�F�b�N
        if (rb == null)
        {
            Debug.LogError("Rigidbody��������܂���B�X�N���v�g��K�؂ȃI�u�W�F�N�g�ɃA�^�b�`���Ă��������B");
        }
    }

    void Update()
    {
        // Update���ŗ͂�������i�����ł͖��t���[���͂��������j
        if (rb != null)
        {
            rb.AddForce(ForceDirection, forceMode);
        }

        // ���݂̉�]���擾
        Quaternion currentRotation = transform.rotation;

        // �I�C���[�p�ɕϊ�
        Vector3 euler = currentRotation.eulerAngles;

        // Y���̉�]���Œ�
        euler.y = FixedYRotation;

        // X����Z���̉�]�𐧌�
        euler.x = ClampAngle(euler.x, -30f, 30f);
        euler.z = ClampAngle(euler.z, -30f, 30f);

        // ��]���X�V
        transform.rotation = Quaternion.Euler(euler);
    }

    // �p�x���N�����v����w���p�[���\�b�h
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 180f)
        {
            angle = Mathf.Clamp(angle, min, max);
        }
        else
        {
            angle = Mathf.Clamp(angle, 360f + min, 360f + max);
        }
        return angle;
    }

}