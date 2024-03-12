global using System.Collections.Immutable;
global using System.Windows.Input;
global using Microsoft.Extensions.DependencyInjection;
global using Windows.Networking.Connectivity;
global using Windows.Storage;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
global using Microsoft.Extensions.Options;
global using TallyCounterUno.Business.Models;
global using TallyCounterUno.Infrastructure;
global using TallyCounterUno.Presentation;
global using TallyCounterUno.DataContracts;
global using TallyCounterUno.DataContracts.Serialization;
global using TallyCounterUno.Services.Caching;
global using TallyCounterUno.Services.Endpoints;
#if MAUI_EMBEDDING
global using TallyCounterUno.MauiControls;
#endif
global using Uno.UI;
global using Windows.ApplicationModel;
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
