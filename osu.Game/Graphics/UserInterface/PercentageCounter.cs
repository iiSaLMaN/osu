﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Game.Graphics.Sprites;
using osu.Game.Utils;

namespace osu.Game.Graphics.UserInterface
{
    /// <summary>
    /// Used as an accuracy counter. Represented visually as a percentage.
    /// </summary>
    public class PercentageCounter : RollingCounter<double>
    {
        protected override double RollingDuration => 750;

        private float epsilon => 1e-10f;

        public void SetFraction(float numerator, float denominator)
        {
            Current.Value = Math.Abs(denominator) < epsilon ? 1.0f : numerator / denominator;
        }

        public PercentageCounter()
        {
            Current.Value = DisplayedCount = 1.0f;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours) => AccentColour = colours.BlueLighter;

        protected override string FormatCount(double count) => count.FormatAccuracy();

        protected override double GetProportionalDuration(double currentValue, double newValue)
        {
            return Math.Abs(currentValue - newValue) * RollingDuration * 100.0f;
        }

        protected override OsuSpriteText CreateSpriteText()
        {
            var spriteText = base.CreateSpriteText();
            spriteText.Font = spriteText.Font.With(fixedWidth: true);
            return spriteText;
        }

        public override void Increment(double amount)
        {
            Current.Value += amount;
        }
    }
}
