//-----------------------------------------------------------------------
// <copyright file="FindMatchingCalculationProfilesContract.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Dynamics.Sustainability.Plugins
{
    public class MatchingCalculationProfile
    {
        public MatchingCalculationProfile(string calcProfileId, string calcProfileName)
        {
            CalculationProfileId = calcProfileId;
            CalculationProfileName = calcProfileName;
        }
        public string CalculationProfileId { get; set; }
        public string CalculationProfileName { get; set; }
    }
}
