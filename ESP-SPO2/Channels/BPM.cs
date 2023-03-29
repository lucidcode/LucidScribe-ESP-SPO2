using System;
using System.Drawing;

namespace lucidcode.LucidScribe.Plugin.ESPSPO2
{
    namespace lucidcode.LucidScribe.Plugin.ESPSPO2
    {
        namespace Bpm
        {
            public class PluginHandler : Interface.LucidPluginBase
            {
                public override string Name { get { return "ESP BPM"; } }

                public override bool Initialize()
                {
                    try
                    {
                        return Device.Initialize();
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception("The '" + Name + "' plugin failed to initialize: " + ex.Message));
                    }
                }

                public override double Value
                {
                    get
                    {
                        return Device.GetBpm();
                    }
                }

                public override void Dispose()
                {
                    Device.Dispose();
                }
            }
        }

        namespace BpmRaw
        {
            public class PluginHandler : Interface.ILluminatedPlugin
            {
                public string Name { get { return "ESP BPM Raw"; } }

                public bool Initialize()
                {
                    try
                    {
                        bool initialized = Device.Initialize();
                        Device.BpmChanged += BpmChanged;
                        return initialized;
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception("The '" + Name + "' plugin failed to initialize: " + ex.Message));
                    }
                }

                public event Interface.SenseHandler Sensed;
                public void BpmChanged(object sender, EventArgs e)
                {
                    if (ClearBuffer)
                    {
                        ClearBuffer = false;
                        BufferData = "";
                    }
                    if (BufferData.Length > 8096) return;

                    BufferData += sender + ",";
                    if (ClearTicks)
                    {
                        ClearTicks = false;
                        TickCount = "";
                    }
                    TickCount += sender + ",";
                }

                public void Dispose()
                {
                    Device.BpmChanged -= BpmChanged;
                    Device.Dispose();
                }

                public Boolean isEnabled = false;
                public Boolean Enabled
                {
                    get
                    {
                        return isEnabled;
                    }
                    set
                    {
                        isEnabled = value;
                    }
                }

                public Color PluginColor = Color.White;
                public Color Color
                {
                    get
                    {
                        return Color;
                    }
                    set
                    {
                        Color = value;
                    }
                }

                private Boolean ClearTicks = false;
                public String TickCount = "";
                public String Ticks
                {
                    get
                    {
                        ClearTicks = true;
                        return TickCount;
                    }
                    set
                    {
                        TickCount = value;
                    }
                }

                private Boolean ClearBuffer = false;
                public String BufferData = "";
                public String Buffer
                {
                    get
                    {
                        ClearBuffer = true;
                        return BufferData;
                    }
                    set
                    {
                        BufferData = value;
                    }
                }

                int lastHour;
                public int LastHour
                {
                    get
                    {
                        return lastHour;
                    }
                    set
                    {
                        lastHour = value;
                    }
                }
            }
        }
    }
}
