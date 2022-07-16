using Assets.Scripts.Cards;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TokenObject : MoveableUIObject
    {
        public BaseToken TokenReference { get; set; }
        public override ITargetable SourceReference => TokenReference;

        private SpriteRenderer TokenSymbol { get; set; }

        public void Initialize(BaseToken tokenReference)
        {
            TokenReference = tokenReference;

            TokenSymbol = transform.Find("TokenSymbol").GetComponent<SpriteRenderer>();

            TokenSymbol.sprite = Resources.Load<Sprite>($"Sprites/Symbols/{tokenReference.Symbol}Symbol");

            base.Initialize();
        }
    }
}
