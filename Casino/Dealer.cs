using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Casino
{
    public class Dealer
    {        
        public string Name { get; set; }
        public Deck Deck { get; set; }
        public int Balance { get; set; }

        public void Deal(List<Card> Hand)
        {
            Hand.Add(Deck.Cards.First());
            string card = string.Format(Deck.Cards.First().ToString() + "\n"); //Log to text file
            Console.WriteLine(card);

            //clean by memory manager
            using (StreamWriter file = new StreamWriter(@"D:\Logs\log.txt", true))
            {
                file.WriteLine(DateTime.Now);
                file.WriteLine(card);
            }

            Deck.Cards.RemoveAt(0);
        }
    }
}
