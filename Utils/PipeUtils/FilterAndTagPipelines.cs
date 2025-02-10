﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ProjetaHDR.Utils
{
    internal class FilterAndTagPipelines
    {
        public IList<Element> Pipes { get; private set; }
        public IList<XYZ> InsertPoints{ get; private set; }
        public IList<string> RelativePosition { get; private set; }
        public ElementId TagId { get; private set; }
        public IList<ElementId> TagsIds { get; private set; }
        public Document Doc { get; set; }
        public string TagMode { get; set; }
        public bool IsHydraulic { get; set; }
        public double LengthOption { get; set; }
        public IList<string> FlowDirections { get; private set; }

        public ViewDirections ViewDirections { get; set; }

        internal FilterAndTagPipelines(Document doc, View view, string tagMode, double lengthOption, bool isHydraulic)
        {
            Doc = doc;
            TagMode = tagMode;
            IsHydraulic = isHydraulic;
            LengthOption = lengthOption;
            ViewDirections = new ViewDirections(view);
        }

        internal void PipelineHydraulic(IList<Element> unfilteredPipes)
        {
            Pipes = PipeFilters.FilterByLength(unfilteredPipes, LengthOption);
            Pipes = PipeFilters.RemoveViewParallels(Pipes, ViewDirections);
        }

        internal void PipelineSanitary(IList<Element> unfilteredPipes)
        {
            Pipes = PipeFilters.FilterByLength(unfilteredPipes, LengthOption);
            Pipes = PipeFilters.RemoveSanitaryVerticals(Pipes);

            if(TagMode == "Inclinacao") PipeMethods.SetPipeSlope(Pipes);
        }

        internal void PipelineCreate()
        {
            RelativePosition = PipeMethods.GetRelativeViewPosition(Pipes, ViewDirections);
            InsertPoints = PipeMethods.GetTaginsertPoint(Pipes, TagMode, ViewDirections, RelativePosition);

            TagId = TagManager.GetTagId(Doc, TagMode);
            TagManager.DeleteExistingTags(Doc, Pipes, TagId);
            TagManager.CreateTags(Doc, Pipes, TagId, InsertPoints);
        }

        internal void PipelineFlow()
        {
            FlowDirections = PipeMethods.GetPipeFlow(Pipes, IsHydraulic);
            InsertPoints = PipeMethods.GetTaginsertPoint(Pipes, TagMode, ViewDirections, RelativePosition);
            TagsIds = new List<ElementId>();

            foreach (var direction in FlowDirections)
            {
                TagId = TagManager.GetTagId(Doc, direction);
                TagsIds.Add(TagId);
                TagManager.DeleteExistingTags(Doc, Pipes, TagId);
            }
            
            TagManager.CreateTags(Doc, Pipes, TagsIds, InsertPoints);
        }

    }
}
