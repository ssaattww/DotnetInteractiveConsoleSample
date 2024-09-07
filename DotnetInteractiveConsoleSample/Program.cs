
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;

using System.Reactive.Linq;

using Formatter = Microsoft.DotNet.Interactive.Formatting.Formatter;

var kernel = new CSharpKernel()
                    .UseNugetDirective()
                    .UseKernelHelpers()
                    .UseWho()
                    .UseValueSharing();
Formatter.SetPreferredMimeTypesFor(typeof(object), "text/plain");
Formatter.Register<object>(o => o.ToString());

while (true)
{
    if (ReadLine() is not { } request)
        continue;
    var toSubmit = new SubmitCode(request);
    var response = await kernel.SendAsync(toSubmit);

    response.KernelEvents.Subscribe(e =>
    {
        switch (e)
        {
            case CommandFailed failed:
                WriteLineError(failed.Message);
                break;
            case DisplayEvent display:
                WriteLine(display.FormattedValues.First().Value);
                break;
        }
    });
}
static string? ReadLine()
{
    Console.Write("\nInput: ");
    return Console.ReadLine();
}

static void WriteLine(string input)
{
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine($"\nOutput: {input}");
    Console.ForegroundColor = ConsoleColor.Gray;
}

static void WriteLineError(string input)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nError: {input}");
    Console.ForegroundColor = ConsoleColor.Gray;
}




