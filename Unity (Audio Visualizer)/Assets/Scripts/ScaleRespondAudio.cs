using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleRespondAudio : MonoBehaviour {
    public bool x;
    public bool y;
    public bool z;

    [Space]
    public AudioVisualizer.SoundRespondType soundRespondType;
    
    [Space]
    public float X = 0;

    Vector3 orgVector;

    void Start()
    { orgVector = transform.localScale; }

    void Update()
    {
        transform.localScale = orgVector;
        if (soundRespondType == AudioVisualizer.SoundRespondType.rms)
        {

            if (x)
                transform.localScale = new Vector3(AudioVisualizer.SmoothRmsValue * X, transform.localScale.y, transform.localScale.z);
            if (y)
                transform.localScale = new Vector3(transform.localScale.x, AudioVisualizer.SmoothRmsValue * X, transform.localScale.z);
            if (z)
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, AudioVisualizer.SmoothRmsValue * X);
        }
        else if (soundRespondType == AudioVisualizer.SoundRespondType.db)
        {
            if (x)
                transform.localScale = new Vector3(AudioVisualizer.SmoothDbValue, transform.localScale.y, transform.localScale.z);
            if (y)
                transform.localScale = new Vector3(transform.localScale.x, AudioVisualizer.SmoothDbValue, transform.localScale.z);
            if (z)
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, AudioVisualizer.SmoothDbValue);
        }

        if (transform.localScale.x < 0)
            transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
        if (transform.localScale.y < 0)
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        if (transform.localScale.z < 0)
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0);
    }
}
