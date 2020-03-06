// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Timing;
using osu.Game.Configuration;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;

namespace osu.Game.Tests.Visual.Gameplay
{
    public class TestSceneGameplayScaling : PlayerTestScene
    {
        public TestSceneGameplayScaling()
            : base(new OsuRuleset())
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            LocalConfig.Set(OsuSetting.Scaling, ScalingMode.Gameplay);
            AddSliderStep("X pos", 0f, 1f, 0.4f, x => LocalConfig.Set(OsuSetting.ScalingPositionX, x));
            AddSliderStep("Y pos", 0f, 1f, 0.4f, y => LocalConfig.Set(OsuSetting.ScalingPositionY, y));
            AddSliderStep("X scale", 0.2f, 1f, 0.4f, x => LocalConfig.Set(OsuSetting.ScalingSizeX, x));
            AddSliderStep("Y scale", 0.2f, 1f, 0.4f, y => LocalConfig.Set(OsuSetting.ScalingSizeY, y));
        }

        [Test]
        public void TestPlayerPreservesFullSize()
        {
            AddAssert("player preserves full size", () => Player.DrawPosition == Content.DrawPosition && Player.DrawSize == Content.DrawSize);
        }

        [Test]
        public void TestPlayerComponentsScale()
        {
            AddAssert("break overlay not scaled", () => !isComponentScaled(Player.BreakOverlay));
            AddAssert("drawable ruleset not scaled", () => !isComponentScaled(Player.ChildrenOfType<DrawableRuleset>().Single()));
            AddAssert("resume overlay scaled", () => isComponentScaled(Player.ChildrenOfType<ResumeOverlay>().Single()));
        }

        private bool isComponentScaled(Drawable component)
        {
            Drawable target = component;

            while ((target = target?.Parent) != null)
            {
                if (target is ScalingContainer sc && sc.TargetMode == ScalingMode.Gameplay)
                    return true;

                if (target is Player)
                    return false;
            }

            return false;
        }

        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new Beatmap
        {
            BeatmapInfo = { LetterboxInBreaks = true },
            HitObjects = new List<HitObject>
            {
                new HitCircle { StartTime = 0 },
            },
            Breaks = new List<BreakPeriod>
            {
                new BreakPeriod { StartTime = 0, EndTime = 30200 },
            },
        };
    }
}
