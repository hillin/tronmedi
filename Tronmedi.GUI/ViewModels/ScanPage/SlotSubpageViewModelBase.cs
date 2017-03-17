using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class SlotSubpageViewModelBase : ScanSubpageViewModel
    {
        private readonly SlotDisplayMode _slotDisplayMode;
        public SlotViewModel[] Slots { get; private set; }

        protected SlotSubpageViewModelBase(SlotDisplayMode slotDisplayMode)
        {
            _slotDisplayMode = slotDisplayMode;
        }

        public override void Initialize(ScanContext scanContext)
        {
            base.Initialize(scanContext);
            this.Slots = new SlotViewModel[scanContext.Slots.Length];
            for (var i = 0; i < Defaults.SlotCount; ++i)
            {
                this.Slots[i] = new SlotViewModel(this, this.ScanContext.Slots[i], _slotDisplayMode);
            }
        }
    }
}
