using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;

namespace OpucForm.Components.Pages
{
    public partial class Home : ComponentBase
    {

        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool SameAsHomeAddress { get; set; } = true;
        private string? QualificationProgram { get; set; } = "";
        private string _ssn = "";
        private string? SelectedFileName;

        public string selectedProvider { get; set; } = "N/A";


        // DB
        //  string zipCode = "";


        // Form Viewing
        private bool showApplicationBody = false;

        // ================== Pagination ==================
        private int CurrentStep = 1;
        private int TotalSteps = 4;

        private async Task GoNext()
        {
            if (CurrentStep < TotalSteps)
                CurrentStep++;

            await ScrollToTop();
        }

        private async Task GoBack()
        {
            if (CurrentStep > 1)
                CurrentStep--;

            await ScrollToTop();
        }

        private async Task GoToStep(int step)
        {
            CurrentStep = step;
            await ScrollToTop();
        }

        private string GetBulletClass(int step)
        {
            if (step < CurrentStep) return "bullet past";        // Completed steps
            if (step == CurrentStep) return "bullet current";    // Current step
            return "bullet future";                              // Upcoming steps
        }

        // Percentage of progress (0% = step 1 not started, 100% = step 4 completed)
        private double ProgressPercentage
        {
            get
            {
                // Step 1 starts full (100% of the first bullet), then progresses gradually
                if (CurrentStep == 1)
                    return 0; // no fill for step 1
                return ((CurrentStep - 1) / (double)(TotalSteps - 1)) * 100;
            }
        }







        // When user selects a provider to apply to, move to step 2.
        private async Task HandleProviderSelected(string provider)
        {
            selectedProvider = provider; // update provider
            CurrentStep = 2; // move to step 2
            await ScrollToTop();

        }

        // Function to scroll smoothly to top when user navigates form.
        private async Task ScrollToTop()
        {
            if (JS != null)
            {
                await JS.InvokeVoidAsync("scrollToTop");
            }
        }

        // ================== Pagination ==================

        // May not be needed anymore if were not using xxx-xx-xxxx format for SSNs
        public string SSN

        {
            get => _ssn;
            set
            {
                // Remove non-digits
                var digits = new string(value.Where(char.IsDigit).ToArray());

                // Limit to 9 digits
                if (digits.Length > 9)
                    digits = digits[..9];

                // Add dashes
                if (digits.Length > 5)
                    _ssn = $"{digits[..3]}-{digits.Substring(3, 2)}-{digits[5..]}";
                else if (digits.Length > 3)
                    _ssn = $"{digits[..3]}-{digits[3..]}";
                else
                    _ssn = digits;
            }
        }


        // May not be needed anymore since this will be handled in step2model.
        private void OnFileSelected(InputFileChangeEventArgs e)
        {
            SelectedFileName = e.FileCount > 0
                ? e.File.Name
                : null;
        }



    }
}
