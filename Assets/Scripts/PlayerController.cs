using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    AudioVisualizer audioVisualizer;
    [SerializeField]
    Button disconnectedButton;

    void Awake()
    {
        audioVisualizer = FindObjectOfType<AudioVisualizer>();
        for (int i = 0; i < audioVisualizer.videoclips.Length; i++)
        {
            int number = i;
            audioVisualizer.songButtons[i].onClick.AddListener(() => CmdSetSong(number));
        }
    }

    private void OnEnable()
    {
        Button disconnectedButtonInstance = Instantiate(disconnectedButton);

        Button button = disconnectedButton.GetComponent<Button>();
        if (isClient)
        {
            button.onClick.AddListener(() => NetworkManager.singleton.StopClient());
            Debug.Log("Client");
        }
        else if (isServer)
        {
            button.onClick.AddListener(() => NetworkManager.singleton.StopServer());
            Debug.Log("Server");
        }

        disconnectedButtonInstance.GetComponent<RectTransform>().position = disconnectedButton.GetComponent<RectTransform>().position;

        disconnectedButtonInstance.name = "Disconnected Button";
        disconnectedButtonInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform.Find("Menu"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CmdSetSong(audioVisualizer.soundTrackNumber - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CmdSetSong(audioVisualizer.soundTrackNumber + 1);
        if (Input.GetKeyDown(KeyCode.P))
            CmdPause();
        if (Input.GetKeyDown("`"))
            CmdStop();
    }

    [Command]
    void CmdSetSong(int songNum)
    {
        audioVisualizer.SetSong(songNum);
        RpcSetSong(songNum);
    }
    [Command]
    void CmdPause()
    {
        if (audioVisualizer.videoPlayer.isPlaying)
            audioVisualizer.videoPlayer.Pause();
        else
            audioVisualizer.videoPlayer.Play();
        RpcPause();
    }
    [Command]
    void CmdStop()
    {
        audioVisualizer.Stop();
        RpcStop();
    }

    [ClientRpc]
    void RpcSetSong(int songNum)
    {
        audioVisualizer.SetSong(songNum);
    }
    [ClientRpc]
    void RpcPause()
    {
        if (audioVisualizer.videoPlayer.isPlaying)
            audioVisualizer.videoPlayer.Pause();
        else
            audioVisualizer.videoPlayer.Play();
    }
    [ClientRpc]
    void RpcStop()
    {
        audioVisualizer.Stop();
    }

    void OnDestroy()
    {
        for (int i = 0; i < audioVisualizer.videoclips.Length; i++)
        {
            int number = i;
            audioVisualizer.songButtons[i].onClick.RemoveListener(() => CmdSetSong(number));
        }
    }
}
