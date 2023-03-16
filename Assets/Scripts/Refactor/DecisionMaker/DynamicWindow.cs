
public interface DynamicWindow {
    /// <summary>
    /// Calculate speed interval between max reach and min efficient 
    /// </summary>
    /// <param name="_speed">Current speed of vehicle</param>
    /// <param name="_deceleration">
    /// The _deceleration of vehicle, used to 
    /// calculate how long can be stopped
    /// </param>
    /// <param name="_weight">How important this factor is</param>
    /// <returns>Magnitude of speed</returns>
    abstract float SpeedGain(float _speed, float _deceleration,float _weight);

    /// <summary>
    /// Calculate whether vehicle is heading to the detination
    /// </summary>
    /// <param name="_angle">angle between forward motion and destination</param>
    /// <param name="_weight">How important this factor is</param>
    /// <param name="_isClose">This should get from:Camera</param>
    /// <returns>Magnitude of way point</returns>
    abstract float DestinationGain(float _angle, float _weight, bool _isClose);

    /// <summary>
    /// Decrease/Increase the confidence when vehicle meets the obstacle
    /// </summary>
    /// <param name="_dis2obs">distance fron vehicle to the obstacle</param>
    /// <param name="_bias">parameter bias</param>
    /// <returns>Magnitude of distance to obstacle</returns>
    abstract float ObstaclePenalty(float _dis2obs, float _bias);

    /// <summary>
    /// The main objective function of DWA method
    /// Add all three parameters above
    /// </summary>
    /// <param name="_speedGain">Value from:SpeedGain(...)</param>
    /// <param name="_destinationGain">Value from:DestinationGain(...)</param>
    /// <param name="_obsPenalty">Value from:ObstaclePenalty(...)</param>
    /// <returns>The value of evaluation</returns>
    abstract float DwaObjective(float _speedGain, float _destinationGain, float _obsPenalty);
}

