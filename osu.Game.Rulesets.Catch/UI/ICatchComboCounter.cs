// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK.Graphics;

namespace osu.Game.Rulesets.Catch.UI
{
    public interface ICatchComboCounter
    {
        /// <summary>
        /// Updates the counter to immediately display the provided <paramref name="combo"/> value as initial combo.
        /// </summary>
        /// <remarks>
        /// This is required for counters loaded in middle of gameplay (via skin change).
        /// </remarks>
        /// <param name="combo"></param>
        void UpdateInitialCombo(int combo);

        void UpdateCombo(int combo, Color4? comboColour);
    }
}
