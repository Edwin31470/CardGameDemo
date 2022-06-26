using Assets.Scripts.Bases;
using Assets.Scripts.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Events.Interfaces
{
    public interface IStatEvent : ISourceEvent
    {
        CreatureCard Target { get; set; }
        int Value { get; set; }
    }

    public interface IDamageEvent : IStatEvent
    {

    }

    public interface IFortifyEvent : IStatEvent
    {

    }

    public interface IWeakenEvent : IStatEvent
    {

    }

    public interface IStrengthenEvent : IStatEvent
    {

    }
}
