using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using ProjetaHDR.UI;

namespace ProjetaHDR.Commands.Services
{
    internal class FixtureFamilyItem : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        private ElementId _instanceElementId;
        public ElementId InstanceElementId
        {
            get => _instanceElementId;
            set
            {
                if (value == null) _instanceElementId = ElementId.InvalidElementId;

                if (_instanceElementId != value)
                {
                    _instanceElementId = value;
                    OnPropertyChanged();
                }
            }
        }

        //private Element _instanceElement;
        //public Element InstanceElement
        //{
        //    get => _instanceElement;
        //    set
        //    {
        //        if (_instanceElement != value)
        //        {
        //            _instanceElement = value;
        //            OnPropertyChanged();
        //            Name = _instanceElement.Name;
        //            Comment= _instanceElement?.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.AsString() ?? "Sem Nome";
        //        }
        //    }
        //}






    }
}
