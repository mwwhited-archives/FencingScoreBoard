using BinaryDataDecoders.ElectronicScoringMachines.Fencing.Common;
using BinaryDataDecoders.ToolKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FencingScoreBoard.Web.Providers
{
    public class SerialPipe : ISerialPipe
    {
        private readonly string _portName;
        private readonly int _baud;
        private readonly ScoreMachineType _type;
        private readonly ILogger _log;
        private IParseScoreMachineState _parser;

        public SerialPipe(
            IConfiguration config,
            ILogger<SerialPipe> log,
            IParseScoreMachineFactory factory
            )
        {
            _portName = config["ScoringMachine:Port"];
            _baud = config.GetValue<int>("ScoringMachine:Baud");
            _type = config.GetValue<ScoreMachineType>("ScoringMachine:MachineType");

            _parser = factory.Create(_type);

            _log = log;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _log.LogInformation($"Trying to Open: {_portName}@{_baud} for {_type}");
                try
                {
                    using (var port = new SerialPort(_portName, _baud))
                    {
                        port.Open();
                        await GetPipeAsync(port, cancellationToken, OnReceived);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError($"Serial Port Error: {_portName}: {ex.Message}");
                }
                if (!cancellationToken.IsCancellationRequested)
                {
                    _log.LogInformation($"Waiting: {_portName}@{_baud} for {_type}");
                    await Task.Delay(500);
                }
            }
        }

        static IScoreMachineState last = ScoreMachineState.Empty;
        private Task OnReceived(IScoreMachineState state)
        {
            if (!last.Equals(state))
            {
                _log.LogInformation($"S> {state}");

                //TODO: post stuff to signalr here

                last = state;
            }

            return Task.FromResult(0);
        }

        private async Task OnReceived(Memory<byte> frame)
        {
            try
            {
                var parsed = _parser.Parse(frame.Span);
                await OnReceived(parsed);
            }
            catch (Exception ex)
            {
                _log.LogError($"Bad Framing: {frame.ToArray().ToHexString()}:: {ex.Message}");
            }
        }

        private Task<string> ReadLineAsync()
        {
            return Task.FromResult(Console.ReadLine());
        }

        private Task GetPipeAsync(SerialPort port, CancellationToken cancellationToken, Func<Memory<byte>, Task> onReceived)
        {
            var pipe = new Pipe();
            var writerTask = FillPipeAsync(port.BaseStream, pipe.Writer, cancellationToken);
            var readerTask = ReadPipeAsync(pipe.Reader, cancellationToken, onReceived);
            return Task.WhenAll(writerTask, readerTask);
        }

        private async Task ReadPipeAsync(PipeReader reader, CancellationToken cancellationToken, Func<Memory<byte>, Task> onReceived)
        {
            while (true)
            {
                var result = await reader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                SequencePosition? startOfFrame = null;
                do
                {
                    // Look for a EOL in the buffer
                    startOfFrame = buffer.PositionOf(Bytes.Soh); //This is for SG
                    if (startOfFrame != null)
                    {
                        var packet = buffer.Slice(startOfFrame.Value);
                        var endOfPacket = packet.PositionOf(Bytes.Eotr); //This is for SG
                        if (endOfPacket != null)
                        {
                            var completeFrame = packet.Slice(0, buffer.GetPosition(1, endOfPacket.Value));
                            await onReceived(completeFrame.ToArray().AsMemory());

                            // Skip the line + the \n character (basically position)
                            buffer = buffer.Slice(buffer.GetPosition(1, endOfPacket.Value));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                while (startOfFrame != null && !cancellationToken.IsCancellationRequested);

                // Tell the PipeReader how much of the buffer we have consumed
                reader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming
                if (result.IsCompleted || cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete
            reader.Complete();
        }

        //https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/
        private async Task FillPipeAsync(Stream stream, PipeWriter writer, CancellationToken cancellationToken, int minBufferSize = 512)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Allocate at least 512 bytes from the PipeWriter
                var memory = writer.GetMemory(minBufferSize);
                try
                {
                    var read = await stream.ReadAsync(memory, cancellationToken);
                    if (read == 0)
                    {
                        break;
                    }

                    // Tell the PipeWriter how much was read from the Socket
                    writer.Advance(read);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    break;
                }

                // Make the data available to the PipeReader
                var result = await writer.FlushAsync();
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Tell the PipeReader that there's no more data coming
            writer.Complete();
        }
    }
}
