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

extern alias MbUnit2;

using System.Collections.Generic;
using Gallio.Data;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(ScalarDataRow<>))]
    public class ScalarDataRowTest
    {
        [Test]
        public void HasNoMetadataIfNullSpecifiedInConstructor()
        {
            ScalarDataRow<object> row = new ScalarDataRow<object>(null, null);
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>(row.GetMetadata());
            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void ContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            ScalarDataRow<object> row = new ScalarDataRow<object>("abc", metadata);

            Assert.AreSame(metadata, row.GetMetadata());
        }

        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfBindingIsNull()
        {
            ScalarDataRow<object> row = new ScalarDataRow<object>(null, null);
            row.GetValue(null);
        }

        [RowTest]
        [Row(null, null, ExpectedException=typeof(DataBindingException))]
        [Row(null, 1, ExpectedException=typeof(DataBindingException))]
        [Row(null, -1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", null, ExpectedException=typeof(DataBindingException))]
        [Row("abc", 1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", -1, ExpectedException=typeof(DataBindingException))]
        [Row(null, 0)]
        [Row("abc", 0)]
        public void GetValueReturnsValueOnlyIfTheBindingIndexIsZero(string path, object index)
        {
            ScalarDataRow<object> row = new ScalarDataRow<object>(42, null);
            object value = row.GetValue(new SimpleDataBinding(typeof(string), path, (int?)index));

            Assert.AreEqual(42, value);
        }
    }
}