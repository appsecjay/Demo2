﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class HeartRateToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultHeartRateTransformedToFhir_ThenCodeAndValuesEqual()
        {
            // ToDo, once deserialization is fixed on SDK, use Deserialize

            var heartRate = new HeartRate(new HealthServiceDateTime(), 65);

            var observation = heartRate.ToFhir() as Observation;
            Assert.IsNotNull(observation);
            Assert.AreEqual(HealthVaultVocabularies.HeartRate, observation.Code);

            var value = observation.Value as Quantity;
            Assert.IsNotNull(value);
            Assert.AreEqual(65, value.Value);
            Assert.AreEqual(UnitAbbreviations.PerMinute, value.Unit);
        }
    }
}
