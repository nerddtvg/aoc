using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{
    using Hand = (char[] cards, int bid, HandType handType, HandType handType2, int rank);

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
    /// Cards in order of rank
    /// </summary>
    public enum CardRank2
    {
        J,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        T,
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
        public Dictionary<char, int> CardMap = new()
        {
            { '2', (int)CardRank.Two },
            { '3', (int)CardRank.Three },
            { '4', (int)CardRank.Four },
            { '5', (int)CardRank.Five },
            { '6', (int)CardRank.Six },
            { '7', (int)CardRank.Seven },
            { '8', (int)CardRank.Eight },
            { '9', (int)CardRank.Nine },
            { 'T', (int)CardRank.T },
            { 'J', (int)CardRank.J },
            { 'Q', (int)CardRank.Q },
            { 'K', (int)CardRank.K },
            { 'A', (int)CardRank.A }
        };

        public Dictionary<char, int> CardMap2 = new()
        {
            { '2', (int)CardRank2.Two },
            { '3', (int)CardRank2.Three },
            { '4', (int)CardRank2.Four },
            { '5', (int)CardRank2.Five },
            { '6', (int)CardRank2.Six },
            { '7', (int)CardRank2.Seven },
            { '8', (int)CardRank2.Eight },
            { '9', (int)CardRank2.Nine },
            { 'T', (int)CardRank2.T },
            { 'J', (int)CardRank2.J },
            { 'Q', (int)CardRank2.Q },
            { 'K', (int)CardRank2.K },
            { 'A', (int)CardRank2.A }
        };

        public List<Hand> hands;

        public Day07() : base(07, 2023, "Camel Cards")
        {
            hands = Input.SplitByNewline().Select(line => parseHand(line)).ToList();
        }

        private Hand parseHand(string line, int part = 1)
        {
            var split = line.Split(" ");

            var cards = split[0].ToCharArray();
            var bid = int.Parse(split[1]);

            return (cards, bid, scoreHand(cards), scoreHand(cards, 2), 0);
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
                    .ThenByDescending(grp => (int)CardMap2[grp.Key])
                    .First()
                    .Key;
                cards = cards.Select(c => c == 'J' ? newCard : c).ToArray();
            }

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
            // Now rank them
            hands = hands
                .OrderByDescending(hand => hand.handType)
                .ThenByDescending(hand => hand.cards, new CompareHands() { map = CardMap })
                // Score them starting at highestRank and moving down
                .Select((hand, idx) => { hand.rank = hands.Count - idx; return hand; })
                .ToList();

            return hands.Sum(hand => hand.bid * hand.rank).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Now rank them
            hands = hands
                .OrderByDescending(hand => hand.handType2)
                .ThenByDescending(hand => hand.cards, new CompareHands() { map = CardMap2 })
                // Score them starting at highestRank and moving down
                .Select((hand, idx) => { hand.rank = hands.Count - idx; return hand; })
                .ToList();

            return hands.Sum(hand => hand.bid * hand.rank).ToString();
        }
    }

    public class CompareHands : IComparer<char[]>
    {
        public Dictionary<char, int> map = new();
        public int Compare(char[] a, char[] b)
        {
            //return positive if a should be higher, return negative if b should be higher
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            ArgumentOutOfRangeException.ThrowIfNotEqual(5, a.Length, nameof(a));
            ArgumentOutOfRangeException.ThrowIfNotEqual(5, b.Length, nameof(b));

            for(int i =0; i<5; i++)
            {
                if (map[a[i]] > map[b[i]])
                    return 1;
                else if (map[a[i]] < map[b[i]])
                    return -1;
            }

            return 0;
        }
    }
}

