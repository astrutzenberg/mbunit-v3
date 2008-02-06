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
using System.Text;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    [TestFixture]
    [TestsOn(typeof(CorrelatedExceptionEventArgs))]
    public class CorrelatedExceptionEventArgsTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenMessageIsNull()
        {
            new CorrelatedExceptionEventArgs(null, new Exception(), "reported by", true);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenExceptionIsNull()
        {
            new CorrelatedExceptionEventArgs("foo", null, "reported by", true);
        }

        [Test, ExpectedArgumentNullException]
        public void AddCorrelationThrowsWhenMessageIsNull()
        {
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", new Exception(), "reported by", true);
            e.AddCorrelationMessage(null);
        }

        [Test]
        public void PropertiesContainSameValuesAsWereSetInTheConstructor()
        {
            Exception ex = new Exception();
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", ex, "reported by", true);

            Assert.AreEqual("foo", e.Message);
            Assert.AreEqual("reported by", e.ReporterStackTrace);
            Assert.AreSame(ex, e.Exception);
            Assert.IsTrue(e.IsRecursive);
        }

        [Test]
        public void AddCorrelationAppendsToTheMessage()
        {
            CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs("foo", new Exception(), "reported by", false);
            e.AddCorrelationMessage("bar");

            Assert.AreEqual("foo\nbar", e.Message);
        }
    }
}