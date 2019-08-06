// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Leaderboards;
using osu.Game.Scoring;
using osu.Game.Users;

namespace osu.Game.Screens.Play
{
    public class InGameLeaderboard : CompositeDrawable
    {
        /// <summary>
        /// Whether this in-game leaderboard has scores.
        /// </summary>
        public bool HasScores => leaderboard?.Scores != null && leaderboard?.Scores.Count() > 1;

        public readonly BindableDouble PlayerCurrentScore = new BindableDouble();

        protected readonly InGameScoreContainer ScoresContainer;
        protected InGameScoreItem PlayerScoreItem;

        private bool isRealTime;

        private User playerUser;

        public User PlayerUser
        {
            get => playerUser;
            set
            {
                playerUser = value;

                if (PlayerScoreItem == null)
                    PlayerScoreItem = ScoresContainer.AddRealTimePlayer(PlayerCurrentScore, playerUser);
            }
        }

        private ILeaderboard leaderboard;

        public ILeaderboard Leaderboard
        {
            get => leaderboard;
            set
            {
                leaderboard = value;

                if (leaderboard == null)
                    return;

                ClearScores();

                // todo: isRealTime should have an actual value once real-time leaderboards are supported
                isRealTime = false;

                ScoresContainer.DeclareNewPosition = (!leaderboard.IsOnlineScope || (leaderboard.Scores?.Count() ?? 0) < 50) || isRealTime;

                if (!isRealTime)
                    addLeaderboardScores();
            }
        }

        public InGameLeaderboard()
        {
            AutoSizeAxes = Axes.Y;

            InternalChild = ScoresContainer = new InGameScoreContainer
            {
                OnScoreChange = updateLeaderboard
            };
        }

        protected void ClearScores() => ScoresContainer.RemoveAll(s => s != PlayerScoreItem);

        private List<ScoreInfo> leaderboardScores;

        /// <summary>
        /// Number of score items to show between a gap.
        /// </summary>
        private const int scores_between_gap = 3;

        private void addLeaderboardScores()
        {
            leaderboardScores = leaderboard?.Scores?.OrderByDescending(s => s.TotalScore).ToList();

            if (leaderboardScores == null)
                return;

            for (int i = 0; i < leaderboardScores.Count; i++)
            {
                if (i >= scores_between_gap && i < leaderboardScores.Count - scores_between_gap)
                    i = leaderboardScores.Count - scores_between_gap;

                ScoresContainer.AddScore(leaderboardScores[i], i + 1);
            }

            //PlayerScoreItem?.OnScoreChange.Invoke();
        }

        private const int default_maximum_scores = scores_between_gap * 2 + 1;

        private void checkScrollUp()
        {
            var orderedScores = ScoresContainer.OrderByDescending(s => s.ScorePosition.HasValue).ThenBy(s => s.ScorePosition).ToList();
            var bottomPosition = orderedScores[scores_between_gap].ScorePosition ?? 0;

            if (bottomPosition <= 0)
                return;

            while (bottomPosition != scores_between_gap + 1 && PlayerScoreItem.ScorePosition - bottomPosition < scores_between_gap)
                ScoresContainer.AddScore(leaderboardScores[bottomPosition - 2], --bottomPosition);

            if (ScoresContainer.Count > default_maximum_scores)
                ScoresContainer.RemoveRange(ScoresContainer.OrderByDescending(s => s.ScorePosition.HasValue).ThenBy(s => s.ScorePosition).Skip(default_maximum_scores));
        }

        private void checkScrollDown()
        {
            var playerPosition = PlayerScoreItem.ScorePosition ?? 0;

            if (playerPosition <= 0 || playerPosition > leaderboardScores.Count)
                return;

            var bottomScore = leaderboardScores[playerPosition - 1];
            while (playerPosition <= leaderboardScores.Count && bottomScore.TotalScore >= PlayerScoreItem.TotalScore)
                ScoresContainer.AddScore(bottomScore = leaderboardScores[playerPosition - 1], playerPosition++);

            if (ScoresContainer.Count > default_maximum_scores)
            {
                ScoresContainer.RemoveRange(ScoresContainer.OrderByDescending(s => s.ScorePosition.HasValue).ThenBy(s => s.ScorePosition).Skip(scores_between_gap).Take(ScoresContainer.Count - default_maximum_scores));

                // it's possible for the player to climb one score item above, check if need to scroll up again.
                checkScrollUp();
            }
        }

        private void updateLeaderboard()
        {
            if (isRealTime || leaderboardScores == null || ScoresContainer.Count < default_maximum_scores)
                return;

            checkScrollUp();
            checkScrollDown();

            ScoresContainer.DeclareNewPosition = PlayerScoreItem.ScorePosition <= leaderboardScores.Count;
        }
    }
}
