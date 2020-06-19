/*
===============================
 AUTHOR: Nicklas Beyer Lydersen (S185105)
 CREATE DATE: 01/06/2020
 PURPOSE: This class is the solver of a solitaire game.
 SPECIAL NOTES:
 https://www.chessandpoker.com/solitaire_strategy.html
===============================
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Deck;

namespace Deck
{
    class SolitaireSolver
    {
        CardLogik cardLogik;

        List<CardModel>[] moves { get; set; }
        List<string> first { get; set; }
        List<string> second { get; set; }
        List<int> score { get; set; }
        List<string> downCard { get; set; }
        List<string> deckCard { get; set; }

        void SolitaireSolverSetup()
        {
            cardLogik = new CardLogik();

            moves = new Object[2].Select(i => new List<CardModel>()).ToArray();
            first = new List<string>();
            second = new List<string>();
            score = new List<int>();
            downCard = new List<string>();
            deckCard = new List<string>();
        }

        /// <summary>
        /// Returns the best Move the player can make
        /// </summary>
        /// <param name="deckCard">The card from the deck</param>
        /// <param name="colorStacks">the ace stack list</param>
        /// <param name="stacks">The board stack list</param>
        public CardModel GetBestMove(CardModel deckCard, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
        {
            SolitaireSolverSetup();



            return null;
        }

        /// <summary>
        /// Strategy rule 1:
        /// 
        /// Always play an Ace or Deuce wherever you can immediately.
        /// </summary>
        /// <param name="deckCard">The card from the deck</param>
        /// <param name="colorStacks">The ace stack list</param>
        /// <param name="stacks">the board stack list</param>
        void MovingAcesAndDeuces(CardModel deckCard, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
        {
            CardModel ace = cardLogik.GetSpecificCardFromStack(stacks, new CardModel("_A"));
            CardModel deuce = cardLogik.GetSpecificCardFromStack(stacks, new CardModel("_2"));

            #region Aces
            // From the stack
            if (ace != null)
            {
                moves[0].Add(ace);
                string aceSuit = cardLogik.GetSuitFromCardModel(ace);
                moves[1].Add(aceSuit.Equals("h") ? new CardModel("1") : aceSuit.Equals("d") ? new CardModel("2") : aceSuit.Equals("c") ? new CardModel("3") : new CardModel("4"));
                return;
            }

            // From the deck
            if (cardLogik.GetValueFromCardModel(deckCard) == 1)
            {
                moves[0].Add(deckCard);
                string deckSuit = cardLogik.GetSuitFromCardModel(deckCard);
                moves[1].Add(deckSuit.Equals("h") ? new CardModel("1") : deckSuit.Equals("d") ? new CardModel("2") : deckSuit.Equals("c") ? new CardModel("3") : new CardModel("4"));
            }
            #endregion

            #region Deuces
            // From the stack
            if (deuce != null && cardLogik.IsAceStackEmpty(deuce, colorStacks))
            {
                moves[0].Add(deuce);
                string deuceSuit = cardLogik.GetSuitFromCardModel(deuce);
                moves[1].Add(deuceSuit.Equals("h") ? colorStacks[0].Last() : deuceSuit.Equals("d") ? colorStacks[1].Last() : deuceSuit.Equals("c") ? colorStacks[2].Last() : colorStacks[3].Last());
            }

            // From the deck
            if (cardLogik.GetValueFromCardModel(deckCard) == 2 && cardLogik.IsAceStackEmpty(deuce, colorStacks))
            {
                moves[0].Add(deckCard);
                string deckSuit = cardLogik.GetSuitFromCardModel(deckCard);
                moves[1].Add(deckSuit.Equals("h") ? colorStacks[0].Last() : deckSuit.Equals("d") ? colorStacks[1].Last() : deckSuit.Equals("c") ? colorStacks[2].Last() : colorStacks[3].Last());
            }
            #endregion
        }
    }

    class CardLogik
    {
        public CardLogik() { }

        /// <summary>
        /// Converts CardType from CardModel into a string Suit
        /// </summary>
        /// <param name="cardModel">The card</param>
        public string GetSuitFromCardModel(CardModel cardModel) =>
            GetStringArrayFromCardModel(cardModel).Last();

        /// <summary>
        /// Converts CardType from CardModel into an int Value
        /// </summary>
        /// <param name="cardModel">The card</param>
        /*public int GetValueFromCardModel(CardModel cardModel) => cardModel switch
        {
            _ when GetStringArrayFromCardModel(cardModel)[1].Equals("A") => 1,
            _ when GetStringArrayFromCardModel(cardModel)[1].Equals("K") => 13,
            _ when GetStringArrayFromCardModel(cardModel)[1].Equals("Q") => 12,
            _ when GetStringArrayFromCardModel(cardModel)[1].Equals("J") => 11,
            _ when GetStringArrayFromCardModel(cardModel).Count() == 3 => int.Parse(GetStringArrayFromCardModel(cardModel)[1]),
            _ when GetStringArrayFromCardModel(cardModel).Count() == 4 => int.Parse(GetStringArrayFromCardModel(cardModel)[1] + GetStringArrayFromCardModel(cardModel)[2]),
            _ => throw new Exception("Error in getting correct value")
        };*/
        public int GetValueFromCardModel(CardModel cardModel)
        {
            string[] args = GetStringArrayFromCardModel(cardModel);

            switch (args[1])
            {
                case "A":
                    return 1;
                case "K":
                    return 13;
                case "Q":
                    return 12;
                case "J":
                    return 11;
                default:
                    return args.Count() == 3 ? int.Parse(args[1]) : int.Parse(args[1] + args[2]);

            }
        }

        /// <summary>
        /// It splits the CardType from CardModel into characters
        /// </summary>
        /// <param name="cardModel">The card</param>
        private string[] GetStringArrayFromCardModel(CardModel cardModel) =>
            cardModel.Type.ToString().Select(i => i.ToString()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

        /// <summary>
        /// Making sure that only the correct colors can stack depending on the suit of the card
        /// </summary>
        /// <param name="cardTop">The card that will go on top</param>
        /// <param name="cardBottom">The card that another card will go on top of</param>
        public bool CanColorStack(CardModel cardTop, CardModel cardBottom) => cardTop switch
        {
            _ when
                (GetSuitFromCardModel(cardTop).Equals("h") || GetSuitFromCardModel(cardTop).Equals("d")) 
            &&
                (GetSuitFromCardModel(cardBottom).Equals("h") || GetSuitFromCardModel(cardBottom).Equals("d")) 
            => false,

            _ when
                (GetSuitFromCardModel(cardTop).Equals("h") || GetSuitFromCardModel(cardTop).Equals("d")) 
            &&
                (GetSuitFromCardModel(cardBottom).Equals("c") || GetSuitFromCardModel(cardBottom).Equals("s")) 
            => true,

            _ when
                (GetSuitFromCardModel(cardTop).Equals("c") || GetSuitFromCardModel(cardTop).Equals("s")) 
            &&
                (GetSuitFromCardModel(cardBottom).Equals("h") || GetSuitFromCardModel(cardBottom).Equals("d")) 
            => true,

            _ when
                (GetSuitFromCardModel(cardTop).Equals("c") || GetSuitFromCardModel(cardTop).Equals("s")) 
            &&
                (GetSuitFromCardModel(cardBottom).Equals("c") || GetSuitFromCardModel(cardBottom).Equals("s")) 
            => false,

            _ => throw new Exception("Color Stacking Error")
        };

        /// <summary>
        /// Making sure that you can only place a value thats lower than the value you are trying to place on
        /// </summary>
        /// <param name="cardTop">The card that will go on top</param>
        /// <param name="cardBottom">The card that another card will go on top of</param>
        public bool CanNumberStack(CardModel cardTop, CardModel cardBottom) =>
            GetValueFromCardModel(cardTop) == GetValueFromCardModel(cardBottom) - 1;

        /// <summary>
        /// It's the same as CanNumberStack, but is used for checking if the number can stack when looking for moves into the Ace Stacks
        /// </summary>
        /// <param name="cardTop">The card that will go on top</param>
        /// <param name="cardBottom">The card that another card will go on top of</param>
        public bool CanNumberColorStack(CardModel cardTop, CardModel cardBottom) =>
            GetValueFromCardModel(cardTop) - 1 == GetValueFromCardModel(cardTop);

        /// <summary>
        /// Making sure that you can only stack the same colors in the Ace Stacks
        /// </summary>
        /// <param name="cardTop">The card that will go on top</param>
        /// <param name="cardBottom">The card that another card will go on top of</param>
        public bool IsSameColor(CardModel cardTop, CardModel cardBottom) =>
            GetSuitFromCardModel(cardTop).Equals(GetSuitFromCardModel(cardBottom));

        public CardModel GetSpecificCardFromStack(List<CardModel>[] stacks, CardModel cardValue)
        {
            for (int i = 0; i < stacks.Length; i++)
            {
                if (stacks[i].Count != 0 && GetValueFromCardModel(stacks[i].Last()).Equals(GetValueFromCardModel(cardValue)))
                {
                    return stacks[i].Last();
                }
            }

            return null;
        }

        public bool IsAceStackEmpty(CardModel card, List<CardModel>[] colorStacks)
        {
            switch (GetSuitFromCardModel(card))
            {
                case "h":
                    if (!colorStacks[0].Any()) return true;
                    break;
                case "d":
                    if (!colorStacks[1].Any()) return true;
                    break;
                case "c":
                    if (!colorStacks[2].Any()) return true;
                    break;
                case "s":
                    if (!colorStacks[3].Any()) return true;
                    break;
            }
            return false;
        }
    }
}
