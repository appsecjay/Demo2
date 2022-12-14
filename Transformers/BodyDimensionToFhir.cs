// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Observation ToFhir(this BodyDimension bodyDimension)
        {
            return BodyDimensionToFhir.ToFhirInternal(bodyDimension, ToFhirInternal<Observation>(bodyDimension));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault body dimension data types into FHIR Observations
    /// </summary>
    internal static class BodyDimensionToFhir
    {
        internal static Observation ToFhirInternal(BodyDimension bodyDimension, Observation observation)
        {
            observation.Category = new List<CodeableConcept> { FhirCategories.VitalSigns };

            observation.Code = HealthVaultVocabularies.GenerateCodeableConcept(HealthVaultThingTypeNameCodes.BodyDimension);

            if (bodyDimension.MeasurementName != null)
            {
                observation.Method = bodyDimension.MeasurementName.ToFhir();
            }
            
            observation.Value = new Quantity((decimal)bodyDimension.Value.Meters, UnitAbbreviations.Meter);

            observation.Effective = bodyDimension.When.ToFhir();

            return observation;
        }
    }
}
