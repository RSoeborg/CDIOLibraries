﻿/*
===============================
 AUTHOR: Rasmus Søborg (S185119)
 CREATE DATE: 06/06/2020
 PURPOSE: This class is a model over the state of the current board.
 SPECIAL NOTES: 
===============================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deck
{
    public class BoardModel
    {
        public CardModel[] Top = new CardModel[4];
        public CardModel[] Bottom = new CardModel[7];

        public CardModel DeckCard;

        public int CardsOnBoard()
        {
            int Count = 0;
            if (DeckCard != default) Count++;
            foreach (var Card in Top) if (Card != default) Count++;
            foreach (var Card in Bottom) if (Card != default) Count++;
            return Count;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = -779638276;
            hashCode = hashCode * -1521134295 + EqualityComparer<CardModel[]>.Default.GetHashCode(Top);
            hashCode = hashCode * -1521134295 + EqualityComparer<CardModel[]>.Default.GetHashCode(Bottom);
            hashCode = hashCode * -1521134295 + EqualityComparer<CardModel>.Default.GetHashCode(DeckCard);
            return hashCode;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
