// SPDX-License-Identifier: BSD-2-Clause

using System.Linq;
using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Assets;
using ClassicUO.Resources;
using System.Collections.Generic;
using ClassicUO.Utility;

namespace ClassicUO.Game.UI.Gumps.CharCreation
{
    public class CreateCharTradeGump : Gump
    {
        private readonly HSliderBar[] _attributeSliders;
        private readonly PlayerMobile _character;
        private readonly Combobox[] _skillsCombobox;
        private readonly HSliderBar[] _skillSliders;
        private readonly List<SkillEntry> _skillList;



        public CreateCharTradeGump(World world, PlayerMobile character, ProfessionInfo profession) : base(world, 0, 0)
        {
            _character = character;

            foreach (Skill skill in _character.Skills)
            {
                skill.ValueFixed = 0;
                skill.BaseFixed = 0;
                skill.CapFixed = 0;
                skill.Lock = Lock.Locked;
            }

            Add
            (
                new ResizePic(2620)
                {
                    X = 700, Y = 630, Width = 470, Height = 372
                }
            );


            bool isAsianLang = string.Compare(Settings.GlobalSettings.Language, "CHT", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(Settings.GlobalSettings.Language, "KOR", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(Settings.GlobalSettings.Language, "JPN", StringComparison.InvariantCultureIgnoreCase) == 0;

            bool unicode = isAsianLang;
            byte font = (byte)(isAsianLang ? 1 : 2);
            ushort hue = (ushort)(isAsianLang ? 0xFFFF : 0x35);

            // title text
            //TextLabelAscii(AControl parent, int x, int y, int font, int hue, string text, int width = 400)
            Add
            (
                new Label(Client.Game.UO.FileManager.Clilocs.GetString(3000326), unicode, hue, font: font)
                {
                    X = 714, Y = 660
                }
            );

            // strength, dexterity, intelligence
            Add
            (
                new Label(Client.Game.UO.FileManager.Clilocs.GetString(3000111), unicode, 53, font: 1)
                {
                    X = 724, Y = 700
                }
            );

            Add
            (
                new Label(Client.Game.UO.FileManager.Clilocs.GetString(3000112), unicode, 53, font: 1)
                {
                    X = 724, Y = 780
                }
            );

            Add
            (
                new Label(Client.Game.UO.FileManager.Clilocs.GetString(3000113), unicode, 53, font: 1)
                {
                    X = 724, Y = 860
                }
            );

            // sliders for attributes
            _attributeSliders = new HSliderBar[3];

            (int[,] defSkillsValues, int[] defStatsValues) = ProfessionInfo.GetDefaults(Client.Game.UO.Version);

            Add
            (
                _attributeSliders[0] = new HSliderBar
                (
                    730,
                    726,
                    93,
                    10,
                    60,
                    defStatsValues[0],
                    HSliderBarStyle.MetalWidgetRecessedBar,
                    true,
                    color: 0xFFFF
                )
            );

            Add
            (
                _attributeSliders[1] = new HSliderBar
                (
                    730,
                    806,
                    93,
                    10,
                    60,
                    defStatsValues[1],
                    HSliderBarStyle.MetalWidgetRecessedBar,
                    true,
                    color: 0xFFFF
                )
            );

            Add
            (
                _attributeSliders[2] = new HSliderBar
                (
                    730,
                    886,
                    93,
                    10,
                    60,
                    defStatsValues[2],
                    HSliderBarStyle.MetalWidgetRecessedBar,
                    true,
                    color: 0xFFFF
                )
            );

            LockedFeatureFlags clientFlags = World.ClientLockedFeatures.Flags;

            _skillList = Client.Game.UO.FileManager.Skills.SortedSkills
                         .Where(s =>
                                     // All standard client versions ignore these skills by defualt
                                     //s.Index != 26 && // MagicResist
                                     s.Index != 47 && // Stealth
                                     s.Index != 48 && // RemoveTrap
                                     s.Index != 54 && // Spellweaving
                                     (character.Race == RaceType.GARGOYLE || s.Index != 57) // Throwing for gargoyle only
                                 )
                          .Where(s =>
                                    clientFlags.HasFlag(LockedFeatureFlags.AOS) ||
                                    (
                                        s.Index != 51 && // Chivlary
                                        s.Index != 50 && // Focus
                                        s.Index != 49    // Necromancy
                                    )
                                )

                          .Where(s =>
                                    clientFlags.HasFlag(LockedFeatureFlags.SE) ||
                                    (
                                        s.Index != 52 && // Bushido
                                        s.Index != 53    // Ninjitsu
                                    )
                                )

                          .Where(s =>
                                    clientFlags.HasFlag(LockedFeatureFlags.SA) ||
                                    (
                                        s.Index != 55 && // Mysticism
                                        s.Index != 56    // Imbuing
                                    )
                                )
                         .ToList();

            // do not include archer if it's a gargoyle
            if (character.Race == RaceType.GARGOYLE)
            {
                SkillEntry archeryEntry = _skillList.FirstOrDefault(s => s.Index == 31);
                if (archeryEntry != null)
                {
                    _skillList.Remove(archeryEntry);
                }
            }

            string[] skillNames = _skillList.Select(s => s.Name).ToArray();

            int y = 710;
            _skillSliders = new HSliderBar[CharCreationGump._skillsCount];
            _skillsCombobox = new Combobox[CharCreationGump._skillsCount];

            for (int i = 0; i < CharCreationGump._skillsCount; i++)
            {
                Add
                (
                    _skillsCombobox[i] = new Combobox
                    (
                        970,
                        y,
                        182,
                        skillNames,
                        -1,
                        200,
                        false,
                        "Click here"
                    )
                );

                Add
                (
                    _skillSliders[i] = new HSliderBar
                    (
                        970,
                        y + 32,
                        93,
                        0,
                        50,
                        defSkillsValues[i, 1],
                        HSliderBarStyle.MetalWidgetRecessedBar,
                        true,
                        color: 0xFFFF
                    )
                );

                y += 70;
            }

            Add
            (
                new Button((int) Buttons.Prev, 0x15A1, 0x15A3, 0x15A2)
                {
                    X = 1100, Y = 975, ButtonAction = ButtonAction.Activate
                }
            );

            Add
            (
                new Button((int) Buttons.Next, 0x15A4, 0x15A6, 0x15A5)
                {
                    X = 1130, Y = 975, ButtonAction = ButtonAction.Activate
                }
            );

            for (int i = 0; i < _attributeSliders.Length; i++)
            {
                for (int j = 0; j < _attributeSliders.Length; j++)
                {
                    if (i != j)
                    {
                        _attributeSliders[i].AddParisSlider(_attributeSliders[j]);
                    }
                }
            }

            for (int i = 0; i < _skillSliders.Length; i++)
            {
                for (int j = 0; j < _skillSliders.Length; j++)
                {
                    if (i != j)
                    {
                        _skillSliders[i].AddParisSlider(_skillSliders[j]);
                    }
                }
            }
        }

        public override void OnButtonClick(int buttonID)
        {
            CharCreationGump charCreationGump = UIManager.GetGump<CharCreationGump>();

            switch ((Buttons) buttonID)
            {
                case Buttons.Prev:
                    charCreationGump.StepBack();

                    break;

                case Buttons.Next:

                    if (ValidateValues())
                    {
                        for (int i = 0; i < _skillsCombobox.Length; i++)
                        {
                            if (_skillsCombobox[i].SelectedIndex != -1)
                            {
                                Skill skill = _character.Skills[_skillList[_skillsCombobox[i].SelectedIndex].Index];
                                skill.ValueFixed = (ushort) _skillSliders[i].Value;
                                skill.BaseFixed = 0;
                                skill.CapFixed = 0;
                                skill.Lock = Lock.Locked;
                            }
                        }

                        _character.Strength = (ushort) _attributeSliders[0].Value;
                        _character.Intelligence = (ushort) _attributeSliders[1].Value;
                        _character.Dexterity = (ushort) _attributeSliders[2].Value;

                        charCreationGump.SetAttributes(true);
                    }

                    break;
            }

            base.OnButtonClick(buttonID);
        }

        private bool ValidateValues()
        {
            if (_skillsCombobox.All(s => s.SelectedIndex >= 0))
            {
                int duplicated = _skillsCombobox.GroupBy(o => o.SelectedIndex).Count(o => o.Count() > 1);

                if (duplicated > 0)
                {
                    UIManager.GetGump<CharCreationGump>()?.ShowMessage(Client.Game.UO.FileManager.Clilocs.GetString(1080032));

                    return false;
                }
            }
            else
            {
                UIManager.GetGump<CharCreationGump>()?.ShowMessage(Client.Game.UO.Version <= ClientVersion.CV_5090 ? ResGumps.YouMustHaveThreeUniqueSkillsChosen : Client.Game.UO.FileManager.Clilocs.GetString(1080032));

                return false;
            }

            return true;
        }

        private enum Buttons
        {
            Prev,
            Next
        }
    }
}