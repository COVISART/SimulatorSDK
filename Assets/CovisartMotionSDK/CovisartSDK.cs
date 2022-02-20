using CovisartCommunicationSDK;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UI;
using SimulatorServiceUnity.Enums;
using System.Net.Sockets;
using System.Net;
using System.Text;
using SimulatorServiceUnity.CommonClasses;

namespace CovisartMotionSDK
{
    public class CovisartSDK : MonoBehaviour
    {
        private static CommunicationSDK _communication;
        private AxisData _axisData;
        private SimulatorCommandData _commandData;
        private ProgressState<bool> _progressState;
        private bool _toggleExactPosition;
        
        public bool isDataTransferStarted;
        public AircraftData aircraftData;
        private Thread _controlThread;
        private CommandData _state;
        private Text _buttonText;
        private readonly Socket _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static readonly IPAddress ServerAddress = IPAddress.Parse("127.0.0.1");
        private readonly IPEndPoint _endPoint = new IPEndPoint(ServerAddress, 555);

        private void Awake()
        {
            _toggleExactPosition = false;
            _commandData = new SimulatorCommandData();
            _state = new CommandData();
            OnStateUpdate();
            _progressState = new ProgressState<bool>();
            
        }

        private void SetButtonText(int buttonNumber, string text)
        {
            // Get child button gameObject then get grandchild gameObject text.
            var textObj = this.gameObject.transform.GetChild(buttonNumber).GetChild(0).gameObject;
            _buttonText = textObj.GetComponent<Text>();

            _buttonText.text = text;
        }
        
        private void ControlTread(Func<byte[]> command, string log)
        {
            if (!(_controlThread?.IsAlive ?? false))
            {
                _controlThread = new Thread(() => SendData(command()));
                _controlThread.Start();
                Debug.Log(log);
            }
            else
            {
                Debug.LogError("Thread is busy");
            }
        }

        private void ControlTread(Func<EngineType ,byte[]> command,EngineType v, string log)
        {
            if (!(_controlThread?.IsAlive ?? false))
            {
                _controlThread = new Thread(() => SendData(command(v)));
                _controlThread.Start();
                Debug.Log(log);
            }
            else
            {
                Debug.LogError("Thread is busy");
            }
        }

        private void ControlTread(Action command)
        {
            if (!(_controlThread?.IsAlive ?? false))
            {
                _controlThread = new Thread(() => command());
                _controlThread.Start();
            }
            else
            {
                Debug.LogError("Thread is busy");
            }
        }

        public void OpenConnection()
        {
            // toggle connection off and on button
            if (!_state.connectionState)
            {
                ControlTread(_commandData.OpenConnection, "Opened connection");
                OnStateUpdate();
                // change button text close connection
                SetButtonText(0, "Disconnect");
                // make power X and Y button active ...
            }
            else
            {
                ControlTread(_commandData.CloseConnection, "Closed connection");
                OnStateUpdate();
                // change button text open connection
                SetButtonText(0, "Connect");
                SetButtonText(1, "Power");
                // make power X and Y button passive ...
            }
        }

        public void PowerMotors()
        {
            if (_state.engineXPowerState && _state.engineYPowerState && _state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOff, "Motors powered off");
                OnStateUpdate();
                // make calibrate X and Y buttons passive

                // change button text power on and change color
                SetButtonText(1, "Power");
                // toggle powerMotors on and change button text
            }
            else if (_state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOn, "Motors Powered");
                OnStateUpdate();
                // make calibrate X and Y buttons active
                
