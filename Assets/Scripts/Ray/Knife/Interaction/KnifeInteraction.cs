using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hanako.Knife.KnifeLevelManager;

namespace Hanako.Knife
{
    public abstract class KnifeInteraction : ScriptableObject
    {
        [System.Serializable]
        public class Information
        {
            [SerializeField]
            string name;
            [SerializeField, Multiline]
            string desc;
            [SerializeField, PreviewField]
            Sprite logo;

            public Information(string name, string desc, Sprite logo)
            {
                this.name = name;
                this.desc = desc;
                this.logo = logo;
            }

            public string Name { get => name; }
            public string Desc { get => desc; }
            public Sprite Logo { get => logo; }
        }

        [SerializeField]
        string interactionName;

        [SerializeField]
        Information information;

        public virtual Information GetInformation()
        {
            return information;
        }

        public abstract void Interact(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager);

        public virtual bool CheckInteractabilityAgainst(PieceCache myPiece, TileCache myTile, PieceCache otherPiece, TileCache otherTile, KnifeLevelManager levelManager)
        {
            return true;
        }

    }
}
