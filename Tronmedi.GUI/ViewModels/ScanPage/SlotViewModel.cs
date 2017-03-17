using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Tronmedi.GUI.Models;
using LayerScanModeEnum = Tronmedi.GUI.Models.LayerScanMode;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class SlotViewModel : PropertyChangedBase
    {
        private readonly ScanSubpageViewModel _ownerPage;
        private readonly ScanSlotContext _context;
        private SlotDisplayMode _displayMode;

        public SlotDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayMode));
            }
        }

        public bool IsInSelectMode => this.DisplayMode == SlotDisplayMode.Select;
        public bool IsInSetupMode => this.DisplayMode == SlotDisplayMode.Setup;
        public bool IsInScanningMode => this.DisplayMode == SlotDisplayMode.Scanning;
        public bool IsInViewMode => this.DisplayMode == SlotDisplayMode.View;

        public bool IsEnabled => this.IsInSelectMode || this.IsSelected;

        public ImageSource Image
        {
            get { return _context.Image; }
            set
            {
                _context.Image = value;
                this.NotifyOfPropertyChange(nameof(this.Image));
            }
        }

        public LayerScanModeEnum[] LayerScanModes { get; } = Enum.GetValues(typeof(LayerScanModeEnum)).Cast<LayerScanModeEnum>().ToArray();

        public LayerScanModeEnum SelectedLayerScanMode
        {
            get { return _context.LayerScanMode; }
            set
            {
                _context.LayerScanMode = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedLayerScanMode));
            }
        }

        public string FileName
        {
            get { return _context.FileName; }
            set
            {
                _context.FileName = value;
                this.NotifyOfPropertyChange(nameof(this.FileName));
            }
        }

        public string Name => _context.Name;

        public bool IsSelected
        {
            get { return _context.IsSelected; }
            set
            {
                _context.IsSelected = value;
                this.NotifyOfPropertyChange(() => this.IsSelected);
            }
        }

        public bool ShouldUpload
        {
            get { return _context.ShouldUpload; }
            set
            {
                _context.ShouldUpload = value;
                this.NotifyOfPropertyChange(() => this.ShouldUpload);
            }
        }

        public SlotViewModel(ScanSubpageViewModel ownerPage, ScanSlotContext context, SlotDisplayMode displayMode)
        {
            _ownerPage = ownerPage;
            _context = context;
            _displayMode = displayMode;

            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                this.FileName = $"{this.Name}.png";
            }
        }

        public void Browse()
        {
            _ownerPage.Browse(this.FileName);
        }
    }
}
