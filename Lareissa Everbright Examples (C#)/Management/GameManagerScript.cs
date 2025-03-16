using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public enum EEnemyBattleScenario
{
    eKobolds,
    eGiantWolfSpiderSwarm,
    eHarpyHuntresses,
    eCentipedeDemon,
    eSkeletons,
    eElementalSkylarks,
    eAurochs,
    eIronborne,
    eSwampSquad,
    eSahuagin,
    eWraith,
    eUmbralMass,
    eVermillionKnight,
    eGrimDivine
}

public enum ESceneType
{
    eTitle,
    eNarrative,
    eSortie,
    eCombat,
    eDemoCredits,
    eCredits
}



public class GameManagerScript : MonoBehaviour {

    //**~~~~~~~~VARIABLES~~~~~~~~**//
    
    public EEnemyBattleScenario currentEnemyBattleScenario = EEnemyBattleScenario.eGiantWolfSpiderSwarm;

    public ESceneType currentScene = ESceneType.eNarrative;

    CombatManagerScript combatManagerReference;

    SortieManagerScript sortieManagerReference;

    NarrativeManagerScript narrativeManagerReference;

    AudioManagerScript audioManagerReference;

    PlayerManagerScript playerManagerReference;

    public GameObject loadingScreenPrefab;

    private GameObject loadingScreenReference;

    public GameObject narrativeSkipPrefab;

    private float elapsedLoadTime;

    public float minLoadTime = 0.5f;

    public bool shouldSaveData = false;

    public bool specifiedEncounterFlag = false;

    public bool comfyModeFlag = false;

    private bool demoFlag = false;

    private EEnemyBattleScenario specifiedEncounterScenario = EEnemyBattleScenario.eKobolds;

    public int currentAct = 1;

    public int currentChapter = 3;

    [Header("Background sprite references")]
    public Sprite forestNightBattleSprite;
    public Sprite plainsTravelSprite;
    public Sprite plainsBattleSprite;
    public Sprite plainsNightBattleSprite;
    public Sprite swampTravelSprite;
    public Sprite swampBattleSprite;
    public Sprite fortSprite;

    private List<Steamworks.Data.Achievement> achievements;

    //**~~~~~~~~FUNCTIONS~~~~~~~~**//

    private void Awake()
    {
        if (FindObjectsOfType<GameManagerScript>().Length > 1)
        {
            Destroy(gameObject);
        }

        else
        {
            // Do steamworks stuff
            try
            {
                Steamworks.SteamClient.Init(1363170, true);
            }
            catch (System.Exception e)
            {
                print(e.ToString());
            }
            Steamworks.SteamUserStats.OnAchievementProgress += AchievementChanged;
        }
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);

        combatManagerReference = GetComponent<CombatManagerScript>();
        sortieManagerReference = GetComponent<SortieManagerScript>();
        narrativeManagerReference = GetComponent<NarrativeManagerScript>();
        audioManagerReference = GetComponent<AudioManagerScript>();
        playerManagerReference = GetComponent<PlayerManagerScript>();

        if (currentScene == ESceneType.eNarrative)
        {
            Invoke("InitialiseNarrativeScene", 0.1f);
            Invoke("ChangeTravelBackgroundFromAct", 0.05f);
        }
        else if (currentScene == ESceneType.eSortie)
        {
            Invoke("InitialiseSortieScene", 0.1f);
        }
        else if (currentScene == ESceneType.eCombat)
        {
            Invoke("InitialiseCombatScene", 0.1f);
        }
        else if (currentScene == ESceneType.eTitle)
        {
            InitialiseTitleScene();
        }

        // Load player preferences if they exist
        LoadPlayerPreferences();

        if (Steamworks.SteamClient.IsValid)
        {
            print("Steam is working for " + Steamworks.SteamClient.Name + ", ID: " + Steamworks.SteamClient.SteamId);

            GetComponent<AchievementManagerScript>().InitialiseAchievementManager();

            //GetComponent<AchievementManagerScript>().ResetAllStatsAndAchievements();

            //print("Steam Cloud Enabled: " + Steamworks.SteamRemoteStorage.IsCloudEnabled);

            //Steamworks.SteamUserStats.OnUserStatsReceived += AchievementTest;
            //print(Steamworks.SteamUserStats.RequestCurrentStats());

            //Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_BROKEN_EQUIP", 99, 100);

            //achievements = new List<Steamworks.Data.Achievement>();

            //foreach( Steamworks.Data.Achievement ach in Steamworks.SteamUserStats.Achievements)
            //{
            //    achievements.Add(ach);
            //}

            //print(achievements.Count);
            //print(achievements[0].Clear())
            //    ;

            //Invoke("StatSetUp", 5.5f);

            //Invoke("AchievementCheat", 6.0f);
            GetComponent<AchievementManagerScript>().CheckStats();
        }
    }

    public void AchievementTest(Steamworks.SteamId steamId, Steamworks.Result result)
    {
        // Set achievement
        Steamworks.SteamServerStats.SetAchievement(steamId, "ACH_BROKEN_EQUIP");
        Steamworks.SteamUserStats.StoreStats();
        print("ACHIEVEMENT SET");
    }

    private void AchievementChanged(Steamworks.Data.Achievement ach, int currentProgress, int progress)
    {
        if (ach.State)
        {
            Debug.Log($"{ach.Name} WAS UNLOCKED!");
        }
    }

    private void StatSetUp()
    {
        print("Stats set up status: " + RequestStats());
    }

    private bool RequestStats()
    {
        // Is the user logged on?  If not we can't get stats.
        if (!Steamworks.SteamClient.IsLoggedOn)
        {
            return false;
        }
        // Request user stats.
        return Steamworks.SteamUserStats.RequestCurrentStats();
    }

    // Update is called once per frame
    void Update () {

		// Listen for input when in demo credits scene
        if (currentScene == ESceneType.eDemoCredits)
        {
            if (Input.anyKeyDown)
            {
                // Return to title on input
                LoadTitleScene();
            }
        }

        // Run steamworks callbacks
        Steamworks.SteamClient.RunCallbacks();
	}

    public void LoadCombatScene()
    {
        // Start loading coroutine

        StartCoroutine(HandleLoadCombatScene());
    }

    private IEnumerator HandleLoadCombatScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;

        // Reset variables
        combatManagerReference.ResetCombatVariables();

        audioManagerReference.StopBGM();

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("testCombat");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        InitialiseCombatScene();

        ChangeCombatBackgroundFromAct();

        // Set the current scene
        currentScene = ESceneType.eCombat;

        yield return null;
    }

    public void LoadSortieScene()
    {
        // Start coroutine

        StartCoroutine(HandleLoadSortieScene());
    }

    private IEnumerator HandleLoadSortieScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;
        
        // Reset variables
        combatManagerReference.ResetCombatVariables();
        playerManagerReference.ResetCombatVariables();
        narrativeManagerReference.ResetNarrativeVariables();

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eSortie;

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("testEquipmentSelection");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Wait an extra frame for animations
        yield return new WaitForEndOfFrame();

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        InitialiseSortieScene();

        yield return null;
    }

    public void LoadNarrativeScene()
    {
        // Start loading coroutine

        StartCoroutine(HandleLoadNarrativeScene());
    }

    private IEnumerator HandleLoadNarrativeScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;

        // Reset variables
        narrativeManagerReference.ResetNarrativeVariables();

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eNarrative;

        // Check if there is a specified encounter
        if (specifiedEncounterFlag == true)
        {
            currentEnemyBattleScenario = specifiedEncounterScenario;
            DetermineCurrentNarrativeScenario();
            specifiedEncounterFlag = false;
        }

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("testNarrative");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        InitialiseNarrativeScene();
        yield return new WaitForEndOfFrame();
        elapsedLoadTime += Time.deltaTime;

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        ChangeTravelBackgroundAndMusicFromAct();

        SpawnNarrativeSkipButton();

        narrativeManagerReference.HandleParagraphSpriteChanges();

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        // Fade in the travel sprite
        yield return new WaitForSeconds(narrativeManagerReference.RevealTravellingSprite());

        // Start narration
        narrativeManagerReference.DisplayParagraph();

        yield return null;
    }

    public void LoadTitleScene()
    {
        // Start loading coroutine

        StartCoroutine(HandleLoadTitleScene());
    }

    private IEnumerator HandleLoadTitleScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;

        // Reset variables
        combatManagerReference.ResetCombatVariables();
        playerManagerReference.ResetCombatVariables();
        narrativeManagerReference.ResetNarrativeVariables();

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eTitle;

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("Title");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        InitialiseTitleScene();

        yield return null;
    }

    public void LoadTitleSceneSeamless()
    {
        StartCoroutine("HandleLoadTitleSceneSeamless");
    }

    private IEnumerator HandleLoadTitleSceneSeamless()
    {
        // Reset variables
        combatManagerReference.ResetCombatVariables();
        playerManagerReference.ResetCombatVariables();

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eTitle;

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("Title");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        InitialiseTitleScene();

        yield return null;
    }

    public void LoadDemoCreditsScene()
    {
        // Start the coroutine
        StartCoroutine(HandleLoadDemoCreditsScene());
    }

    private IEnumerator HandleLoadDemoCreditsScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eDemoCredits;

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("DemoCredits");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        InitialiseDemoCreditsScene();

        yield return null;
    }

    public void LoadEndingCreditsScene()
    {
        // Start the coroutine
        StartCoroutine(HandleLoadEndingCreditsScene());
    }

    private IEnumerator HandleLoadEndingCreditsScene()
    {
        // Reset load time
        elapsedLoadTime = 0.0f;

        // Reset variables
        narrativeManagerReference.ResetNarrativeVariables();

        audioManagerReference.StopBGM();

        // Set the current scene
        currentScene = ESceneType.eCredits;

        // Spawn loading screen
        SpawnLoadingScreen();

        // Start async scene transition
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("EndingCredits");

        // Wait until scene is loaded
        while (loadScene.progress != 1)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        // Check that elapsed load time is at least above minimum
        while (elapsedLoadTime < minLoadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedLoadTime += Time.deltaTime;
        }

        InitialiseEndingCreditsScene();

        // Destroy loading screen
        Destroy(loadingScreenReference);
        loadingScreenReference = null;

        // Start narration
        narrativeManagerReference.DisplayParagraph();

        yield return null;
    }

    private void SpawnLoadingScreen()
    {
        loadingScreenReference = Instantiate(loadingScreenPrefab);

        DontDestroyOnLoad(loadingScreenReference);
    }

    // Called at the end of combat to progress the game
    public void HandleCombatVictory()
    {
        // See if this is the final chapter of the act
        if (currentChapter == 4)
        {
            // Check if demo
            if (demoFlag == true)
            {
                // Set to demo end screen
                currentScene = ESceneType.eDemoCredits;
            }
            else
            {
                currentChapter = 1;
                currentAct += 1;
            }
        }
        // Check if act 4
        else if (currentAct == 4)
        {
            // Check which chapter
            if (currentChapter == 1)
            {
                // Deal with determining if an ending is achieved or continue to the next fight.
                if (narrativeManagerReference.routeA == true)
                {
                    // See if good or bad ending is achieved
                    if (playerManagerReference.CombatInventoryContains("Everlustre"))
                    {
                        // BAD END
                        currentAct = 5;
                        currentChapter = 1;
                    }
                    else
                    {
                        // GOOD END
                        currentAct = 5;
                        currentChapter = 2;
                    }
                }
                else
                {
                    // True route, continue to next chapter
                    currentChapter += 1;
                }
            }
            else
            {
                // Get the true ending.
                currentAct = 5;
                currentChapter = 3;
            }
        }
        else
        {
            currentChapter += 1;
        }

        // Check if should go to credits
        if (currentScene == ESceneType.eDemoCredits)
        {
            LoadDemoCreditsScene();
        }
        else if (currentAct == 5)
        {
            LoadEndingCreditsScene();
        }
        // Otherwise continue as normal
        else
        {
            // Save current player equipment info
            playerManagerReference.SaveCurrentEquipmentData();

            // See if should save
            if (shouldSaveData)
            {
                SaveGameData();
            }

            // Change the enemy combat scenario to match the stage
            DetermineCurrentCombatScenario();

            // Load the narrative scene
            LoadNarrativeScene();
        }
    }

    private void InitialiseCombatScene()
    {
        combatManagerReference.InitialiseCombatScene();
        combatManagerReference.CommenceCombatSimulation();
        if (currentChapter == 4)
        {
            audioManagerReference.PlayCombatMusic(true);
            combatManagerReference.StartEntityIdleCoroutine(160.0f);
        }
        else
        {
            if (currentAct == 4)
            {
                if (currentChapter == 1)
                {
                    // Vermilion Knight
                    audioManagerReference.PlayVermilionKnightMusic();

                    combatManagerReference.StartEntityIdleCoroutine(160.0f);
                }
                else
                {
                    // Grim Divine
                    audioManagerReference.PlayGrimDivineMusic();

                    combatManagerReference.StartEntityIdleCoroutine(160.0f);
                }
            }
            else
            {
                audioManagerReference.PlayCombatMusic(false);
                combatManagerReference.StartEntityIdleCoroutine(145.0f);
            }
        }

        //GameObject.Find("SaveTest").GetComponent<Button>().onClick.AddListener(SaveGameData);
    }

    private void InitialiseSortieScene()
    {
        sortieManagerReference.SpawnPlayerEquipmentInventory();

        audioManagerReference.PlaySortieMusic();
    }

    private void InitialiseNarrativeScene()
    {
        narrativeManagerReference.InitialiseNarrativeScene(GetCurrentStageAsString());
    }

    private void InitialiseTitleScene()
    {
        // Link start and continue buttons with the appropriate functions
        GameObject.Find("New Game").GetComponent<Button>().onClick.AddListener(HandleNewGame);
        GameObject.Find("Continue").GetComponent<Button>().onClick.AddListener(HandleContinueGame);
        GameObject.Find("Quit").GetComponent<Button>().onClick.AddListener(QuitGame);

        // Check if this is demo to set appropriate scene
        if (demoFlag == true)
        {
            currentAct = 2;
            currentChapter = 1;
            DetermineCurrentCombatScenario();
        }

        audioManagerReference.PlayTitleBGM();
    }

    private void InitialiseDemoCreditsScene()
    {
        // Start playing demo credits music
        audioManagerReference.PlayForestBGM();


    }

    private void InitialiseEndingCreditsScene()
    {
        narrativeManagerReference.InitialiseNarrativeScene(GetCurrentStageAsString());
    }

    private void DetermineCurrentCombatScenario()
    {
        // ACT 1 ENCOUNTERS
        if (currentAct == 1)
        {
            if (currentChapter == 1)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eKobolds;
            }
            else if (currentChapter == 2)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eGiantWolfSpiderSwarm;
            }
            else if (currentChapter == 3)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eHarpyHuntresses;
            }
            else if (currentChapter == 4)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eCentipedeDemon;
            }
        }
        // ACT 2 ENCOUNTERS
        else if (currentAct == 2)
        {
            if (currentChapter == 1)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eSkeletons;
            }
            else if (currentChapter == 2)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eElementalSkylarks;
            }
            else if (currentChapter == 3)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eAurochs;
            }
            else if (currentChapter == 4)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eIronborne;
            }
        }
        // ACT 3 ENCOUNTERS
        else if (currentAct == 3)
        {
            if (currentChapter == 1)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eSwampSquad;
            }
            else if (currentChapter == 2)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eSahuagin;
            }
            else if (currentChapter == 3)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eWraith;
            }
            else if (currentChapter == 4)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eUmbralMass;
            }
        }
        // ACT 4 ENCOUNTERS
        else if (currentAct == 4)
        {
            if (currentChapter == 1)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eVermillionKnight;
            }
            else if (currentChapter == 2)
            {
                currentEnemyBattleScenario = EEnemyBattleScenario.eGrimDivine;
            }
        }
    }

    private void DetermineCurrentNarrativeScenario()
    {
        if (currentEnemyBattleScenario == EEnemyBattleScenario.eKobolds)
        {
            currentAct = 1;
            currentChapter = 1;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eGiantWolfSpiderSwarm)
        {
            currentAct = 1;
            currentChapter = 2;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eHarpyHuntresses)
        {
            currentAct = 1;
            currentChapter = 3;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eCentipedeDemon)
        {
            currentAct = 1;
            currentChapter = 4;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSkeletons)
        {
            currentAct = 2;
            currentChapter = 1;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eElementalSkylarks)
        {
            currentAct = 2;
            currentChapter = 2;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eAurochs)
        {
            currentAct = 2;
            currentChapter = 3;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eIronborne)
        {
            currentAct = 2;
            currentChapter = 4;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSwampSquad)
        {
            currentAct = 3;
            currentChapter = 1;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSahuagin)
        {
            currentAct = 3;
            currentChapter = 2;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eWraith)
        {
            currentAct = 3;
            currentChapter = 3;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eUmbralMass)
        {
            currentAct = 3;
            currentChapter = 4;
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eVermillionKnight)
        {
            currentAct = 4;
            currentChapter = 1;
        }
    }

    private void ChangeTravelBackgroundAndMusicFromAct()
    {
        // Forest
        if (currentAct == 1)
        {
            // Change music
            audioManagerReference.PlayForestBGM();

            // Also change gwen's position
            GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 140.0f);
        }
        // Plains
        else if (currentAct == 2)
        {
            GameObject.Find("Background").GetComponent<Image>().sprite = plainsTravelSprite;

            // Also change gwen's position
            GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 36.0f);

            // Change music
            audioManagerReference.PlayPlainsBGM();
        }
        // Swamp
        else if (currentAct == 3)
        {
            GameObject.Find("Background").GetComponent<Image>().sprite = swampTravelSprite;

            // Also change gwen's position
            GameObject.Find("Gwen").GetComponent<RectTransform>().anchoredPosition = new Vector3(480.0f, 120.0f);

            if (currentChapter == 1)
            {
                // Change music
                audioManagerReference.PlayPlainsBGM();
            }
            else
            {
                // Change music
                audioManagerReference.PlaySwampBGM();
            }
        }
        // Fort
        else if (currentAct == 4)
        {

        }
    }

    private void ChangeCombatBackgroundFromAct()
    {
        //  Forest
        if (currentAct == 1 && narrativeManagerReference.nightFlag == true)
        {
            GameObject.Find("Background").GetComponent<Image>().sprite = forestNightBattleSprite;
        }

        // Plains
        if (currentAct == 2)
        {
            if (narrativeManagerReference.nightFlag == true)
            {
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsNightBattleSprite;
            }
            else
            {
                GameObject.Find("Background").GetComponent<Image>().sprite = plainsBattleSprite;
            }

        }
        // Swamp
        else if (currentAct == 3)
        {
            GameObject.Find("Background").GetComponent<Image>().sprite = swampBattleSprite;
        }
        // Castle
        else if (currentAct == 4)
        {
            GameObject.Find("Background").GetComponent<Image>().sprite = fortSprite;
        }
    }

    private void SpawnNarrativeSkipButton()
    {
        // See if progressed far enough to skip the narrative
        if (CompareNarrativeProgression() == true)
        {
            GameObject newButton = Instantiate(narrativeSkipPrefab, GameObject.Find("Canvas").transform);

            // Link the button
            newButton.GetComponent<Button>().onClick.AddListener(LoadSortieScene);

            print("Narrative button spawned");
        }
    }

    private void HandleNewGame()
    {
        // Make sure to initialise fresh equipment
        playerManagerReference.InitialiseEquipment();

        // Check if there is a specified encounter
        if (specifiedEncounterFlag == true)
        {
            currentEnemyBattleScenario = specifiedEncounterScenario;

            // Make sure to set the narrative scene as well
            DetermineCurrentNarrativeScenario();

            specifiedEncounterFlag = false;
        }

        // Check if this is demo to set appropriate scene
        else if (demoFlag == true)
        {
            currentAct = 2;
            currentChapter = 1;
            DetermineCurrentCombatScenario();
        }

        else
        {
            // Reset progress
            currentAct = 1;
            currentChapter = 1;

            // Change the enemy combat scenario to match the stage
            DetermineCurrentCombatScenario();
        }

        // Reset the pole axe damage achievement counter
        GetComponent<AchievementManagerScript>().ResetPoleAxeDamage();

        // Then start the game
        LoadNarrativeScene();
    }

    // Used on the title screen
    private void HandleContinueGame()
    {
        // First load in the player saved data
        if (LoadGameData())
        {
            // Check if there is a specified encounter
            if (specifiedEncounterFlag == true)
            {
                currentEnemyBattleScenario = specifiedEncounterScenario;

                // Make sure to set the narrative scene as well
                DetermineCurrentNarrativeScenario();

                specifiedEncounterFlag = false;
            }

            else
            {
                // Change the enemy combat scenario to match the stage
                DetermineCurrentCombatScenario();
            }

            // Then start the game
            LoadNarrativeScene();
        }

        // Otherwise there is no player saved data so nothing happens.
        
    }

    public void SetSpecifiedEncounter(EEnemyBattleScenario battleScenario)
    {
        specifiedEncounterScenario = battleScenario;
        specifiedEncounterFlag = true;
    }

    public static EEnemyBattleScenario ConvertIntToEncounter(int encounterNumber)
    {
        if (encounterNumber == 0)
        {
            return EEnemyBattleScenario.eKobolds;
        }
        else if (encounterNumber == 1)
        {
            return EEnemyBattleScenario.eGiantWolfSpiderSwarm;
        }
        else if (encounterNumber == 2)
        {
            return EEnemyBattleScenario.eHarpyHuntresses;
        }
        else if (encounterNumber == 3)
        {
            return EEnemyBattleScenario.eCentipedeDemon;
        }
        else if (encounterNumber == 4)
        {
            return EEnemyBattleScenario.eSkeletons;
        }
        else if (encounterNumber == 5)
        {
            return EEnemyBattleScenario.eElementalSkylarks;
        }
        else if (encounterNumber == 6)
        {
            return EEnemyBattleScenario.eAurochs;
        }
        else if (encounterNumber == 7)
        {
            return EEnemyBattleScenario.eIronborne;
        }
        else if (encounterNumber == 8)
        {
            return EEnemyBattleScenario.eSwampSquad;
        }
        else if (encounterNumber == 9)
        {
            return EEnemyBattleScenario.eSahuagin;
        }
        else if (encounterNumber == 10)
        {
            return EEnemyBattleScenario.eWraith;
        }
        else if (encounterNumber == 11)
        {
            return EEnemyBattleScenario.eUmbralMass;
        }
        else if (encounterNumber == 12)
        {
            return EEnemyBattleScenario.eVermillionKnight;
        }
        print("No encounter number found. Set encounter in Game Manager Script");
        return EEnemyBattleScenario.eKobolds;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (shouldSaveData)
        {
            //SaveGameData();
        }
    }

    private void SaveGameData()
    {
        // This thing turns the data to binary
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file;

        // Check if demo
        if (demoFlag == true)
        {
            // Open the file
            file = File.Open(Application.persistentDataPath + "/playerDataDemo.dat", FileMode.OpenOrCreate);
        }
        // Open the normal save
        else
        {
            // Open the file
            file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.OpenOrCreate);
        }
        
        // Make a data container
        PlayerData data = new PlayerData();

        // Do a manual save
        //playerManagerReference.SaveCurrentEquipmentData();

        // Get the data to be saved
        data.SaveProgressionData(currentAct, currentChapter);
        data.SaveDurabilityData(playerManagerReference.GetEquipmentDurabilityInfo());
        data.SaveComfyModeState(comfyModeFlag);

        // Write to the file
        bf.Serialize(file, data);

        // Make sure to close it
        file.Close();

        print("Player data successfully saved!");
    }

    public void SavePlayerPreferenceData()
    {
        // This thing turns the data to binary
        BinaryFormatter bf = new BinaryFormatter();

        // Open the file
        FileStream file = File.Open(Application.persistentDataPath + "/playerPreferences.dat", FileMode.OpenOrCreate);

        // Make a data container
        PlayerPreferences data = new PlayerPreferences();

        // Do a manual save
        //playerManagerReference.SaveCurrentEquipmentData();

        // Get the data to be saved
        data.SavePlayerPreferenceData(audioManagerReference.GetBGMVolumeState(), audioManagerReference.GetSFXVolumeState());

        // Write to the file
        bf.Serialize(file, data);

        // Make sure to close it
        file.Close();

        print("Player preferences successfully saved!");
    }

    private bool LoadGameData()
    {
        // Check if demo
        if (demoFlag == true)
        {
            // Make sure the file exists
            if (File.Exists(Application.persistentDataPath + "/playerDataDemo.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerDataDemo.dat", FileMode.Open);

                // Get the data by deserializing it from binary
                PlayerData data = (PlayerData)bf.Deserialize(file);
                file.Close();

                // Overwrite the act and chapter information
                currentAct = data.GetCurrentAct();
                currentChapter = data.GetCurrentChapter();

                // Overwrite the durability data in player manager
                playerManagerReference.OverwriteEquipmentDurability(data.GetDurabilityData());

                // Overwrite comfy mode status
                comfyModeFlag = data.GetComfyModeFlag();

                print("Player data successfully loaded!");
                return true;
            }
            else
            {
                return false;
            }
        }
        // Load the info normally
        else
        {
            // Make sure the file exists
            if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);

                // Get the data by deserializing it from binary
                PlayerData data = (PlayerData)bf.Deserialize(file);
                file.Close();

                // Overwrite the act and chapter information
                currentAct = data.GetCurrentAct();
                currentChapter = data.GetCurrentChapter();

                // Overwrite the durability data in player manager
                playerManagerReference.OverwriteEquipmentDurability(data.GetDurabilityData());

                // Overwrite comfy mode status
                comfyModeFlag = data.GetComfyModeFlag();

                print("Player data successfully loaded!");
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

    private bool LoadPlayerPreferences()
    {
        // Make sure the file exists
        if (File.Exists(Application.persistentDataPath + "/playerPreferences.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerPreferences.dat", FileMode.Open);

            // Get the data by deserializing it from binary
            PlayerPreferences data = (PlayerPreferences)bf.Deserialize(file);
            file.Close();

            // Overwrite the audio bgm and sfx volume
            audioManagerReference.SetBGMVolume(data.GetBGMVolume());
            audioManagerReference.SetSFXVolume(data.GetSFXVolume());

            print("Player preferences successfully loaded!");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SavePlayerProgressionData()
    {
        // Does the file exist?
        if (File.Exists(Application.persistentDataPath + "/playerProgression.dat"))
        {
            // Retrieve the data and then compare to see if should save
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerProgression.dat", FileMode.Open);

            // Get the data by deserializing it from binary
            PlayerProgression data = (PlayerProgression)bf.Deserialize(file);

            // Make sure to close it
            file.Close();

            bool changesMade = false;

            // Compare data to see if should overwrite
            if (currentAct > data.GetNarrativeAct())
            {
                // Act is more than saved act, so overwrite everything
                data.SavePlayerProgressionData(currentAct, currentChapter);
                changesMade = true;
            }
            else if (currentAct == data.GetNarrativeAct())
            {
                if (currentChapter > data.GetNarrativeChapter())
                {
                    // Chapter is more than saved chapter, so overwrite
                    data.SavePlayerProgressionData(currentAct, currentChapter);
                    changesMade = true;
                }
            }

            // Check if should save data due to changes
            if (changesMade == true)
            {
                file = File.Open(Application.persistentDataPath + "/playerProgression.dat", FileMode.OpenOrCreate);

                // Write to the file
                bf.Serialize(file, data);

                // Make sure to close it
                file.Close();
            }

            print("Player progression successfully saved!");
        }
        // Create a new file and just save the data
        else
        {
            // This thing turns the data to binary
            BinaryFormatter bf = new BinaryFormatter();

            // Open the file
            FileStream file = File.Open(Application.persistentDataPath + "/playerProgression.dat", FileMode.OpenOrCreate);

            // Make a data container
            PlayerProgression data = new PlayerProgression();

            // Do a manual save
            //playerManagerReference.SaveCurrentEquipmentData();

            // Get the data to be saved
            data.SavePlayerProgressionData(currentAct, currentChapter);

            // Write to the file
            bf.Serialize(file, data);

            // Make sure to close it
            file.Close();

            print("Player progression successfully created and saved!");
        }
    }

    public bool CompareNarrativeProgression()
    {
        // Go ahead and check if file exists
        if (File.Exists(Application.persistentDataPath + "/playerProgression.dat"))
        {
            // Open file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerProgression.dat", FileMode.Open);

            // Get the data by deserializing it from binary
            PlayerProgression data = (PlayerProgression)bf.Deserialize(file);

            // Make sure to close it
            file.Close();

            // Go ahead and compare if saved progression is more or equal to current
            if (data.GetNarrativeAct() > currentAct)
            {
                return true;
            }
            else if (data.GetNarrativeAct() == currentAct)
            {
                if (data.GetNarrativeChapter() >= currentChapter)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            print("Player progression data does not exist");
            return false;
        }
    }

    public bool IsDemo()
    {
        return demoFlag;
    }

    public string GetCurrentStageAsString()
    {
        string currentStage = currentAct.ToString() + '-' + currentChapter.ToString();
        return currentStage;
    }

    public string GetEnemyPreviewInfo()
    {
        // Figure out what current enemy battle scenario
        if (currentEnemyBattleScenario == EEnemyBattleScenario.eKobolds)
        {
            return "Two bothersome kobolds with a variety of weak attacks. Getting hit by their torch may reduce your accuracy slightly.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eGiantWolfSpiderSwarm)
        {
            return "Vicious spiders of monstrous size. Their webbing will impair your speed and their bites will make you bleed.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eHarpyHuntresses)
        {
            return "A pair of harpies that work in tandem to inflict slows and bleeding.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eCentipedeDemon)
        {
            return "This huge insect has deadly fangs that deal a lot of damage, even more so after it shrieks. Anger it enough and you’ll be stricken with fatal venom.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSkeletons)
        {
            return "A large group of skeletons have arisen! Pick your targets carefully.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eElementalSkylarks)
        {
            return "These pesky birds will sap your strengths, chill your bones, and slam you with fire until you’re down.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eAurochs)
        {
            return "Don’t be fooled by their appearance. Though they attack slowly, getting hit even a few times might spell the end.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eIronborne)
        {
            return "A colossal being that wields a hefty blade. Will rotate between cleaving and bludgeoning with its sword. Be careful weakening this foe.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSwampSquad)
        {
            return "A strange gathering of monsters has appeared. The shadow skylark is a grave omen.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eSahuagin)
        {
            return "Spear wielding brutes that have tipped their sharp weapons with venom. The mud they spit will weaken you and they are known to go into a frenzy.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eWraith)
        {
            return "Feared as a master of undeath, this creature binds skeletons to its service. Its touch can banish your equipment.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eUmbralMass)
        {
            return "A truly horrific entity. Low sobs can be heard as it eyes your weapons fearfully.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eVermillionKnight)
        {
            return "Never before have you felt a knight with such an otherworldly presence. Her skill is undeniable.";
        }
        else if (currentEnemyBattleScenario == EEnemyBattleScenario.eGrimDivine)
        {
            return "The true form of Karmillion. With the power she wields, death may be difficult to stave off.";
        }

        return "";
    }
}

[Serializable]
class PlayerData
{
    // Variables
    private List<int> equipmentDurability;
    private int currentAct;
    private int currentChapter;
    private int maxAct;
    private int maxChapter;
    private bool comfyModeFlag;

    // Functions

    public List<int> GetDurabilityData()
    {
        return equipmentDurability;
    }

    public int GetCurrentAct()
    {
        return currentAct;
    }

    public int GetCurrentChapter()
    {
        return currentChapter;
    }

    public bool GetComfyModeFlag()
    {
        return comfyModeFlag;
    }

    public void SaveDurabilityData(List<int> durabilityData)
    {
        equipmentDurability = durabilityData;
    }

    public void SaveProgressionData(int act, int chapter)
    {
        currentAct = act;
        currentChapter = chapter;
    }

    public void SaveComfyModeState(bool comfyModeState)
    {
        comfyModeFlag = comfyModeState;
    }

    public void SaveNarrativeData(int narrativeAct, int narrativeChapter)
    {

    }
}

[Serializable]
class PlayerPreferences
{
    // Variables
    int bgmVolumePreference;
    int sfxVolumePreference;

    // Functions

    public int GetBGMVolume()
    {
        return bgmVolumePreference;
    }

    public int GetSFXVolume()
    {
        return sfxVolumePreference;
    }

    public void SavePlayerPreferenceData(int bgmVolume, int sfxVolume)
    {
        bgmVolumePreference = bgmVolume;
        sfxVolumePreference = sfxVolume;
    }
}

[Serializable]
class PlayerProgression
{
    // Variables
    int narrativeAct;
    int narrativeChapter;

    // Functions

    public int GetNarrativeAct()
    {
        return narrativeAct;
    }

    public int GetNarrativeChapter()
    {
        return narrativeChapter;
    }

    public void SavePlayerProgressionData(int newAct, int newChapter)
    {
        narrativeAct = newAct;
        narrativeChapter = newChapter;
    }
}
