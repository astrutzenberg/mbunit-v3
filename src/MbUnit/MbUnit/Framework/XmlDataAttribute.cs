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

using System;
using System.IO;
using System.Xml.XPath;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides data from XML contents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An XML data set selects nodes from an XML document using XPath
    /// expressions.  The selected nodes are returned as <see cref="XPathNavigator" /> objects.
    /// </para>
    /// <para>
    /// Two XPath expressions are used.
    /// <list type="bullet">
    /// <item>
    /// <term>Item Path</term>
    /// <description>An XPath expression that selects a set of nodes that are used to
    /// uniquely identify records.  For example, the item path might be used to select
    /// the containing element of each Book element in an XML document of Books.
    /// The item path is specified in the constructor.</description>
    /// </item>
    /// <item>
    /// <term>Binding Path</term>
    /// <description>An XPath expression that selects a node relative to the item path
    /// that contains a particular data value of interest.  For example, the binding path
    /// might be used to select the Author attribute of a Book element in an XML document of Books.
    /// The binding path is specified by the <see cref="DataBinding" />.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// <para>
    /// The XML may contain metadata at the row level by adding metadata elements in the
    /// http://www.gallio.org/ namespace containing metadata entries.  In the following example,
    /// the row values would be selected using an Item Path of "//row" and a Binding Path of "@value".
    /// Additionally, some rows would have metadata as specified.
    /// </para>
    /// <code>
    /// <![CDATA[
    /// <data>
    ///   <row value="somevalue">
    ///     <metadata xmlns="http://www.gallio.org/">
    ///       <entry key="Description" value="A row..." />
    ///       <entry key="Author" value="Me" />
    ///     </metadata>
    ///   </row>
    ///   <row value="anothervalue" />
    /// </data>
    /// ]]>
    /// </code>
    /// </example>
    /// <example>
    /// <para>
    /// This example reads data from an Embedded Resource called Data.xml within the
    /// same namespace as the test fixture.  Notice that the binding xpath expressions
    /// are specified for each parameter using the <see cref="BindAttribute" /> attribute.
    /// </para>
    /// <para>
    /// Data files:
    /// </para>
    /// <code>
    /// &lt;shoppingCart&gt;
    ///   &lt;item name="Bananas"&gt;
    ///     &lt;unitPrice&gt;0.85&lt;/unitPrice&gt;
    ///     &lt;quantity&gt;3&lt;/quantity&gt;
    ///   &lt;/item&gt;
    ///   &lt;item name="Cookies"&gt;
    ///     &lt;unitPrice&gt;0.10&lt;/unitPrice&gt;
    ///     &lt;quantity&gt;10&lt;/quantity&gt;
    ///   &lt;/item&gt;
    ///   &lt;-- Comment: mmm! --&gt;
    ///   &lt;item name="Shortbread"&gt;
    ///     &lt;unitPrice&gt;2.25&lt;/unitPrice&gt;
    ///     &lt;quantity&gt;1&lt;/quantity&gt;
    ///   &lt;/item&gt;
    /// &lt;/shoppingCart&gt;
    /// </code>
    /// <para>
    /// A simple test.
    /// </para>
    /// <code>
    /// public class AccountingTests
    /// {
    ///     [Test]
    ///     [XmlData("//item", ResourcePath = "Data.xml")]
    ///     public void ShoppingCartTotalWithSingleItem([Bind("@name")] string item, [Bind("unitPrice")] decimal unitPrice, [Bind("quantity")] decimal quantity)
    ///     {
    ///         ShoppingCart shoppingCart = new ShoppingCart();
    ///         shoppingCart.Add(item, unitprice, quantity);
    ///         Assert.AreEqual(unitPrice * quantity, shoppingCart.TotalCost);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="XmlDataSet"/>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class XmlDataAttribute : ContentAttribute
    {
        private readonly string itemPath;

        /// <summary>
        /// Specifies a XML-based data source.
        /// </summary>
        /// <param name="itemPath">The XPath expression used to select items within the document</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="itemPath"/> is null</exception>
        public XmlDataAttribute(string itemPath)
        {
            if (itemPath == null)
                throw new ArgumentNullException("itemPath");

            this.itemPath = itemPath;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            XmlDataSet dataSet = new XmlDataSet(delegate { return OpenXPathDocument(codeElement); }, itemPath, IsDynamic);
            dataSet.DataLocationName = GetDataLocationName();

            dataSource.AddDataSet(dataSet);
        }

        private XPathDocument OpenXPathDocument(ICodeElementInfo codeElement)
        {
            using (TextReader reader = OpenTextReader(codeElement))
                return new XPathDocument(reader);
        }
    }
}
