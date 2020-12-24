using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Collections;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{
    enum CartDirection {
        Up,
        Down,
        Left,
        Right
    }

    enum TurnDirection {
        Left,
        Straight,
        Right
    }

    enum TrackType {
        Horizontal,
        Vertical,
        Intersection,
        TurnUL,         // Has a turn that goes to/from up and left
        TurnDL,         // Has a turn that goes to/from down and left
        TurnUR,         // Has a turn that goes to/from up and right
        TurnDR          // Has a turn that goes to/from down and right
    }

    class MineCart {
        public CartDirection direction {get;set;}
        private TurnDirection turnCounter {get;set;}

        public int x {get;set;}
        public int y {get;set;}

        public MineCart() {
            // Starts with turning left at the first intersection
            this.turnCounter = TurnDirection.Left;
        }
    }

    class MineTrackTile {
        public int x {get;set;}
        public int y {get;set;}
        public TrackType type {get;set;}
    }

    class Day13 : ASolution
    {
        List<MineTrackTile> trackTiles = new List<MineTrackTile>();
        List<MineCart> carts = new List<MineCart>();

        // Looking up helpers
        // Same order as CartDirection to make it easier
        private readonly string cartTiles = "^v<>";
        
        // Different turns
        private readonly string turnTiles = "\\/";

        public Day13() : base(13, 2018, "")
        {
            LoadInput();
        }

        private void LoadInput() {
            this.trackTiles = new List<MineTrackTile>();
            this.carts = new List<MineCart>();

            // We need to go over the input one character at a time and determine what we have and where we have it
            int y = 0;
            foreach(var line in Input.SplitByNewline(false)) {
                // Split the line out into individual characters
                for(int x=0; x<line.Length; x++) {
                    // Get this position
                    char c = line.Substring(x, 1).ToCharArray()[0];

                    // Nothing
                    if (c == ' ') continue;

                    // Get Neighbor Tiles (help figure out things later)
                    var neighbors = getNeighborTiles(x, y);

                    if (cartTiles.Contains(c)) {
                        // This is a cart
                        // Add a new cart with the direction given
                        this.carts.Add(new MineCart() {
                            x = x,
                            y = y,
                            direction = (CartDirection) (this.cartTiles.IndexOf(c))
                        });

                        // We also need to determine what tile is underneath this position
                        // We can only find up and left neighbors (we haven't gotten further right or down)
                        // Since we can't land on an intersection or turn (rules), we can just check if we have a left
                        if (neighbors[CartDirection.Left] != null) {
                            // This is a horizontal tile
                            this.trackTiles.Add(new MineTrackTile() {
                                x = x,
                                y = y,
                                type = TrackType.Horizontal
                            });
                        } else {
                            // Vertical
                            this.trackTiles.Add(new MineTrackTile() {
                                x = x,
                                y = y,
                                type = TrackType.Vertical
                            });
                        }
                    } else {
                        // We have a track tile type here
                        if (c == '+') {
                            // Intersection
                            this.trackTiles.Add(new MineTrackTile() {
                                x = x,
                                y = y,
                                type = TrackType.Intersection
                            });
                        } else if (this.turnTiles.Contains(c)) {
                            // Turn! Need to figure out what direction

                            // We can't search downwards because we haven't gotten there
                            if (neighbors[CartDirection.Up] != null) {
                                // One of the UL or UR tiles
                                // We can't search right because we haven't gotten there
                                if (neighbors[CartDirection.Left] != null)
                                    this.trackTiles.Add(new MineTrackTile() {
                                        x = x,
                                        y = y,
                                        type = TrackType.TurnUL
                                    });
                                else
                                    this.trackTiles.Add(new MineTrackTile() {
                                        x = x,
                                        y = y,
                                        type = TrackType.TurnUR
                                    });
                            } else {
                                // One of the DL or DR tiles
                                // We can't search right because we haven't gotten there
                                if (neighbors[CartDirection.Left] != null)
                                    this.trackTiles.Add(new MineTrackTile() {
                                        x = x,
                                        y = y,
                                        type = TrackType.TurnDL
                                    });
                                else
                                    this.trackTiles.Add(new MineTrackTile() {
                                        x = x,
                                        y = y,
                                        type = TrackType.TurnDR
                                    });
                            }
                        } else if (c == '-') {
                            this.trackTiles.Add(new MineTrackTile() {
                                x = x,
                                y = y,
                                type = TrackType.Horizontal
                            });
                        } else if (c == '|') {
                            this.trackTiles.Add(new MineTrackTile() {
                                x = x,
                                y = y,
                                type = TrackType.Vertical
                            });
                        }
                    }
                }

                // New line!
                y++;
            }
        }

        private MineTrackTile? getTile(int x, int y) =>
            this.trackTiles.FirstOrDefault(a => a.x == x && a.y == y);

        private Hashtable getNeighborTiles(int x, int y) =>
            new Hashtable() {
                // Left
                {CartDirection.Left, this.getTile(x-1, y)},
                // Right
                {CartDirection.Right, this.getTile(x+1, y)},
                // Up
                {CartDirection.Up, this.getTile(x, y-1)},
                // Down
                {CartDirection.Down, this.getTile(x, y+1)}
            };

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
