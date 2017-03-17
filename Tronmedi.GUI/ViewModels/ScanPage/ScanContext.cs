using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tronmedi.GUI.Models;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class ScanContext
    {
        public ScanSlotContext[] Slots { get; }
        public ScanContext()
        {
            this.Slots = new ScanSlotContext[Defaults.SlotCount];
            for (var i = 0; i < Defaults.SlotCount; ++i)
            {
                this.Slots[i] = new ScanSlotContext
                {
                    Name = $"Slot {i + 1}"
                };
            }
        }
    }
}
