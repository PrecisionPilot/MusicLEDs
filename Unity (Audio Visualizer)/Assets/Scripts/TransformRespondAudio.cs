using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRespondAudio : MonoBehaviour {
    public Space space;
    [Space]
    public bool x;
    public bool y;
    public bool z;
    [Space]
    public bool rmsValue;
    public bool dbValue;
    public bool pitchValue;

    [Space]
    public float X = 0;

    Vector3 orgVector;

    void Start()
    { orgVector = transform.localPosition; }

    void Update () {
        if(space == Space.Self)
        {
            if (rmsValue)
            {
                if (x)
                    transform.position = orgVector + AudioVisualizer.RmsValue * transform.right * X;
                else if (y)
                    transform.position = orgVector + AudioVisualizer.RmsValue * transform.up * X;
                else if (z)
                    transform.position = orgVector + AudioVisualizer.RmsValue * transform.forward * X;
            }
            else if (dbValue)
            {
                if (x)
                    transform.localPosition = orgVector + AudioVisualizer.DbValue * transform.right * X;
                else if (y)
                    transform.localPosition = orgVector + AudioVisualizer.DbValue * transform.up * X;
                else if (z)
                    transform.localPosition = orgVector + AudioVisualizer.DbValue * transform.forward * X;
            }
        }
        else
        {
            if (rmsValue)
            {
                if (x)
                    transform.localPosition = new Vector3(AudioVisualizer.RmsValue * X + orgVector.x, orgVector.y, orgVector.z);
                else if (y)
                    transform.localPosition = new Vector3(orgVector.x, AudioVisualizer.RmsValue * X + orgVector.y, orgVector.z);
                else if (z)
                    transform.localPosition = new Vector3(orgVector.x, orgVector.y, AudioVisualizer.RmsValue * X + orgVector.z);
            }
            else if (dbValue)
            {
                if (x)
                    transform.localPosition = new Vector3(AudioVisualizer.DbValue * X + orgVector.x, orgVector.y, orgVector.z);
                else if (y)
                    transform.localPosition = new Vector3(orgVector.x, AudioVisualizer.DbValue * X + orgVector.y, orgVector.z);
                else if (z)
                    transform.localPosition = new Vector3(orgVector.x, orgVector.y, AudioVisualizer.DbValue * X + orgVector.z);
            }
        }
    }
}
