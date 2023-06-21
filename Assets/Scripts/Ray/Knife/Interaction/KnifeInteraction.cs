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
            public enum InformationShowMode { Hide, Panel }

            [SerializeField]
            string name;
            [SerializeField, Multiline]
            string desc;
            [SerializeField, PreviewField]
            Sprite logo;
            [SerializeField]
            InformationShowMode showMode = InformationShowMode.Panel;


            public Information(string name, string desc, Sprite logo, InformationShowMode showMode)
            {
                this.name = name;
                this.desc = desc;
                this.logo = logo;
                this.showMode = showMode;
            }

            public string Name { get => name; }
            public string Desc { get => desc; }
            public Sprite Logo { get => logo; }
            public InformationShowMode ShowMode { get => showMode; }
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
