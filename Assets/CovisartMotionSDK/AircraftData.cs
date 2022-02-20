using SimulatorServiceUnity.CommonClasses;
using UnityEngine;

namespace CovisartMotionSDK
{
    public class AircraftData : MonoBehaviour
    {
        public GameObject jet;
        public double axisX;
        public double axisY;
        public double axisZ;
        public Vector3 eulerAngle;
        public Vector3 position;


        private SimulatorCommandData _commandData;
        private void OnEnable()
        {
            _commandData = new SimulatorCommandData();
        }

        private void Update()
        {
            var localEulerAngles = jet.transform.localEulerAngles;
            axisX = localEulerAngles.x ;
            axisY = localEulerAngles.y ;
            axisZ = localEulerAngles.z ;
            eulerAngle = jet.transform.eulerAngles;
            position = jet.transform.localPosition;
        }
        
        public void StartDataTransfer()
        {
            SendData(_commandData.OpenConnection());
            SendData(_commandData.PowerOn());
            SendData(_commandData.EnableExactPositionX());
            SendData(_commandData.EnableExactPositionY());
            var state = SendData(_commandData.GetState());
            Debug.Log(state);
        }

        private static  string SendData(byte[] bits)
        {
            return SimulatorTcpClient.Connect("127.0.0.1", bits);
        }
    }
}

