using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    using Hand = (CardRank[] cards, int bid, HandType handType, int rank);

    /// <summary>
    /// Cards in order of rank
    /// </summary>
    public enum CardRank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        T,
        J,
        Q,
        K,
        A
    }

    /// <summary>
    /// Poker hands in order of rank (None == 0 for scoring)
    /// </summary>
    public enum HandType
    {
        None,
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
        public Dictionary<char, CardRank> CardMap = new()
        {
            { '2', CardRank.Two },
            { '3', CardRank.Three },
            { '4', CardRank.Four },
            { '5', CardRank.Five },
            { '6', CardRank.Six },
            { '7', CardRank.Seven },
            { '8', CardRank.Eight },
            { '9', CardRank.Nine },
            { 'T', CardRank.T },
            { 'J', CardRank.J },
            { 'Q', CardRank.Q },
            { 'K', CardRank.K },
            { 'A', CardRank.A }
        };

        public List<Hand> hands;

        public Day07() : base(07, 2023, "Camel Cards")
        {
            hands = Input.SplitByNewline().Select(line => parseHand(line)).ToList();
            var highestRank = hands.Count;

            // Now rank them
            hands = hands
                .OrderByDescending(hand => hand.handType)
                .ThenByDescending(hand => hand.cards, new CompareHands())
                // Score them starting at highestRank and moving down
                .Select((hand, idx) => { hand.rank = highestRank - idx; return hand; })
                .ToList();
        }

        private Hand parseHand(string line)
        {
            var split = line.Split(" ");

            var cards = split[0].Select(c => CardMap[c]).ToArray();
            var bid = int.Parse(split[1]);

            // Determine the hand type now
            var handType = HandType.High;
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

            return (cards, bid, handType, 0);
        }

        protected override string? SolvePartOne()
        {
            return hands.Sum(hand => hand.bid * hand.rank).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }

    public class CompareHands : IComparer<CardRank[]>
    {
        public int Compare(CardRank[] a, CardRank[] b)
        {
            //return positive if a should be higher, return negative if b should be higher
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            ArgumentOutOfRangeException.ThrowIfNotEqual(5, a.Length, nameof(a));
            ArgumentOutOfRangeException.ThrowIfNotEqual(5, b.Length, nameof(b));

            for(int i =0; i<5; i++)
            {
                if (a[i] > b[i])
                    return 1;
                else if (a[i] < b[i])
                    return -1;
            }

            return 0;
        }
    }
}

