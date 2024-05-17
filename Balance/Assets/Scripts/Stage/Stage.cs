using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // �Œ肵����Y���̉�]�p�x
    public float fixedYRotation = 0f;

    void Update()
    {
        // ���݂̉�]���擾
        Quaternion currentRotation = transform.rotation;

        // �I�C���[�p�ɕϊ�
        Vector3 euler = currentRotation.eulerAngles;

        // Y���̉�]���Œ�
        euler.y = fixedYRotation;

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
