/*
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

        public CardModel TopCard;
    }
}
