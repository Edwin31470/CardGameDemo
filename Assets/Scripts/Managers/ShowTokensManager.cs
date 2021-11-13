using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class ShowTokensManager : MonoBehaviour
    {
        public bool IsActive { get; set; }

        // Prefabs
        private static GameObject ContainerPrefab { get; set; }

        // Symbols
        private static Sprite ClawSymbol { get; set; }
        private static Sprite BluntSymbol { get; set; }
        private static Sprite ShellSymbol { get; set; }
        private static Sprite CrackedSymbol { get; set; }


        // Runtime Variables
        private static GameObject Container { get; set; }
        private static Collider2D[] Colliders { get; set; }
        private static Vector2 FirstSymbolSpace { get; set; }
        private static HashSet<GameObject> SymbolObjects { get; set; }

        public void Start()
        {
            // Load Resources
            ContainerPrefab = Resources.Load<GameObject>("Prefabs/ViewCardPile");

            // Load Textures
            ClawSymbol = Resources.Load<Sprite>("Sprites/Symbols/ScratchSymbol");
            BluntSymbol = Resources.Load<Sprite>("Sprites/Symbols/RingSymbol");
            ShellSymbol = Resources.Load<Sprite>("Sprites/Symbols/ShellSymbol");
            CrackedSymbol = Resources.Load<Sprite>("Sprites/Symbols/RingSymbol");

        }

        //Space Between Cards
        private static readonly int _xOffset = (32) / 10;
        private static readonly int _yOffset = (32) / 10;

        public void ShowTokens(IEnumerable<TokenType> tokens)
        {
            IsActive = true;

            // Load Variables
            Container = Instantiate(ContainerPrefab, Vector2.zero, Quaternion.identity);
            Colliders = Container.GetComponents<Collider2D>();
            FirstSymbolSpace = ContainerPrefab.transform.Find("FirstCardSpace").transform.position;
            SymbolObjects = new HashSet<GameObject>();


            // Starting Grid Position
            var x = 0;
            var y = 0;

            foreach (var token in tokens)
            {
                var pos = new Vector2(FirstSymbolSpace.x + x * _xOffset, FirstSymbolSpace.y + y * -_yOffset);

                var cardObject = CreateSymbol(token, pos);
                SymbolObjects.Add(cardObject);

                x++;

                if (x == 16)
                {
                    x = 0;
                    y++;
                }
            }
        }

        public void ClosePile()
        {
            foreach (var symbolObject in SymbolObjects)
            {
                Destroy(symbolObject);
            }

            SymbolObjects.Clear();
            Destroy(Container);

            IsActive = false;
        }

        private static GameObject CreateSymbol(TokenType tokenType, Vector2 position)
        {
            Sprite sprite;

            switch (tokenType)
            {
                case TokenType.Claw:
                    sprite = ClawSymbol;
                    break;
                case TokenType.Blunt:
                    sprite = BluntSymbol;
                    break;
                case TokenType.Shell:
                    sprite = ShellSymbol;
                    break;
                case TokenType.Cracked:
                    sprite = CrackedSymbol;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokenType),
                        $"TokenType must be {TokenType.Claw}, {TokenType.Blunt}, {TokenType.Shell} or {TokenType.Cracked}");
            }

            var symbol = new GameObject();
            symbol.transform.position = position;

            var spriteRenderer = symbol.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Card Overlay";
            spriteRenderer.sprite = sprite;

            return symbol;
        }

        private void Update()
        {
            if (!IsActive)
                return;

            // On click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("Overlay"));

                if (hit.collider != null && Colliders.Contains(hit.collider))
                {
                    ClosePile();
                }
            }
        }
    }
}
