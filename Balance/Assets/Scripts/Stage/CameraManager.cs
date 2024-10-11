using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera cmForward;
    public CinemachineVirtualCamera cmBackward;
    public CinemachineVirtualCamera cmLeft;
    public CinemachineVirtualCamera cmRight;
    public CinemachineVirtualCamera cmBattle;

    private CMSwitch activeSwitch; // アクティブなスイッチを参照する
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
        // 初期状態を前方とする
        currentCameraState = CameraState.Forward;
    }

    void Update()
    {
        // アクティブなスイッチがあるか確認
        if (CMSwitch.activeSwitch != null)
        {
            activeSwitch = CMSwitch.activeSwitch;

            // スイッチがオンの場合にカメラを切り替える
            if (activeSwitch.IsSwitchOn)
            {
                ResetCameraPriorities();
                SwitchCameraBySwitchType(activeSwitch.switchType);
            }
        }
    }

    // 全てのカメラの優先度をリセット
    void ResetCameraPriorities()
    {
        cmForward.Priority = 0;
        cmBackward.Priority = 0;
        cmLeft.Priority = 0;
        cmRight.Priority = 0;
    }

    // スイッチのタイプに応じてカメラを切り替える
    void SwitchCameraBySwitchType(CMSwitch.SwitchType switchType)
    {
        CameraState targetCameraState = currentCameraState;

        // スイッチタイプに基づいて目標カメラ状態を設定
        switch (switchType)
        {
            case CMSwitch.SwitchType.Front:
                targetCameraState = CameraState.Forward;
                break;
            case CMSwitch.SwitchType.Back:
                targetCameraState = CameraState.Backward;
                break;
            case CMSwitch.SwitchType.Left:
                targetCameraState = CameraState.Left;
                break;
            case CMSwitch.SwitchType.Right:
                targetCameraState = CameraState.Right;
                break;
        }

        // カメラ状態が変更されていた場合、カメラを切り替える
        if (targetCameraState != currentCameraState)
        {
            StartCoroutine(SwitchCamera(targetCameraState));
        }
    }

    // カメラを直接切り替えるメソッド
    IEnumerator SwitchCamera(CameraState targetCameraState)
    {
        // 反対側のカメラへ切り替える場合は、右回りでカメラを経由する
        if (IsOppositeSide(currentCameraState, targetCameraState))
        {
            yield return StartCoroutine(ClockwiseCamera(targetCameraState));
        }
        else
        {
            // 直接切り替える場合
            SetCameraPriority(targetCameraState);
            currentCameraState = targetCameraState;
        }
    }

    // カメラが反対側かどうかを判定するメソッド
    bool IsOppositeSide(CameraState fromState, CameraState toState)
    {
        // 前と後ろ、左と右が反対側
        return (fromState == CameraState.Forward && toState == CameraState.Backward) ||
               (fromState == CameraState.Backward && toState == CameraState.Forward) ||
               (fromState == CameraState.Left && toState == CameraState.Right) ||
               (fromState == CameraState.Right && toState == CameraState.Left);
    }

    // カメラを右回りで順番に切り替えるコルーチン
    IEnumerator ClockwiseCamera(CameraState targetCameraState)
    {
        // カメラを右回りで順番に経由して切り替える
        while (currentCameraState != targetCameraState)
        {
            switch (currentCameraState)
            {
                case CameraState.Forward: //前方のカメラの場合
                    cmRight.Priority = 10; // 右カメラをアクティブ
                    yield return new WaitForSeconds(1.7f); // 待機時間
                    cmForward.Priority = 0; // 前方カメラの優先度をリセット
                    currentCameraState = CameraState.Right; // 現在の状態を右に設定
                    break;
                case CameraState.Right:
                    cmBackward.Priority = 10;
                    yield return new WaitForSeconds(1.7f);
                    cmRight.Priority = 0;
                    currentCameraState = CameraState.Backward;
                    break;
                case CameraState.Backward:
                    cmLeft.Priority = 10;
                    yield return new WaitForSeconds(1.7f);
                    cmBackward.Priority = 0;
                    currentCameraState = CameraState.Left;
                    break;
                case CameraState.Left:
                    cmForward.Priority = 10;
                    yield return new WaitForSeconds(1.7f);
                    cmLeft.Priority = 0;
                    currentCameraState = CameraState.Forward;
                    break;
            }
        }

        // 最後に目標カメラをアクティブに設定
        SetCameraPriority(targetCameraState);
    }

    // 最短距離でカメラを切り替える
    void SetCameraPriority(CameraState state)
    {
        ResetCameraPriorities(); // 優先度をリセット

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
