using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeController
{
    public class SerialDriver
    {
        /// <summary>
        /// Cube dimension.
        /// </summary>
        private const int DIMENSION = 8;

        /// <summary>
        /// Active serial port.
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// Default constructor for SerialDriver.
        /// </summary>
        public SerialDriver ()
        {
            _serialPort = new SerialPort ();

            // Explicitly set for clarity
            //_serialPort.PortName = 
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
        }

        /// <summary>
        /// Writes a buffer to the ATMega via the configured serial port.
        /// </summary>
        /// <param name="cubeState">The voxel state you wish to send to the cube.</param>
        public void WriteCube (bool[][][] cubeState)
        {
            byte[] buffer = ConvertCubeStateToByteArray (cubeState);
            _serialPort.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Closes the serial port that has been opened.
        /// </summary>
        public void ClosePort ()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Opens a serial port.
        /// </summary>
        public void OpenPort ()
        {
            _serialPort.Open();
        }

        /// <summary>
        /// Configures two serial settings on the port.
        /// </summary>
        /// <param name="baudRate">The serial baud rate to be set.</param>
        /// <param name="writeTimeout">The number of miliseconds before a time-out occurs when a write operation does not finish.</param>
        public void ConfigurePort (int baudRate, int writeTimeout)
        {
            _serialPort.BaudRate = baudRate;
            _serialPort.WriteTimeout = writeTimeout;
        }

        public void SendEscapeSequence()
        {
            byte[] buffer = new byte[] { 0xff, 0x00 };
            _serialPort.Write(buffer, 0, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cubeState">The voxel state you wish to send to the cube.</param>
        /// <returns>A buffer to be sent over serial comm</returns>
        private byte[] ConvertCubeStateToByteArray (bool[][][] cubeState)
        {
            List<byte> cubeStateBuffer = new List<byte> ();

            for (int x = 0; x < DIMENSION; x++)
            {
                for (int y = 0; y < DIMENSION; y++)
                {
                    byte row = 0x00;
                    for (int z = 0; z < DIMENSION; z++)
                    {
                        if (cubeState[x][y][z])
                            row |= (byte) (0x1 << z);
                    }

                    cubeStateBuffer.Add (row);
                    // 0xFF is an escape code, so it must be sent twice if is actually 
                    // intended to turn 8 LEDs on
                    if (row == (byte)0xff)
                        cubeStateBuffer.Add (row);
                }
            }

            return cubeStateBuffer.ToArray();
        }
    }
}
