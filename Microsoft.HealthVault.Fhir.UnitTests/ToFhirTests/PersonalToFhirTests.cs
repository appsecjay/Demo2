﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class PersonalToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultPersonalTransformedToFhir_ThenValuesEqual()
        {
            var personal = new Personal
            {
                Name = new Name
                {
                    Full = "Dr. John Phillip Doe, Jr.",
                    First = "John",
                    Middle = "Phillip",
                    Last = "Doe",
                    Suffix = new CodableValue("Junior", "Jr", "name-suffixes", "wc", "1"),
                    Title = new CodableValue("Dr.", "Dr", "name-prefixes", "wc", "1"),
                },
                BirthDate = new HealthServiceDateTime
                {
                    Date = new HealthServiceDate(1975, 2, 5),
                    Time = new ApproximateTime(1, 30, 34, 15),
                },
                DateOfDeath = new ApproximateDateTime
                {
                    ApproximateDate = new ApproximateDate(2075, 5, 7),
                },
                SocialSecurityNumber = "000-12-3456",
                BloodType = new CodableValue("A+", "A+", "blood-types", "wc", "1"),
                Religion = new CodableValue("Agnostic", "Agn", "religion", "wc", "1"),
                MaritalStatus = new CodableValue("Never Married", "NM", "marital-status", "wc", "1"),
                EmploymentStatus = "Employed",
                IsDeceased = true,
                IsVeteran = true,
                Ethnicity = new CodableValue("Other Race", "8", "ethnicity-types", "wc", "1"),
                HighestEducationLevel = new CodableValue("College Graduate", "ColG", "Education-level", "wc", "1"),
                OrganDonor = "Organ Donor",
                IsDisabled = false,
            };

            var patient = personal.ToFhir();

            Assert.IsNotNull(patient);
            Assert.AreEqual("Dr. John Phillip Doe, Jr.", patient.Name[0].Text);
            Assert.AreEqual("John", patient.Name[0].Given.ToList()[0]);
            Assert.AreEqual("Phillip", patient.Name[0].Given.ToList()[1]);
            Assert.AreEqual("Doe", patient.Name[0].Family);
            Assert.AreEqual("Dr.", patient.Name[0].Prefix.ToList()[0]);
            Assert.AreEqual("Junior", patient.Name[0].Suffix.ToList()[0]);
            Assert.AreEqual("1975-02-05", patient.BirthDate);
            Assert.AreEqual("000-12-3456", patient.Identifier[0].Value);
            Assert.AreEqual("2075-05-07", ((FhirDateTime)patient.Deceased).Value);

            var personalExtension = patient.GetExtension(HealthVaultExtensions.PatientPersonal);
            Assert.AreEqual("A+", personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBloodType).Coding[0].Code);
            Assert.AreEqual("Agn", personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientReligion).Coding[0].Code);
            Assert.AreEqual("NM", personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientMaritalStatus).Coding[0].Code);
            Assert.AreEqual("8", personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientEthnicity).Coding[0].Code);
            Assert.AreEqual("ColG", personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientHighestEducationLevel).Coding[0].Code);
            Assert.AreEqual("Employed", personalExtension.GetStringExtension(HealthVaultExtensions.PatientEmploymentStatus));
            Assert.AreEqual("Organ Donor", personalExtension.GetStringExtension(HealthVaultExtensions.PatientOrganDonor));
            Assert.AreEqual(true, personalExtension.GetBoolExtension(HealthVaultExtensions.PatientIsVeteran));
            Assert.AreEqual(false, personalExtension.GetBoolExtension(HealthVaultExtensions.PatientIsDisabled));
        }
    }
}
