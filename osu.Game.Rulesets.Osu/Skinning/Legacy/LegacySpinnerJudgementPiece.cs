// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Judgements;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Osu.Skinning.Legacy
{
    public class LegacySpinnerJudgementPiece : CompositeDrawable, IAnimatableJudgement
    {
        [BackgroundDependencyLoader]
        private void load(ISkinSource source)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            // osu!stable positions spinner components in window-space (as opposed to gamefield-space). This is a 640x480 area taking up the entire screen.
            // In lazer, the gamefield-space positional transformation is applied in OsuPlayfieldAdjustmentContainer, which is inverted here to make this area take up the entire window space.
            Size = new Vector2(640, 480);
            Position = new Vector2(0, -8f);

            InternalChild = new Sprite
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.Centre,
                Texture = source.GetTexture("spinner-osu"),
                Scale = new Vector2(LegacySpinner.SPRITE_SCALE),
                Y = 180,
            };
        }

        public void PlayAnimation()
        {
            const double fade_in_length = 120;
            const double post_empt_length = 500;
            const double fade_out_length = 600;

            this.FadeInFromZero(fade_in_length);
            this.Delay(post_empt_length).FadeOut(fade_out_length);
        }

        Drawable IAnimatableJudgement.GetAboveHitObjectsProxiedContent() => throw new NotImplementedException();
    }
}
