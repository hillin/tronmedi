using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class ScanPageViewModel : PageViewModel
    {
        private ScanContext _scanContext;

        public override string PageTitle => "Scan";

        public ScanStepViewModel[] Steps
        {
            get { return _steps; }
            private set
            {
                _steps = value;
                this.NotifyOfPropertyChange(() => this.Steps);
            }
        }

        public ScanStepViewModel[] VisibleSteps
        {
            get { return _visibleSteps; }
            private set
            {
                _visibleSteps = value;
                this.NotifyOfPropertyChange(() => this.VisibleSteps);
            }
        }

        private ScanStepViewModel _currentStep;
        private ScanStepViewModel[] _steps;
        private ScanStepViewModel[] _visibleSteps;

        public ScanStepViewModel CurrentStep
        {
            get { return _currentStep; }
            private set
            {
                _currentStep = value;
                if (_currentStep.ViewModel != null)
                {
                    _currentStep.ViewModel.Owner = this;
                    _currentStep.ViewModel.Initialize(_scanContext);
                }
                this.NotifyOfPropertyChange(nameof(this.CurrentStep));
            }
        }


        public ScanPageViewModel()
        {
            this.StartNewScan();
        }

        public void GoBack()
        {
            if (this.CurrentStep == this.Steps[0])
                throw new InvalidOperationException();

            this.CurrentStep = this.Steps[Array.IndexOf(this.Steps, this.CurrentStep) - 1];
            this.CurrentStep.IsDone = false;
        }

        public void NextStep()
        {
            if (this.CurrentStep == this.Steps.Last())
                throw new InvalidOperationException();

            this.CurrentStep.IsDone = true;
            this.CurrentStep = this.Steps[Array.IndexOf(this.Steps, this.CurrentStep) + 1];
        }

        public void StartNewScan()
        {
            this.InitializeSteps();
            _scanContext = new ScanContext();
            this.CurrentStep = this.Steps[0];
        }

        private void InitializeSteps()
        {
            this.Steps = new[]
            {
                new ScanStepViewModel
                {
                    Title = "Load",
                    IconKey = "Airplay",
                    ViewModel = new LoadSampleViewModel()
                },
                new ScanStepViewModel
                {
                    Title = "Select Slot",
                    IconKey = "CheckboxMultipleMarkedOutline",
                    ViewModel = new SelectSlotViewModel()
                },
                new ScanStepViewModel
                {
                    Title = "Scan Setup",
                    IconKey = "Settings",
                    ViewModel = new ScanSetupViewModel()
                },
                new ScanStepViewModel
                {
                    Title = "Scanning",
                    IconKey = "BlurLinear",
                    ViewModel = new ScanningViewModel()
                },
                new ScanStepViewModel
                {
                    Title = "Finish",
                    ViewModel = new CompletedViewModel()
                },
            };


            this.VisibleSteps = this.Steps.Take(4).ToArray();
        }


        public void Browse(string fileName)
        {
            MainViewModel.Browse(fileName);
        }
    }
}
