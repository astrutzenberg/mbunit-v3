// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System;
using System.Collections.Generic;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Model
{
    /// <summary>
    /// A test framework implementation used when no test framework supports a given test file.
    /// </summary>
    public class FallbackTestFramework : BaseTestFramework
    {
        /// <summary>
        /// The test framework option key used to explain why the fallback test framework is being used.
        /// </summary>
        /// <remarks>
        /// The associated value should be a plain test message describing the problem with
        /// the test file.
        /// </remarks>
        public static readonly string FallbackExplanationKey = "FallbackExplanation";

        /// <inheritdoc />
        sealed public override TestDriverFactory GetTestDriverFactory()
        {
            return CreateTestDriver;
        }

        private static ITestDriver CreateTestDriver(
            IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandles,
            TestFrameworkOptions testFrameworkOptions,
            ILogger logger)
        {
            return new FallbackTestDriver(testFrameworkOptions);
        }
    }
}
