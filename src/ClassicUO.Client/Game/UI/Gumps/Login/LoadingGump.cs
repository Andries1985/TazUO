// SPDX-License-Identifier: BSD-2-Clause

using System;
using ClassicUO.Configuration;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Assets;
using SDL3;

namespace ClassicUO.Game.UI.Gumps.Login
{
    [Flags]
    public enum LoginButtons
    {
        None = 1,
        OK = 2,
        Cancel = 4
    }

    public class LoadingGump : Gump
    {
        private readonly Action<int> _buttonClick;
        private readonly Label _label;

        public LoadingGump(World world, string labelText, LoginButtons showButtons, Action<int> buttonClick = null) : base(world, 0, 0)
        {
            _buttonClick = buttonClick;
            CanCloseWithRightClick = false;
            CanCloseWithEsc = false;

            bool isAsianLang = string.Compare(Settings.GlobalSettings.Language, "CHT", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(Settings.GlobalSettings.Language, "KOR", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(Settings.GlobalSettings.Language, "JPN", StringComparison.InvariantCultureIgnoreCase) == 0;

            bool unicode = isAsianLang;
            byte font = (byte)(isAsianLang ? 1 : 2);
            ushort hue = (ushort)(isAsianLang ? 0xFFFF : 0x0386);

            _label = new Label
            (
                labelText,
                unicode,
                hue,
                326,
                font,
                align: TEXT_ALIGN_TYPE.TS_CENTER
            )
            {
                X = 780,
                Y = 470
            };

            Add
            (
                new ResizePic(0x0A3C)
                {
                    X = 760, Y = 425, Width = 366, Height = 212
                }
            );

            Add(_label);

            if (showButtons == LoginButtons.OK)
            {
                Add
                (
                    new Button((int) LoginButtons.OK, 0x0481, 0x0483, 0x0482)
                    {
                        X = 930, Y = 600, ButtonAction = ButtonAction.Activate
                    }
                );
            }
            else if (showButtons == LoginButtons.Cancel)
            {
                Add
                (
                    new Button((int) LoginButtons.Cancel, 0x047E, 0x0480, 0x047F)
                    {
                        X = 930,
                        Y = 600,
                        ButtonAction = ButtonAction.Activate
                    }
                );
            }
            else if (showButtons == (LoginButtons.OK | LoginButtons.Cancel))
            {
                Add
                (
                    new Button((int) LoginButtons.OK, 0x0481, 0x0483, 0x0482)
                    {
                        X = 888, Y = 600, ButtonAction = ButtonAction.Activate
                    }
                );

                Add
                (
                    new Button((int) LoginButtons.Cancel, 0x047E, 0x0480, 0x047F)
                    {
                        X = 972, Y = 600, ButtonAction = ButtonAction.Activate
                    }
                );
            }
        }

        public void SetText(string text) => _label.Text = text;

        public override void OnKeyDown(SDL.SDL_Keycode key, SDL.SDL_Keymod mod)
        {
            if (key == SDL.SDL_Keycode.SDLK_KP_ENTER || key == SDL.SDL_Keycode.SDLK_RETURN)
            {
                OnButtonClick((int) LoginButtons.OK);
            }
        }


        public override void OnButtonClick(int buttonID)
        {
            _buttonClick?.Invoke(buttonID);
            base.OnButtonClick(buttonID);
        }
    }
}
