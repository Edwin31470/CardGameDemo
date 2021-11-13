using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public static class CardGraphicsManager
    {
        public static Sprite GetSymbolSprite(BaseCard card)
        {
            string symbol;
            switch (card.Name)
            {
                case "Entropy":
                    symbol = "Ring";
                    break;
                case "The Thing That Eats at the Hollow Mountain":
                    symbol = "Crab";
                    break;
                case "It That Speaks in Whispers":
                case "Rift at Halacarnasus":
                case "Feaster of Will":
                    symbol = "Tentacle";
                    break;
                case "Menagerie":
                case "Bloodmouth Vicious":
                    symbol = "Scratch";
                    break;
                case "Apple of the Orchard":
                    symbol = "Apple";
                    break;
                case "Tailwing Moth":
                    symbol = "Insect";
                    break;
                case "Arishi, King of Leaves":
                    symbol = "Leaf";
                    break;
                case "Steadfast Behemoth":
                    symbol = "Shell";
                    break;
                case "Turtleshield Vanguard":
                    symbol = "Shield";
                    break;
                case "Bird of Paradise":
                    symbol = "Feather";
                    break;
                case "Flowstone Rumble":
                case "Roiling Elemental":
                    symbol = "Elemental";
                    break;
                case "Eruption Idol":
                    symbol = "Mountain";
                    break;
                case "Pollen Storm":
                case "Boiling Elemental":
                case "It That Becomes the Sea":
                    symbol = "Bubbles";
                    break;
                case "Rite of Returning":
                case "Lost in the Forest":
                case "Illusionary Speculum":
                    symbol = "Recycle";
                    break;
                case "Catastrophic Vibrations":
                case "Winds of Change":
                case "Wave of Concentration":
                    symbol = "Wave";
                    break;
                case "Hazormat Gauntlet":
                case "Subdued Conscript":
                    symbol = "Helmet";
                    break;
                case "Chromatic Basilisk":
                case "Shard of Arah":
                    symbol = "Gemstone";
                    break;
                case "Betrayer at Arkus":
                    symbol = "Dagger";
                    break;
                case "Rolling Thunder":
                case "Aether Flow":
                case "Technomancer":
                    symbol = "Lightning";
                    break;
                case "Midnight Moon":
                    symbol = "Moon";
                    break;
                case "Tempest Cup":
                case "Water Bearer":
                case "Old Soup":
                    symbol = "Bowl";
                    break;
                case "Unexpected Protégé":
                case "Word of the Belied":
                    symbol = "Scroll";
                    break;
                case "Smouldering Draug":
                case "Soaring Damned":
                case "Willing Sacrifice":
                case "Bride of the Forest":
                    symbol = "Skull";
                    break;
                case "Flame-Tongue Kijiti":
                case "All Out Attack":
                    symbol = "Sword";
                    break;
                case "Flickering Spark":
                case "Soul of the World":
                case "Will of the Wisps":
                    symbol = "Wisp";
                    break;
                default:
                    symbol = card.Type switch
                    {
                        CardType.Creature => "Creature",
                        CardType.Action => "Lightning",
                        _ => "Permanent"
                    };
                    break;
            }

            return Resources.Load<Sprite>($"Sprites/Symbols/{symbol}Symbol");
        }

        public static Color GetSymbolColor(BaseCard card)
        {
            return new Color(59, 32, 39);

            //switch (card.Name)
            //{
            //    // Brown
            //    case "Flowstone Rumble":
            //    case "Old Soup":
            //    case "All Out Attack":
            //        return new Color(78, 52, 46);
            //    // Red
            //    case "Roiling Elemental":
            //    case "Shard of Arah":
            //    case "Rolling Thunder":
            //        return new Color(208, 23, 22);
            //    // Cyan
            //    case "Flickering Spark":
            //    case "Tempest Cup":
            //    case "Technomancer":
            //    case "Hazormat Gauntlet":
            //        return new Color(0, 131, 143);
            //    // Blue
            //    case "Boiling Elemental":
            //    case "Water Bearer":
            //    case "Betrayer at Arkus":
            //    case "Aether Flow":
            //        return new Color(42, 54, 177);
            //    // Orange
            //    case "Eruption Idol":
            //    case "Smouldering Draug":
            //    case "Flame-Tongue Kijiti":
            //        return new Color(230, 81, 0);
            //    // Purple
            //    case "":
            //        return new Color(74, 20, 140);
            //    // Blue-Purple
            //    case "Rite of Returning":
            //        return new Color(26, 35, 126);
            //    // Pale Gray
            //    case "Midnight Moon":
            //    case "Catastrophic Vibrations":
            //        return new Color(96, 125, 139);
            //    // Gray
            //    case "Soaring Damned":
            //    case "Unexpected Protégé":
            //        return new Color(55, 71, 79);
            //    // Black
            //    case "Willing Sacrifice":
            //        return new Color(38, 50, 56);
            //    // Default (reddish brown)
            //    default:
            //        return new Color(59, 32, 39);
            //}
        }
    }
}
