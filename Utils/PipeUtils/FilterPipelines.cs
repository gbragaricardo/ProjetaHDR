using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ProjetaHDR.Utils
{
    internal static class FilterPipelines
    {
        public static IList<XYZ> SanitaryInsertPoints{ get; private set; }
        public static IList<string> SanitaryPlanPosition { get; private set; }

        public static ElementId TagId { get; private set; }

        internal static IList<Element> PipelineSanitary(Document doc, double lengthOption, string tagMode)
        {
            var pipes = PipeFilters.GetPipesOnView(doc);
            pipes = PipeFilters.FilterByLength(pipes, lengthOption);
            var sanitaryPipes = PipeFilters.RemoveVerticals(pipes);

            SanitaryPlanPosition = PipeUtils.GetPositionOnPlan(sanitaryPipes);

            SanitaryInsertPoints = PipeUtils.GetTaginsertPoint(sanitaryPipes, SanitaryPlanPosition, tagMode);

            TagId = TagManager.GetTagId(doc, tagMode);

            return sanitaryPipes;
        }

        internal static void PipelineHydraulic()
        {

        }
    }
}
