namespace TallyCounterUno;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
        builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("TallyCounterUno/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("TallyCounterUno/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}
