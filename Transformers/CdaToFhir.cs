// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation cdas (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static DocumentReference ToFhir(this CDA cda)
        {
            return CdaToFhir.ToFhirInternal(cda, ToFhirInternal<DocumentReference>(cda));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault CDA data types into FHIR DocumentReferences
    /// </summary>
    internal static class CdaToFhir
    {
        internal static DocumentReference ToFhirInternal(CDA cda, DocumentReference documentReference)
        {
            string xml = ThingBaseToFhirDocumentReference.GetXmlFromXPathNavigator(cda.TypeSpecificData.CreateNavigator());

            ThingBaseToFhirDocumentReference.XmlToDocumentReference(documentReference, xml);

            return documentReference;
        }
    }
}