                // change button text power off and change color
                SetButtonText(1, "Power Off");
                // toggle powerMotors off and button text
            }
            else
            {
                Debug.LogError("Can not power motors");
            }
        }

        public void PowerAxisX()
        {
            if (_state.engineXPowerState && _state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOffX, "Motors X powered off");
                OnStateUpdate();
                // make calibrate X button passive
                // change button text "power X on"

            }
            else if (_state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOnX, "X Powered");
                OnStateUpdate();
                // make calibrate X button active
                // change button text "power X off"

            }
            else
            {
                Debug.LogError("Can not power X motors");
            }
        }

        public void PowerAxisY()
        {
            if (_state.engineYPowerState && _state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOffY, "Motors Y powered off");
                OnStateUpdate();
                // make calibrate Y button passive
                // change button text "power Y on"
                // toggle powerMotors text on
            }
            else if (_state.connectionState && !_progressState.hasError)
            {
                ControlTread(_commandData.PowerOnY, "Y Powered");
                OnStateUpdate();
                // make calibrate Y button 
                // change button text "power Y off"
                // toggle powerMotors text Y off
            }
            else
            {
                Debug.LogError("Can not power Y motors");
            }
        }

        public void CalibrateAxisX()
        {
            if (_state.engineXPowerState && !_state.engineXEnableExactPositionState && !_progressState.hasError && !_state.engineXCalibrationState && _state.connectionState)
            {
                ControlTread(_commandData.CalibrateX, "X Calibrated");
                OnStateUpdate();
                // make ... active
                // toggle Calibrate X on and off
            }
            else
            {
                Debug.LogError("Can not cannot calibrate Axis X");
            }
        }

        public void CalibrateAxisY()
        {
            if (_state.engineYPowerState && !_state.engineYEnableExactPositionState && !_progressState.hasError && !_state.engineYCalibrationState && _state.connectionState)
            {
                ControlTread(_commandData.CalibrateY, "Y Calibrated");
                OnStateUpdate();
                // make ... active
                // toggle Calibrate Y on and off
            }
            else
            {
                Debug.LogError("Can not cannot calibrate Axis Y");
            }
        }

        public void ResetError()
        {
            if (_state.connectionState && _progressState.hasError)
            {
                ControlTread(_commandData.ResetError, "Error Reset");
                OnStateUpdate();
            }
            else
            {
                Debug.LogError("Progress state has no error");
            }
        }

        public void ResetErrorX()
        {
            if (_state.connectionState && _progressState.hasError)
            {
                ControlTread(_commandData.ResetErrorX, "Error X Reset");
                OnStateUpdate();
            }
            else
            {
                Debug.LogError("Progress state has no error");
            }
        }

        public void ResetErrorY()
        {
            if (_state.connectionState && _progressState.hasError)
            {
                ControlTread(_commandData.ResetErrorY, "Error Y Reset");
                OnStateUpdate();
            }
            else
            {
                Debug.LogError("Progress state has no error");
            }
        }

        private void StartExactPositionThread()
        {
            switch (_state.engineXPowerState)
            {
                // Enable exact position
                case true when _state.engineYPowerState && !_state.engineXEnableExactPositionState &&
                               !_state.engineYEnableExactPositionState && !_progressState.hasError &&
                               _state.engineXCalibrationState && _state.engineYCalibrationState &&
                               _state.connectionState:
                    SendData(_commandData.EnableExactPositionX());
                    SendData(_commandData.EnableExactPositionY());
                    OnStateUpdate();
                
                    Debug.Log("Exact Position Enabled");
                    PrintStateInfo();
                    break;
                // disable exact position
                case true when _state.engineYPowerState && _state.engineXEnableExactPositionState &&
                               _state.engineYEnableExactPositionState && !_progressState.hasError &&
                               _state.connectionState:
                    SendData(_commandData.DisableExactPositionX());
                    SendData(_commandData.DisableExactPositionY());
                    OnStateUpdate();

                    Debug.Log("Exact Position Disabled");
                    PrintStateInfo();
                    break;
                default:
                    Debug.LogError("Invalid");
                    break;
            }
        }

        public void StartCovisartUDP()
        {
            ControlTread(_commandData.StartCovisartUdpServer, "Covisart UDP started");
            OnStateUpdate();
        }

        public void StopCovisartUDP()
        {
            ControlTread(_commandData.StopCovisartUdpServer, "Covisart UDP stopped");
            OnStateUpdate();
        }

        public void StartExactPosition()
        {
            
            ControlTread(StartExactPositionThread);

            // Toggle button text
            if (_toggleExactPosition == false)
            {
                SetButtonText(6, "DisableExactPosition");
                _toggleExactPosition = true;
            } 
            else
            {
                SetButtonText(6, "StartExactPosition");
                _toggleExactPosition = false;
            }
        }

        public void StartDataListener()
        {
            if (!_state.engineXCalibrationState || !_state.engineYCalibrationState ||
                _state.engineXEnableExactPositionState || _state.engineYEnableExactPositionState ||
                _state.engineXErrorState || _state.engineYErrorState) return;
            ControlTread(_commandData.StartArmaThread, "Data listener started.");
            OnStateUpdate();
        }

        private void StartDataTransferThread()
        {
            _communication = new CommunicationSDK();
            _progressState = _communication.StartCommunication();
            if (_progressState.hasError)
                Debug.LogError(_progressState.errorMessage);
            else
            {
                isDataTransferStarted = true;
            }
        }

        public void StartDataTransfer()
        {
            ControlTread(StartDataTransferThread);
        }

        private void StopDataTransferThread()
        {
            _communication ??= new CommunicationSDK();
            _progressState = _communication.StopCommunication();
            if (_progressState.hasError)
                Debug.LogError(_progressState.errorMessage);
            else
            {
                isDataTransferStarted = false;
            }
        }

        public void StopDataTransfer()
        {
            ControlTread(StopDataTransferThread);
        }

        public void ManuelControlUp()
        {
            ControlTread(_commandData.ManuelControlUp, EngineType.X, "Motors X up");
            ControlTread(_commandData.ManuelControlUp, EngineType.Y, "Motors Y up");
            OnStateUpdate();
        }

        public void ManuelControlDown()
        {
            ControlTread(_commandData.ManuelControlDown, EngineType.X, "Motors X Down");
            ControlTread(_commandData.ManuelControlDown, EngineType.Y, "Motors Y Down");
            OnStateUpdate();
        }

        /*private void SendOfData(string axisX, string axisY)
        {
            _axisData = new AxisData { AxisX = axisX, AxisY = axisY };
            _progressState = _communication.SendData(_axisData);
            if (_progressState.hasError)
                Debug.Log(_progressState.errorMessage);
        }*/

        private void Update()
        {
            //if (!IsDataTransferStarted) return;
            //var x = Aircraft.transform.eulerAngles.x.ToString();
            //var y = Aircraft.transform.eulerAngles.y.ToString();
            //SendOfData(x, y);

            // Send aircraft data every frame
            if (_state.covisartUdpServerState)
            {
                SendUDPData(aircraftData);
            }
        }

        // Updates Json
        public void OnStateUpdate()
        {
            var json = SendData(_commandData.GetState());
            _state = JsonUtility.FromJson<CommandData>(json);
        }

        private void PrintStateInfo()
        {
            var t = _state.GetType();
            var fields = t.GetFields();

            foreach (var field in fields)
            {
                Debug.Log(field.Name + " " + field.FieldType.Name + " " + field.GetValue(_state));
            }
        }


        /*void FixedUpdate()
        {
            var state = SendData(_commandData.GetState());
            Debug.Log(state);
        }*/

        private static string SendData(byte[] bits)
        {
            return SimulatorTcpClient.Connect("127.0.0.1", bits);
        }
        
        private void SendUDPData(AircraftData data)
        {
            var text = data.axisX + "*";
            text += data.axisY + "*";
            text += data.axisZ + "*";
            text += data.eulerAngle.x + "*";
            text += data.eulerAngle.y + "*";
            text += data.eulerAngle.z + "*";

            var sendBuffer = Encoding.ASCII.GetBytes(text);

            _sock.SendTo(sendBuffer, _endPoint);
        }
    }
}