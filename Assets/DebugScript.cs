using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    bool show = false;

    void Start()
    {
        this.gameObject.SetActive(show);
    }
}
