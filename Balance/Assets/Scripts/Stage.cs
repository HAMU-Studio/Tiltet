using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // ŒÅ’è‚µ‚½‚¢Y²‚Ì‰ñ“]Šp“x
    public float fixedYRotation = 0f;

    void Update()
    {
        // Œ»İ‚Ì‰ñ“]‚ğæ“¾
        Quaternion currentRotation = transform.rotation;

        // ƒIƒCƒ‰[Šp‚É•ÏŠ·
        Vector3 euler = currentRotation.eulerAngles;

        // Y²‚Ì‰ñ“]‚ğŒÅ’è
        euler.y = fixedYRotation;

        // ‰ñ“]‚ğXV
        transform.rotation = Quaternion.Euler(euler);
    }
}
