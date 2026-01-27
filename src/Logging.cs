namespace Trick;

static class Logging {
  
  public static void Setup(this ILoggingBuilder builder) {
    builder.ClearProviders();
    builder.AddConsole(opt => {
      opt.FormatterName = "simple";    
    });

    builder.AddSimpleConsole(opt => {
      opt.SingleLine = true;
      opt.TimestampFormat = null;
      opt.UseUtcTimestamp = false;
    });
  }

}
