using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using ProjetaHDR.UI;

namespace ProjetaHDR
{
    internal class FixtureFamilyItem : ObservableObject
    {
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

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged();
                }
            }
        }

        private Element _instanceElement;
        public Element InstanceElement
        {
            get => _instanceElement;
            set
            {
                if (_instanceElement != value)
                {
                    _instanceElement = value;
                    OnPropertyChanged();

                    // Atualiza os valores assim que a propriedade é alterada
                    Name = _instanceElement?.Name ?? "Sem Nome";
                    DisplayName = _instanceElement?.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.AsString() ?? "Sem Nome";
                }
            }
        }
    }
}
