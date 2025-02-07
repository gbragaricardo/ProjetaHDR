﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitContext;

namespace ProjetaHDR.RevitContext
{
    public abstract class RevitCommandBase
    {
        protected RevitContext Context { get; private set; }

        protected void InitializeContext(ExternalCommandData commandData)
        {
            Context = new RevitContext(commandData.Application);
        }
    }
}
