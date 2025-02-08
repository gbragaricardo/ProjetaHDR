﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.OnStartup;
using ProjetaHDR.RevitContext;
using ProjetaHDR.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class SlopeTag : RevitCommandBase, IExternalCommand
    {

        public double LengthFilterOption { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);
            LengthFilterOption = 0.2;

            using (Transaction transacao = new Transaction(Context.Doc, "Tag Inclinacao"))
            {
                transacao.Start();

                var unfilteredPipes = PipeFilters.GetPipesOnView(Context.Doc);
                var IsHydraulic = PipeFilters.HasPvcMarromPipes(Context.Doc, unfilteredPipes);

                if (IsHydraulic == true)
                {
                   return Result.Cancelled;
                }
                else
                {
                    var SanitaryPipeline = new FilterAndTagPipelines(Context.Doc, "Inclinacao", LengthFilterOption, false);

                    SanitaryPipeline.PipelineSanitary(unfilteredPipes);
                }

                transacao.Commit();
                return Result.Succeeded;
            }
        }
    }
}

