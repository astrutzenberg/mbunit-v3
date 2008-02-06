// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class PerformanceMonitor : DockContent
    {
        public PerformanceMonitor()
        {
            InitializeComponent();
            graphFilter.SelectedIndex = 0;
            // refresh graph
            testResultsGraph.DisplayGraph();
        }

        public void UpdateTestResults(string testOutcome, string typeName, string namespaceName, string assemblyName)
        {
            testResultsGraph.UpdateTestResults(testOutcome, typeName, namespaceName, assemblyName);
        }

        private void graphFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            testResultsGraph.Mode = (string)graphFilter.SelectedItem;
        }
    }
}