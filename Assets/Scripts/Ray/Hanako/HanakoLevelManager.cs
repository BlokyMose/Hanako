using Encore.Utility;
using Hanako.Knife;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility;
using static Hanako.Hanako.HanakoEnemySequence;

namespace Hanako.Hanako
{
    public class HanakoLevelManager : MonoBehaviour
    {
        public enum HanakoGameState { Init, Play, Won, Lost }
        
        [Title("")]
        [SerializeField]
        LevelInfoInitMode levelInfoInit = LevelInfoInitMode.SceneLoadingData;

        [SerializeField, ShowIf("@" + nameof(levelInfoInit) + "==" + nameof(LevelInfoInitMode) + "." + nameof(LevelInfoInitMode.LevelInfo))]
        LevelInfo levelInfo;

        [SerializeField, ShowIf("@" + nameof(levelInfoInit) + "==" + nameof(LevelInfoInitMode) + "." + nameof(LevelInfoInitMode.LevelProperties))]
        HanakoLevel levelProperties;

        [SerializeField]
        bool isAutoStart = true;

        [SerializeField]
        float autoStartDuration = 2.5f;

        [SerializeField, ShowIf("@!"+nameof(levelProperties))]
        HanakoEnemySequence enemySequence;

        [SerializeField, ShowIf("@!"+nameof(levelProperties))]
        HanakoDestinationSequence destinationSequence;

        [SerializeField, ShowIf("@!"+nameof(levelProperties))]
        HanakoDistractionSequence distractionSequence;

        [Header("Score")]
        [SerializeField]
        string killCountParamName = "kill";
        
        [SerializeField]
        string multiKillCountParamName = "multiKill";

        [SerializeField]
        string playTimeParamName = "playTime";

        [Header("Components")]
        [SerializeField]
        HanakoCursor cursor;

        [SerializeField]
        Transform enemiesParent;

        [SerializeField]
        Transform destinationsParent;

        [SerializeField]
        Transform distractionsParent;

        [SerializeField]
        HanakoDestination_ExitDoor exitDoor;

        [Header("UI")]
        [SerializeField]
        HanakoEnemyList enemyList;

        [SerializeField]
        int previewPanelCount = 3;

        [SerializeField]
        Image startBut;

        [SerializeField]
        HanakoGameInfoCanvas gameInfoCanvas;

        [SerializeField]
        ScoreCanvas scoreCanvas;

        [SerializeField]
        LostCanvas lostCanvas;
        
        [Header("Player: Hanako Crawl")]
        [SerializeField]
        float attackCooldown = 0.8f;

        [SerializeField]
        float hanakoMoveDuration = 1f;

        [SerializeField]
        float enemyReceiveAttackDelay = 0.25f;

        [SerializeField]
        HanakoCrawl hanakoCrawl;

        [Header("Initial Animations")]
        [SerializeField]
        GameObject initProtagonist;

        [SerializeField]
        Animator initHanako;

        [SerializeField]
        float delayEnemyList = 2.5f;

        [SerializeField]
        float delayStartBut = 0.5f;

        [SerializeField]
        float intoToiletDuration = 0.5f;

        [SerializeField]
        float initHanakoSpeed = 0.2f;

        [Header("VFX")]
        [SerializeField]
        List<GameObject> bloodSplatters = new();

        [SerializeField]
        List<GameObject> bloodSplattersBig = new();

        [Header("Customization")]
        [SerializeField]
        HanakoColors colors;
        
        [SerializeField]
        HanakoIcons icons;

        List<HanakoDestination> destinations = new();
        List<HanakoDistraction> distractions= new();
        List<HanakoEnemy> enemies = new();
        int tri_intoToilet;
        bool isPlayerMoving = false;
        Coroutine corInitializingGame;
        HanakoGameState gameState = HanakoGameState.Init;
        int killCount = 0;
        int exitCount = 0;
        int multiKillCount = 0;
        float playTime;
        public event Action<int, int> OnAddKillCount;

        public HanakoDestination Door { get => exitDoor; }
        public HanakoColors Colors { get => colors; }
        public float AttackCooldown { get => attackCooldown; }
        public float HanakoMoveDuration { get => hanakoMoveDuration; }
        public float EnemyReceiveAttackDelay { get => enemyReceiveAttackDelay; }
        public HanakoGameState GameState { get => gameState;  }

        void Awake()
        {
            if (scoreCanvas == null)
                scoreCanvas = FindObjectOfType<ScoreCanvas>();
            scoreCanvas.gameObject.SetActive(false);

            if (lostCanvas == null)
                lostCanvas = FindObjectOfType<LostCanvas>();
            lostCanvas.gameObject.SetActive(false);

        }

