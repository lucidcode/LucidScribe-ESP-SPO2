using System;
using System.Collections.Generic;
using System.Drawing;

namespace lucidcode.LucidScribe.Plugin.ESPSPO2
{
    namespace REM
    {
        public class PluginHandler : Interface.LucidPluginBase
        {

            static int TicksSinceLastArtifact = 0;
            static int TicksAbove = 0;

            public override string Name
            {
                get
                {
                    return "ESP SPO2 REM";
                }
            }

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

            List<int> m_arrHistory = new List<int>();

            public override double Value
            {
                get
                {

                    if (Device.Algorithm == "REM Detection")
                    {
                        // Update the mem list
                        m_arrHistory.Add(Convert.ToInt32(Device.GetIr()));
                        if (m_arrHistory.Count > 512) { m_arrHistory.RemoveAt(0); }

                        // Check for blinks
                        int intBlinks = 0;
                        bool boolBlinking = false;

                        int intBelow = 0;
                        int intAbove = 0;

                        bool boolDreaming = false;
                        foreach (Double dblValue in m_arrHistory)
                        {
                            if (dblValue > Device.Threshold)
                            {
                                intAbove += 1;
                                intBelow = 0;
                            }
                            else
                            {
                                intBelow += 1;
                                intAbove = 0;
                            }

                            if (!boolBlinking)
                            {
                                if (intAbove >= 1)
                                {
                                    boolBlinking = true;
                                    intBlinks += 1;
                                    intAbove = 0;
                                    intBelow = 0;
                                }
                            }
                            else
                            {
                                if (intBelow >= Device.BlinkInterval)
                                {
                                    boolBlinking = false;
                                    intBelow = 0;
                                    intAbove = 0;
                                }
                                else
                                {
                                    if (intAbove >= 12)
                                    {
                                        // reset
                                        boolBlinking = false;
                                        intBlinks = 0;
                                        intBelow = 0;
                                        intAbove = 0;
                                    }
                                }
                            }

                            if (intBlinks > 6)
                            {
                                boolDreaming = true;
                                break;
                            }

                            if (intAbove > 12)
                            { // reset
                                boolBlinking = false;
                                intBlinks = 0;
                                intBelow = 0;
                                intAbove = 0; ;
                            }
                            if (intBelow > 80)
                            { // reset
                                boolBlinking = false;
                                intBlinks = 0;
                                intBelow = 0;
                                intAbove = 0; ;
                            }
                        }

                        if (boolDreaming)
                        { return 888; }

                        if (intBlinks > 10) { intBlinks = 10; }
                        return intBlinks * 100;
                    }
                    else if (Device.Algorithm == "Motion Detection")
                    {
                        if (Device.GetIr() > 105335)
                        {
                            TicksAbove++;
                            if (TicksAbove > 5)
                            {
                                TicksAbove = 0;
                                TicksSinceLastArtifact = 0;
                                if (TicksSinceLastArtifact > 19200)
                                {
                                    return 888;
                                }
                            }
                        }

                        TicksSinceLastArtifact++;
                        return 0;
                    }

                    return 0;
                }
            }

            public override void Dispose()
            {
                Device.Dispose();
            }
        }
    }
}
