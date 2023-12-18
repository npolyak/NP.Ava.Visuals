﻿// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia.Controls;
using Avalonia.Styling;
using NP.Avalonia.Visuals.Controls;
using System;

namespace NP.Demos.MultiPlatformWindowDemo
{
    public class StartupTestWindow : CustomWindow
    {
        protected override Type StyleKeyOverride => typeof(StartupTestWindow);

        public void StartWindowWithCustomHeaderAndViewModel()
        {
            var window = new CustomWindow
            {
                DragOnBeginMove = false
            };

            window.Classes.Add("WindowContentHeaderAndViewModel");

            window.Show();
        }


        public void StartWindowWithCompleteHeaderRestyling()
        {
            var window = new CustomWindow
            {
                //Classes = new Classes("CompleteHeaderRestyling")
            };

            Classes.Add("CompleteHeaderRestyling");
            window.Show();
        }
    }
}
