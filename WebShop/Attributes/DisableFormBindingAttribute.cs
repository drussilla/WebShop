using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PriceImporter.Attributes
{
    /// <summary>
    /// Use this attribute to disable standard mvc model binding so we could read the content of the file from the stream
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="IResourceFilter" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.ValueProviderFactories.RemoveType<FormValueProviderFactory>();
            context.ValueProviderFactories.RemoveType<JQueryFormValueProviderFactory>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}