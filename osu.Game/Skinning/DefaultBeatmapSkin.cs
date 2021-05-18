// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Textures;
using osu.Framework.Graphics.Textures;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.IO;

namespace osu.Game.Skinning
{
    /// <summary>
    /// The default skin for a beatmap.
    /// </summary>
    /// <remarks>
    /// This doesn't yet have any implementation so it always returns null and falls back to the currently selected skin.
    /// </remarks>
    public class DefaultBeatmapSkin : DefaultSkin, IBeatmapSkin
    {
        public DefaultBeatmapSkin(BeatmapInfo beatmapInfo, IStorageResourceProvider resources)
            : base(IBeatmapSkin.CreateSkinInfo(beatmapInfo), resources)
        {
        }

        public override Texture GetTexture(string componentName, WrapMode wrapModeS, WrapMode wrapModeT) => null;

        public override ISample GetSample(ISampleInfo sampleInfo) => null;

        public override Drawable GetDrawableComponent(ISkinComponent component) => null;

        public override IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup) => null;
    }
}
