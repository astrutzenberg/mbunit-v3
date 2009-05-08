// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using Gallio.Common.Reflection;

namespace Gallio.Icarus
{
    public partial class CodeWindow : DockWindow
    {
        private readonly CodeLocation codeLocation;

        public CodeWindow(CodeLocation codeLocation)
        {
            this.codeLocation = codeLocation;

            InitializeComponent();

            if (codeLocation == CodeLocation.Unknown)
                Text = "(unknown)";
            else
            {
                Text = Path.GetFileName(codeLocation.Path) ?? "(unknown)";
                textEditorControl.LoadFile(codeLocation.Path);
                if (codeLocation.Line != 0)
                    textEditorControl.ActiveTextAreaControl.JumpTo(codeLocation.Line, codeLocation.Column);
            }

            textEditorControl.ShowEOLMarkers = false;
            textEditorControl.ShowHRuler = false;
            textEditorControl.ShowSpaces = false;
            textEditorControl.ShowTabs = false;
            textEditorControl.ShowVRuler = false;
            textEditorControl.ShowInvalidLines = false;

            textEditorControl.ActiveTextAreaControl.TextArea.KeyDown += (sender, e) =>
            {
                e.SuppressKeyPress = true;
            };
        }

        public void JumpTo(int line, int column)
        {
            textEditorControl.ActiveTextAreaControl.JumpTo(line, column);
        }

        public override string ToString()
        {
            return codeLocation.Path ?? "(unknown)";
        }
    }
}
