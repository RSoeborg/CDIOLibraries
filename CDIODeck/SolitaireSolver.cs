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
using System.Linq;

namespace Deck
{
    public class SolitaireSolver
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
        /// <param name="boardModel">The board that contains the cards</param>
        public List<CardModel> GetBestMove(BoardModel boardModel)
        {
            SolitaireSolverSetup();

            MovingAcesAndDeuces(boardModel);

            if (!moves.Any())
            {
                FindAllMoves(boardModel);

                foreach (CardModel move in moves[0]) score.Add(0);

                DownCardScore();

                KingMovement(boardModel);

                BuildAceStack(boardModel);
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
        /// <param name="boardModel">The board that contains the cards</param>
        void MovingAcesAndDeuces(BoardModel boardModel)
        {
            CardModel ace = cardLogik.GetSpecificCardFromStack(boardModel.Bottom, new CardModel("_A"));
            CardModel deuce = cardLogik.GetSpecificCardFromStack(boardModel.Bottom, new CardModel("_2"));

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
            if (cardLogik.GetValueFromCardModel(boardModel.TopCard) == 1)
            {
                moves[0].Add(boardModel.TopCard);
                string deckSuit = cardLogik.GetSuitFromCardModel(boardModel.TopCard);
                moves[1].Add(deckSuit.Equals("h") ? new CardModel("1") : deckSuit.Equals("d") ? new CardModel("2") : deckSuit.Equals("c") ? new CardModel("3") : new CardModel("4"));
            }
            #endregion

            #region Deuces
            // From the stack
            if (deuce != null && cardLogik.IsAceStackEmpty(deuce, boardModel.Top))
            {
                moves[0].Add(deuce);
                string deuceSuit = cardLogik.GetSuitFromCardModel(deuce);
                moves[1].Add(deuceSuit.Equals("h") ? boardModel.Top[0].Last() : deuceSuit.Equals("d") ? boardModel.Top[1].Last() : deuceSuit.Equals("c") ? boardModel.Top[2].Last() : boardModel.Top[3].Last());
            }

            // From the deck
            if (cardLogik.GetValueFromCardModel(boardModel.TopCard) == 2 && cardLogik.IsAceStackEmpty(deuce, boardModel.Top))
            {
                moves[0].Add(boardModel.TopCard);
                string deckSuit = cardLogik.GetSuitFromCardModel(boardModel.TopCard);
                moves[1].Add(deckSuit.Equals("h") ? boardModel.Top[0].Last() : deckSuit.Equals("d") ? boardModel.Top[1].Last() : deckSuit.Equals("c") ? boardModel.Top[2].Last() : boardModel.Top[3].Last());
            }
            #endregion
        }

        /// <summary>
        /// Find all possible moves from the boardModel values.
        /// </summary>
        /// <param name="boardModel">The board that contains the cards</param>
        void FindAllMoves(BoardModel boardModel)
        {
            // Deck to Stack
            if (boardModel.TopCard != null)
            {
                for (int i = boardModel.Bottom.Length - 1; i >= 0; i--)
                {
                    if (boardModel.Bottom[i].Count != 0)
                    {
                        if (cardLogik.CanColorStack(boardModel.TopCard, boardModel.Bottom[i].Last()) && cardLogik.CanNumberStack(boardModel.TopCard, boardModel.Bottom[i].Last()))
                        {
                            moves[0].Add(boardModel.TopCard);
                            moves[1].Add(boardModel.Bottom[i].Last());

                            downCard.Add(0);
                            deckCard.Add(boardModel.Bottom[i].Last());
                        }
                    }
                    else if (cardLogik.GetValueFromCardModel(boardModel.TopCard) == 13)
                    {
                        moves[0].Add(boardModel.TopCard);
                        moves[1].Add(new CardModel("Empty"));

                        downCard.Add(0);
                        deckCard.Add(new CardModel("Empty"));
                    }
                }
            }

            // Stack to stack
            for (int i = boardModel.Bottom.Length - 1; i >= 0; i--)
            {
                if (boardModel.Bottom[i].Count != 0)
                {
                    int n = boardModel.Bottom[i].Count - 1;
                    if (n > 0)
                    {
                        while (boardModel.Bottom[i][n - 1].Uncovered)
                        {
                            n--;
                            if (n == 0) break;
                        }
                    }

                    for (int s = boardModel.Bottom.Length - 1; i >= 0; i--)
                    {
                        if (boardModel.Bottom[s].Count != 0)
                        {
                            if (cardLogik.CanColorStack(boardModel.Bottom[i][n], boardModel.Bottom[s].Last()) && cardLogik.CanNumberStack(boardModel.Bottom[i][n], boardModel.Bottom[s].Last()))
                            {
                                moves[0].Add(boardModel.Bottom[i][n]);
                                moves[1].Add(boardModel.Bottom[s].Last());

                                downCard.Add(n);
                                deckCard.Add(new CardModel(""));
                            }
                        }
                        else if (cardLogik.GetValueFromCardModel(boardModel.Bottom[i][n]) == 13 && n != 0)
                        {
                            moves[0].Add(boardModel.Bottom[i][n]);
                            moves[1].Add(boardModel.Bottom[s].Last());

                            downCard.Add(n);
                            deckCard.Add(new CardModel(""));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Strategy rule 2 & 3:
        /// 
        /// Always make the play or transfer that frees a downcard, regardless of any other considerations.
        /// When faced with a choice, always make the play or transfer that frees the downcard from the biggest pile of downcards
        /// </summary>
        void DownCardScore()
        {
            for (int i = 0; i < downCard.Count; i++)
            {
                score[i] += downCard[i];
            }
        }

        /// <summary>
        /// Strategy rule 5 & 6:
        /// 
        /// Don't clear a spot unless there's a King IMMEDIATELY waiting to occupy it.
        /// Only play a King that will benefit the column(s) with the biggest pile of downcards, unless the play of another King will at least allow a transfer that frees a downcard.
        /// </summary>
        /// <param name="boardModel">The board that contains the cards</param>
        void KingMovement(BoardModel boardModel)
        {
            if (moves[0].Count == 1 && !deckCard.Any() && downCard[0] == 0)
            {
                int coveredCards = 0;

                bool kingOnTable = false;
                bool kingInDeck = false;

                if (cardLogik.GetValueFromCardModel(boardModel.TopCard) == 13)
                {
                    kingInDeck = true;
                }

                foreach(List<CardModel> stack in boardModel.Bottom)
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
                        if (cardLogik.CanColorStack(stack[n], boardModel.TopCard) && cardLogik.CanNumberStack(stack[n], boardModel.TopCard)) kingInDeck = true;
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

        /// <summary>
        /// Strategy rule 7:
        /// 
        /// Only build your Ace stacks (with anything other than an Ace or Deuce) when the play will:
        /// * Not interfere with your Next Card Protection
        /// * Allow a play or transfer that frees (or allows a play that frees) a downcard
        /// * Open up a space for a same-color card pile transfer that allows a downcard to be freed
        /// * Clear a spot for an IMMEDIATE waiting King (it cannot be to simply clear a spot)
        /// </summary>
        /// <param name="boardModel">The board that contains the cards</param>
        void BuildAceStack(BoardModel boardModel)
        {
            if (moves.Any()) return;

            // stacks to Color Stacks
            int n = 0;
            foreach(List<CardModel> stack in boardModel.Bottom)
            {
                string stackCardSuit = cardLogik.GetSuitFromCardModel(stack.Last());
                int stackCardIndex = stackCardSuit.Equals("h") ? 0 : stackCardSuit.Equals("d") ? 1 : stackCardSuit.Equals("c") ? 2 : 3;

                if (boardModel.Top[stackCardIndex].Any() && cardLogik.CanNumberStack(boardModel.Top[stackCardIndex].Last(), stack.Last()))
                {
                    moves[0].Add(stack.Last());
                    moves[1].Add(boardModel.Top[stackCardIndex].Last());

                    score.Add(1);
                }

                n++;
            }

            // Deck to Color Stacks
            string deckCardSuit = cardLogik.GetSuitFromCardModel(boardModel.TopCard);
            int deckCardIndex = deckCardSuit.Equals("h") ? 0 : deckCardSuit.Equals("d") ? 1 : deckCardSuit.Equals("c") ? 2 : 3;

            if (boardModel.Top[deckCardIndex].Any() && cardLogik.CanNumberStack(boardModel.Top[deckCardIndex].Last(), boardModel.TopCard))
            {
                moves[0].Add(boardModel.TopCard);
                moves[1].Add(boardModel.Top[deckCardIndex].Last());

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

        /// <summary>
        /// Get a specific card from the stack, which is used in the ace and deuce rule
        /// </summary>
        /// <param name="stacks">The board stack list</param>
        /// <param name="cardValue">The value you wanna find</param>
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

        /// <summary>
        /// Making sure that an ace stack is not empty before placing a deuce in it.
        /// </summary>
        /// <param name="card">The specified card to chech suit ace stack</param>
        /// <param name="colorStacks">The ace stack list</param>
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
