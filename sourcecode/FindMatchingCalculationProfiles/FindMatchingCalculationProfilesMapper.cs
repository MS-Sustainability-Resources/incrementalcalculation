//-----------------------------------------------------------------------
// <copyright file="FindMatchingCalculationProfilesMapper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Dynamics.Sustainability.Plugins
{
    public static class FindMatchingCalculationProfilesMapper
    {
        public static FindMatchingCalculationProfilesContract ToFindMatchingCalculationProfilesContract(this IPluginContext context)
        {
            return new FindMatchingCalculationProfilesContract()
            {
               DataDefinitionId = context.GetInputParameter<string>(Constants.DataDefinitionIdInputParameter),
               ConnectionRefreshId = context.GetInputParameter<string>(Constants.ConnectionRefreshIdInputParameter)
            };
        }
    }
}