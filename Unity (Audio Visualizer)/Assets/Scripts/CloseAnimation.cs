using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAnimation : MonoBehaviour
{
    private void OnDisable()
    {
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return null;
        gameObject.SetActive(true);
    }
}