using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Events;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models.Enums;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels.Wrappers;
using ProjetaHDR.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels
{
    public class CreateWaterproofTypeViewModel : ObservableObject
    {
        private readonly Action _closeFlyoutAction;

        private readonly ExternalEvent _externalEvent;
        private readonly WaterproofingHandler _eventHandler;

        public ObservableCollection<WaterproofingLayerItemViewModel> WaterproofingLayers { get; set; } = new ObservableCollection<WaterproofingLayerItemViewModel>();

        public RelayCommand AddNewLayerCommand { get; set; }
        public RelayCommand RemoveLayerCommand { get; set; }
        public RelayCommand CreateTypeCommand { get; set; }

        public CreateWaterproofTypeViewModel(ExternalEvent externalEvent, WaterproofingHandler waterproofingHandler, Action fecharFlyoutAction)
        {
            _externalEvent = externalEvent;
            _eventHandler = waterproofingHandler;

            AddNewLayer(null);

            AddNewLayerCommand = new RelayCommand(AddNewLayer);
            RemoveLayerCommand = new RelayCommand((layerParam) => RemoveLayer(layerParam));
            CreateTypeCommand = new RelayCommand(CreateType);
        }

        private void AddNewLayer(object parameter) => WaterproofingLayers.Add(new WaterproofingLayerItemViewModel());
        

        private void RemoveLayer(object parameter)
        {
            if (parameter is WaterproofingLayerItemViewModel layer && WaterproofingLayers.Count > 1)
                WaterproofingLayers.Remove(layer);

        }
        private void CreateType(object parameter)
        {
            if (ValidateLayers() == false)
                return;

            _eventHandler.WaterproofingAction = WaterproofingAction.CreateNewType;
            _eventHandler.WaterproofingLayers = WaterproofingLayers.ToList();
            _eventHandler.OnExecuteCompleted = _closeFlyoutAction.Invoke;
            _externalEvent.Raise();
        }

        private bool ValidateLayers()
        {
            foreach (var layer in WaterproofingLayers)
            {
                if (string.IsNullOrEmpty(layer.Name) || layer.Thickness <= 0)
                    return false;
            }
            return true;
        }
    }

}
