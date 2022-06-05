#pragma warning disable CS1591

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBC.Util
{
    public static class ModelStateExtensions
    {
        public static string GetErrorsString(this ModelStateDictionary modelState)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var modelStateEntry in modelState)
            {
                if (modelStateEntry.Value.Errors.Count > 0)
                {
                    string key = modelStateEntry.Key;
                    string error = string.Join(", ", modelStateEntry.Value.Errors.Select(e => e.ErrorMessage));
                    sb.AppendFormat("{0}: {1}; ", key, error);
                }
            }
            return sb.ToString();
        }
    }
}
