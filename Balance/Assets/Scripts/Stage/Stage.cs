using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    //�Œ肵����Y���̉�]
    [SerializeField] float FixedYRotation = 0f;

    // AddForce�Ŏg�p����͂̑傫��
    private Vector3 m_forceDirection = new Vector3(0, 0, 10);
    private ForceMode m_forceMode = ForceMode.Force;

    private Rigidbody m_rb;

    void Start()
    {
        // Rigidbody�R���|�[�l���g���擾
        m_rb = GetComponent<Rigidbody>();

        // null�`�F�b�N
        if (m_rb == null)
        {
            Debug.LogError("Rigidbody��������܂���B�X�N���v�g��K�؂ȃI�u�W�F�N�g�ɃA�^�b�`���Ă��������B");
        }
    }

    void Update()
    {
        //�O���ŕʂ�Rigidbody���Q�Ƃ����Ƃ��ɔ����������h�~���邽�߂ɖ��t���[��Null�`�F�b�N���Ă���
        // Update���ŗ͂�������iForceMode��Force�ȊO�̏ꍇ�j
        if (m_rb != null && m_forceMode != ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
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

    void FixedUpdate()
    {
        // ForceMode��Force�̏ꍇ��FixedUpdate���ŗ͂�������
        if (m_rb != null && m_forceMode == ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
        }
    }

    //�O������m_forceDirection��m_forceMode���擾����ꍇ�Ɏg���A�N�Z�T���\�b�h�i���͎g�p���Ă��Ȃ��j
    // �͂̕����Ƒ傫����ݒ肷�郁�\�b�h
    public void SetForceDirection(Vector3 direction)
    {
        m_forceDirection = direction;
    }

    // �͂̕����Ƒ傫�����擾���郁�\�b�h
    public Vector3 GetForceDirection()
    {
        return m_forceDirection;
    }

    // ForceMode��ݒ肷�郁�\�b�h
    public void SetForceMode(ForceMode mode)
    {
        m_forceMode = mode;
    }

    // ForceMode���擾���郁�\�b�h
    public ForceMode GetForceMode()
    {
        return m_forceMode;
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