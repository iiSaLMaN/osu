// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Input.Bindings;

namespace osu.Game.Skinning.Editor
{
    /// <summary>
    /// A container which handles loading a skin editor on user request.
    /// </summary>
    public class SkinEditorOverlay : OsuFocusedOverlayContainer
    {
        private const double transition_duration = 500;
        private const float visible_target_scale = 0.8f;

        private readonly ScalingContainer target;
        private SkinEditor skinEditor;

        public override bool IsPresent => base.IsPresent || Scheduler.HasPendingTasks;

        // Skin editor overlay needs to receive toggle key binding while hidden to activate.
        // And override BlockNonPositionalInput to block only when overlay is visible,
        // otherwise it would assume so via the overriden property, which is not correct now.
        // todo: this may need to be moved to a higher level with a conditional instead.
        public override bool PropagateNonPositionalInputSubTree => true;
        protected override bool BlockNonPositionalInput => State.Value == Visibility.Visible;

        protected override bool DimMainContent => false;

        [Resolved]
        private OsuColour colours { get; set; }

        public SkinEditorOverlay(ScalingContainer target)
        {
            this.target = target;
            RelativeSizeAxes = Axes.Both;
        }

        public override bool OnPressed(GlobalAction action)
        {
            switch (action)
            {
                case GlobalAction.ToggleSkinEditor:
                    ToggleVisibility();
                    return true;
            }

            return State.Value == Visibility.Visible && base.OnPressed(action);
        }

        protected override void UpdateState(ValueChangedEvent<Visibility> state)
        {
            if (state.NewValue == Visibility.Visible && !(skinEditor?.LoadState >= LoadState.Ready))
            {
                Hide();

                if (skinEditor == null)
                {
                    LoadComponentAsync(skinEditor = new SkinEditor(target), c =>
                    {
                        AddInternal(c);
                        Show();
                    });
                }

                return;
            }

            base.UpdateState(state);
        }

        protected override void PopIn() => Schedule(() =>
        {
            target.ScaleTo(visible_target_scale, transition_duration, Easing.OutQuint);

            target.Masking = true;
            target.BorderThickness = 5;
            target.BorderColour = colours.Yellow;
            target.AllowScaling = false;

            this.FadeIn(transition_duration, Easing.OutQuint);
        });

        protected override void PopOut() => Schedule(() =>
        {
            this.FadeOut(transition_duration, Easing.OutQuint);

            target.BorderThickness = 0;
            target.AllowScaling = true;

            target.ScaleTo(1, transition_duration, Easing.OutQuint).OnComplete(_ => target.Masking = false);
        });

        /// <summary>
        /// Exit any existing skin editor due to the game state changing.
        /// </summary>
        public void Reset()
        {
            skinEditor?.Hide();
            skinEditor?.Expire();
            skinEditor = null;
        }
    }
}
