using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPScontroller : MonoBehaviour {

    Text fpsText;
    public float delay = 1.0f;

    void Start () {
        fpsText = GetComponent<Text>();
        StartCoroutine(FPSupdate());
    }
    IEnumerator FPSupdate()
    {
        yield return new WaitForSeconds(delay);
        float fps = Mathf.Round(1 / Time.deltaTime);
        fpsText.text = fps.ToString();

        if (fps >= 40) { fpsText.color = Color.green; }
        else
        { fpsText.color = Color.yellow; }

        StartCoroutine(FPSupdate());
    }
}
