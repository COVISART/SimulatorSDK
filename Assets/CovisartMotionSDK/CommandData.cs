using System;

[Serializable]
public class CommandData
{
    /// <summary>
    /// Command Data
    /// </summary>
    public bool engineXErrorState;
    public bool engineYErrorState;
    public bool engineXPowerState;
    public bool engineYPowerState;
    public bool engineXEnableExactPositionState;
    public bool engineYEnableExactPositionState;
    public double currentX;
    public double currentY;
    public bool serviceRestartRequired;
    public bool engineXCalibrationState;
    public bool engineYCalibrationState;
    public bool connectionState;
    public bool covisartUdpServerState;
    public double covisartUdpServerX;
    public double covisartUdpServerY;
    public double actPositionX;
    public double actPositionY;
    public double moduloActPosX;
    public double moduloActPosY;
    public double actVelocityX;
    public double actVelocityY;
    public double posDifferenceX;
    public double posDifferenceY;
    public double actAccelerationX;
    public double actAccelerationY;
    public bool operationalX;
    public bool operationalY;
}
