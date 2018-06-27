﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK.Graphics;
using osu.Framework.Input;
using System;

namespace osu.Game.Graphics.UserInterface
{
    /// <summary>
    /// A textbox which holds focus eagerly.
    /// </summary>
    public class FocusedTextBox : OsuTextBox
    {
        protected override Color4 BackgroundUnfocused => new Color4(10, 10, 10, 255);
        protected override Color4 BackgroundFocused => new Color4(10, 10, 10, 255);

        public Action Exit;

        private bool focus;
        public bool HoldFocus
        {
            get { return focus; }
            set
            {
                focus = value;
                if (!focus && HasFocus)
                    //todo: replace with KillInput after ppy/osu-framework#1656 is merged.
                    GetContainingInputManager().ChangeFocus(null);
            }
        }

        // We may not be focused yet, but we need to handle keyboard input to be able to request focus
        public override bool HandleKeyboardInput => HoldFocus || base.HandleKeyboardInput;

        protected override void OnFocus(InputState state)
        {
            base.OnFocus(state);
            BorderThickness = 0;
        }

        protected override void KillFocus()
        {
            base.KillFocus();
            Exit?.Invoke();
        }

        public override bool RequestsFocus => HoldFocus;
    }
}