        void Start()
        {
            AdjustLevelInfo();
            Init(levelProperties);

            if (isAutoStart)
            {
                StartCoroutine(Delay(autoStartDuration));
                IEnumerator Delay(float delay)
                {
                    enemyList.gameObject.SetActive(true);
                    startBut.gameObject.SetActive(false);
                    enemyList.Init(enemySequence, previewPanelCount);
                    yield return new WaitForSeconds(delay);
                    StartGame();
                }
            }
            else
            {
                StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    initProtagonist.gameObject.SetActive(true);
                    enemyList.gameObject.SetActive(false);
                    yield return new WaitForSeconds(delayEnemyList);
                    enemyList.gameObject.SetActive(true);
                    enemyList.Init(enemySequence, previewPanelCount);
                    yield return new WaitForSeconds(delayStartBut);
                    InitStartBut();

                    void InitStartBut()
                    {
                        startBut.gameObject.SetActive(true);

                        var startButAnimator = startBut.GetComponent<Animator>();
                        var startButAudio = startBut.GetComponent<AudioSourceRandom>();
                        int boo_hover, boo_show;
                        boo_hover = Animator.StringToHash(nameof(boo_hover));
                        boo_show = Animator.StringToHash(nameof(boo_show));

                        EventTrigger startBut_et = startBut.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry startBut_entry_enter = new EventTrigger.Entry();
                        startBut_entry_enter.eventID = EventTriggerType.PointerEnter;
                        startBut_entry_enter.callback.AddListener((data) =>
                        {
                            startButAnimator.SetBool(boo_hover, true);
                            startButAudio.PlayOneClipFromPack("sfxHover");
                        });
                        startBut_et.triggers.Add(startBut_entry_enter);

                        EventTrigger.Entry startBut_entry_exit = new EventTrigger.Entry();
                        startBut_entry_exit.eventID = EventTriggerType.PointerExit;
                        startBut_entry_exit.callback.AddListener((data) =>
                        {
                            startButAnimator.SetBool(boo_hover, false);
                            startButAudio.PlayOneClipFromPack("sfxHover");
                        });
                        startBut_et.triggers.Add(startBut_entry_exit);

                        EventTrigger.Entry startBut_entry_click = new EventTrigger.Entry();
                        startBut_entry_click.eventID = EventTriggerType.PointerClick;
                        startBut_entry_click.callback.AddListener((data) =>
                        {
                            StartGame();
                            startButAnimator.SetBool(boo_show, false);
                            startButAudio.PlayAllClipsFromPack("sfxClick");
                        });
                        startBut_et.triggers.Add(startBut_entry_click);
                    }
                }
            }
        }

        void AdjustLevelInfo()
        {
            var sceneLoading = FindObjectOfType<SceneLoadingManager>();
            var allGamesInfoManager = FindObjectOfType<AllGamesInfoManager>();
            var isUsingHub = sceneLoading != null && allGamesInfoManager != null && sceneLoading.SceneLoadingData.LastLoadedLevel == allGamesInfoManager.AllGamesInfo.HubLevelInfo;

            if (isUsingHub || levelInfoInit == LevelInfoInitMode.SceneLoadingData)
            {
                if (sceneLoading == null)
                    Debug.LogWarning("SceneLoadingManager doesn't exist in this scene");

                else if (sceneLoading.SceneLoadingData.LevelInfoToLoad.GameType == GameType.Hanako)
                {
                    levelInfo = sceneLoading.SceneLoadingData.LevelInfoToLoad;
                    levelProperties = levelInfo.HanakoLevel;
                    allGamesInfoManager.AllGamesInfo.SetCurrentLevel(levelInfo);
                }
                else Debug.LogWarning("SceneLoadingData.LevelInfoToLoad doesn't match this gameType");
            }
            else if (levelInfoInit == LevelInfoInitMode.LevelInfo && levelInfo != null && levelInfo.HanakoLevel != null)
            {
                levelProperties = levelInfo.HanakoLevel;
                if (allGamesInfoManager != null)
                    allGamesInfoManager.AllGamesInfo.SetCurrentLevel(levelInfo);
            }
            else Debug.LogWarning("LevelInfo is not set or LevelInfo.HanakoLevel is not set; Error might occur");

            if (levelProperties == null) Debug.LogWarning("LevelProperties is not set; Error might occur");
        }

