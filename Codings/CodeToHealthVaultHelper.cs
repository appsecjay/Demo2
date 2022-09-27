﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Codings
{
    internal static class CodeToHealthVaultHelper
    {
        internal static CodableValue CreateCodableValueFromQuantityValues(string system, string code, string unit)
        {
            var familyVocab = HealthVaultVocabularies.ExtractFamilyAndVocabularyFromSystemUrl(system);
            return new CodableValue(unit, code, familyVocab.vocabulary, familyVocab.family, null);
        }

        internal static Type DetectHealthVaultTypeFromObservation(Observation observation)
        {
            if (observation.Code != null && observation.Code.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (!String.IsNullOrWhiteSpace(code.System) && code.CodeElement != null)
                    {
                        if (HealthVaultVocabularies.SystemContainsHealthVaultUrl(code.System))
                        {
                            var uri = new Uri(code.System.ToLowerInvariant());
                            return DetectFromHealthVaultCode(uri.Segments.Last(), code.CodeElement.Value);
                        }

                        switch (code.System.ToLowerInvariant())
                        {
                            case VocabularyUris.SnomedCd:
                                return DetectFromSnomedCd(code.CodeElement.Value);
                            case VocabularyUris.Loinc:
                                return DetectFromLoincCodes(code.CodeElement.Value);
                        }
                    }
                }
            }

            throw new NotSupportedException();
        }

        private static Type DetectType(Dictionary<string, string> codeDictionary, string code)
        {
            if (codeDictionary != null && codeDictionary.ContainsKey(code))
            {
                return Type.GetType($"{codeDictionary[code]}, Microsoft.HealthVault");
            }

            throw new NotSupportedException("The provided code is not supported");
        }

        private static Type DetectFromSnomedCd(string code)
        {
            return DetectType(CodeToHealthVaultDictionaries.Instance.Snomed, code);
        }

        private static Type DetectFromLoincCodes(string code)
        {
            return DetectType(CodeToHealthVaultDictionaries.Instance.Loinc, code);
        }

        private static Type DetectFromHealthVaultCode(string vocabName, string code)
        {

            switch (vocabName.ToLowerInvariant())
            {
                case HealthVaultVocabularies.VitalStatistics:
                    switch (code.ToLowerInvariant())
                    {
                        case HealthVaultVitalStatisticsCodes.BodyWeightCode:
                            return typeof(Weight);
                        case HealthVaultVitalStatisticsCodes.BodyHeightCode:
                            return typeof(Height);
                        case HealthVaultVitalStatisticsCodes.HeartRateCode:
                            return typeof(HeartRate);
                        case HealthVaultVitalStatisticsCodes.BloodPressureDiastolicCode:
                        case HealthVaultVitalStatisticsCodes.BloodPressureSystolicCode:
                            return typeof(BloodPressure);
                    }

                    break;

                case HealthVaultVocabularies.ThingTypeNames:
                    switch (code)
                    {
                        case HealthVaultThingTypeNameCodes.BloodGlucoseCode:
                            return typeof(BloodGlucose);
                        case HealthVaultThingTypeNameCodes.ExerciseCode:
                            return typeof(Exercise);
                        case HealthVaultThingTypeNameCodes.SleepJournalAMCode:
                            return typeof(SleepJournalAM);
                        case HealthVaultThingTypeNameCodes.BodyCompositionCode:
                            return typeof(BodyComposition);
                        case HealthVaultThingTypeNameCodes.BodyDimensionCode:
                            return typeof(BodyDimension);
                    }

                    break;
            }

            throw new NotSupportedException("The provided code is not supported");
        }

        internal static Type DetectHealthVaultTypeFromDocumentReference(DocumentReference documentReference)
        {
            var contentComponent = documentReference.GetFirstContentComponentWithData();

            if (contentComponent.Attachment.ContentType == "text/xml"
                || contentComponent.Attachment.ContentType == "application/xml")
            {
                string xml = Encoding.UTF8.GetString(contentComponent.Attachment.Data);

                XPathDocument xpDoc;
                try
                {
                    using (System.IO.TextReader txtReader = new System.IO.StringReader(xml))
                    {
                        xpDoc = new XPathDocument(txtReader);
                    }

                    XPathNavigator xpNav = xpDoc.CreateNavigator();
                    xpNav.MoveToFirstChild();

                    do
                    {
                        if (xpNav.NodeType == XPathNodeType.Element)
                        {
                            switch (xpNav.Name)
                            {
                                case "ContinuityOfCareRecord":
                                    return typeof(CCR);
                                case "ClinicalDocument":
                                    return typeof(CDA);
                            }
                        }
                    } while (xpNav.MoveToNext());
                }
                catch (XmlException)
                {
                   
                }
            }

            return typeof(File);
        }

        internal static CodableValue GetRecurrenceIntervalFromPeriodUnit(Timing.UnitsOfTime period)
        {
            switch (period)
            {
                case Timing.UnitsOfTime.S:
                    return HealthVaultRecurrenceIntervalCodes.Second;
                case Timing.UnitsOfTime.Min:
                    return HealthVaultRecurrenceIntervalCodes.Minute;
                case Timing.UnitsOfTime.H:
                    return HealthVaultRecurrenceIntervalCodes.Hour;
                case Timing.UnitsOfTime.D:
                    return HealthVaultRecurrenceIntervalCodes.Day;
                case Timing.UnitsOfTime.Wk:
                    return HealthVaultRecurrenceIntervalCodes.Week;
                case Timing.UnitsOfTime.Mo:
                    return HealthVaultRecurrenceIntervalCodes.Month;
                case Timing.UnitsOfTime.A:
                    return HealthVaultRecurrenceIntervalCodes.Year;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
