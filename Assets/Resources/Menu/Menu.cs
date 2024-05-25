using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string MenuName;
    public bool Open;

    public void ChangeState(bool state)
    {
        gameObject.SetActive(state);
        Open = state;
    }
}
