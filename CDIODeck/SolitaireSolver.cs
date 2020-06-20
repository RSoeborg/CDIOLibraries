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
using System.Xml.Linq;
using Deck;

namespace Deck
{
    class SolitaireSolver
    {
        CardLogik cardLogik;

        List<CardModel>[] moves { get; set; }
        List<int> score { get; set; }
        List<int> downCard { get; set; }
        List<CardModel> deckCard { get; set; }

        void SolitaireSolverSetup()
        {
            cardLogik = new CardLogik();

            moves = new List<CardModel>[2];
            score = new List<int>();
            downCard = new List<int>();
            deckCard = new List<CardModel>();
        }

        /// <summary>
        /// Returns the best Move the player can make
        /// </summary>
        /// <param name="deck">The card from the deck</param>
        /// <param name="colorStacks">the ace stack list</param>
        /// <param name="stacks">The board stack list</param>
        public List<CardModel> GetBestMove(CardModel deck, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
        {
            SolitaireSolverSetup();

            MovingAcesAndDeuces(deck, colorStacks, stacks);

            if (!moves.Any())
            {
                FindAllMoves(deck, colorStacks, stacks);

                foreach (CardModel move in moves[0]) score.Add(0);

                DownCardScore();

                KingMovement(deck, stacks);

                BuildAceStack(deck, colorStacks, stacks);
            }
            else foreach (CardModel move in moves[0]) score.Add(0);

            List<CardModel> temp = new List<CardModel>();

            if (!moves.Any())
            {
                temp.Add(new CardModel("New Deck Card"));
                temp.Add(new CardModel("New Deck Card"));
            }
            else
            {
                for (int i = 0; i > moves.Length; i++)
                {
                    if (score[i] == score.Max())
                    {
                        temp.Add(moves[0][i]);
                        temp.Add(moves[1][i]);
                    }
                }
            }

            return temp;
        }

        /// <summary>
        /// Strategy rule 1:
        /// 
        /// Always play an Ace or Deuce wherever you can immediately.
        /// </summary>
        /// <param name="deck">The card from the deck</param>
        /// <param name="colorStacks">The ace stack list</param>
        /// <param name="stacks">the board stack list</param>
        void MovingAcesAndDeuces(CardModel deck, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
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
            if (cardLogik.GetValueFromCardModel(deck) == 1)
            {
                moves[0].Add(deck);
                string deckSuit = cardLogik.GetSuitFromCardModel(deck);
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
            if (cardLogik.GetValueFromCardModel(deck) == 2 && cardLogik.IsAceStackEmpty(deuce, colorStacks))
            {
                moves[0].Add(deck);
                string deckSuit = cardLogik.GetSuitFromCardModel(deck);
                moves[1].Add(deckSuit.Equals("h") ? colorStacks[0].Last() : deckSuit.Equals("d") ? colorStacks[1].Last() : deckSuit.Equals("c") ? colorStacks[2].Last() : colorStacks[3].Last());
            }
            #endregion
        }

        void FindAllMoves(CardModel deck, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
        {
            // Deck to Stack
            if (deck != null)
            {
                for (int i = stacks.Length - 1; i >= 0; i--)
                {
                    if (stacks[i].Count != 0)
                    {
                        if (cardLogik.CanColorStack(deck, stacks[i].Last()) && cardLogik.CanNumberStack(deck, stacks[i].Last()))
                        {
                            moves[0].Add(deck);
                            moves[1].Add(stacks[i].Last());

                            downCard.Add(0);
                            deckCard.Add(stacks[i].Last());
                        }
                    }
                    else if (cardLogik.GetValueFromCardModel(deck) == 13)
                    {
                        moves[0].Add(deck);
                        moves[1].Add(new CardModel("Empty"));

                        downCard.Add(0);
                        deckCard.Add(new CardModel("Empty"));
                    }
                }
            }

            // Stack to stack
            for (int i = stacks.Length - 1; i >= 0; i--)
            {
                if (stacks[i].Count != 0)
                {
                    int n = stacks[i].Count - 1;
                    if (n > 0)
                    {
                        while (stacks[i][n - 1].Uncovered)
                        {
                            n--;
                            if (n == 0) break;
                        }
                    }

                    for (int s = stacks.Length - 1; i >= 0; i--)
                    {
                        if (stacks[s].Count != 0)
                        {
                            if (cardLogik.CanColorStack(stacks[i][n], stacks[s].Last()) && cardLogik.CanNumberStack(stacks[i][n], stacks[s].Last()))
                            {
                                moves[0].Add(stacks[i][n]);
                                moves[1].Add(stacks[s].Last());

                                downCard.Add(n);
                                deckCard.Add(new CardModel(""));
                            }
                        }
                        else if (cardLogik.GetValueFromCardModel(stacks[i][n]) == 13 && n != 0)
                        {
                            moves[0].Add(stacks[i][n]);
                            moves[1].Add(stacks[s].Last());

                            downCard.Add(n);
                            deckCard.Add(new CardModel(""));
                        }
                    }
                }
            }
        }

        void DownCardScore()
        {
            for (int i = 0; i < downCard.Count; i++)
            {
                score[i] += downCard[i];
            }
        }

        void KingMovement(CardModel deck, List<CardModel>[] stacks)
        {
            if (moves[0].Count == 1 && !deckCard.Any() && downCard[0] == 0)
            {
                int coveredCards = 0;

                bool kingOnTable = false;
                bool kingInDeck = false;

                if (cardLogik.GetValueFromCardModel(deck) == 13)
                {
                    kingInDeck = true;
                }

                foreach(List<CardModel> stack in stacks)
                {
                    int n = stack.Count - 1;
                    if (n > 0)
                    {
                        while (stack[n - 1].Uncovered)
                        {
                            n--;
                            if (n == 0) break;
                        }
                    }

                    coveredCards += n;

                    if (stack.Count > 1 && cardLogik.GetValueFromCardModel(stack[n]) == 13)
                    {
                        kingOnTable = true;
                    }

                    if (stack.Count != 0 && kingInDeck)
                    {
                        if (cardLogik.CanColorStack(stack[n], deck) && cardLogik.CanNumberStack(stack[n], deck)) kingInDeck = true;
                        else kingInDeck = false;
                    }
                }

                if (!kingOnTable && !kingInDeck && coveredCards > 0)
                {
                    moves[0].Clear();
                    moves[1].Clear();

                    score.Clear();
                    downCard.Clear();
                    deckCard.Clear();
                }
            }
        }

        void BuildAceStack(CardModel deck, List<CardModel>[] colorStacks, List<CardModel>[] stacks)
        {
            if (moves.Any()) return;

            // stacks to Color Stacks
            int n = 0;
            foreach(List<CardModel> stack in stacks)
            {
                string stackCardSuit = cardLogik.GetSuitFromCardModel(stack.Last());
                int stackCardIndex = stackCardSuit.Equals("h") ? 0 : stackCardSuit.Equals("d") ? 1 : stackCardSuit.Equals("c") ? 2 : 3;

                if (colorStacks[stackCardIndex].Any() && cardLogik.CanNumberStack(colorStacks[stackCardIndex].Last(), stack.Last()))
                {
                    moves[0].Add(stack.Last());
                    moves[1].Add(colorStacks[stackCardIndex].Last());

                    score.Add(1);
                }

                n++;
            }

            // Deck to Color Stacks
            string deckCardSuit = cardLogik.GetSuitFromCardModel(deck);
            int deckCardIndex = deckCardSuit.Equals("h") ? 0 : deckCardSuit.Equals("d") ? 1 : deckCardSuit.Equals("c") ? 2 : 3;

            if (colorStacks[deckCardIndex].Any() && cardLogik.CanNumberStack(colorStacks[deckCardIndex].Last(), deck))
            {
                moves[0].Add(deck);
                moves[1].Add(colorStacks[deckCardIndex].Last());

                score.Add(1);
            }
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
