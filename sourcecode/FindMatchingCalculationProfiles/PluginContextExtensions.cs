//-----------------------------------------------------------------------
// <copyright file="PluginContextExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Dynamics.Sustainability.Plugins
{ 
    using System;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;
    

    /// <summary>
    /// Plugin Context object.
    /// </summary>
    public static class PluginContextExtensions
    {
        /// <summary>
        /// The input parameter target
        /// </summary>
        private const string TargetInputParameterName = "Target";
        private const string TargetsInputParameterName = "Targets";
        private const string BulkPreEntityImages = "BulkPreEntityImages";
        /// <summary>
        /// The request header for shared variables
        /// </summary>


        public const string SharedVariableRequestHeader = "tag";

        /// <summary>
        /// Name of the create message
        /// </summary>
        public const string CreateMessageName = "Create";

        /// <summary>
        /// Writes a Trace Messaged to the CRM Trace Log.
        /// </summary>
        /// <param name="pluginContext">Plugin context.</param>
        /// <param name="message">Message name to trace.</param>
        public static void Trace(this IPluginContext pluginContext, string message)
        {
            if (string.IsNullOrWhiteSpace(message) || pluginContext.TracingService == null || pluginContext.PluginExecutionContext == null)
            {
                return;
            }

            pluginContext.TracingService.Trace(
                "{0}, Correlation Id: {1}, Initiating User: {2}",
                message,
                pluginContext.PluginExecutionContext.CorrelationId,
                pluginContext.PluginExecutionContext.InitiatingUserId);
        }

        /// <summary>
        /// Gets the input parameter object.
        /// </summary>
        /// <typeparam name="T">Expected type of the input parameter object.</typeparam>
        /// <param name="pluginContext">Plugin context.</param>
        /// <param name="inputParameterName">Input parameter name.</param>
        /// <returns>Input parameter object in the expected type.</returns>
        public static T GetInputParameter<T>(this IPluginContext pluginContext, string inputParameterName)
        {
            T parameter = default(T);

            if (pluginContext.PluginExecutionContext.InputParameters.Contains(inputParameterName))
            {
                parameter = (T)pluginContext.PluginExecutionContext.InputParameters[inputParameterName];
            }

            return parameter;
        }

        /// <summary>
        /// Gets the Target input parameter. Throws an InvalidPluginExecutionException if target is null and throwIfNull is true
        /// </summary>
        /// <param name="pluginContext">The pluginContext</param>
        /// <param name="throwIfNull">If true then throw an invalid plugin exception stating that target is null.</param>
        /// <typeparam name="T">Expected type of the Target object.</typeparam>
        /// <returns>Target input parameter.</returns>
        public static T GetTargetFromInputParameters<T>(this IPluginContext pluginContext, bool throwIfNull = false) where T : Entity
        {
            var target = default(T);

            if (pluginContext.PluginExecutionContext.InputParameters.Contains(TargetInputParameterName))
            {
                var interimType = pluginContext.PluginExecutionContext.InputParameters[TargetInputParameterName];
                if ((interimType as Entity) != null)
                {
                    var entityTarget = (Entity)interimType;
                    target = entityTarget.ToEntity<T>();
                }
                else
                {
                    target = (T)interimType;
                }
            }

            if (target == null && throwIfNull)
            {
                throw new InvalidPluginExecutionException("Target is null.");
            }

            return target;
        }

        /// <summary>
        /// Gets the Targets input parameter. Throws an InvalidPluginExecutionException if target is null and throwIfNull is true
        /// </summary>
        /// <param name="pluginContext">The pluginContext</param>
        /// <param name="throwIfNull">If true then throw an invalid plugin exception stating that target is null.</param>
        /// <typeparam name="T">Expected type of the Target object.</typeparam>
        /// <returns>Target input parameter.</returns>
        public static EntityCollection GetTargetsEntityCollectionFromInputParameters(this IPluginContext pluginContext, bool throwIfNull = false)
        {
            EntityCollection entityCollection = null;

            if (pluginContext.PluginExecutionContext.InputParameters.Contains(TargetsInputParameterName))
            {
                entityCollection = pluginContext.PluginExecutionContext.InputParameters[TargetsInputParameterName] as EntityCollection;
            }

            if (entityCollection == null && throwIfNull)
            {
                throw new InvalidPluginExecutionException($"Input parameters should contain {TargetsInputParameterName} for plugin registered on xMultiple.");
            }

            return entityCollection;
        }

        /// <summary>
        /// Gets the Target input entity reference. Throws an InvalidPluginExecutionException if target is null and throwIfNull is true
        /// </summary>
        /// <param name="pluginContext">The pluginContext</param>
        /// <param name="throwIfNull">If true then throw an invalid plugin exception stating that target is null.</param>
        /// <returns>Target input entity reference.</returns>
        public static EntityReference GetTargetEntityReference(this IPluginContext pluginContext, bool throwIfNull = false)
        {
            var target = default(EntityReference);

            if (pluginContext.PluginExecutionContext.InputParameters.Contains(TargetInputParameterName))
            {
                var interimType = pluginContext.PluginExecutionContext.InputParameters[TargetInputParameterName];
                target = (EntityReference)interimType;
            }

            if (target == null && throwIfNull)
            {
                throw new InvalidPluginExecutionException("Target is null.");
            }

            return target;
        }

        /// <summary>
        /// Gets the specified pre image. Throws an InvalidPluginExecutionException if PreImage is null and throwIfNull is true
        /// </summary>
        /// <typeparam name="T">Expected type of the image.</typeparam>
        /// <param name="pluginContext">The pluginContext</param>
        /// <param name="preImageName">Image name.</param>
        /// <param name="throwIfNull">If true then throw an invalid plugin exception stating that preImage is null.</param>
        /// <returns>Pre image object in the expected type.</returns>
        public static T GetPreImage<T>(this IPluginContext pluginContext, string preImageName, bool throwIfNull = false) where T : Entity
        {
            var preImage = GetImage<T>(
                pluginContext.PluginExecutionContext.PreEntityImages,
                preImageName);

            if (preImage == null && throwIfNull)
            {
                throw new InvalidPluginExecutionException("PreImage is Null.");
            }

            return preImage;
        }

        /// <summary>
        /// Gets the specified post image. Throws an InvalidPluginExecutionException if PostImage is null and throwIfNull is true
        /// </summary>
        /// <typeparam name="T">Expected type of the image.</typeparam>
        /// <param name="pluginContext">The pluginContext</param>
        /// <param name="postImageName">Image name.</param>
        /// <param name="throwIfNull">If true then throw an invalid plugin exception stating that postImage is null.</param>
        /// <returns>Pre image object in the expected type.</returns>
        public static T GetPostImage<T>(this IPluginContext pluginContext, string postImageName, bool throwIfNull = false) where T : Entity
        {
            var postImage = GetImage<T>(
                pluginContext.PluginExecutionContext.PostEntityImages,
                postImageName);

            if (postImage == null && throwIfNull)
            {
                throw new InvalidPluginExecutionException("PostImage is Null.");
            }

            return postImage;
        }

        /// <summary>
        /// Sets the output parameter object.
        /// </summary>
        /// <typeparam name="T">Expected type of the output parameter object.</typeparam>
        /// <param name="pluginContext">Plugin context.</param>
        /// <param name="outputParameterName">Output parameter name.</param>
        /// <param name="parameter">Parameter object to store.</param>
        /// <param name="createParameter">Creates the parameter key if not found in the parameters collection.</param>
        /// <remarks>
        /// This will raise an exception when createParameter flag is set to false
        /// and the parameter name is not found in the OutputParameters collection.
        /// </remarks>
        public static void SetOutputParameter<T>(this IPluginContext pluginContext, string outputParameterName, T parameter, bool createParameter = true)
        {
            if (pluginContext.PluginExecutionContext.OutputParameters.Contains(outputParameterName))
            {
                pluginContext.PluginExecutionContext.OutputParameters[outputParameterName] = parameter;
            }
            else if (createParameter)
            {
                pluginContext.PluginExecutionContext.OutputParameters.Add(outputParameterName, parameter);
            }
            else
            {
                // throw new CrmInvalidOperationException(Labels.CannotSetInexistentParameter);
            }
        }

        /// <summary>
        /// Determines if an operation is trigger a plugin by the given plugin message id
        /// </summary>
        /// <param name="pluginContext">Plugin context.</param>
        /// <param name="sdkMessageId">The plugin message that should have triggered this plugin message from a parent context</param>
        /// <returns>true or false indicating if an operation was triggered by the given plugin message id</returns>
        public static bool IsTriggeredBySdkMessageFromParentContext(this IPluginContext pluginContext, Guid sdkMessageId)
        {
            var parentContext = pluginContext.PluginExecutionContext.ParentContext;

            while (parentContext != null)
            {
                var owningExtension = parentContext.OwningExtension;

                if (Guid.Equals(owningExtension?.Id, sdkMessageId))
                {
                    return true;
                }

                parentContext = parentContext.ParentContext;
            }

            return false;
        }

        public static bool IsNotTriggeredByParentEntity(this IPluginContext pluginContext)
        {
            var currentPrimaryEntityName = pluginContext.PluginExecutionContext.PrimaryEntityName;
            var parentPrimaryEntityName = pluginContext.PluginExecutionContext.ParentContext?.PrimaryEntityName;

            return parentPrimaryEntityName == null || currentPrimaryEntityName.Equals(parentPrimaryEntityName);
        }

        public static Dictionary<string, object> GetSharedVariables(this IPluginContext pluginContext)
        {
            try
            {
                if (pluginContext.PluginExecutionContext.SharedVariables != null && pluginContext.PluginExecutionContext.SharedVariables.ContainsKey(SharedVariableRequestHeader))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>
                        (pluginContext.PluginExecutionContext.SharedVariables[SharedVariableRequestHeader].ToString());
                }

                return null;
            }
            // Catching exception to avoid plugin failure
            catch (Exception ex)
            {
                // Log the exception
                pluginContext.Trace($"Error while getting shared variables: {ex}");
                return null;
            }
        }

        private static T GetImage<T>(EntityImageCollection imageCollection, string imageName) where T : Entity
        {
            T image = default;

            if (imageCollection.Contains(imageName))
            {
                var interimType = imageCollection[imageName];
                if ((interimType as Entity) != null)
                {
                    image = interimType.ToEntity<T>();
                }
                else
                {
                    image = (T)interimType;
                }
            }

            return image;
        }
    }
}