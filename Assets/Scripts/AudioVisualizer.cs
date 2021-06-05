using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

public class AudioVisualizer : MonoBehaviour
{
    public enum SoundRespondType { rms, db }
    public VideoClip[] videoclips;
    public AudioClip[] audioClips;
    public AudioClip[] workoutPlaylist;
    bool isPlayingWorkoutPlaylist = false;
    public int soundTrackNumber;

    [SerializeField]
    int[] playlistTrackNumber;

    [SerializeField]
    GameObject changeSongButton;
    [SerializeField]
    GameObject workoutPlaylistButton;

    public bool playVideo = false;
    public VideoPlayer videoPlayer;
    AudioSource audioSource;

    public float maxVisualScale = 25.0f;
    public float visualModifier = 50.0f;
    public float smoothSpeed = 10.0f;
    public float keepPercentage = 0.5f;
    [HideInInspector]
    public Transform[] visualList;
    float[] visualScale;
    public int amnVisual = 10;

    public GameObject songButtonPrefab;

    public Text songBoard;

    const int sampleSize = 1024;
    float[] sampleRate = new float[sampleSize];
    float[] spectrum;
    float[] samles;

    private static float rmsValue;
    private static float dbValue;
    private static float foregroundValue;

    public static float RmsValue { get => rmsValue; set => rmsValue = value; }
    public static float DbValue { get => dbValue; set => dbValue = value; }
    public static float SmoothRmsValue { get => smoothRmsValue; set => smoothRmsValue = value; }
    public static float SmoothDbValue { get => smoothDbValue; set => smoothDbValue = value; }

    private static float smoothRmsValue;
    private static float smoothDbValue;
    private static float foregroundValueSmooth;

    private float brightness = 255;

    [SerializeField]
    GameObject menu;
    [SerializeField]
    GameObject menuButton;
    [SerializeField]
    GameObject songList;

    public Arduino arduino1;
    public Arduino arduino2;

    public Button[] songButtons;

    public static bool isMultiplayer = false;

