﻿using System;
using System.Collections.Generic;

namespace Casino
{
    public class Player
    {
        // Start constructor
        public Player(string name) : this(name,100)
        {
            //don't need to anything here. 
        }
        // end  constructor
        public Player(string  name,int beginningBalance)
        {
            Hand = new List<Card>();
            Balance  = beginningBalance;
            Name =  name;
        }

        private List<Card> _hand = new List<Card>();
        public List<Card> Hand { get { return _hand; } set { _hand = value; } } // instantiate with empty List  _hand
        public int Balance { get; set; }
        public string Name { get; set; }
        public bool isActivelyPlaying { get; set; }
        public bool Stay { get; set; }
        public Guid Id { get; set; }
        public bool Bet(int amount) //Check player balance
        {
            if (Balance - amount < 0) {
                Console.WriteLine("You do not have enough to place a bet that size.");
                return false;
            }
            else
            {
                Balance -= amount;
                return true;
            }
        }

        public static Game operator +(Game game, Player player)
        {
            game.Players.Add(player);
            return game;
        }

        public static Game operator -(Game game, Player player)
        {
            game.Players.Remove(player);
            return game;
        }
    }
}
