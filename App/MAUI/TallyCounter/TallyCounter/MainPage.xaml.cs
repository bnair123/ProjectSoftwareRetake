namespace TallyCounter;


public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;
    private Counter _currentCounter;
    private int _currentCounterId;
    private System.Timers.Timer _liveModeTimer;


    public MainPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        //CounterSelector.SelectedIndex = 0; // Default to the first counter
        //CounterSelector.SelectedIndexChanged += CounterSelector_SelectedIndexChanged;
        InitializeLiveModeTimer();
        LoadCounter(0); // Load the first counter by default
    }
    
    /// <summary>
    /// Initializes the timer for live mode, which polls for updates to the current counter every 2 seconds.
    /// </summary>
    private void InitializeLiveModeTimer()
    {
        _liveModeTimer = new System.Timers.Timer(2000); // Poll every 2 seconds
        _liveModeTimer.Elapsed += async (sender, e) => 
        {
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                await LoadCounter(_currentCounterId);
            });
        };
        _liveModeTimer.AutoReset = true;
    }

    /// <summary>
    /// Loads a counter by its ID, updating the current counter and UI accordingly. Calls the apiHandler.cs (called apiService inside tho
    /// </summary>
    /// <param name="id"></param>
    private async Task LoadCounter(int id)
    {
        _currentCounterId = id;
        _currentCounter = await _apiService.GetCounterAsync(id);
        UpdateUI();
    }

    /// <summary>
    /// Updates UI with the values
    /// </summary>
    private void UpdateUI()
    {
        CounterValueLabel.Text = $"Value: {_currentCounter.Value}";
        CounterLimitLabel.Text = $"Limit: {_currentCounter.Limit}";
    }

    /// <summary>
    /// Increments counter by 1 when button is pressed. WILL OVERRIDE CURRENT VALUES FULLY, MAYBE UPDATE BEFORE SUBMITTING?
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Increment_Clicked(object sender, EventArgs e)
    {
        if (_currentCounter.Value < _currentCounter.Limit)
        {
            _currentCounter.Value++;
            var success = await _apiService.UpdateCounterAsync(_currentCounterId, _currentCounter);
            if (success) UpdateUI();
        }
    }

    /// <summary>
    /// Same as Increment with same issue. Again calls apiService
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Decrement_Clicked(object sender, EventArgs e)
    {
        if (_currentCounter.Value > 0)
        {
            _currentCounter.Value--;
            var success = await _apiService.UpdateCounterAsync(_currentCounterId, _currentCounter);
            if (success) UpdateUI();
        }
    }

    /// <summary>
    /// Reads teh given value for limit and passes it with the current value and updates counter. Still same bug
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ChangeLimit_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(NewLimitEntry.Text, out int newLimit) && newLimit >= _currentCounter.Value)
        {
            _currentCounter.Limit = newLimit;
            var success = await _apiService.UpdateCounterAsync(_currentCounterId, _currentCounter);
            if (success) UpdateUI();
            NewLimitEntry.Text = string.Empty; // Clear the input field
        }
        else
        {
            await DisplayAlert("Error", "Invalid limit. Ensure it's a number greater than or equal to the current value.", "OK");
        }
    }

    /// <summary>
    /// I can't even tell anymore
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /*private void CounterSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (CounterSelector.SelectedIndex != -1)
        {
            LoadCounter(CounterSelector.SelectedIndex);
        }
    }*/
    
    /// <summary>
    /// Function to change between the 3 different counters
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCounterChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null) return;

            int selectedCounterId = 0;
            if (radioButton == Counter0RadioButton) selectedCounterId = 0;
            else if (radioButton == Counter1RadioButton) selectedCounterId = 1;
            else if (radioButton == Counter2RadioButton) selectedCounterId = 2;

            LoadCounter(selectedCounterId);
        }
    }
    
    /// <summary>
    /// The same as the other increments/decrement except its by 10
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void IncrementByTen_Clicked(object sender, EventArgs e)
    {
        await AdjustCounterValue(10);
    }

    /// <summary>
    /// still the same
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DecrementByTen_Clicked(object sender, EventArgs e)
    {
        await AdjustCounterValue(-10);
    }

    /// <summary>
    /// Takes the current counter value and then adds however much was added to it
    /// </summary>
    /// <param name="adjustment"></param>
    private async Task AdjustCounterValue(int adjustment)
    {
        var newValue = _currentCounter.Value + adjustment;
        if (newValue >= 0 && newValue <= _currentCounter.Limit)
        {
            _currentCounter.Value = newValue;
            var success = await _apiService.UpdateCounterAsync(_currentCounterId, _currentCounter);
            if (success) UpdateUI();
        }
    }
    
    /// <summary>
    /// Enables the live mode which refreshes the value every certain amount of time (can be adjusted when init the live counter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LiveModeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            _liveModeTimer.Start();
        }
        else
        {
            _liveModeTimer.Stop();
        }
    }


}



