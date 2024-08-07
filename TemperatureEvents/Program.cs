
using System.ComponentModel;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Press any key to start device...");
        Console.ReadKey();

        IDevice device = new Device();

        device.RunDevice();

        Console.ReadKey();
    }
}

public class TemperatureEventArgs : EventArgs
{
    public double Temperature { get; set; }

    public DateTime CurrentDateTime { get; set; }
}

public interface ISensor
{
    event EventHandler<TemperatureEventArgs> TemperatureReachEmergencyLevelEventHandle; //紧急
    event EventHandler<TemperatureEventArgs> TemperatureReachWarningLevelEventHandle; //警告
    event EventHandler<TemperatureEventArgs> TemperatureReachBelowWarningLevelEventHandle;//正常

    void RunSensor();
}

public class Sensor : ISensor
{
    double _warningLevel = 0;
    double _emergencyLevel = 0;

    protected EventHandlerList _listEventDelegates = new EventHandlerList();

    static readonly object _temperatureBelowWarningLevelKey = new object();
    static readonly object _temperatureReachWarningLevelKey = new object();
    static readonly object _temperatureReachEmergencyLevelKey = new object();
    bool _hasReachedWarningTemperature = false;

    private double[] _temperatureData = null;

    public Sensor(double warningLevel, double emergencyLevel)
    {
        _warningLevel = warningLevel;
        _emergencyLevel = emergencyLevel;
        SeedData();
    }

    private void SeedData()
    {
        _temperatureData = new double[] { 12, 23, 43, 56, 34, 56, 78, 34, 123, 67, 324, 234, 456, 34.5456, 23456.56, 34, 123, 5678, 345, 324, 33, 2, 4, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }

    private void MonitorTemperature()
    {
        foreach (var temperature in _temperatureData)
        {
            Console.ResetColor();
            Console.WriteLine($"DateTime: {DateTime.Now} , Temperature: {temperature}");

            if (temperature >= _emergencyLevel)
            {
                TemperatureEventArgs e = new TemperatureEventArgs
                {
                    Temperature = temperature,
                    CurrentDateTime = DateTime.Now,
                };

                OnTemperatureReachEmergencyLevel(e);
            }
            else if (temperature < _emergencyLevel && temperature >= _warningLevel)
            {
                _hasReachedWarningTemperature = true;
                TemperatureEventArgs e = new TemperatureEventArgs
                {
                    Temperature = temperature,
                    CurrentDateTime = DateTime.Now,
                };

                OnTemperatureReachWarningLevel(e);
            }
            else
            {
                if (_hasReachedWarningTemperature)
                {
                    _hasReachedWarningTemperature = false;
                    TemperatureEventArgs e = new TemperatureEventArgs
                    {
                        Temperature = temperature,
                        CurrentDateTime = DateTime.Now,
                    };

                    OnTemperatureBelowWarningLevel(e);
                }
            }

            Thread.Sleep(1000);
        }
    }

    protected void OnTemperatureReachWarningLevel(TemperatureEventArgs e)
    {
        EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureReachWarningLevelKey];

        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected void OnTemperatureBelowWarningLevel(TemperatureEventArgs e)
    {
        EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureBelowWarningLevelKey];

        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected void OnTemperatureReachEmergencyLevel(TemperatureEventArgs e)
    {
        EventHandler<TemperatureEventArgs> handler = (EventHandler<TemperatureEventArgs>)_listEventDelegates[_temperatureReachEmergencyLevelKey];

        if (handler != null)
        {
            handler(this, e);
        }
    }

    event EventHandler<TemperatureEventArgs> ISensor.TemperatureReachEmergencyLevelEventHandle
    {
        add
        {
            _listEventDelegates.AddHandler(_temperatureReachEmergencyLevelKey, value);
        }

        remove
        {
            _listEventDelegates.RemoveHandler(_temperatureReachEmergencyLevelKey, value);
        }
    }

    event EventHandler<TemperatureEventArgs> ISensor.TemperatureReachWarningLevelEventHandle
    {
        add
        {
            _listEventDelegates.AddHandler(_temperatureReachWarningLevelKey, value);
        }

        remove
        {
            _listEventDelegates.RemoveHandler(_temperatureReachWarningLevelKey, value);
        }
    }

    event EventHandler<TemperatureEventArgs> ISensor.TemperatureReachBelowWarningLevelEventHandle
    {
        add
        {
            _listEventDelegates.AddHandler(_temperatureBelowWarningLevelKey, value);
        }

        remove
        {
            _listEventDelegates.RemoveHandler(_temperatureBelowWarningLevelKey, value);
        }
    }

    void ISensor.RunSensor()
    {
        Console.WriteLine("Temperature Sensor is running....");
        MonitorTemperature();
    }
}

public interface ICoolingMechanism
{
    void On();
    void Off();
}

public class CoolingMechanism : ICoolingMechanism
{
    void ICoolingMechanism.Off()
    {
        Console.WriteLine("Switching cooling mechanism off ....");
    }

