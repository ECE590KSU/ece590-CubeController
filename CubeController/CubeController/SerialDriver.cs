using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeController
{
    class SerialDriver
    {
        private const int DIMENSION = 8;

        /// <summary>
        /// Active serial port
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Included for testing purposes. We can decide how
        /// these two classes communicate later.
        /// </summary>
        private Cube _cube;

        /// <summary>
        /// Default constructor for SerialDriver
        /// </summary>
        public SerialDriver ()
        {
            _cube = new Cube ();
            _serialPort = new SerialPort ();

            // Explicitly set for clarity
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;            
        }

        public void WriteCube ()
        {
            byte[] buffer = ConvertCubeStateToByteArray();
            _serialPort.Write(buffer, 0, buffer.Length);
        }

        private byte[] ConvertCubeStateToByteArray ()
        {
            bool[][][] cubeState = _cube.GetCubeState ();
            Stack<byte> cubeStateBuffer = new Stack<byte>();
            byte row;
            for (int x = 0; x < DIMENSION; x++)
            {
                for (int y = 0; y < DIMENSION; y++)
                {
                    row = 0x00;
                    for (int z = 0; z < DIMENSION; z++)
                    {
                        if (cubeState[x][y][z])
                            row |= (byte)(0x1 << z);
                    }

                    cubeStateBuffer.Push(row);
                    if (row == (byte)0xff)
                        cubeStateBuffer.Push(row);
                }
            }

            return cubeStateBuffer.ToArray();
        }
    }
}
