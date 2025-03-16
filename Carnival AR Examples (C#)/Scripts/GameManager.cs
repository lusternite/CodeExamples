using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public AudioSource[] BackGroundMusic;
    bool soundOn = true;
    string currentGame;

    int score;
    public bool[] PrizesObtained;

    void Start()
    {
        score = ReadScoreFromFile();
        ReadPrizesFromFile();

        if (!instance)
        {
            instance = this;
            print("Game Manager Created");
        }
        else
        {
            Destroy(this.gameObject);
            print("Destroyed duplicate");
        }

        //BackGroundMusic.loop = true;
        //BackGroundMusic.Play();
        BackGroundMusic = GetComponents<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetCurrentLevel(string level)
    {
        currentGame = level;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnApplicationQuit()
    {
        WriteToFile();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetText() != null)
        {
            GetText().text = score.ToString();
        }
    }

    public void ToggleSound()
    {
        if (BackGroundMusic[0].isPlaying)
        {
            BackGroundMusic[0].Stop();
            BackGroundMusic[1].Stop();
            soundOn = false;
        }
        else
        {
            BackGroundMusic[0].Play();
            BackGroundMusic[1].Play();
            soundOn = true;
        }
    }

    public void TogglePause()
    {
        GameObject PauseCanvas = gameObject.transform.GetChild(2).gameObject;
        PauseCanvas.gameObject.SetActive(!PauseCanvas.activeSelf);
        if (PauseCanvas.activeSelf == true)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        
    }

    public bool GetSoundOn()
    {
        return soundOn;
    }

    Text GetText()
    {
        GameObject canv = GameObject.Find("Tickets Text");
        if (canv == null)
            return null;
        else
            return canv.GetComponent<Text>();
    }

    public void SetScore(int i)
    {
        score += i;
    }

    public int GetScore()
    {
        Debug.Log(score);
        return score;
    }

    int ReadScoreFromFile()
    {
        int i;
        StreamReader theReader = new StreamReader(Application.dataPath + "\\Files\\score.txt");
        using (theReader)
        {
            i = int.Parse(theReader.ReadLine());
            theReader.Close();
        }

        return i;
    }

    void ReadPrizesFromFile()
    {
        int i;
        StreamReader theReader = new StreamReader(Application.dataPath + "\\Files\\prizes.txt");
        using (theReader)
        {
            for (int x = 0; x < PrizesObtained.Length; ++x)
            {
                i = int.Parse(theReader.ReadLine());
                if (i == 0)
                    PrizesObtained[x] = true;
                else
                    PrizesObtained[x] = false;
            }
            theReader.Close();
        }
        for (int x = 0; x < PrizesObtained.Length; ++x)
            Debug.Log(x.ToString() + ": " + PrizesObtained[x]);
    }

    public void WriteToFile()
    {
        var a = File.CreateText(Application.dataPath + "\\Files\\score.txt");
        a.WriteLine(score);
        a.Close();

        var b = File.CreateText(Application.dataPath + "\\Files\\prizes.txt");
        for (int x = 0; x < PrizesObtained.Length; ++x)
        {
            if (PrizesObtained[x] == true)
                b.WriteLine("0");
            else
                b.WriteLine("1");
        }

        b.Close();
    }
}
