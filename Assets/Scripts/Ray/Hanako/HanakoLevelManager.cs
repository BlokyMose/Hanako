using Encore.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako.Hanako
{
    public class HanakoLevelManager : MonoBehaviour
    {
        [SerializeField]
        HanakoEnemySequence enemySequence;

        [SerializeField]
        HanakoDestinationSequence destinationSequence;

        [Header("Components")]
        [SerializeField]
        Transform enemiesParent;

        [SerializeField]
        Transform destinationsParent;

        [SerializeField]
        HanakoDestination door;

        List<HanakoDestination> destinations = new();
        List<HanakoEnemy> enemies = new();

        public HanakoDestination Door { get => door; }

        private void Awake()
        {
            destinations = InstantiateDestinations(destinationSequence);
            destinations = SortDestinations(destinations);
            enemies = InstantiateEnemies(enemySequence);

            List<HanakoDestination> InstantiateDestinations(HanakoDestinationSequence destinationSequence)
            {
                var destinations = new List<HanakoDestination>();
                foreach (var destination in destinationSequence.Sequence)
                {
                    var destinationGO = Instantiate(destination.Prefab, destinationsParent);
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
                        if (destination.Position.position.x < sortedDestination.Position.position.x)
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
                    var enemyGO = Instantiate(enemy.Prefab, enemiesParent);
                    var enemyComponent = enemyGO.GetComponent<HanakoEnemy>();
                    enemyComponent.Init(this, enemy.DestinationSequence);
                    enemies.Add(enemyComponent);
                }

                return enemies;
            }
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                int index = 0;
                foreach (var enemy in enemySequence.Sequence)
                {
                    yield return new WaitForSeconds(enemy.Delay);
                    enemies[index].MoveToCurrentDestination();
                    index++;
                }
            }
        }

        public HanakoDestination GetDestinationPoint(int index)
        {
            if (destinations.HasIndex(index))
            {
                return destinations[index];
            }
            else
            {
                Debug.Log("Cannot find destination index: "+index);
                return door;
            }
        }

    }
}
