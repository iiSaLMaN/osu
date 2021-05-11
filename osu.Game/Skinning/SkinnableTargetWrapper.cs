// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Skinning
{
    /// <summary>
    /// A container which is serialised and can encapsulate multiple skinnable elements into a single return type.
    /// Will also optionally apply default cross-element layout dependencies when initialised from a non-deserialised source.
    /// </summary>
    public class SkinnableTargetWrapper : Container, ISkinSerialisable
    {
        private readonly Action<Container> applyDefaults;

        /// <summary>
        /// Construct a wrapper with defaults that should be applied once.
        /// </summary>
        /// <param name="applyDefaults">A function with default to apply after the initial layout (ie. consuming autosize)</param>
        public SkinnableTargetWrapper(Action<Container> applyDefaults)
            : this()
        {
            this.applyDefaults = applyDefaults;
        }

        public SkinnableTargetWrapper()
        {
            RelativeSizeAxes = Axes.Both;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // schedule is required to allow children to run their LoadComplete and take on their correct sizes.
            Schedule(() => applyDefaults?.Invoke(this));
        }
    }
}