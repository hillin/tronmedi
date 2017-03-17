using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class ScanningViewModel : ScanSubpageViewModel
    {
        private int _currentSlotIndex;
        private double _currentProgress;
        private double _overallProgress;
        public string Title => $"Scanning ({this.CurrentSlotIndex + 1}/{this.ScanContext.Slots.Length})";

        public SlotViewModel CurrentSlot { get; set; }
        public string CurrentFileName => this.CurrentSlot.FileName;

        public int CurrentSlotIndex
        {
            get { return _currentSlotIndex; }
            set
            {
                _currentSlotIndex = value;
                this.NotifyOfPropertyChange(() => this.CurrentSlotIndex);
                this.NotifyOfPropertyChange(() => this.Title);
            }
        }

        public double CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                this.NotifyOfPropertyChange(() => this.CurrentProgress);
                this.NotifyOfPropertyChange(() => this.OverallProgress);
            }
        }

        public double OverallProgress => (this.CurrentSlotIndex + this.CurrentProgress) / this.ScanContext.Slots.Length;

        public override void Initialize(ScanContext scanContext)
        {
            base.Initialize(scanContext);
            this.CurrentSlot = new SlotViewModel(this, scanContext.Slots[0], SlotDisplayMode.Scanning);
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };
            timer.Tick += this.Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.CurrentProgress < 1)
                this.CurrentProgress += 0.1;

            if (this.CurrentProgress >= 1)
            {
                ++this.CurrentSlotIndex;
                this.CurrentProgress = 0;
                if (this.CurrentSlotIndex >= this.ScanContext.Slots.Length)
                {
                    ((DispatcherTimer)sender).Stop();
                    this.NextStep();
                }
            }
        }
    }
}
