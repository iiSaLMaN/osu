// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Overlays.Dialog;

namespace osu.Game.Overlays.BeatmapSet
{
    public class BeatmapDownloadCancelDialog : PopupDialog
    {
        [Resolved(canBeNull: true)]
        private BeatmapManager beatmaps { get; set; }

        public BeatmapDownloadCancelDialog(BeatmapSetInfo beatmap)
        {
            HeaderText = "Confirm cancellation of";
            BodyText = $@"{beatmap.Metadata?.Artist} - {beatmap.Metadata?.Title}";

            Icon = FontAwesome.Solid.Times;
            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = "Yes. Cancel it please.",
                    Action = () => beatmaps?.GetExistingDownload(beatmap).Cancel(),
                },
                new PopupDialogCancelButton
                {
                    Text = "No, keep downloading."
                }
            };
        }
    }
}
