// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online;
using osu.Game.Online.API;
using osu.Game.Overlays.Direct;
using osu.Game.Users;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays.BeatmapSet.Buttons
{
    public class HeaderDownloadButton : BeatmapDownloadTrackingComposite, IHasTooltip
    {
        private readonly bool noVideo;

        public string TooltipText => button.Enabled.Value ? "Download this beatmap" : "Login to download";

        private readonly IBindable<User> localUser = new Bindable<User>();

        private ShakeContainer shakeContainer;
        private HeaderButton button;

        public HeaderDownloadButton(BeatmapSetInfo beatmapSet, bool noVideo = false)
            : base(beatmapSet)
        {
            this.noVideo = noVideo;

            Width = 120;
            RelativeSizeAxes = Axes.Y;
        }

        private FillFlowContainer textSprites;
        private DownloadProgressBar progressBar;
        private SpriteIcon icon1, icon2;

        [BackgroundDependencyLoader(true)]
        private void load(IAPIProvider api, BeatmapManager beatmaps, DialogOverlay dialogOverlay)
        {
            AddRangeInternal(new Drawable[]
            {
                shakeContainer = new ShakeContainer
                {
                    Depth = -1,
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        button = new HeaderButton { RelativeSizeAxes = Axes.Both },
                        new Container
                        {
                            // cannot nest inside here due to the structure of button (putting things in its own content).
                            // requires framework fix.
                            Padding = new MarginPadding { Horizontal = 10 },
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                textSprites = new TestFillFlowContainer
                                {
                                    Depth = -1,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                },
                                new Container
                                {
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.CentreRight,
                                    AutoSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Right = 25 },
                                    Children = new[]
                                    {
                                        icon1 = new SpriteIcon
                                        {
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.Solid.Download,
                                            Size = new Vector2(16),
                                        },
                                        icon2 = new SpriteIcon
                                        {
                                            X = 8,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Icon = FontAwesome.Solid.TimesCircle,
                                            Size = Vector2.Zero,
                                        }
                                    }
                                }
                            }
                        },
                        progressBar = new DownloadProgressBar(BeatmapSet.Value)
                        {
                            Depth = -2,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                        },
                    },
                },
            });

            button.Action = () =>
            {
                switch (State.Value)
                {
                    case DownloadState.NotDownloaded:
                        beatmaps.Download(BeatmapSet.Value, noVideo);
                        break;

                    case DownloadState.Downloading:
                        dialogOverlay?.Push(new BeatmapDownloadCancelDialog(BeatmapSet.Value));
                        break;

                    default:
                        shakeContainer.Shake();
                        break;
                }
            };

            localUser.BindTo(api.LocalUser);
            localUser.BindValueChanged(userChanged, true);
            button.Enabled.BindValueChanged(enabledChanged, true);

            State.BindValueChanged(_ => updateState(), true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateState();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateState();
            base.OnHoverLost(e);
        }

        private OsuSpriteText downloadingText;

        private void updateState()
        {
            switch (State.Value)
            {
                case DownloadState.Downloading:
                    textSprites.Child = downloadingText = new OsuSpriteText { Text = IsHovered ? "Cancel download" : "Downloading...", Font = OsuFont.GetFont(size: 13, weight: FontWeight.Bold) };
                    break;

                case DownloadState.LocallyAvailable:
                    this.FadeOut(200);
                    break;

                case DownloadState.NotDownloaded:
                    this.FadeIn(200);
                    break;
            }
        }

        private void userChanged(ValueChangedEvent<User> e) => button.Enabled.Value = !(e.NewValue is GuestUser);

        private void enabledChanged(ValueChangedEvent<bool> e) => this.FadeColour(e.NewValue ? Color4.White : Color4.Gray, 200, Easing.OutQuint);

        private class TestFillFlowContainer : FillFlowContainer
        {
            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
            }
        }
    }
}
