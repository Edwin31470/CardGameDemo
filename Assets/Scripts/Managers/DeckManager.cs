using Assets.Scripts.Cards;
using Assets.Scripts.Enums;
using Assets.Scripts.Events;
using Assets.Scripts.LegendkeeperIntegration;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class DeckManager
    {
        public Dictionary<string, CardInfo> CardLibrary { get; set; }

        public DeckManager()
        {
            var fileContent = File.ReadAllText($"Assets\\Data\\project.json");
            var project = JsonConvert.DeserializeObject<Project>(fileContent);
            var pages = project.Resources.Where(x => !x.Name.Contains("Template") &&
                (x.Tags.Contains("Creature Card") ||
                x.Tags.Contains("Action Card") ||
                x.Tags.Contains("Permanent Card")))
                .ToArray();

            //var cardInfos = pages.Select(ParseCard);

            //CardLibrary = cardInfos.ToDictionary(x => x.Id, x => x);
        }

        //private CardInfo ParseCard(Page page)
        //{
        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(page.MainContent);

        //    var content = doc.DocumentNode.SelectNodes("//h3[@class='csg-h3']");

        //    var cardInfo = new CardInfo
        //    {
        //        Id = page.Id,
        //        Colour = content.ParseEnumField<Colour>("Colour"),
        //        Cost = content.ParseIntField("Cost"),
        //        CardType = content.ParseEnumField<CardType>("Type"),
        //        Name = page.Name,
        //        SubTypes = content.ParseMultipleEnumField<SubType>("Sub-Types").Aggregate(SubType.None, (i, x) => i |= x),
        //        Attack = content.ParseIntField("Attack"),
        //        Defence = content.ParseIntField("Defence"),
        //        EffectText = content.ParseStringField("Effect"),
        //        Flavour = content.ParseStringField("Flavour")
        //    };

        //    if (cardInfo.EffectText.Contains("This card has persistence"))
        //        cardInfo.HasPersistence = true;

        //    return cardInfo;
        //}

        //public List<CardInfo> GetDeck(string deckName)
        //{
        //    var fileContent = File.ReadAllText($"Assets\\Data\\project.json");
        //    var project = JsonConvert.DeserializeObject<Project>(fileContent);
        //    var page = project.Resources.First(x => x.Name.Contains(deckName));

        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(page.MainContent);

        //    var content = doc.DocumentNode.SelectNodes("//a");
        //    var cardIds = content.Select(x => x.GetAttributeValue("href", string.Empty).Replace(".html", ""));

        //    return cardIds.Select(x => CardLibrary[x]).ToList();
        //}

    }
}
