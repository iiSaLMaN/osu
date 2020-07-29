// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Screens.Play;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;
using static osu.Game.Skinning.LegacySkinConfiguration;

namespace osu.Game.Rulesets.Catch.Skinning
{
    internal class LegacyComboCounter : CompositeDrawable, ICatchComboCounter
    {
        private readonly ISkin skin;
        private readonly string font;

        private readonly LegacyRollingCounter counter;
        private LegacyRollingCounter lastExplosion;

        private readonly IBindable<bool> isBreakTime = new BindableBool();

        [Resolved(canBeNull: true)]
        private GameplayBeatmap beatmap { get; set; }

        public LegacyComboCounter(ISkin skin, string font)
        {
            this.skin = skin;
            this.font = font;

            AutoSizeAxes = Axes.Both;

            Alpha = 0f;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Scale = new Vector2(0.8f);

            InternalChild = counter = new LegacyRollingCounter(skin, font)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (beatmap != null)
            {
                isBreakTime.BindTo(beatmap.IsBreakTime);
                isBreakTime.BindValueChanged(v =>
                {
                    // Hide on break time, will be shown back when the combo is updated again.
                    if (v.NewValue)
                        this.FadeOut(200.0, Easing.InQuint);
                }, true);
            }
        }

        public void UpdateInitialCombo(int combo) => counter.SetCountWithoutRolling(combo);

        public void UpdateCombo(int combo, Color4? comboColour)
        {
            // Combo fell to zero, roll down and fade out the counter.
            if (combo == 0)
            {
                counter.Current.Value = 0;
                if (lastExplosion != null)
                    lastExplosion.Current.Value = 0;

                this.FadeOut(300.0);
                return;
            }

            // There may still be previous transforms being applied, finish them and update explosion lifetime.
            FinishTransforms(true);
            lastExplosion?.Expire(true);

            this.FadeIn();

            // For simplicity, in the case of rewinding we'll just set the counter to the current combo value.
            if (Time.Elapsed < 0)
            {
                counter.SetCountWithoutRolling(combo);
                return;
            }

            counter.ScaleTo(1.5f).ScaleTo(0.75f, 250.0, Easing.Out)
                   .OnComplete(c => c.SetCountWithoutRolling(combo));

            counter.Delay(250.0).ScaleTo(1f).ScaleTo(5f, 60.0);//.Then().ScaleTo(1f, 60.0);

            var explosion = new LegacyRollingCounter(skin, font)
            {
                Alpha = 0.75f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(1.5f),
                Colour = comboColour ?? Color4.White,
                Depth = 1f,
            };

            explosion.SetCountWithoutRolling(combo);

            AddInternal(explosion);
            explosion.ScaleTo(1.95f, 350.0)
                     .FadeOut(350.0)
                     .Expire(true);

            lastExplosion = explosion;
        }

        private class LegacyRollingCounter : RollingCounter<int>
        {
            private readonly ISkin skin;
            private readonly string font;

            protected override double RollingDuration => 1200;
            protected override Easing RollingEasing => Easing.Out;

            public LegacyRollingCounter(ISkin skin, string font)
            {
                this.skin = skin;
                this.font = font;
            }

            public override void Increment(int amount) => Current.Value += amount;

            protected override OsuSpriteText CreateSpriteText()
            {
                var fontOverlap = skin.GetConfig<LegacySetting, float>(LegacySetting.ComboOverlap)?.Value ?? -2f;

                return new LegacySpriteText(skin, font)
                {
                    Spacing = new Vector2(-fontOverlap, 0f)
                };
            }
        }
    }
}
