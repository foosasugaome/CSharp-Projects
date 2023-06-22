using System;
using System.Collections.Generic;

namespace Casino
{
    public class Deck
    {
        // Constructor begin
        public Deck() 
        {           
            Cards = new List<Card>();

            for  (int i = 0; i < 13; i++)
            {
                for  (int j = 0; j < 4; j++) 
                {
                    Card card = new Card();
                    card.Face = (Face)i;  // Casting  to  datatype Face
                    card.Suit = (Suit)j;  // Casting to datatype Suit
                    Cards.Add(card);
                }
            }



        }
        //  Constructor end
         
        public List<Card>  Cards { get;  set;}

        public void Shuffle(int times = 1)  //  assigning  a default value to a  second  parameter makes  it optional
        {            
            // add  a for  loop using the optional parameter
            for (int i = 0; i < times; i++)
            {
         
                List<Card> TempList = new List<Card>();
                Random rnd = new Random();

                while (Cards.Count > 0)
                {
                    int randomIndex = rnd.Next(0, this.Cards.Count);
                    TempList.Add(Cards[randomIndex]);
                    Cards.RemoveAt(randomIndex);
                }
                this.Cards = TempList;
            }            
        }
    }
}
