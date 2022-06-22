﻿using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Managers
{
    // DEPRECIATED
    // Will remove when all effects have been copied to the new system

    public static class CardEffectsManager
    {
        private static Random _random = new Random((int)DateTime.Now.Ticks);

        private static bool FlipCoin() => _random.Next(0, 2) == 0;

        public static IEnumerable<BaseEvent> GetCardEvents(CreatureCard card)
        {
            yield break;

            //    case "Flame-Tongue Kijiti":
            //        yield return new AddTokensEvent(card.Owner.PlayerType, TokenType.Claw, 2);
            //        yield break;

            //    case "Soaring Damned":
            //        IEnumerable<BaseEvent> SoaringDamned()
            //        {
            //            yield return new DestroyTargetsEvent(
            //                new TargetConditions
            //                {
            //                    CardType = CardType.Creature
            //                },
            //                1);
            //        }
            //        yield return new OnDestroyedEvent(
            //            card,
            //            SoaringDamned);
            //        yield break;

            //    case "Eruption Idol":

            //        IEnumerable<BaseEvent> EruptionIdol(IEnumerable<CreatureCard> targets)
            //        {
            //            foreach (var target in targets)
            //            {
            //                if (FlipCoin())
            //                    yield return new WeakenCreatureEvent(target, 5);
            //                else
            //                    yield return new DamageCreatureEvent(target, 5);
            //            }
            //        };

            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner.PlayerType.GetOpposite()
            //            },
            //            EruptionIdol);
            //        yield break;
            //    #endregion

            //    #region Green Creatures
            //    case "Bird of Paradise":
            //        yield return new AddManaPlayerEvent(card.Owner.PlayerType, Colour.Green, 1);
            //        yield break;

            //    case "Chromatic Basilisk":
            //        //yield return new CustomSingleTargetEvent(
            //        //    new TargetConditions
            //        //    {
            //        //        CardType = CardType.Creature,
            //        //    },
            //        //    SelectionType.Neutral,
            //        //    "Choose a creature to have no abilities",
            //        //    (target) =>
            //        //    {
            //        //        // TODO: this probably isn't working
            //        //        var triggerEvent = new CustomTriggerEvent(card, (triggeringEvent) =>
            //        //        {
            //        //            if (triggeringEvent is BasePassiveEvent passiveEvent)
            //        //            {
            //        //                MainController.AddEvent(new CustomInteruptEvent(card, (interuptingEvent) =>
            //        //                {
            //        //                    return true;
            //        //                }));
            //        //            }
            //        //        });

            //        //        foreach (var passiveEvent in MainController.GetPassiveEvents().ToList())
            //        //        {
            //        //            if (passiveEvent.Owner == target)
            //        //                MainController.RemoveEvent(passiveEvent);
            //        //        }

            //        //        foreach (var interuptEvent in MainController.GetInteruptEvents().ToList())
            //        //        {
            //        //            if (interuptEvent.Owner == target)
            //        //                MainController.RemoveEvent(interuptEvent);
            //        //        }

            //        //        foreach (var triggerEvent in MainController.GetTriggerEvents().ToList())
            //        //        {
            //        //            if (triggerEvent.Owner == target)
            //        //                MainController.RemoveEvent(triggerEvent);
            //        //        }
            //        //    });
            //        //yield break;

            //    case "Steadfast Behemoth":
            //        yield return new CustomInteruptEvent(
            //            card,
            //            (interuptedEvent) =>
            //            {
            //                if (interuptedEvent is DamageCreatureEvent damageEvent && damageEvent.Card == card)
            //                {
            //                    damageEvent.Value = 0;
            //                    return true;
            //                }

            //                return false;
            //            });
            //        yield break;

            //    case "Turtleshield Vanguard":
            //        yield return new AddTokensEvent(card.Owner.PlayerType, TokenType.Shell, 2);
            //        yield break;

            //    case "Tailwing Moth":
            //        yield return new AddLifePlayerEvent(card.Owner.PlayerType, 5);
            //        yield break;

            //    case "Bride of the Forest":
            //        IEnumerable<BaseEvent> BrideOfTheForest(BaseCard target)
            //        {
            //            yield return new GainControlEvent(target as CreatureCard);
            //        }

            //        yield return new CustomSingleTargetEvent(new TargetConditions
            //        {
            //            PlayerType = card.Owner.PlayerType.GetOpposite(),
            //            CardType = CardType.Creature,
            //            MaxDefence = 8
            //        },
            //        SelectionType.Neutral,
            //        "Gain control of a creature with 8 or less defence",
            //        BrideOfTheForest);
            //        yield break;

            //    case "Arishi, King of Leaves":
            //        IEnumerable<BaseEvent> ArishiKingOfLeaves()
            //        {
            //            var player = card.Owner;

            //            var greenMana = player.GetManaAmount(Colour.Green);

            //            yield return new StrengthenCreatureEvent(card, greenMana * 2);
            //            yield return new FortifyCreatureEvent(card, greenMana * 2);
            //        }

            //        yield return new CustomActiveEvent(
            //            card,
            //            ArishiKingOfLeaves);
            //        yield break;

            //    #endregion

            //    #region Blue Creatures
            //    case "Flickering Spark":
            //        IEnumerable<BaseEvent> FlickeringSpark(BaseEvent triggeringEvent)
            //        {
            //            var eventOwner = (triggeringEvent as EnterFieldEvent).Card as CreatureCard;
            //            yield return new StrengthenCreatureEvent(eventOwner, 1);
            //            yield return new FortifyCreatureEvent(eventOwner, 2);
            //        }

            //        yield return new CustomTriggerEvent(
            //            card,
            //            triggeringEvent => 
            //                triggeringEvent is EnterFieldEvent enterFieldEvent && 
            //                enterFieldEvent.Card is CreatureCard creatureCard &&
            //                creatureCard.Owner == card.Owner,
            //            FlickeringSpark,
            //            true);
            //        yield break;

            //    case "Unexpected Protégé":
            //        IEnumerable<BaseEvent> UnexpectedProtege()
            //        {
            //            var thisPlayer = card.Owner;
            //            var otherPlayer = card.Owner;

            //            if (thisPlayer.TotalAttack < otherPlayer.TotalAttack && thisPlayer.TotalDefence < otherPlayer.TotalDefence)
            //            {
            //                yield return new StrengthenCreatureEvent(card, 2);
            //                yield return new FortifyCreatureEvent(card, 2);
            //            }
            //        }

            //        yield return new CustomActiveEvent(
            //            card,
            //            UnexpectedProtege);
            //        yield break;

            //    case "Water Bearer":
            //        IEnumerable<BaseEvent> WaterBearer(BaseEvent triggeringEvent)
            //        {
            //            yield return new FortifyTargetsEvent(new TargetConditions(), 1, 2);
            //        }

            //        yield return new CustomTriggerEvent(
            //            card,
            //            triggeringEvent =>
            //                triggeringEvent is EnterFieldEvent enterFieldEvent &&
            //                enterFieldEvent.Card is ActionCard actionCard &&
            //                actionCard.Owner == card.Owner,
            //            WaterBearer);
            //        yield break;

            //    case "Betrayer at Arkus":
            //        IEnumerable<BaseEvent> BetrayerAtArkus(BaseEvent triggeringEvent)
            //        {
            //            var eventOwner = ((EnterFieldEvent) triggeringEvent).Card as CreatureCard;
            //            yield return new DamageCreatureEvent(eventOwner, 2);
            //        }

            //        yield return new CustomTriggerEvent(
            //            card,
            //            triggeringEvent =>
            //                triggeringEvent is EnterFieldEvent enterFieldEvent &&
            //                enterFieldEvent.Card is CreatureCard,
            //            BetrayerAtArkus);
            //        yield break;

            //    case "Technomancer":
            //        yield return new CustomPassiveAllCreaturesEvent(
            //            card,
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner.PlayerType
            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    target.BonusAttack.Add(2);
            //                    target.BonusDefence.Add(2);
            //                }
            //            });
            //        yield break;
            //    #endregion

            //    #region Purple Creatures
            //    case "Subdued Conscript":
            //        yield return new CustomInteruptEvent(
            //            card,
            //            (interuptedEvent) =>
            //            {
            //                if (interuptedEvent is BaseStatEvent statEvent && statEvent.Card == card)
            //                {
            //                    statEvent.Value = statEvent.Value * 2;
            //                    return true;
            //                }

            //                return false;
            //            });
            //        yield break;

            //    case "Feaster of Will":
            //        IEnumerable<BaseEvent> FeasterOfWill(IEnumerable<CreatureCard> targets)
            //        {
            //            foreach (var target in targets)
            //            {
            //                yield return new WeakenCreatureEvent(target, 1);
            //                yield return new DamageCreatureEvent(target, 1);
            //            }
            //        };

            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner.PlayerType.GetOpposite()
            //            },
            //            FeasterOfWill);
            //        yield break;

            //    case "It That Speaks in Whispers":
            //        IEnumerable<BaseEvent> ItThatSpeaksInWhispers(IEnumerable<CreatureCard> targets)
            //        {
            //            if (targets.Count() < 2)
            //                yield break;

            //            var damageDealer = targets.Shuffle().First();
            //            var target = targets.First(x => x != damageDealer);

            //            yield return new DamageCreatureEvent(target, damageDealer.Attack);
            //        };

            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner.PlayerType.GetOpposite()
            //            },
            //            ItThatSpeaksInWhispers);
            //        yield break;

            //    case "Bloodmouth Vicious":
            //        yield return new DestroyTargetsEvent(
            //            new TargetConditions
            //            {
            //                CardType = CardType.Creature,
            //                MaxDefence = 4
            //            },
            //            1);
            //        yield break;

            //    case "It That Becomes the Sea":
            //        IEnumerable<BaseEvent> ItThatBecomesTheSea(BaseEvent triggeringEvent)
            //        {
            //            yield return new DamageCreatureEvent(card, 2);
            //        }

            //        yield return new CustomTriggerEvent(
            //            card,
            //            triggeringEvent => triggeringEvent is EnterFieldEvent enterFieldEvent && enterFieldEvent.Card.Type == CardType.Action,
            //            ItThatBecomesTheSea);
            //        yield break;
            //        #endregion
            //}
        }

        public static IEnumerable<BaseEvent> GetCardEvents(ActionCard card)
        {
            yield break;

            //switch (card.Name)
            //{
            //    #region Red Actions
            //    case "Shard of Arah":
            //        yield return new DamageTargetsEvent(new TargetConditions(), 1, 3);
            //        yield return new StrengthenTargetsEvent(new TargetConditions(), 1, 3);
            //        yield break;

            //    case "Tempest Cup":
            //        yield return new AddTokensEvent(card.Owner, TokenType.Claw, 3);
            //        yield return new AddTokensEvent(card.Owner, TokenType.Cracked, 2);
            //        yield return new AddLifePlayerEvent(card.Owner, 5);
            //        yield break;

            //    case "Willing Sacrifice":
            //        yield return new DestroyTargetsEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner,
            //                CardType = CardType.Creature
            //            },
            //            1);
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    MainController.AddEvent(new StrengthenCreatureEvent(target, 3));
            //                    MainController.AddEvent(new FortifyCreatureEvent(target, 3));
            //                }
            //            });
            //        break;

            //    case "Rolling Thunder":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {

            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    MainController.AddEvent(new DamageCreatureEvent(target, target.Owner == card.Owner ? 1 : 3));
            //                }
            //            });
            //        yield break;

            //    #endregion

            //    #region Green Actions
            //    case "Apple of the Orchard":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    MainController.AddEvent(new FortifyCreatureEvent(target, 3));
            //                }
            //            });
            //        yield break;

            //    case "Soul of the World":
            //        yield return new DrawCardEvent(card.Owner);
            //        yield break;

            //    case "Lost in the Forest":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions
            //            {
            //                CardType = CardType.Creature
            //            },
            //            SelectionType.Neutral,
            //            "Choose a card to return to its owner's hand",
            //            (target) => 
            //            {
            //                MainController.AddEvent(new ReturnToHandEvent(target));
            //            });
            //        yield break;

            //    case "Menagerie":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) => 
            //            {
            //                MainController.AddEvent(new AddManaPlayerEvent(card.Owner, Colour.Green, targets.Count()));
            //            });
            //        yield break;
            //    #endregion

            //    #region Blue Actions
            //    case "Flash of Insight":
            //        yield return null;
            //        yield break;

            //    case "Old Soup":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions
            //            {
            //                CardType = CardType.Creature
            //            },
            //            SelectionType.Neutral,
            //            "Choose a creature to get Old Soup",
            //            (target) => 
            //            {
            //                if (FlipCoin())
            //                    MainController.AddEvent(new StrengthenCreatureEvent(target as CreatureCard, 5));
            //                else
            //                    MainController.AddEvent(new DamageCreatureEvent(target as CreatureCard, 3));
            //            });
            //        yield break;

            //    case "Rite of Returning":
            //        yield return new CustomActiveEvent(
            //            card,
            //            () => 
            //            {
            //                var player = MainController.GetPlayer(card.Owner);
            //                var actionCard = player.GetDestroyed().Where(x => x.Type == CardType.Action && x.Cost <= 1 && x.Name != card.Name).Shuffle().FirstOrDefault();

            //                if (actionCard != null)
            //                {
            //                    var cardInfo = actionCard.ToCardInfo();
            //                    MainController.AddEvent(new SummonCardEvent(cardInfo, card.Owner));
            //                }
            //            });
            //        yield break;

            //    case "Midnight Moon":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions 
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) => 
            //            {
            //                if (MainController.GetRoundNumber() % 2 == 0)
            //                {
            //                    foreach (var target in targets)
            //                    {
            //                        MainController.AddEvent(new StrengthenCreatureEvent(target, 2));
            //                        MainController.AddEvent(new FortifyCreatureEvent(target, 3));
            //                    }
            //                }
            //            });
            //        yield break;

            //    case "Aether Flow":
            //        yield return new CustomActiveEvent(
            //            card,
            //            () => 
            //            {
            //                var player = MainController.GetPlayer(card.Owner);
            //                var manaAmount = player.GetManaAmount(Colour.Blue);
            //                player.RemoveMana(Colour.Blue, manaAmount);

            //                MainController.AddEvent(new AddStatsTargetsEvent(
            //                    new TargetConditions(),
            //                    1,
            //                    manaAmount * 2,
            //                    manaAmount * 2,
            //                    SelectionType.Positive
            //                    ));
            //            });
            //        yield break;
            //    #endregion

            //    #region Purple Actions
            //    case "Will of the Wisps":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {

            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    var value = target.Owner == card.Owner ? 2 : 1;

            //                    MainController.AddEvent(new StrengthenCreatureEvent(target, value));
            //                    MainController.AddEvent(new FortifyCreatureEvent(target, value));
            //                }
            //            });
            //        yield break;

            //    case "Word of the Belied":
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) =>
            //            {
            //                if (targets.Count() == 0)
            //                    return;

            //                var value = targets.OrderBy(x => x.Attack).First().Attack;
            //                MainController.AddEvent(new DamageTargetsEvent(
            //                    new TargetConditions(),
            //                    1,
            //                    value));
            //            });
            //        yield break;

            //    case "Entropy":
            //        yield return new EliminateTargetsEvent(new TargetConditions(), 1);
            //        yield return new DamagePlayerEvent(card.Owner, 5);
            //        yield break;

            //    case "Wave of Concentration":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions
            //            {
            //                Area = Area.Hand
            //            },
            //            SelectionType.Neutral,
            //            "Choose a card to return to your deck",
            //            (target) =>
            //            {
            //                MainController.AddEvent(new ReturnToDeckEvent(target));
            //                MainController.AddEvent(new DrawCardEvent(card.Owner));
            //                MainController.AddEvent(new DrawCardEvent(card.Owner));
            //            });
            //        yield break;

            //    case "Illusionary Speculum":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions
            //            {
            //                CardType = CardType.Creature
            //            },
            //            SelectionType.Positive,
            //            "Summon a copy of target creature",
            //            (target) =>
            //            {
            //                MainController.AddEvent(new SummonCardEvent(target.ToCardInfo(), card.Owner));
            //            });
            //        yield break;

            //    case "Rift at Halacarnasus":
            //        yield return new CustomActiveEvent(
            //            card,
            //            () =>
            //            {
            //                var tendrilOfHet = new CardInfo 
            //                {
            //                    Name = "Tendril of Het",
            //                    CardType = CardType.Creature,
            //                    Attack = 5,
            //                    Defence = 7,
            //                    Colour = Colour.Purple,
            //                    Cost = 0
            //                };

            //                var uselessPortal = new CardInfo
            //                {
            //                    Name = "Useless Portal",
            //                    CardType = CardType.Creature,
            //                    Attack = 0,
            //                    Defence = 7,
            //                    Colour = Colour.Purple,
            //                    Cost = 0
            //                };

            //                for (int i = 0; i < _random.Next(1, 4); i++)
            //                {
            //                    MainController.AddEvent(new SummonCardEvent(tendrilOfHet, card.Owner));
            //                }

            //                for (int i = 0; i < _random.Next(1, 4); i++)
            //                {
            //                    MainController.AddEvent(new SummonCardEvent(uselessPortal, card.Owner.GetOpposite()));
            //                }
            //            });
            //        yield break;
            //        #endregion
            //}
        }

        public static IEnumerable<BaseEvent> GetCardEvents(PermanentCard card)
        {
            yield break;
            //switch (card.Name)
            //{
            //    #region Red Permanents
            //    case "All Out Attack":
            //        yield return new CustomPassiveAllCreaturesEvent(
            //            card,
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner
            //            },
            //            (targets) =>
            //            {
            //                foreach (var target in targets)
            //                {
            //                    target.BonusAttack.Add(5);
            //                    target.BonusDefence.Remove(1);
            //                }
            //            });
            //        yield return new CustomTriggerOnceEvent(
            //            card,
            //            (triggeringEvent) =>
            //            {
            //                if (triggeringEvent is NewPhaseEvent phaseEvent && phaseEvent.Phase == Phase.Mana)
            //                {
            //                    MainController.ClearPhaseQueue();
            //                    MainController.AddEvent(new CustomGameEndEvent($"{card.Owner} Player loses due to All Out Attack!"));
            //                    return true;
            //                }

            //                return false;
            //            });
            //        yield break;

            //    #endregion

            //    #region Green Permanents
            //        // This should probably be done another way
            //    case "Pollen Storm":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions
            //            {
            //                CardType = CardType.Creature
            //            },
            //            SelectionType.Negative,
            //            "Choose a creature to have 0 attack",
            //            (target) =>
            //            {
            //                MainController.AddEvent(new CustomPassiveEvent(
            //                    card,
            //                    () =>
            //                    {
            //                        var creatureTarget = target as CreatureCard;
            //                        creatureTarget.BonusAttack.Remove(10000);
            //                    }));
            //            });
            //        yield break;

            //    case "Winds of Change":
            //        yield return new CustomSingleTargetEvent(
            //            new TargetConditions 
            //            {
            //                CardType = CardType.Creature
            //            },
            //            SelectionType.Neutral,
            //            "Choose a creature to redirect actions to",
            //            (target) => 
            //            {
            //                MainController.AddEvent(new CustomInteruptEvent(
            //                    card,
            //                    (interuptedEvent) =>
            //                    {
            //                        if (interuptedEvent is BaseCardSelectionEvent selectionEvent && selectionEvent.Count == 1)
            //                        {
            //                            // override target
            //                            selectionEvent.OverrideTargets = Enumerable.Repeat(target, 1);
            //                            return true;
            //                        }

            //                        return false;
            //                    }));
            //            });
            //        yield break;
            //    #endregion

            //    #region Blue Permanents
            //    case "Catastrophic Vibrations":
            //        yield return new CustomPassiveAllCreaturesEvent(
            //            card,
            //            new TargetConditions 
            //            {
            //                Colour = Colour.NonBlue
            //            },
            //            (targets) => 
            //            {
            //                foreach (var target in targets)
            //                {
            //                    target.BonusDefence.Remove(4);
            //                }
            //            });
            //        yield break;

            //    case "Hazormat Gauntlet":
            //        yield return new DamageTargetsEvent(new TargetConditions(), 1, 5);
            //        yield return new CustomAllCreaturesEvent(
            //            new TargetConditions
            //            {
            //                PlayerType = card.Owner.GetOpposite(),
            //                Area = Area.Hand,
            //            },
            //            (targets) => 
            //            {
            //                var randomCard = targets.Shuffle().FirstOrDefault();

            //                if (randomCard != null)
            //                    MainController.AddEvent(new DestroyCardEvent(randomCard));
            //            });
            //        yield break;
            //        #endregion

            //    #region Purple Permanents

            //        #endregion
            //}
        }
    }
}
