using System;
using SwinGameSDK;

#if DEBUG
using NUnit.Framework;
#endif

namespace CardGames.GameLogic
{
    /// <summary>
    /// The Snap card game in which the user scores a point if they
    /// click when the rank of the last two cards match.
    /// </summary>
    public class Snap
    {
        // Keep only the last two cards...
        private readonly Card[] _topCards = new Card[2];

        // Have a Deck of cards to play with.
        private readonly Deck _deck;

        // Use a timer to allow the game to draw cards at timed intervals
        private readonly Timer _gameTimer;

        // The amount of time that must pass before a card is flipped
        private int _flipTime = 1000;

        // The score for the 2 players
        private int[] _score = new int[2];

        private bool _started = false;

        /// <summary>
        /// Create a new game of Snap!
        /// </summary>
        public Snap()
        {
            _deck = new Deck();
            _gameTimer = SwinGame.CreateTimer();
        }

        /// <summary>
        /// Gets the card on the top of the "flip" stack. This card will be face up.
        /// </summary>
        public Card TopCard
        {
            get { return _topCards[1]; }
        }

        /// <summary>
        /// Indicates if there are cards remaining in the Snap game's Deck.
        /// </summary>
        public bool CardsRemain
        {
            get { return _deck.CardsRemaining > 0; }
        }

        /// <summary>
        /// Determines how many milliseconds need to pass before a new card is drawn.
        /// </summary>
        public int FlipTime
        {
            get { return _flipTime; }
            set { _flipTime = value; }
        }

        /// <summary>
        /// Indicates if the game has already been started.
        /// </summary>
        public bool IsStarted
        {
            get { return _started; }
        }

        /// <summary>
        /// Start the Snap game playing!
        /// </summary>
        public void Start()
        {
            if (!IsStarted) // only start if not already started
            {
                _started = true;
                _deck.Shuffle(); // Shuffle the deck
                FlipNextCard();  // Flip the first card
                _gameTimer.Start();
            }
        }

        /// <summary>
        /// Flip the next card in the deck.
        /// </summary>
        public void FlipNextCard()
        {
            if (_deck.CardsRemaining > 0)
            {
                _topCards[0] = _topCards[1];   // move top to second card
                _topCards[1] = _deck.Draw();   // draw new top card
                _topCards[1].TurnOver();       // reveal it
            }
        }

        /// <summary>
        /// Update the game. Automatically flip cards based on timer.
        /// </summary>
        public void Update()
        {
            if (_gameTimer.Ticks > _flipTime)
            {
                _gameTimer.Reset();
                FlipNextCard();
            }
        }

        /// <summary>
        /// Gets the player's score.
        /// </summary>
        public int Score(int idx)
        {
            if (idx >= 0 && idx < _score.Length)
                return _score[idx];
            else
                return 0;
        }

        /// <summary>
        /// The player hit the top of the cards "snap"!
        /// Check if the top two cards' ranks match.
        /// </summary>
        public void PlayerHit(int player)
        {
            if (player >= 0 && player < _score.Length && IsStarted)
            {
                if (_topCards[0] != null && _topCards[0].Rank == _topCards[1].Rank)
                {
                    _score[player]++;
                    // TODO: play sound effect if desired
                }
                else
                {
                    _score[player]--; // Optional: deduct score for wrong snap
                }

                _started = false;
                _gameTimer.Stop();
            }
        }

        #region Snap Game Unit Tests
#if DEBUG
        public class SnapTests
        {
            [Test]
            public void TestSnapCreation()
            {
                Snap s = new Snap();
                Assert.IsTrue(s.CardsRemain);
                Assert.IsNull(s.TopCard);
            }

            [Test]
            public void TestFlipNextCard()
            {
                Snap s = new Snap();
                Assert.IsTrue(s.CardsRemain);
                Assert.IsNull(s.TopCard);

                s.FlipNextCard();

                Assert.IsNull(s._topCards[0]);
                Assert.IsNotNull(s._topCards[1]);
            }
        }
#endif
        #endregion
    }
}