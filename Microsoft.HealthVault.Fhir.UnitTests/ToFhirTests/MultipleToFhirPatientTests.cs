﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class MultipleToFhirPatientTests
    {
        [TestMethod]
        public void WhenMultipleHeathVaultThingsTransformedToFhirPatient_ThenCodeAndValuesEqual()
        {
            var basic = new BasicV2
            {
                Gender = Gender.Female,
                BirthYear = 1975,
                City = "Redmond",
                StateOrProvince = new CodableValue("Washington", "WA", "states", "wc", "1"),
                PostalCode = "98052",
                Country = new CodableValue("United States of America", "US", "iso3166", "iso", "1"),
                FirstDayOfWeek = DayOfWeek.Sunday,
                Languages =
                {
                    new Language(new CodableValue("English", "en", "iso639-1", "iso", "1"), true),
                    new Language(new CodableValue("French", "fr", "iso639-1", "iso", "1"), false),
                }
            };

            var patient = basic.ToFhir();

            var contact = new Contact();
            contact.ContactInformation.Address.Add(new ItemTypes.Address
            {
                Street = { "123 Main St.", "Apt. 3B" },
                City = "Redmond",
                PostalCode = "98052",
                County = "King",
                State = "WA",
                Country = "USA",
                Description = "Home address",
                IsPrimary = true,
            });

            contact.ContactInformation.Address.Add(new ItemTypes.Address
            {
                Street = { "1 Back Lane" },
                City = "Holmfirth",
                PostalCode = "HD7 1HQ",
                County = "HUDDERSFIELD",
                Country = "UK",
                Description = "business address",
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person1@example.com",
                Description = "Address 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person2@example.com",
                Description = "Address 2",
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "1-425-555-0100",
                Description = "Phone 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "0491 570 156",
                Description = "Phone 2",
            });

            contact.ToFhir(patient);

            var personalImage = new PersonalImage();
            string resourceName = "Microsoft.HealthVault.Fhir.UnitTests.Samples.HealthVaultIcon.png";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        personalImage.WriteImage(reader.BaseStream, "image/png");
                    }
                }
            }

            personalImage.ToFhir(patient);

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

            personal.ToFhir(patient);

            var json = FhirSerializer.SerializeToJson(patient);

            Assert.IsNotNull(patient);
            // Basic Portion
            Assert.AreEqual(AdministrativeGender.Female, patient.Gender.Value);
            var basicV2Extension = patient.GetExtension(HealthVaultExtensions.PatientBasicV2);
            Assert.AreEqual(1975, basicV2Extension.GetIntegerExtension(HealthVaultExtensions.PatientBirthYear));
            Assert.AreEqual("0", basicV2Extension.GetExtensionValue<Coding>(HealthVaultExtensions.PatientFirstDayOfWeek).Code);
            Assert.AreEqual("Sunday", basicV2Extension.GetExtensionValue<Coding>(HealthVaultExtensions.PatientFirstDayOfWeek).Display);

            var basicAddress = basicV2Extension.GetExtension(HealthVaultExtensions.PatientBasicAddress);
            Assert.AreEqual("Redmond", basicAddress.GetStringExtension(HealthVaultExtensions.PatientBasicAddressCity));
            Assert.AreEqual("WA", basicAddress.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBasicAddressState).Coding[0].Code);
            Assert.AreEqual("98052", basicAddress.GetStringExtension(HealthVaultExtensions.PatientBasicAddressPostalCode));
            Assert.AreEqual("US", basicAddress.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBasicAddressCountry).Coding[0].Code);

            Assert.AreEqual(2, patient.Communication.Count);
            Assert.AreEqual("English", patient.Communication[0].Language.Text);
            Assert.AreEqual(true, patient.Communication[0].Preferred);

            // Contact portion
            Assert.AreEqual(2, patient.Address.Count);
            var address1 = patient.Address[0];
            Assert.AreEqual(2, address1.Line.Count());
            Assert.AreEqual("123 Main St.", address1.Line.First());
            Assert.AreEqual("Redmond", address1.City);
            Assert.AreEqual("King", address1.District);
            Assert.AreEqual("WA", address1.State);
            Assert.AreEqual("98052", address1.PostalCode);
            Assert.AreEqual("USA", address1.Country);
            Assert.AreEqual("Home address", address1.Text);
            Assert.AreEqual(true, address1.GetBoolExtension(HealthVaultExtensions.IsPrimary));

            Assert.AreEqual(4, patient.Telecom.Count);
            var email1 = patient.Telecom[0];
            Assert.AreEqual("person1@example.com", email1.Value);
            Assert.AreEqual("Address 1", email1.GetStringExtension(HealthVaultExtensions.Description));
            Assert.AreEqual(1, email1.Rank);

            var phone1 = patient.Telecom[2];
            Assert.AreEqual("1-425-555-0100", phone1.Value);
            Assert.AreEqual("Phone 1", phone1.GetStringExtension(HealthVaultExtensions.Description));
            Assert.AreEqual(1, phone1.Rank);

            // Personal Image portion
            Assert.IsNotNull(patient);
            Assert.AreEqual(1757, patient.Photo[0].Data.Length);

            // Personal portion
            Assert.IsNotNull(patient);
            var personalExtension = patient.GetExtension(HealthVaultExtensions.PatientPersonal);
            Assert.AreEqual("Dr. John Phillip Doe, Jr.", patient.Name[0].Text);
            Assert.AreEqual("John", patient.Name[0].Given.ToList()[0]);
            Assert.AreEqual("Phillip", patient.Name[0].Given.ToList()[1]);
            Assert.AreEqual("Doe", patient.Name[0].Family);
            Assert.AreEqual("Dr.", patient.Name[0].Prefix.ToList()[0]);
            Assert.AreEqual("Junior", patient.Name[0].Suffix.ToList()[0]);
            Assert.AreEqual("1975-02-05", patient.BirthDate);
            Assert.AreEqual("000-12-3456", patient.Identifier[0].Value);
            Assert.AreEqual("2075-05-07", ((FhirDateTime)patient.Deceased).Value);
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
