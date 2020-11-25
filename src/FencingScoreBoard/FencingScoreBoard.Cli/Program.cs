using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;
using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Favero;
using FencingScoreBoard.WpCommonf;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace FencingScoreBoard.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            var top = Application.Top;

            // Creates the top-level window to show
            var win = new Window("Fencing Score Board")
            {
                X = 0,
                Y = 1, // Leave one row for the toplevel menu

                // By using Dim.Fill(), it will automatically resize without manual intervention
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(win);

            // Creates a menubar, the item "New" has a help menu.
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    //new MenuItem ("_New", "Creates new file", NewFile),
                    //new MenuItem ("_Close", "", () => Close ()),
                    new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
                }),
                //new MenuBarItem ("_Edit", new MenuItem [] {
                //    new MenuItem ("_Copy", "", null),
                //    new MenuItem ("C_ut", "", null),
                //    new MenuItem ("_Paste", "", null)
                //})
            });
            top.Add(menu);

            var ms = new MemoryStream();
            var hexViewer = new HexView(ms);

            IParseScoreMachineState parser = new FaveroStateParser();
            var provider = new SerialPortProvider(data =>
            {
                Debug.WriteLine(data);
                ms.Write(data.ToArray(), 0, data.Length);

                return Task.FromResult(0);
            });

            var deviceTypes = new ListView(new[] { "SG", "Favero" }.ToList());
            var deviceTypeFrame = new FrameView("Devices")
            {
                Height = Dim.Percent(50),
                Width = Dim.Percent(25),
            };
            deviceTypeFrame.Add(deviceTypes);

            var ports = new ListView(provider.ListPorts().ToList());
            var portFrame = new FrameView("Serial Ports")
            {
                Y = Pos.Percent(50),
                Height = Dim.Percent(100),
                Width = Dim.Percent(25),
            };
            portFrame.Add(ports);

            //var connect = new Button("Open")
            //{
            //    Y = Pos.Bottom(portFrame),
            //};
            //connect.Clicked = () =>
            //{
            //    var portName = ports.Source is List<string> names ? names[ports.SelectedItem] : null;
            //    Debug.WriteLine(portName);
            //    if (!string.IsNullOrWhiteSpace(portName))
            //        provider.Open(portName);
            //};
            //portFrame.Add(connect);

            var scoreFrame = new FrameView("Scores")
            {
                X = Pos.Percent(25),
                Height = Dim.Percent(85),
                Width = Dim.Percent(100),
            };

            var dataFrame = new FrameView("Data")
            {
                X = Pos.Percent(25),
                Y = Pos.Percent(85),
                Height = Dim.Percent(100),
                Width = Dim.Percent(100),
            };


            win.Add(deviceTypeFrame);
            win.Add(portFrame);
            win.Add(scoreFrame);
            win.Add(dataFrame);


            //var ports =  provider.ListPorts();
            // foreach(var port in ports)
            // {
            //     Console.WriteLine(port);
            // }

            Application.Run();
        }

        static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
            return n == 0;
        }

    }
}
