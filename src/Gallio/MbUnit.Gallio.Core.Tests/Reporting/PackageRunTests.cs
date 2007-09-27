// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

extern alias MbUnit2;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Core.Reporting;
using MbUnit.Framework.Xml;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(PackageRun))]
    [Author("Vadim")]
    public class PackageRunTests
    {
        private PackageRun _packageRun;

        [SetUp]
        public void TestStart()
        {
            _packageRun = new PackageRun();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.AreEqual(0, _packageRun.TestRuns.Count);
        }

        [Test]
        public void StatisticsGetTest()
        {
            Assert.IsNotNull(_packageRun.Statistics);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StatisticsSetWithNullValueTest()
        {
            _packageRun.Statistics = null;
        }

        [Test]
        public void StatisticsSetTest()
        {
            PackageRunStatistics statistics = new PackageRunStatistics();
            _packageRun.Statistics = statistics;
            Assert.AreSame(statistics, _packageRun.Statistics);
        }

        [Test]
        public void ReportTypeIsXmlSerializable()
        {
            XmlSerializationAssert.IsXmlSerializable(_packageRun.GetType());
        }

        [Test]
        public void RoundTripXmlSerialization()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PackageRun));
            StringWriter writer = new StringWriter();
            _packageRun.TestRuns.Add(new TestRun("testId", new StepRun("stepId", "stepName", "stepFullName")));
            serializer.Serialize(writer, _packageRun);

            PackageRun deserializedPackageRun = (PackageRun)serializer.Deserialize(new StringReader(writer.ToString()));
            CoreAssert.AreEqual(_packageRun, deserializedPackageRun);
        }

        [Test]
        public void StepRuns()
        {
            StepRun stepRun = new StepRun("stepId", "stepName", "stepFullName");
            stepRun.Children.Add(new StepRun("childId", "child", "fullName"));
            TestRun testRun = new TestRun("testId", stepRun);
            _packageRun.TestRuns.Add(testRun);

            CollectionAssert.AreElementsEqual(new StepRun[] { stepRun, stepRun.Children[0] },
                _packageRun.StepRuns);
        }
    }
}