    void Start()
    {
        samles = new float[1024];
        spectrum = new float[1024];

        audioSource = GetComponent<AudioSource>();

        playlistTrackNumber = new int[workoutPlaylist.Length];

        audioSource.clip = audioClips[0];
        songButtons = new Button[audioClips.Length];

        SpawnLine();

        for (int i = 0; i < audioClips.Length; i++)
        {
            GameObject go = Instantiate(songButtonPrefab, songList.transform);
            int buttonNo = i;
            go.name = audioClips[i].name;
            go.GetComponentInChildren<Text>().text = audioClips[i].name;
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() => SetSong(buttonNo));
            button.onClick.AddListener(() => CloseMenu());
            songButtons[buttonNo] = button;
        }
    }

    void Update()
    {
        AnalyzeSound();
        UpdateVisual();

        if (SmoothRmsValue < RmsValue)
            SmoothRmsValue = rmsValue;
        else
            SmoothRmsValue -= Time.deltaTime * 2;

        if (smoothDbValue < DbValue)
            SmoothDbValue = DbValue;
        else
            SmoothDbValue -= Time.deltaTime * 2;

        if (Input.GetKeyDown(KeyCode.P))
            Pause();
        /*
        if (isMultiplayer)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Prev();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                Next();
            if (Input.GetKeyDown(KeyCode.P))
                Pause();
            if (Input.GetKeyDown("`"))
                Stop();
        }*/

        int value1 = Mathf.RoundToInt(smoothRmsValue * 255);
        int value2 = Mathf.RoundToInt(SmoothDbValue * 255);
        
        float number = 0;
        for (int i = Mathf.RoundToInt(amnVisual / 4); i < amnVisual; i++)
        {
            number += visualList[i].localScale.y;
        }
        foregroundValue = Mathf.RoundToInt((number / Mathf.Round(amnVisual - amnVisual / 4) - 1) * 100);

        if (foregroundValue > 255)
        { foregroundValue = 255; }
        if (foregroundValue < 0)
        { foregroundValue = 0; }

        if (foregroundValueSmooth < foregroundValue)
            foregroundValueSmooth = foregroundValue;
        else
            foregroundValueSmooth -= Time.deltaTime * 1000;
        int value3 = Mathf.RoundToInt(foregroundValueSmooth);

        if (value3 < 0)
        { value3 = 0; }
        if(value3 > 255)
        { value3 = 255; }

        if (value1 > 255)
        { value1 = 255; }
        if(value1 < 0)
        { value1 = 0; }

        if (value2 > 255)
        { value2 = 255; }
        if (value2 < 0)
        { value2 = 0; }

        /*
        if (value1.ToString().Length >= 3)
            arduino1.Send(value1 + "" + brightness);
        if (value1.ToString().Length == 2)
            arduino1.Send(value1 + " " + brightness);
        if (value1.ToString().Length == 1)
            arduino1.Send(value1 + "  " + brightness);*/
        arduino1.Send(value2);

        if (value3.ToString().Length >= 3)
            arduino2.Send(value3 + "" + brightness);
        if (value3.ToString().Length == 2)
            arduino2.Send(value3 + " " + brightness);
        if (value3.ToString().Length == 1)
            arduino2.Send(value3 + "  " + brightness);
    }
    void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        float averageSize = (sampleSize * keepPercentage) / amnVisual;

        while (visualIndex < amnVisual)
        {
            int i = 0;
            float sum = 0;

            while (i < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                i++;
            }

            float scaleY = sum / averageSize * visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];

            visualIndex++;
        }
    }
    void SpawnLine()
    {
        GameObject audioSpectrum = GameObject.FindGameObjectWithTag("AudioSpectrum");

        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = new GameObject();
            go.AddComponent<Image>();
            go.transform.SetParent(audioSpectrum.transform);
            go.name = i.ToString();
            visualList[i] = go.transform;
            go.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / (float)amnVisual, 10);
            visualList[i].position = new Vector3(Screen.width / (float)amnVisual * i + (Screen.width / (float)amnVisual) / 2, Screen.height / 4);
            go.GetComponent<Image>().raycastTarget = false;
        }
    }
    void AnalyzeSound()
    {
        audioSource.GetOutputData(samles, 0);

        // Get RMS
        float sum = 0;
        for (int i = 0; i < sampleSize; i++)
        {
            sum += samles[i] * samles[i];
        }
        RmsValue = Mathf.Sqrt(sum / sampleSize) * 2;

        // Get the DB value
        DbValue = Mathf.Log10(RmsValue / 0.1f);

        // Get Sound Spectrum
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }
    /*
    void AnalyzeSound()
    {
        audioSource.GetOutputData(sampleRate, 0);
        float sum = 0;
        foreach (float x in sampleRate)
        {
            sum += x * x;
        }
        RmsValue = Mathf.Sqrt(sum / sampleSize) * 2;

        DbValue = Mathf.Log10(RmsValue / 0.1f);
    }*/

    public void Sort()
    {
        //videoclips = videoclips.OrderBy(soundTracks => soundTracks.name).ToArray();
        audioClips = audioClips.OrderBy(soundTracks => soundTracks.name).ToArray();
    }
    public void Shuffle()
    {/*
        VideoClip[] tmpArray = new VideoClip[videoclips.Length];

        for (int i = 0; i < videoclips.Length; i++)
        {
            int random = Random.Range(0, videoclips.Length);

            while (tmpArray[random] != null)
            {
                random = Random.Range(0, videoclips.Length);
            }
            tmpArray[random] = videoclips[i];
        }

        videoclips = tmpArray;*/

        AudioClip[] tmpArray = new AudioClip[audioClips.Length];

        for (int i = 0; i < audioClips.Length; i++)
        {
            int random = Random.Range(0, audioClips.Length);

            while (tmpArray[random] != null)
            {
                random = Random.Range(0, audioClips.Length);
            }
            tmpArray[random] = audioClips[i];
        }

        audioClips = tmpArray;
    }

    public void SetBrightness(float value)
    {
        brightness = value;
    }

    public void ChangePlayVideoOption()
    {
        if (!playVideo)
        {
            playVideo = true;
            videoPlayer.enabled = true;
        }
        else
        {
            playVideo = false;
            videoPlayer.enabled = false;
        }
    }

    public void SetSong(int songNumber)
    {
        Debug.Log("Song Set to: " + songNumber);
        if (playVideo)
        {
            videoPlayer.Stop();
            soundTrackNumber = songNumber;
            videoPlayer.clip = videoclips[soundTrackNumber];
            videoPlayer.Play();
            songBoard.text = videoclips[soundTrackNumber].name;
        }
        else
        {
            audioSource.Stop();
            soundTrackNumber = songNumber;
            audioSource.clip = audioClips[soundTrackNumber];
            audioSource.Play();
            songBoard.text = audioClips[soundTrackNumber].name;
        }
    }

    public void Pause()
    {
        if(playVideo)
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.Play();
        }
    }

    public void Next()
    {
        videoPlayer.Stop();
        if (soundTrackNumber >= videoclips.Length - 1)
            soundTrackNumber = 0;
        else
            soundTrackNumber++;
        videoPlayer.clip = videoclips[soundTrackNumber];
        videoPlayer.Play();
        songBoard.text = videoclips[soundTrackNumber].name;
    }

    public void Prev()
    {
        videoPlayer.Stop();
        if (soundTrackNumber <= 0)
            soundTrackNumber = videoclips.Length - 1;
        else
            soundTrackNumber--;
        videoPlayer.clip = videoclips[soundTrackNumber];
        videoPlayer.Play();
        songBoard.text = videoclips[soundTrackNumber].name;
    }

    public void Stop()
    {
        videoPlayer.Stop();
        songBoard.text = "";
    }

    public void CloseMenu()
    {

        menuButton.SetActive(true);
        menu.SetActive(false);
        songList.SetActive(false);
    }

    public void OpenMenu()
    {
        menuButton.SetActive(false);
        menu.SetActive(true);
        songList.SetActive(false);
    }
    public void WorkoutPlaylist()
    {
        if(!isPlayingWorkoutPlaylist)
        {
            changeSongButton.SetActive(false);
            workoutPlaylistButton.transform.GetChild(0).GetComponent<Text>().text = "Stop";

            

            for (int i = 0; i < playlistTrackNumber.Length; i++)
            {
                int randomNumber = Random.Range(0, workoutPlaylist.Length);

                while (true)//keep running this loop until the random number isn't taken
                {
                    bool taken = false;
                    foreach (int ptn in playlistTrackNumber)
                    {
                        if (ptn == randomNumber)
                        {
                            taken = true;
                            break;
                        }
                    }
                    if (taken)
                        randomNumber = Random.Range(0, workoutPlaylist.Length);
                    else
                        break;
                }

                playlistTrackNumber[i] = randomNumber;

            }
            foreach (int ptn in playlistTrackNumber)
            {
                Debug.Log(ptn);
            }

            menu.SetActive(false);
            menuButton.SetActive(true);

            isPlayingWorkoutPlaylist = true;
        }
        else
        {
            changeSongButton.SetActive(true);
            workoutPlaylistButton.transform.GetChild(0).GetComponent<Text>().text = "Workout Playlist";
            menu.SetActive(false);
            menuButton.SetActive(true);

            isPlayingWorkoutPlaylist = false;
        }
    }
    public void OpenSongList()
    {
        menuButton.SetActive(false);
        menu.SetActive(false);
        songList.SetActive(true);
    }
}
