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

        // ��]���X�V
        transform.rotation = Quaternion.Euler(euler);
    }
}
