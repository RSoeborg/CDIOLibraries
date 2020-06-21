using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Deck
{
    public class StateController
    {
        private List<CardModel>[] Top = new List<CardModel>[4];
        private List<CardModel>[] Bottom = new List<CardModel>[7];

        private CardModel DeckCard;
        private BoardModel LastState;

        public StateController()
        {
            var CoveredCard = new CardModel();
            CoveredCard.Type = CardType.Covered;
            CoveredCard.Uncovered = false;

            Bottom[1].Add(CoveredCard);
            
            for (int i = 0; i < 2; i++)
                Bottom[2].Add(CoveredCard);

            for (int i = 0; i < 3; i++)
                Bottom[3].Add(CoveredCard);

            for (int i = 0; i < 4; i++)
                Bottom[4].Add(CoveredCard);

            for (int i = 0; i < 5; i++)
                Bottom[5].Add(CoveredCard);

            for (int i = 0; i < 6; i++)
                Bottom[6].Add(CoveredCard);
        }

        public void UpdateBoardState(BoardModel CurrentState)
        {
            if (LastState == default)
            {
                for (int i = 0; i < CurrentState.Bottom.Length; i++)
                    if (CurrentState.Bottom[i] != default)
                        Bottom[i].Add(CurrentState.Bottom[i]);

                for (int i = 0; i < CurrentState.Top.Length; i++)
                    if (CurrentState.Top[i] != default)
                        Top[i].Add(CurrentState.Bottom[i]);
            }

            if (LastState != default && !LastState.Equals(CurrentState))
            {
                // TODO: Implement logic overlapping of cards
            }


            LastState = CurrentState;
        }

        public List<CardModel>[] GetTopDeck() => Top;
        public List<CardModel>[] GetBottomDeck() => Bottom;

        public CardModel GetDeckCard() => DeckCard;
    }
}
