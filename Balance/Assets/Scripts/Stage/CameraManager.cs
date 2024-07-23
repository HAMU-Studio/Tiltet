using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera cmForward;
    public CinemachineVirtualCamera cmBackward;
    public CinemachineVirtualCamera cmLeft;
    public CinemachineVirtualCamera cmRight;
    public CinemachineVirtualCamera cmBattle;

    private StageMovement stageMovement;
    private CameraState currentCameraState;

    // カメラの状態を表すEnum
    private enum CameraState
    {
        Forward,
        Backward,
        Left,
        Right
    }

    void Start()
    {
        // StageMovementコンポーネントを取得
        stageMovement = GetComponent<StageMovement>();
        // 初期状態を前方とする
        currentCameraState = CameraState.Forward;
    }

    void Update()
    {
        // 現在の移動力を取得
        Vector3 movementForce = stageMovement.GetCurrentMovementForce();

        // カメラの優先度をリセット
        ResetCameraPriorities();

        // 移動方向に応じたカメラの状態を更新
        UpdateCameraState(movementForce);
    }

    // 全てのカメラの優先度をリセット
    void ResetCameraPriorities()
    {
        cmForward.Priority = 0;
        cmBackward.Priority = 0;
        cmLeft.Priority = 0;
        cmRight.Priority = 0;
    }

    // 移動方向に応じたカメラの状態を更新
    void UpdateCameraState(Vector3 movementForce)
    {
        CameraState newCameraState = currentCameraState;

        // 移動方向に基づいて新しいカメラ状態を決定
        if (movementForce.z > 0)
        {
            newCameraState = CameraState.Forward;
        }
        else if (movementForce.z < 0)
        {
            newCameraState = CameraState.Backward;
        }
        else if (movementForce.x > 0)
        {
            newCameraState = CameraState.Left;
        }
        else if (movementForce.x < 0)
        {
            newCameraState = CameraState.Right;
        }

        // 新しいカメラ状態が現在のカメラ状態と異なる場合、カメラを切り替える
        if (newCameraState != currentCameraState)
        {
            StartCoroutine(SwitchCamera(newCameraState));
        }
    }

    // 左回りでカメラを切り替えるコルーチン
    IEnumerator SwitchCamera(CameraState newCameraState)
    {
        switch (currentCameraState)
        {
            case CameraState.Forward:
                if (newCameraState == CameraState.Backward)
                {
                    // 前方から後方に切り替える場合、左回りで切り替える
                    cmLeft.Priority = 10;
                    yield return new WaitForSeconds(1.5f);
                    cmBackward.Priority = 10;
                }
                else
                {
                    SetCameraPriority(newCameraState);
                }
                break;
            case CameraState.Backward:
                if (newCameraState == CameraState.Forward)
                {
                    // 後方から前方に切り替える場合、左回りで切り替える
                    cmRight.Priority = 10;
                    yield return new WaitForSeconds(1.5f);
                    cmForward.Priority = 10;
                }
                else
                {
                    SetCameraPriority(newCameraState);
                }
                break;
            case CameraState.Left:
                if (newCameraState == CameraState.Right)
                {
                    // 左から右に切り替える場合、左回りで切り替える
                    cmForward.Priority = 10;
                    yield return new WaitForSeconds(1.5f);
                    cmRight.Priority = 10;
                }
                else
                {
                    SetCameraPriority(newCameraState);
                }
                break;
            case CameraState.Right:
                if (newCameraState == CameraState.Left)
                {
                    // 右から左に切り替える場合、左回りで切り替える
                    cmBackward.Priority = 10;
                    yield return new WaitForSeconds(1.5f);
                    cmLeft.Priority = 10;
                }
                else
                {
                    SetCameraPriority(newCameraState);
                }
                break;
        }

        // 新しいカメラ状態を現在のカメラ状態として設定
        currentCameraState = newCameraState;
    }

    // 指定されたカメラ状態に基づいてカメラの優先度を設定
    void SetCameraPriority(CameraState state)
    {
        switch (state)
        {
            case CameraState.Forward:
                cmForward.Priority = 10;
                break;
            case CameraState.Backward:
                cmBackward.Priority = 10;
                break;
            case CameraState.Left:
                cmLeft.Priority = 10;
                break;
            case CameraState.Right:
                cmRight.Priority = 10;
                break;
        }
    }

    // バトルカメラに切り替える関数
    public void BattleCamera()
    {
        // 全てのカメラの優先度をリセット
        ResetCameraPriorities();

        // バトルカメラの優先度を設定
        cmBattle.Priority = 10;
    }
}
