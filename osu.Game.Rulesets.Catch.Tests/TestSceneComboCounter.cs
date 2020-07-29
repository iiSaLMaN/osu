// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Screens.Play;
using osuTK;

namespace osu.Game.Rulesets.Catch.Tests
{
    public class TestSceneComboCounter : CatchSkinnableTestScene
    {
        private ScoreProcessor scoreProcessor;
        private GameplayBeatmap gameplayBeatmap;
        private readonly Bindable<bool> isBreakTime = new BindableBool();

        [BackgroundDependencyLoader]
        private void load()
        {
            gameplayBeatmap = new GameplayBeatmap(CreateBeatmapForSkinProvider());
            gameplayBeatmap.IsBreakTime.BindTo(isBreakTime);
            Dependencies.Cache(gameplayBeatmap);
            Add(gameplayBeatmap);
        }

        [SetUp]
        public void SetUp() => Schedule(() =>
        {
            scoreProcessor = new ScoreProcessor();

            SetContents(() => new CatchComboDisplay
            {
                AutoSizeAxes = Axes.None,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(2f),
            });
        });

        [Test]
        public void TestCatchComboCounter()
        {
            AddRepeatStep("perform hit", () => performJudgement(HitResult.Perfect), 20);
            AddStep("perform miss", () => performJudgement(HitResult.Miss));
        }

        [Test]
        public void TestNoDefaultComboCounterYet()
        {
            // Ensure the default osu!lazer skin has no combo counter yet and is not somehow using the legacy one.
            AddAssert("default skin has no counter yet", () =>
                CreatedDrawables.Cast<CatchComboDisplay>().First().ComboCounter == null);
        }

        private void performJudgement(HitResult type, Judgement judgement = null)
        {
            var result = new JudgementResult(new HitObject(), judgement ?? new Judgement()) { Type = type };
            scoreProcessor.ApplyResult(result);

            foreach (var counter in CreatedDrawables
                                    .Cast<CatchComboDisplay>()
                                    .Where(d => d.ComboCounter != null))
                counter.OnNewResult(null, result);
        }
    }
}
