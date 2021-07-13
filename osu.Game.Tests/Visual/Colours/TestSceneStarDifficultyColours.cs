// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterfaceV2;
using osuTK;

namespace osu.Game.Tests.Visual.Colours
{
    public class TestSceneStarDifficultyColours : OsuTestScene
    {
        [Resolved]
        private OsuColour colours { get; set; }

        [Test]
        public void TestColours()
        {
            AddStep("load colour displays", () =>
            {
                Child = new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5f),
                    ChildrenEnumerable = Enumerable.Range(0, 10).Select(i => new FillFlowContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(10f),
                        ChildrenEnumerable = Enumerable.Range(0, 10).Select(j => new ColourDisplay
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Current =
                            {
                                Value = colours.ForStarDifficulty(1f * i + 0.1f * j),
                                Disabled = true,
                            },
                            ColourName = $"*{(1f * i + 0.1f * j):0.00}",
                            CircleSize = new Vector2(75f, 25f),
                        })
                    })
                };
            });
        }
    }
}
