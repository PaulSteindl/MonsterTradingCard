﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCard.Enums;

namespace MonsterTradingCard.Abstract.Card
{
    public abstract class Card
    {
        public int Dmg { get; set; }
        public readonly string Name;
        public readonly Element Element;
        public readonly CardType CardType;

        public Card(byte dmg, Element element, CardType cardType)
        {
            Dmg = dmg;
            Element = element;
            CardType = cardType;
            Name = element.ToString() + cardType.ToString();
        }
    }
}