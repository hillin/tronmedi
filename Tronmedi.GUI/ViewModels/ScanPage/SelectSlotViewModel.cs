using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    internal class SelectSlotViewModel : SlotSubpageViewModelBase
    {
        private bool _isPrescanning;

        public bool IsPrescanning
        {
            get { return _isPrescanning; }
            set
            {
                _isPrescanning = value;
                this.NotifyOfPropertyChange(() => this.IsPrescanning);
            }
        }

        public int SelectedSlotCount => this.Slots.Count(s => s.IsSelected);

        public SelectSlotViewModel()
            : base(SlotDisplayMode.Select)
        {

        }

        public async Task StartPrescan()
        {
            if (this.SelectedSlotCount == 0)
            {
                return;
            }

            this.IsPrescanning = true;
            await Task.Delay(1000 * this.SelectedSlotCount);
            this.IsPrescanning = false;
            this.NextStep();
        }
    }
}
