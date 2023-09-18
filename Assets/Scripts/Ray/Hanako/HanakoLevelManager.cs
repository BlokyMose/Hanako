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

        [SerializeField]
        bool isAutoStart = true;

        [SerializeField]
        float autoStartDuration = 2.5f;

        [SerializeField]
        HanakoLevel level;

        [SerializeField, ShowIf("@!"+nameof(level))]
        HanakoEnemySequence enemySequence;

        [SerializeField, ShowIf("@!"+nameof(level))]
        HanakoDestinationSequence destinationSequence;

        [SerializeField, ShowIf("@!"+nameof(level))]
        HanakoDistractionSequence distractionSequence;

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
        HanakoGameOverCanvas gameOverCanvas;
        
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
        float gameTime;
        public event Action<int, int> OnAddKillCount;

        public HanakoDestination Door { get => exitDoor; }
        public HanakoColors Colors { get => colors; }
        public float AttackCooldown { get => attackCooldown; }
        public float HanakoMoveDuration { get => hanakoMoveDuration; }
        public float EnemyReceiveAttackDelay { get => enemyReceiveAttackDelay; }
        public HanakoGameState GameState { get => gameState;  }

        private void Awake()
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
                exitDoor.Init(colors,icons, () => GameState,0,0);

                var destinations = new List<HanakoDestination>();
                var destinationIDCounter = new Dictionary<HanakoDestinationID, int>();
                foreach (var destination in destinationSequence.Sequence)
                {
                    var destinationGO = Instantiate(destination.Prefab, destinationsParent);
                    destinationGO.name = destinations.Count+"_"+ destinationGO.name;
                    destinationGO.transform.localPosition = destination.Position;

                    var destinationComponent = destinationGO.GetComponent<HanakoDestination>();

                    if (destinationIDCounter.ContainsKey(destinationComponent.ID))
                        destinationIDCounter[destinationComponent.ID]++;
                    else
                        destinationIDCounter.Add(destinationComponent.ID, 0);

                    if (destinationComponent is HanakoDestination_Toilet)
                    {
                        var toilet = destinationComponent as HanakoDestination_Toilet;
                        toilet.Init(colors, 
                            icons, 
                            () => GameState, 
                            destinations.Count, 
                            destinationIDCounter[destinationComponent.ID], 
                            LostGame, 
                            PlayBloodSplatter);
                    }
                    else
                    {
                        destinationComponent.Init(colors,icons, () => GameState, destinations.Count, destinationIDCounter[destinationComponent.ID]);
                    }


                    destinations.Add(destinationComponent);
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
                            sortedDestinations.Insert(0,destination);
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
                    distractionComponent.Init(colors, icons, () => GameState);

                    distractions.Add(distractionComponent);
                }
                return distractions;
            }

            List<HanakoEnemy> InstantiateEnemies(HanakoEnemySequence enemySequence)
            {
                var enemies = new List<HanakoEnemy>();
                foreach (var enemy in enemySequence.Sequence)
                {
                    var enemyGO = Instantiate(enemy.ID.Prefab, enemiesParent);
                    enemyGO.name = enemies.Count + "_" + enemyGO.name;
                    enemyGO.transform.localPosition = Vector3.zero;
                    var enemyComponent = enemyGO.GetComponent<HanakoEnemy>();
                    enemyComponent.Init(enemy.DestinationSequence,exitDoor, GetDestinationByIDAndIndexOfSameID,()=>GameState, colors,icons, AddKillCount, AddExitCount);
                    enemies.Add(enemyComponent);
                }

                return enemies;
            }
        }

        private void Start()
        {
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
                    enemyList.StartLoadingBarOfFirstPanel(enemy.Delay, colors.LoadingBarColor);
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
                    gameTime += Time.deltaTime;
                    yield return null;
                }
            }   
        }

        public void LostGame()
        {
            gameState = HanakoGameState.Lost;
            StopCoroutine(corInitializingGame);
            foreach (var enemy in enemies)
                enemy.DetectHanako(cursor.PossessedToilet.transform.position);
            StopAllCoroutines();

            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                gameOverCanvas.Init(false, level, killCount, enemies.Count, gameTime);
            }
        }

        public void WonGame()
        {
            gameState = HanakoGameState.Won;
            StopAllCoroutines();

            StartCoroutine(Delay(1f));
            IEnumerator Delay(float delay)
            {
                yield return new WaitForSeconds(delay);
                gameOverCanvas.Init(true, level, killCount, enemies.Count, gameTime);
            }
        }

        private HanakoDestination_Toilet GetInitialToilet()
        {
            HanakoDestination_Toilet initialToilet = null;
            int toiletIndex = 0;
            int targetIndex = 3;
            foreach (var destination in destinations)
            {
                if (destination is HanakoDestination_Toilet)
                {
                    initialToilet = destination as HanakoDestination_Toilet;
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

        public void SetHanakoCrawl(HanakoDestination_Toilet fromToilet, HanakoDestination_Toilet toToilet)
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
    }
}
