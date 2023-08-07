using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hanako.Hanako
{
    public class HanakoLevelManager : MonoBehaviour
    {
        [SerializeField]
        bool isAutoStart = true;

        [SerializeField]
        float autoStartDuration = 2.5f;

        [SerializeField]
        HanakoEnemySequence enemySequence;

        [SerializeField]
        HanakoDestinationSequence destinationSequence;

        [Header("Components")]
        [SerializeField]
        HanakoCursor cursor;

        [SerializeField]
        Transform enemiesParent;

        [SerializeField]
        Transform destinationsParent;

        [SerializeField]
        HanakoDestination door;

        [Header("UI")]
        [SerializeField]
        HanakoEnemyList enemyList;

        [SerializeField]
        int previewPanelCount = 3;

        [SerializeField]
        Image startBut;

        [Header("Hanako Crawl")]
        [SerializeField]
        float hanakoMoveDuration = 1f;

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

        [Header("Customization")]
        [SerializeField]
        HanakoColors colors;

        List<HanakoDestination> destinations = new();
        List<HanakoEnemy> enemies = new();
        int tri_intoToilet;

        public HanakoDestination Door { get => door; }
        public HanakoColors Colors { get => colors; }

        private void Awake()
        {
            destinations = InstantiateDestinations(destinationSequence);
            destinations = SortDestinations(destinations);
            enemies = InstantiateEnemies(enemySequence);

            if (cursor == null)
                cursor = FindAnyObjectByType<HanakoCursor>();

            if (enemyList == null)
                enemyList = FindAnyObjectByType<HanakoEnemyList>();

            tri_intoToilet = Animator.StringToHash(nameof(tri_intoToilet));
            var initialToilet = GetInitialToilet();
            initHanako.transform.position = (Vector2)initialToilet.InteractablePos.position + new Vector2(-1, -1);
            initProtagonist.transform.position = (Vector2)initialToilet.InteractablePos.position + new Vector2(-1, -1);
            initProtagonist.gameObject.SetActive(false);    
            initHanako.gameObject.SetActive(false);
            startBut.gameObject.SetActive(false);



            List<HanakoDestination> InstantiateDestinations(HanakoDestinationSequence destinationSequence)
            {
                door.Init(this);

                var destinations = new List<HanakoDestination>();
                foreach (var destination in destinationSequence.Sequence)
                {
                    var destinationGO = Instantiate(destination.Prefab, destinationsParent);
                    destinationGO.name = destinations.Count+"_"+ destinationGO.name;
                    destinationGO.transform.localPosition = destination.Position;

                    var destinationComponent = destinationGO.GetComponent<HanakoDestination>();
                    destinationComponent.Init(this);
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

            List<HanakoEnemy> InstantiateEnemies(HanakoEnemySequence enemySequence)
            {
                var enemies = new List<HanakoEnemy>();
                foreach (var enemy in enemySequence.Sequence)
                {
                    var enemyGO = Instantiate(enemy.ID.Prefab, enemiesParent);
                    enemyGO.transform.localPosition = Vector3.zero;
                    var enemyComponent = enemyGO.GetComponent<HanakoEnemy>();
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
                    yield return new WaitForSeconds(delay);
                    StartGame();
                }
            }
            else
            {
                StartCoroutine(Delay());
                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(delayEnemyList);
                    enemyList.Init(enemySequence, previewPanelCount);
                    yield return new WaitForSeconds(delayStartBut);
                    InitStartBut();

                    void InitStartBut()
                    {
                        startBut.gameObject.SetActive(true);

                        var startButAnimator = startBut.GetComponent<Animator>();
                        int boo_hover, boo_show;
                        boo_hover = Animator.StringToHash(nameof(boo_hover));
                        boo_show = Animator.StringToHash(nameof(boo_show));

                        EventTrigger startBut_et = startBut.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry startBut_entry_enter = new EventTrigger.Entry();
                        startBut_entry_enter.eventID = EventTriggerType.PointerEnter;
                        startBut_entry_enter.callback.AddListener((data) =>
                        {
                            startButAnimator.SetBool(boo_hover, true);
                        });
                        startBut_et.triggers.Add(startBut_entry_enter);

                        EventTrigger.Entry startBut_entry_exit = new EventTrigger.Entry();
                        startBut_entry_exit.eventID = EventTriggerType.PointerExit;
                        startBut_entry_exit.callback.AddListener((data) =>
                        {
                            startButAnimator.SetBool(boo_hover, false);
                        });
                        startBut_et.triggers.Add(startBut_entry_exit);

                        EventTrigger.Entry startBut_entry_click = new EventTrigger.Entry();
                        startBut_entry_click.eventID = EventTriggerType.PointerClick;
                        startBut_entry_click.callback.AddListener((data) =>
                        {
                            StartGame();
                            startButAnimator.SetBool(boo_show, false);
                        });
                        startBut_et.triggers.Add(startBut_entry_click);
                    }
                }
            }
        }

        public void StartGame()
        {
            StartCoroutine(Delay());

            StartCoroutine(Update());
            IEnumerator Update()
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

            IEnumerator Delay()
            {
                initHanako.SetTrigger(tri_intoToilet);
                yield return new WaitForSeconds(intoToiletDuration/1.5f);

                if (cursor != null)
                {
                    var initialToilet = GetInitialToilet();
                    initialToilet.Possess(true);
                    cursor.Init(initialToilet, SetHanakoCrawl);
                }

                int index = 0;
                foreach (var enemy in enemySequence.Sequence)
                {
                    yield return new WaitForSeconds(enemy.Delay);
                    enemies[index].Init(this, enemy.DestinationSequence);
                    enemies[index].MoveToCurrentDestination();
                    enemyList.RemoveFirstPanel();
                    index++;
                    if (index > previewPanelCount &&
                        index < enemySequence.Sequence.Count)
                        enemyList.AddPanel(enemy.ID, enemy.DestinationSequence);
                }
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

        public HanakoDestination GetUnoccupiedDestination(HanakoDestinationID id, Vector2 enemyPos)
        {
            HanakoDestination closestDestination = null;
            float closestDistance = 0f;

            foreach (var destination in destinations)
                if (destination.ID == id && destination.Occupation == HanakoDestination.OccupationMode.Unoccupied)
                {
                    if (closestDestination == null)
                    {
                        closestDestination = destination;
                        closestDistance = Vector2.Distance(closestDestination.transform.position, enemyPos);
                    }
                    else 
                    {
                        var distance = Vector2.Distance(destination.transform.position, enemyPos);
                        if (distance < closestDistance)
                        {
                            closestDestination = destination;
                            closestDistance = distance;
                        }
                    }
                }

            return closestDestination;
        }

        public void SetHanakoCrawl(HanakoDestination_Toilet fromToilet, HanakoDestination_Toilet toToilet)
        {
            hanakoCrawl.Crawl(fromToilet, toToilet, hanakoMoveDuration);
        }

    }
}
