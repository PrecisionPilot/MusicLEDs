using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorController : MonoBehaviour
{
    public int lifeTime;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void Update()
    {
        GetComponent<Text>().color = GetComponent<Text>().color - new Color(0, 0, 0, Time.deltaTime / lifeTime);
    }
}
