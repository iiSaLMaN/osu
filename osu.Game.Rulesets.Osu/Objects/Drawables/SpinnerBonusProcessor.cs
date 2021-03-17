// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    /// <summary>
    /// Represents a component calculating gained bonus from spinning.
    /// </summary>
    public class SpinnerBonusProcessor : Component
    {
        private static readonly int score_per_tick = new SpinnerBonusTick.OsuSpinnerBonusTickJudgement().MaxNumericResult;

        private readonly IReadOnlyList<DrawableSpinnerTick> ticks;

        private DrawableSpinner drawableSpinner;

        public IBindable<double> Result => result;

        private readonly Bindable<double> result = new BindableDouble();

        public SpinnerBonusProcessor(IReadOnlyList<DrawableSpinnerTick> ticks)
        {
            this.ticks = ticks;
        }

        [BackgroundDependencyLoader]
        private void load(DrawableHitObject drawableHitObject)
        {
            drawableSpinner = (DrawableSpinner)drawableHitObject;
        }

        private int wholeSpins;

        protected override void Update()
        {
            base.Update();

            if (ticks.Count == 0)
                return;

            int spins = (int)(drawableSpinner.Result.RateAdjustedRotation / 360);

            if (spins < wholeSpins)
            {
                // rewinding, silently handle
                wholeSpins = spins;
                return;
            }

            while (wholeSpins != spins)
            {
                var tick = ticks.FirstOrDefault(t => !t.Result.HasResult);

                // tick may be null if we've hit the spin limit.
                if (tick != null)
                {
                    tick.TriggerResult(true);

                    if (tick is DrawableSpinnerBonusTick)
                        result.Value = score_per_tick * (spins - drawableSpinner.HitObject.SpinsRequired);
                }

                wholeSpins++;
            }
        }
    }
}
