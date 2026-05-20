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

        private ExternalEvent _externalEvent;
        private WaterproofingHandler _eventHandler;

        private readonly WaterproofingTypeService _waterproofingTypeService;
        public ObservableCollection<WaterproofingType> WaterproofingFloorTypes { get; set; } = new ObservableCollection<WaterproofingType>();

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

        private double _floorThickness;
        public double WaterproofThickness
        {
            get => _floorThickness;
            set
            {
                if (_floorThickness != value)
                {
                    _floorThickness = value;
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

                    var selectedType = WaterproofingFloorTypes.FirstOrDefault(t => _selectedFloorTypeId == t.ElementTypeId);
                    WaterproofThickness = selectedType != null ? selectedType.Thickness : 0.0;

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

            UpdateWaterproofingFloorTypes();

            if (SelectedFloorTypeId == null)
                SelectedFloorTypeId = WaterproofingFloorTypes.First().ElementTypeId;

            PickRegionsCommand = new RelayCommand(PickRegions);
            PickOffsetTargetCommand = new RelayCommand(PickOffsetTarget);
            CancelCommand = new RelayCommand(CancelOperation);
        }

        public void UpdateWaterproofingFloorTypes()
        {
            var availableTypes = _waterproofingTypeService.GetAvailableTypes();

            WaterproofingFloorTypes.Clear();

            foreach (WaterproofingType type in availableTypes.OrderBy(t => t.Name))
            {
                WaterproofingFloorTypes.Add(type);
            }
        }

        private void PickRegions(object parameter)
        {
            HideUI();
            _eventHandler.WaterproofingAction = WaterproofingAction.PickRegions;
            _eventHandler.SelectedFloorTypeId = SelectedFloorTypeId;
            _eventHandler.FloorLevelOffset = Offset;
            _eventHandler.WaterproofThickness = WaterproofThickness;
            _eventHandler.WaterproofingHeight = BaseboardHeigth;

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

        public void UpdateEvent(ExternalEvent newEvent, WaterproofingHandler newHandler)
        {
            this._externalEvent = newEvent;
            this._eventHandler = newHandler;
        }

        public void HideUI() => RequestHide?.Invoke();
        public void ShowUI() => RequestShow?.Invoke();
        public void CloseUI() => RequestClose?.Invoke();
    }
}