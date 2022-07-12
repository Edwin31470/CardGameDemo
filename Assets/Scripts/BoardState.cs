using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Items;
using Assets.Scripts.Terrains;

namespace Assets.Scripts
{
    public class BoardState
    {
        private Player FrontPlayer { get; set; }
        private Player BackPlayer { get; set; }
        public IEnumerable<Player> BothPlayers => new List<Player> { FrontPlayer, BackPlayer };

        public int RoundNumber { get; set; }
        public Phase CurrentPhase { get; set; }

        public BoardState(Player frontPlayer, Player backPlayer)
        {
            FrontPlayer = frontPlayer;
            BackPlayer = backPlayer;
            RoundNumber = 0;
        }

        public Player GetPlayer(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.Front:
                    return FrontPlayer;
                case PlayerType.Back:
                    return BackPlayer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerType), $"Must be either {PlayerType.Front} or {PlayerType.Back}");
            }
        }

        public IEnumerable<T> GetMatchingTargets<T>(TargetConditions targetConditions) where T : ITargetable
        {
            var players = GetMatchingPlayers(targetConditions.PlayerType);

            if (typeof(BaseCard).IsAssignableFrom(typeof(T)))
            {
                return GetMatchingAreas(players, targetConditions.Area).Where(targetConditions.IsMatch).Cast<T>();
            } 
            else if (typeof(FieldSlot).IsAssignableFrom(typeof(T))) 
            {
                return players.SelectMany(x => x.Field).Cast<T>();
            }
            else if (typeof(BaseTerrain).IsAssignableFrom(typeof(T))) 
            {
                return players.SelectMany(x => x.Field).Where(x => x.Terrain != null).Cast<T>();
            }

            throw new ArgumentOutOfRangeException(nameof(T), $"Must be sub class of {nameof(BaseCard)}, {nameof(FieldSlot)} or {nameof(BaseTerrain)}");
        }

        public IEnumerable<FieldSlot> GetPlayerSlots(PlayerType playerType)
        {
            var slots = new List<FieldSlot>();

            foreach(var player in BothPlayers)
            {
                if (playerType.HasFlag(player.PlayerType))
                {
                    slots.AddRange(player.Field);
                }
            }

            return slots;
        }

        private IEnumerable<Player> GetMatchingPlayers(PlayerType playerType)
        {
            return BothPlayers.Where(player => playerType.HasFlag(player.PlayerType));
        }

        private IEnumerable<BaseCard> GetMatchingAreas(IEnumerable<Player> players, Area area)
        {
            foreach (var player in players)
            {
                if (area.HasFlag(Area.Hand))
                {
                    foreach (var card in player.Hand)
                    {
                        yield return card;
                    }
                }

                if (area.HasFlag(Area.Field))
                {
                    foreach (var card in player.FieldCards)
                    {
                        yield return card;
                    }
                }

                if (area.HasFlag(Area.Deck))
                {
                    foreach (var card in player.Deck)
                    {
                        yield return card;
                    }
                }

                if (area.HasFlag(Area.Destroyed))
                {
                    foreach (var card in player.Destroyed)
                    {
                        yield return card;
                    }
                }

                if (area.HasFlag(Area.Eliminated))
                {
                    foreach (var card in player.Eliminated)
                    {
                        yield return card;
                    }
                }
            }
        }

        public Player GetSourceOwner(BaseSource source)
        {
            foreach (var player in BothPlayers) {
                if (source == player)
                    return player;

                if (player.IsSourceOwner(source))
                    return player;
            }

            throw new ArgumentOutOfRangeException(nameof(source), "Source must be owned by a player");
        }

        public Player GetSourceOwner(FieldSlot fieldSlot)
        {
            foreach (var player in BothPlayers)
            {
                if (player.Field.Contains(fieldSlot))
                    return player;
            }

            throw new ArgumentOutOfRangeException(nameof(fieldSlot), "Source must be owned by a player");
        }
    }
}
