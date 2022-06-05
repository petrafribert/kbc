#pragma warning disable CS1591

namespace KBC.ViewModels.JTable
{
    public class CreateResult : JTableAjaxResult
    {
        public CreateResult(object record) : base()
        {
            Record = record;
        }

        public object Record { get; set; }
    }
}