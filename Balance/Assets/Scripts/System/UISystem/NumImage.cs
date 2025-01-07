using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace System
{
    [CreateAssetMenu(fileName = "NumImages", menuName = "ScriptableObjects/NumImages")]
    public class NumImage : ScriptableObject
    {
         public Sprite[] numbers;
    }
}