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
        public TurnDirection turnCounter {get;set;}

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
        Dictionary<int, MineCart> carts = new Dictionary<int, MineCart>();

        Queue<(int x, int y)> collisions = new Queue<(int x, int y)>();

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
            this.carts = new Dictionary<int, MineCart>();
            this.collisions = new Queue<(int x, int y)>();

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
                        this.carts.Add(this.carts.Count, new MineCart() {
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
        
        private (int x, int y) getNewXY(MineCart cart) {
            switch(cart.direction) {
                case CartDirection.Up:
                    return (cart.x, cart.y-1);
                    
                case CartDirection.Down:
                    return (cart.x, cart.y+1);
                    
                case CartDirection.Left:
                    return (cart.x-1, cart.y);
                    
                case CartDirection.Right:
                default:
                    return (cart.x+1, cart.y);
            }
        }

        private void RunTick() {
            // Get the order of carts to operate on in order of x then y
            List<int> ids = this.carts.OrderBy(a => a.Value.x).ThenBy(a => a.Value.y).Select(a => a.Key).ToList();

            // Now for each cart, we go through and update its location information
            ids.ForEach(id => {
                var cart = this.carts[id];

                // Get our new position
                var newXY = this.getNewXY(cart);

                // Get the tile
                var newTile = this.trackTiles.First(a => a.x == newXY.x && a.y == newXY.y);

                // Update the XY
                cart.x = newXY.x;
                cart.y = newXY.y;

                if (newTile.type == TrackType.Horizontal || newTile.type == TrackType.Vertical) {
                    // Nothing to do, just update the xy
                    return;
                }

                // Check for an intersection
                if (newTile.type == TrackType.Intersection) {
                    // Change our direction!
                    var d = (int) cart.direction;
                    if (cart.turnCounter == TurnDirection.Left) {
                        d--;
                    } else if (cart.turnCounter == TurnDirection.Right) {
                        d++;
                    }

                    // Reset to a valid direction
                    while(d<0) d+=4;
                    d %= 4;

                    // Save it
                    cart.direction = (CartDirection) ((int) cart.direction + d);
                    cart.turnCounter = (TurnDirection) ((((int) cart.turnCounter) + 1) % 3);
                } else {
                    // We've hit a corner
                    // Change direction!
                    if(cart.direction == CartDirection.Left) {
                        // UL or DL
                        if (newTile.type == TrackType.TurnUL)
                            cart.direction = CartDirection.Up;
                        else
                            cart.direction = CartDirection.Down;
                    } else if(cart.direction == CartDirection.Right) {
                        // UR or DR
                        if (newTile.type == TrackType.TurnUR)
                            cart.direction = CartDirection.Up;
                        else
                            cart.direction = CartDirection.Down;
                    } else if(cart.direction == CartDirection.Up) {
                        // UL or UR
                        if (newTile.type == TrackType.TurnUL)
                            cart.direction = CartDirection.Left;
                        else
                            cart.direction = CartDirection.Right;
                    } else if(cart.direction == CartDirection.Down) {
                        // DL or DR
                        if (newTile.type == TrackType.TurnDL)
                            cart.direction = CartDirection.Left;
                        else
                            cart.direction = CartDirection.Right;
                    }
                }
            });

            // Find any collisions
            List<(int x, int y)> c = new List<(int x, int y)>();

            // Go through each cart that isn't this one (based on id) and see if they share the same XY
            foreach(var kvp in this.carts)
                if (this.carts.Count(a => a.Key != kvp.Key && a.Value.x == kvp.Value.x && a.Value.y == kvp.Value.y) > 0)
                    c.Add((kvp.Value.x, kvp.Value.y));
            
            // Break the loop
            foreach(var a in c.OrderBy(a => a.x).ThenBy(a => a.y).Distinct().Where(a => !c.Contains(a)))
                this.collisions.Enqueue(a);
        }

        protected override string SolvePartOne()
        {
            // Run the ticks until we have a collision
            while(this.collisions.Count == 0)
                RunTick();

            return $"{this.collisions.First().x},{this.collisions.First().y}";
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
