using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;

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

        public IEnumerable<BaseCard> GetMatchingCards(TargetConditions targetConditions)
        {
            var players = GetMatchingPlayers(targetConditions.PlayerType);
            var areaCards = GetMatchingAreas(players, targetConditions.Area);

            return areaCards.Where(targetConditions.IsMatch);
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
                if (player.IsSourceOwner(source))
                    return player;
            }

            throw new ArgumentOutOfRangeException(nameof(source), "Source must be owned by a player");
        }
    }
}
