using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{
    using Hand = (char[] cards, int bid, HandType handType, HandType handType2, int rank, int rank2);

    /// <summary>
    /// Poker hands in order
    /// </summary>
    public enum HandType
    {
        High,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    class Day07 : ASolution
    {
        public List<Hand> hands;

        string CardRank = "23456789TJQKA";
        string CardRank2 = "J23456789TQKA";


        public Day07() : base(07, 2023, "Camel Cards")
        {
            hands = Input.SplitByNewline().Select(line => parseHand(line)).ToList();

            // Now rank them
            hands = hands
                .OrderByDescending(hand => hand.handType)
                .ThenByDescending(hand => hand.cards, new CompareHands() { map = CardRank })
                // Score them starting at highestRank and moving down
                .Select((hand, idx) => { hand.rank = hands.Count - idx; return hand; })
                .ToList();

            // Now rank them again
            hands = hands
                .OrderByDescending(hand => hand.handType2)
                .ThenByDescending(hand => hand.cards, new CompareHands() { map = CardRank2 })
                // Score them starting at highestRank and moving down
                .Select((hand, idx) => { hand.rank2 = hands.Count - idx; return hand; })
                .ToList();
        }

        private Hand parseHand(string line, int part = 1)
        {
            var split = line.Split(" ");

            var cards = split[0].ToCharArray();
            var bid = int.Parse(split[1]);

            return (cards, bid, scoreHand(cards), scoreHand(cards, 2), 0, 0);
        }

        private HandType scoreHand(char[] cards, int part = 1)
        {
            // Determine the hand type now
            var handType = HandType.High;

            if (part == 2)
            {
                // If we have any pairs, we should pick the biggest group
                // If there are more than one groups with the same count, pick the highest value card
                // If it is all J's then we stick with it
                var newCard = cards
                    .Where(c => c != 'J')
                    .DefaultIfEmpty('J')
                    .GroupBy(c => c)
                    .OrderByDescending(grp => grp.Count())
                    .ThenByDescending(grp => CardRank2.IndexOf(grp.Key))
                    .First()
                    .Key;

                // Replace any 'J' with the identified newCard
                cards = cards.Select(c => c == 'J' ? newCard : c).ToArray();
            }

            // Group the cards together
            var groups = cards.GroupBy(c => c);
            var groupsCount = groups.Count();

            if (groupsCount == 1)
                handType = HandType.FiveOfAKind;
            else if (groupsCount == 2)
            {
                if (groups.Any(grp => grp.Count() == 4))
                    handType = HandType.FourOfAKind;
                else
                    handType = HandType.FullHouse;
            }
            else if (groupsCount == 3)
            {
                if (groups.Any(grp => grp.Count() == 3))
                    handType = HandType.ThreeOfAKind;
                else if (groups.Where(grp => grp.Count() == 2).Count() == 2)
                    handType = HandType.TwoPair;
                else
                    handType = HandType.OnePair;
            }
            else if (groupsCount == 4)
                handType = HandType.OnePair;

            return handType;
        }

        protected override string? SolvePartOne()
        {
            return hands.Sum(hand => hand.bid * hand.rank).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return hands.Sum(hand => hand.bid * hand.rank2).ToString();
        }
    }

    public class CompareHands : IComparer<char[]>
    {
        public string map = string.Empty;
        public int Compare(char[]? a, char[]? b)
        {
            //return positive if a should be higher, return negative if b should be higher
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            ArgumentOutOfRangeException.ThrowIfNotEqual(5, a.Length, nameof(a));
            ArgumentOutOfRangeException.ThrowIfNotEqual(5, b.Length, nameof(b));

            for(int i =0; i<5; i++)
            {
                if (map.IndexOf(a[i]) > map.IndexOf(b[i]))
                    return 1;
                else if (map.IndexOf(a[i]) < map.IndexOf(b[i]))
                    return -1;
            }

            return 0;
        }
    }
}

