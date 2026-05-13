using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Events;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models.Enums;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public event Action RequestHide;
        public event Action RequestShow;
        public event Action RequestClose;

        private readonly ExternalEvent _externalEvent;
        private readonly WaterproofingHandler _eventHandler;

        private readonly WaterproofingTypeService _waterproofingTypeService;
        public ObservableCollection<WaterproofingType> AvailableFloorTypes { get; set; } = new ObservableCollection<WaterproofingType>
        {
            new WaterproofingType{Name= "Manta Asfaltica",},
            new WaterproofingType{Name= "Outro Tipo", },
            new WaterproofingType{Name= "Tipo 3 Asfaltica"}
        };

        public bool IsConfirmed { get; set; }

        private WaterproofingType _waterproofingType;
        public WaterproofingType WaterproofingType
        {
            get => _waterproofingType;
            set
            {
                if (_waterproofingType != value)
                {
                    _waterproofingType = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _baseboardHeigth = 0.0;
        public double BaseboardHeigth
        {
            get => _baseboardHeigth;
            set
            {
                if (_baseboardHeigth != value)
                {
                    _baseboardHeigth = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _offset = 0.0;
        public double Offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    OnPropertyChanged();
                }
            }
        }

        private ElementId _selectedFloorTypeId;
        public ElementId SelectedFloorTypeId
        {
            get => _selectedFloorTypeId;
            set
            {
                if (_selectedFloorTypeId != value)
                {
                    _selectedFloorTypeId = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand PickRegionsCommand { get; set; }
        public RelayCommand PickOffsetTargetCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public MainViewModel(WaterproofingTypeService waterproofingTypeService, ExternalEvent externalEvent, WaterproofingHandler eventHandler)
        {
            _eventHandler = eventHandler;
            _externalEvent = externalEvent;
            _waterproofingTypeService = waterproofingTypeService;
            PopulateAvailableTypesCollection(_waterproofingTypeService.GetAvailableTypes());

            PickRegionsCommand = new RelayCommand(PickRegions);
            PickOffsetTargetCommand = new RelayCommand(PickOffsetTarget);
            CancelCommand = new RelayCommand(CancelOperation);
        }

        private void PopulateAvailableTypesCollection(IList<WaterproofingType> availableTypes)
        {
            AvailableFloorTypes.Clear();

            foreach (WaterproofingType type in availableTypes.OrderBy(t => t.Name))
            {
                AvailableFloorTypes.Add(type);
            }
        }

        private void PickRegions(object parameter)
        {
            HideUI();
            _eventHandler.WaterproofingAction = WaterproofingAction.PickRegions;
            _eventHandler.SelectedFloorTypeId = SelectedFloorTypeId;
            _eventHandler.FloorLevelOffset = Offset;
            _eventHandler.WaterproofingBaseboardHeight = BaseboardHeigth;

            _eventHandler.OnExecuteCompleted = () => ShowUI();

            _externalEvent.Raise();
        }

        private void PickOffsetTarget(object parameter)
        {
            HideUI();
            _eventHandler.WaterproofingAction = WaterproofingAction.PickOffsetTarget;

            _eventHandler.OnElevationPicked = (offsetValue) => Offset = offsetValue;
            _eventHandler.OnExecuteCompleted = () => ShowUI(); 

            _externalEvent.Raise();
        }
        private void CancelOperation(object parameter)
        {
            CloseUI();
        }

        public void HideUI() => RequestHide?.Invoke();
        public void ShowUI() => RequestShow?.Invoke();
        public void CloseUI() => RequestClose?.Invoke();
    }
}