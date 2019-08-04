// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Online.Leaderboards;
using osu.Game.Scoring;
using osu.Game.Screens.Play;
using osu.Game.Users;
using osuTK;

namespace osu.Game.Tests.Visual.Gameplay
{
    [TestFixture]
    public class TestSceneInGameLeaderboard : OsuTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(InGameLeaderboard),
            typeof(InGameScoreContainer),
        };

        private readonly TestInGameLeaderboard leaderboard;
        private readonly BindableDouble playerScore;

        public TestSceneInGameLeaderboard()
        {
            Add(leaderboard = new TestInGameLeaderboard
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(2),
                RelativeSizeAxes = Axes.X,
                PlayerCurrentScore = { BindTarget = playerScore = new BindableDouble(1222333) }
            });

            AddStep("add player user", () => leaderboard.PlayerUser = new User { Username = "You" });
            AddSliderStep("set player score", 50, 5000000, 1222333, v => playerScore.Value = v);
        }

        [SetUp]
        public void SetUp()
        {
            leaderboard.ClearScores();
            leaderboard.PlayerPosition = 1;
            playerScore.Value = 1222333;
        }

        [Test]
        public void TestPlayerScore()
        {
            var player2Score = new BindableDouble(1234567);
            var player3Score = new BindableDouble(1111111);

            AddStep("add player 2", () => leaderboard.AddDummyPlayer(player2Score, "Player 2"));
            AddStep("add player 3", () => leaderboard.AddDummyPlayer(player3Score, "Player 3"));

            AddAssert("is player 2 position #1", () => leaderboard.CheckPositionByUsername("Player 2", 1));
            AddAssert("is player position #2", () => leaderboard.CheckPositionByUsername("You", 2));
            AddAssert("is player 3 position #3", () => leaderboard.CheckPositionByUsername("Player 3", 3));

            AddStep("set score above player 3", () => player2Score.Value = playerScore.Value - 500);
            AddAssert("is player position #1", () => leaderboard.CheckPositionByUsername("You", 1));
            AddAssert("is player 2 position #2", () => leaderboard.CheckPositionByUsername("Player 2", 2));
            AddAssert("is player 3 position #3", () => leaderboard.CheckPositionByUsername("Player 3", 3));

            AddStep("set score below players", () => player2Score.Value = playerScore.Value - 123456);
            AddAssert("is player position #1", () => leaderboard.CheckPositionByUsername("You", 1));
            AddAssert("is player 3 position #2", () => leaderboard.CheckPositionByUsername("Player 3", 2));
            AddAssert("is player 2 position #3", () => leaderboard.CheckPositionByUsername("Player 2", 3));
        }

        private List<ScoreInfo> setLeaderboard(int scoreCount, bool onlineScope)
        {
            var scores = new List<ScoreInfo>();
            for (int i = 2; i < scoreCount + 2; i++)
                scores.Add(new ScoreInfo { UserString = $"Player {i}", TotalScore = 1000000 * i });

            AddStep($"set leaderboard {(onlineScope ? "online" : "offline")}-{scoreCount}", () => leaderboard.Leaderboard = new TestLeaderboard { Scores = scores, SetOnlineScope = onlineScope });

            return scores.OrderByDescending(s => s.TotalScore).ToList();
        }

        [Test]
        public void TestScoresScrolling()
        {
            var scores = setLeaderboard(7, false);

            void isScrolled(bool down)
            {
                AddAssert($"{(down ? "no " : "")}player position #4", () => leaderboard.PositionExists(4) != down);
                AddAssert($"{(down ? "" : "no ")}player position #8", () => leaderboard.PositionExists(8) == down);
            }

            isScrolled(true);

            AddStep("set player above player 4", () => playerScore.Value = scores[4].TotalScore + 500000);
            isScrolled(false);

            AddStep("set player below player 2", () => playerScore.Value = scores[6].TotalScore - 500000);
            isScrolled(true);
        }

        [Test]
        public void TestDeclareNewPosition()
        {
            AddStep("set player position null", () => leaderboard.PlayerPosition = null);

            var scores = setLeaderboard(50, true);

            AddStep("set player score below last", () => playerScore.Value = 1000000);
            AddAssert("no position declared for player", () => leaderboard.PlayerPosition == null);

            setLeaderboard(50, false);

            AddStep("set player score below last", () => playerScore.Value = 1000000);
            AddAssert("new position declared for player", () => leaderboard.PlayerPosition == scores.Count + 1);
        }

        private class TestInGameLeaderboard : InGameLeaderboard
        {
            public int? PlayerPosition
            {
                get => PlayerScoreItem.ScorePosition;
                set => PlayerScoreItem.ScorePosition = value;
            }

            public new void ClearScores() => base.ClearScores();

            public bool PositionExists(int position) => ScoresContainer.Any(s => s.ScorePosition == position);

            public bool CheckPositionByUsername(string username, int? estimatedPosition)
            {
                var scoreItem = ScoresContainer.FirstOrDefault(i => i.User.Username == username);

                return scoreItem != null && scoreItem.ScorePosition == estimatedPosition;
            }

            public void AddDummyPlayer(BindableDouble currentScore, string username) => ScoresContainer.AddRealTimePlayer(currentScore, new User { Username = username });
        }

        private class TestLeaderboard : ILeaderboard
        {
            public IEnumerable<ScoreInfo> Scores { get; set; }

            public bool SetOnlineScope;
            public bool IsOnlineScope => SetOnlineScope;

            public void RefreshScores()
            {
            }
        }
    }
}