        void Init(HanakoLevel level)
        {
            if (level != null)
            {
                destinationSequence = level.DestinationSequence;
                enemySequence = level.EnemySequence;
                distractionSequence = level.DistractionSequence;
            }

            destinations = InstantiateDestinations(destinationSequence);
            destinations = SortDestinations(destinations);
            distractions = InstantiateDistractions(distractionSequence);
            enemies = InstantiateEnemies(enemySequence);
            gameInfoCanvas.Init(enemies.Count, ref OnAddKillCount);

            if (cursor == null)
                cursor = FindAnyObjectByType<HanakoCursor>();

            if (enemyList == null)
                enemyList = FindAnyObjectByType<HanakoEnemyList>();

            foreach (var bloodSplatter in bloodSplatters)
                bloodSplatter.SetActive(false);

            foreach (var bloodSplatter in bloodSplattersBig)
                bloodSplatter.SetActive(false);

            tri_intoToilet = Animator.StringToHash(nameof(tri_intoToilet));
            var initialToilet = GetInitialToilet();
            initHanako.transform.position = (Vector2)initialToilet.InteractablePos.position + new Vector2(-1, -1);
            initProtagonist.transform.position = (Vector2)initialToilet.InteractablePos.position + new Vector2(-1, -1);


            List<HanakoDestination> InstantiateDestinations(HanakoDestinationSequence destinationSequence)
            {
                destinationsParent.DestroyChildren();
                exitDoor.Init(colors, icons, () => GameState, 0, 0);

                var destinations = new List<HanakoDestination>();
                var destinationIDCounter = new Dictionary<HanakoDestinationID, int>();
                foreach (var destination in destinationSequence.Sequence)
                {
                    var destinationGO = Instantiate(destination.Prefab, destinationsParent);
                    destinationGO.name = destinations.Count + "_" + destinationGO.name;
                    destinationGO.transform.localPosition = destination.Position;

                    var destinationComponent = destinationGO.GetComponent<HanakoDestination>();

                    if (destinationIDCounter.ContainsKey(destinationComponent.ID))
                        destinationIDCounter[destinationComponent.ID]++;
                    else
                        destinationIDCounter.Add(destinationComponent.ID, 0);

                    if (destinationGO.TryGetComponent<HanakoInteractable_Toilet>(out var toilet))
                        toilet.Init(LostGame, PlayBloodSplatter, OnEnemiesKilled);
                    
                    destinationComponent.Init(colors, icons, () => GameState, destinations.Count, destinationIDCounter[destinationComponent.ID]);

                    destinations.Add(destinationComponent);

                    void OnEnemiesKilled(int killCount)
                    {
                        if (killCount > 1) AddMultiKillCount();
                    }
                }
                return destinations;
            }

            List<HanakoDestination> SortDestinations(List<HanakoDestination> destinations)
            {
                var sortedDestinations = new List<HanakoDestination>();
                foreach (var destination in destinations)
                {
                    bool isInserted = false;
                    foreach (var sortedDestination in sortedDestinations)
                    {
                        if (destination.InteractablePos.position.x < sortedDestination.InteractablePos.position.x)
                        {
                            sortedDestinations.Insert(0, destination);
                            isInserted = true;
                            break;
                        }
                    }

                    if (!isInserted)
                        sortedDestinations.Add(destination);
                }

                return sortedDestinations;
            }

            List<HanakoDistraction> InstantiateDistractions(HanakoDistractionSequence distractionSequence)
            {
                distractionsParent.DestroyChildren();

                var distractions = new List<HanakoDistraction>();
                foreach (var distraction in distractionSequence.Sequence)
                {
                    var distractionGO = Instantiate(distraction.Prefab, distractionsParent);
                    distractionGO.name = distractions.Count + "_" + distractionGO.name;
                    distractionGO.transform.localPosition = distraction.Position;

                    var distractionComponent = distractionGO.GetComponent<HanakoDistraction>();
                    distractionComponent.Init(colors, icons);

                    distractions.Add(distractionComponent);
                }
                return distractions;
            }

            List<HanakoEnemy> InstantiateEnemies(HanakoEnemySequence enemySequence)
            {
                enemiesParent.DestroyChildren();
                var enemies = new List<HanakoEnemy>();
                foreach (var enemy in enemySequence.Sequence)
                {
                    var enemyGO = Instantiate(enemy.ID.Prefab, enemiesParent);
                    enemyGO.name = enemies.Count + "_" + enemyGO.name;
                    enemyGO.transform.localPosition = Vector3.zero;
                    var enemyComponent = enemyGO.GetComponent<HanakoEnemy>();
                    enemyComponent.Init(
                        enemy.DestinationSequence,
                        exitDoor,
                        GetDestinationByIDAndIndexOfSameID,
                        () => GameState,
                        colors, icons,
                        AddKillCount,
                        AddExitCount);
                    enemies.Add(enemyComponent);
                }

                return enemies;
            }
        }

