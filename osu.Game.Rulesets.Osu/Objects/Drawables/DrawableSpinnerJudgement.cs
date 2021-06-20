// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Game.Rulesets.Judgements;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public class DrawableSpinnerJudgement : SkinnableDrawable
    {
        private JudgementResult result;

        public DrawableSpinnerJudgement()
            : base(new OsuSkinComponent(OsuSkinComponents.SpinnerJudgement), _ => Empty())
        {
        }

        /// <summary>
        /// Associates a new result with this judgement. Should be called when retrieving a judgement from a pool.
        /// </summary>
        /// <param name="result">The judgement result.</param>
        public void Apply([NotNull] JudgementResult result)
        {
            this.result = result;
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            using (BeginAbsoluteSequence(result.TimeAbsolute))
            {
                if (Drawable is IAnimatableJudgement animatable)
                    animatable.PlayAnimation();
            }
        }
    }
}
