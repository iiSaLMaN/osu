// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch.Objects.Drawables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Catch.UI
{
    public class CatchComboDisplay : SkinnableDrawable
    {
        private int currentCombo;

        public ICatchComboCounter ComboCounter => Drawable as ICatchComboCounter;

        public CatchComboDisplay()
            : base(new CatchSkinComponent(CatchSkinComponents.CatchComboCounter), _ => null)
        {
            RelativeSizeAxes = Axes.None;
            AutoSizeAxes = Axes.Both;
        }

        protected override void SkinChanged(ISkinSource skin, bool allowFallback)
        {
            base.SkinChanged(skin, allowFallback);
            ComboCounter?.UpdateInitialCombo(currentCombo);
        }

        public void OnNewResult(DrawableCatchHitObject judgedObject, JudgementResult result)
        {
            if (!result.Judgement.AffectsCombo || !result.HasResult)
                return;

            if (result.Type == HitResult.Miss)
            {
                updateCombo(0);
                return;
            }

            updateCombo(result.ComboAtJudgement + 1, judgedObject?.AccentColour.Value);
        }

        public void OnRevertResult(DrawableCatchHitObject judgedObject, JudgementResult result)
        {
            if (!result.Judgement.AffectsCombo || !result.HasResult)
                return;

            updateCombo(result.ComboAtJudgement);
        }

        private void updateCombo(int newCombo, Color4? comboColour = null)
        {
            currentCombo = newCombo;
            ComboCounter?.UpdateCombo(currentCombo, comboColour);
        }
    }
}