        public void StartGame()
        {
            gameState = HanakoGameState.Play;
            StartCoroutine(AnimatingInitHanako());
            corInitializingGame = StartCoroutine(InitializingGame());
            StartCoroutine(CountingGameTime());

            IEnumerator AnimatingInitHanako()
            {
                var time = 0f;
                var initialToilet = GetInitialToilet();
                while (time < intoToiletDuration)
                {
                    time += Time.deltaTime;
                    initHanako.transform.position = Vector2.MoveTowards(initHanako.transform.position, initialToilet.transform.position, initHanakoSpeed*Time.deltaTime);
                    yield return null;
                }
            }

            IEnumerator InitializingGame()
            {
                initHanako.SetTrigger(tri_intoToilet);
                yield return new WaitForSeconds(intoToiletDuration/1.5f);

                if (cursor != null)
                {
                    var initialToilet = GetInitialToilet();
                    initialToilet.Possess(hanakoMoveDuration, true);
                    cursor.Init(this, initialToilet, SetHanakoCrawl);
                }

                int index = 0;
                foreach (var enemy in enemySequence.Sequence)
                {
                    enemyList.StartLoadingBarOfFirstPanel(enemy.Delay, colors.LoadingBarColor, colors.LoadingBarWarningColor);
                    yield return new WaitForSeconds(enemy.Delay);
                    enemies[index].StartInitialMove();
                    enemyList.RemoveFirstPanel();
                    index++;

                    // Add panel of the rest of the enemies
                    if (index <= enemySequence.Sequence.Count - previewPanelCount)
                        enemyList.AddPanel(enemy.ID, enemy.DestinationSequence);
                    enemyList.IncrementPanelsScale();
                }
            }

            IEnumerator CountingGameTime()
            {
                while (true)
                {
                    playTime += Time.deltaTime;
                    yield return null;
                }
            }   
        }

        void EndGame()
        {
            if (cursor != null)
                cursor.Exit(SetHanakoCrawl);
        }

        public void LostGame()
        {
            EndGame();
            gameState = HanakoGameState.Lost;
            StopCoroutine(corInitializingGame);
            foreach (var enemy in enemies)
                enemy.DetectHanako(cursor.PossessedToilet.transform.position);
            StopAllCoroutines();

            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                lostCanvas.gameObject.SetActive(true);
            }
        }

        public void WonGame()
        {
            EndGame();
            gameState = HanakoGameState.Won;
            StopAllCoroutines();

            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                scoreCanvas.gameObject.SetActive(true);
                scoreCanvas.Init(levelInfo, new List<ScoreDetail>()
                {
                    new(killCountParamName,killCount),
                    new(multiKillCountParamName,multiKillCount),
                    new(playTimeParamName,(int)playTime)
                });
            }
        }

        private HanakoInteractable_Toilet GetInitialToilet()
        {
            HanakoInteractable_Toilet initialToilet = null;
            int toiletIndex = 0;
            int targetIndex = 3;
            foreach (var destination in destinations)
            {
                if (destination.TryGetComponent<HanakoInteractable_Toilet>(out var toilet))
                {
                    initialToilet = toilet;
                    toiletIndex++;
                    if (toiletIndex == targetIndex)
                        break;
                }
            }

            return initialToilet;
        }

        public List<HanakoDestination> GetDestinationsByID(HanakoDestinationID id)
        {
            return destinations.FindAll(x => x.ID == id);
        }

        public HanakoDestination GetDestinationByIDAndIndexOfSameID(HanakoDestinationID id, int indexOfSameID)
        {
            var foundDestinations = GetDestinationsByID(id);
            return foundDestinations.Find(x=>x.IndexOfSameID == indexOfSameID);
        }

        public void SetHanakoCrawl(HanakoInteractable_Toilet fromToilet, HanakoInteractable_Toilet toToilet)
        {
            hanakoCrawl.Crawl(fromToilet, toToilet, hanakoMoveDuration);
        }

        public void PlayBloodSplatter(int killedEnemiesCount = 1, float delay = 0.1f)
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSeconds(delay);
                var bloodSplatter = killedEnemiesCount > 1 ? bloodSplattersBig.GetRandom() : bloodSplatters.GetRandom();
                bloodSplatter.SetActive(true);
                yield return new WaitForSeconds(2.15f);
                bloodSplatter.SetActive(false);
            }
        }

        void AddKillCount()
        {
            killCount++;
            OnAddKillCount(killCount,enemies.Count);
            CheckWinningCondition();
        }

        void AddMultiKillCount()
        {
            multiKillCount++;
        }

        void AddExitCount()
        {
            exitCount++;
            CheckWinningCondition();
        }

        void CheckWinningCondition()
        {
            if (killCount + exitCount >= enemies.Count)
                WonGame();
        }

        [Button, PropertyOrder(-100)]
        void RemoveLastLoadedLevel()
        {
            var sceneLoading = FindObjectOfType<SceneLoadingManager>();
            if (sceneLoading != null)
                sceneLoading.SceneLoadingData.ResetData();
        }
    }
}
