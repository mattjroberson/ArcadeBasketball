using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointScript : MonoBehaviour
{
    public bool validate;
    public TextMeshPro text;

    private void OnValidate()
    {
        text.text = name;
    }
}
