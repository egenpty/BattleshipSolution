using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Battleship.Server.Models
{
    /// <summary>
    /// Represents a ship on the Battleship board.
    /// Tracks its positions and hit status.
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// The fixed positions occupied by the ship on the board.
        /// </summary>
        public IReadOnlyList<Position> Positions { get; }

        /// <summary>
        /// Internal tracking of hit positions.
        /// </summary>
        private readonly HashSet<Position> _hits = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class
        /// with fixed positions.
        /// </summary>
        /// <param name="positions">The positions the ship occupies.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="positions"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="positions"/> is empty.</exception>
        [JsonConstructor]
        public Ship(IReadOnlyList<Position> positions)
        {
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));

            if (positions.Count == 0)
                throw new ArgumentException("A ship must occupy at least one position.", nameof(positions));

            Positions = positions;
        }

        /// <summary>
        /// Gets a value indicating whether the ship is fully sunk,
        /// i.e., all positions have been hit.
        /// </summary>
        public bool IsSunk => Positions.All(pos => _hits.Contains(pos));

        /// <summary>
        /// Marks a hit on the ship if the given position is occupied.
        /// </summary>
        /// <param name="attack">The attack position.</param>
        /// <returns>True if the ship was hit; otherwise, false.</returns>
        public bool IsHit(Position attack)
        {
            if (Positions.Contains(attack))
            {
                _hits.Add(attack);
                return true;
            }
            return false;
        }
    }
}
