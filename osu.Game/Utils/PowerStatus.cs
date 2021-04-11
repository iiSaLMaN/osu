// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Utils
{
    /// <summary>
    /// Provides access to the system's power status.
    /// Currently implemented on iOS and Android only.
    /// </summary>
    public abstract class PowerStatus
    {
        /// <summary>
        /// The maximum battery level considered as low, from 0 to 1.
        /// </summary>
        public virtual double BatteryCutoff { get; } = 0;

        /// <summary>
        /// The charge level of the battery, from 0 to 1.
        /// </summary>
        public virtual double ChargeLevel { get; } = 0;

        public virtual bool IsCharging { get; } = false;

        /// <summary>
        /// Whether the battery is currently low in charge.
        /// Returns true if not charging and current charge level is lower than or equal to <see cref="BatteryCutoff"/>.
        /// </summary>
        public bool IsLowBattery => !IsCharging && ChargeLevel <= BatteryCutoff;
    }
}