    void ICoolingMechanism.On()
    {
        Console.WriteLine("Switching cooling mechanism on ....");
    }
}

public interface IDevice
{
    double WarningLevel { get; }

    double EmergencyLevel { get; }

    void RunDevice();
    void HandleEmergency();
}

public class Device : IDevice
{
    const double _warningLevel = 38;
    const double _emergencyLevel = 80;

    public double WarningLevel => _warningLevel;

    public double EmergencyLevel => _emergencyLevel;

    void IDevice.HandleEmergency()
    {
        Console.WriteLine("Sending out notifications to emergency services personal...");
        Console.WriteLine("Shutting down devices...");
    }

    void IDevice.RunDevice()
    {
        Console.WriteLine("Device is running...");
        ICoolingMechanism coolingMechanism = new CoolingMechanism();
        Sensor sensor = new Sensor(_warningLevel, _emergencyLevel);

        IThermostat thermostat = new Thermostat(this, sensor, coolingMechanism);
        thermostat.RunThermostat();
    }
}

public interface IThermostat
{
    void RunThermostat();
}

public class Thermostat : IThermostat
{
    private ICoolingMechanism _coolingMechanism;
    private ISensor _sensor;
    private IDevice _device;

    public Thermostat(IDevice device, ISensor sensor, ICoolingMechanism coolingMechanism)
    {
        _device = device;
        _sensor = sensor;
        _coolingMechanism = coolingMechanism;
    }

    private void WireUpEventsToEventHandlers()
    {
        _sensor.TemperatureReachWarningLevelEventHandle += HeatSensor_TemperatureReachWarningLevelEventHandler;
        _sensor.TemperatureReachEmergencyLevelEventHandle += HeatSensor_TemperatureReachEmergencyLevelEventHandler;
        _sensor.TemperatureReachBelowWarningLevelEventHandle += HeatSensor_TemperatureBelowWarningLevelEventHandler;
    }

    private void HeatSensor_TemperatureReachWarningLevelEventHandler(object sender, TemperatureEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Warning! (Warning level is between {_device.WarningLevel} and {_device.EmergencyLevel})");
        _coolingMechanism.On();
        Console.ResetColor();
    }

    private void HeatSensor_TemperatureReachEmergencyLevelEventHandler(object sender, TemperatureEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Emergency! (Emergency level is {_device.EmergencyLevel} and above.)");
        _device.HandleEmergency();
        Console.ResetColor();
    }

    private void HeatSensor_TemperatureBelowWarningLevelEventHandler(object sender, TemperatureEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Infomation! (Temperature is below {_device.WarningLevel})");
        _coolingMechanism.Off();
        Console.ResetColor();
    }

    void IThermostat.RunThermostat()
    {
        Console.WriteLine("Thermostat is running...");
        WireUpEventsToEventHandlers();
        _sensor.RunSensor();
    }
}