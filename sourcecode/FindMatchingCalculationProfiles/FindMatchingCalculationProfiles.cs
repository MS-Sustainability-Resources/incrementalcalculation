//-----------------------------------------------------------------------
// <copyright file="FindMatchingCalculationProfilesPlugin.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Dynamics.Sustainability.Plugins
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Dynamics.Sustainability.Common;
    using Microsoft.Graph;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.PluginTelemetry;
    using Microsoft.Xrm.Sdk.Query;
    using Newtonsoft.Json;
    

    public class FindMatchingCalculationProfilesPlugin : PluginBase
    {
        ILogger logger;
        public FindMatchingCalculationProfilesPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(FindMatchingCalculationProfilesPlugin))
        {
        }

        public FindMatchingCalculationProfilesPlugin()
            : base(typeof(FindMatchingCalculationProfilesPlugin))
        {
        }

        public override async Task ExecuteCrmPlugin(IPluginContext context)
        {
            logger = context.Logger;
            
            ConcurrentBag<string> matchingCalculationProfiles = new ConcurrentBag<string>();
            var contract = context.ToFindMatchingCalculationProfilesContract();
            var columnSet = new ColumnSet(CalculationProfileEntity.NameField, CalculationProfileEntity.FilterStringField, Constants.DataDefinitionFieldName);
            var sourceCondition = new ConditionExpression(Constants.DataDefinitionFieldName, ConditionOperator.Equal, new Guid(contract.DataDefinitionId));
            var dataQuery = new QueryExpression
            {
                EntityName = CalculationProfileEntity.LogicalName,
                ColumnSet = columnSet,
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression(CalculationProfileEntity.RunOnNewDataField, ConditionOperator.Equal, true),
                            sourceCondition,
                        },
                },
            };

            var calculationProfiles = context.OrganizationService.RetrieveMultiple(dataQuery);

            foreach (var calculationProfile in calculationProfiles.Entities)
            {

                await this.logger.ExecuteAsync(this.PluginClassName, async () => {
                    try
                    {
                        QueryExpression query = await RetrieveActivitiesQueryForCalculationProfile(context, calculationProfile.GetAttributeValue<string>(CalculationProfileEntity.FilterStringField), new Guid(contract.ConnectionRefreshId), new Guid(contract.DataDefinitionId));
                        IEnumerable<Entity> resultActivities = context.OrganizationService.RetrieveMultiple(query).Entities;

                        if (resultActivities.GetEnumerator().MoveNext())
                        { 
                            matchingCalculationProfiles.Add(JsonConvert.SerializeObject(new MatchingCalculationProfile(calculationProfile.Id.ToString(), calculationProfile.GetAttributeValue<string>(CalculationProfileEntity.NameField))));
                        }
                        else
                        {
                            this.logger.LogInformation("Calculation profile does not have any new activities to process. Not returning the Calculation Profile Id for this reason.");
                        }
                    }
                    catch (Exception e)
                    {

                        this.logger.LogError($"Failed to check Activities for Calculation Profile: {calculationProfile.Id} CorrelationID: {context.PluginExecutionContext.CorrelationId}. Exception: {e}.");
                    }
                });
            }

            context.SetOutputParameter<string[]>(Constants.CalculationProfileIdsOutputParameter, matchingCalculationProfiles.ToArray());
        }


        public class _1
        { 
            public string fetchXML { get; set; }
        }

        public class Root
        {
            [JsonProperty("1")]
            public _1 _1 { get; set; }
        }

        private Task<QueryExpression> RetrieveActivitiesQueryForCalculationProfile(IPluginContext context, string ActivityFetchXML, Guid ConnectionRefreshId, Guid SustainabilityDataDefinitionId)
        {
            var data = JsonConvert.DeserializeObject<Root>(ActivityFetchXML);

            var filterXmlToQueryRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = data._1.fetchXML //ActivityFetchXML,
            };

            var filterXmlToQueryResponse = (FetchXmlToQueryExpressionResponse)context.OrganizationService.Execute(filterXmlToQueryRequest);
            var dataQuery = filterXmlToQueryResponse.Query;

            if (ConnectionRefreshId != null && ConnectionRefreshId != Guid.Empty)
            {
                dataQuery.Criteria.AddCondition(Constants.ConnectionRefreshReferenceField, ConditionOperator.Equal, ConnectionRefreshId);
            }

            List<ConditionExpression> conditionExpressions = new List<ConditionExpression>();
            conditionExpressions.Add(new ConditionExpression()
            {
                AttributeName = Constants.DataDefinitionFieldName,
                Operator = ConditionOperator.Equal,
                Values = { SustainabilityDataDefinitionId },
            });

            conditionExpressions.ForEach(dataQuery.Criteria.AddCondition);
            dataQuery.ColumnSet = new ColumnSet();

            return Task.FromResult(dataQuery);
        }
    }    
}