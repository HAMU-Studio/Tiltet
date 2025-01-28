using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Selected: MonoBehaviour
{
    [Header("このシーンにある選択するもの上から順に")]
    [SerializeField] private GameObject[] Select;

    [SerializeField] EventSystem eventSystem;

    private string selected;
    //private int nowSerect;
    //private var x;
    //private var y;

    // Start is called before the first frame update
    void Start()
    {
        //nowSerect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        selected = eventSystem.currentSelectedGameObject.gameObject.name;

        if (selected == "BGMVolume")
        {
            Select[0].SetActive(true);
            Select[1].SetActive(false);
            Select[2].SetActive(false);
        }
        else if (selected == "SEVolume")
        {
            Select[0].SetActive(false);
            Select[1].SetActive(true);
            Select[2].SetActive(false);
        }
        else if (selected == "Light")
        {
            Select[0].SetActive(false);
            Select[1].SetActive(false);
            Select[2].SetActive(true);
        }
        else
        {
            Select[0].SetActive(false);
            Select[1].SetActive(false);
            Select[2].SetActive(false);
        }
    }

    /*private void CheckStick()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        x = gamepad.leftStick.x.ReadValue();
        y = gamepad.leftStick.y.ReadValue();
        /*var up = gamepad.rightStick.up.ReadValue();
        var down = gamepad.rightStick.down.ReadValue();
        var left = gamepad.rightStick.left.ReadValue();
        var right = gamepad.rightStick.right.ReadValue();*/

        // 全ての入力をログ出力
        //Debug.Log($"x: {x}, y: {y}, up: {up}, down: {down}, left: {left}, right: {right}");
        //Debug.Log($"x: {x}, y: {y}");*/
}
