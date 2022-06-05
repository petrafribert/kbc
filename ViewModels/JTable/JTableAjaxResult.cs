#pragma warning disable CS1591

using System;

namespace KBC.ViewModels.JTable
{
    public class JTableAjaxResult
    {
        public string Result { get; set; }
        public string Message { get; set; }

        protected JTableAjaxResult()
        {
            Result = "OK";
        }

        protected JTableAjaxResult(string errorMessage)
        {
            Result = "ERROR";
            Message = errorMessage;
        }

        public static JTableAjaxResult OK => new JTableAjaxResult { Result = "OK" };

        public static JTableAjaxResult Error(string errorMessage) =>
            new JTableAjaxResult { Result = "ERROR", Message = errorMessage };

        internal static JTableAjaxResult Error(object p)
        {
            throw new NotImplementedException();
        }
    }
}