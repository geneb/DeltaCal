using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltalCal {

    class classDeltaCalEngine {
        const double degreesToRadians = Math.PI / 180.0;
        const int numPoints = 10; // number of points to probe.
                                  // machine IDs (these match the SeeMeCNC fork of Repetier).
        const int SMC_ORION = 1;
        const int SMC_MAX_V2 = 2;
        const int SMC_ERIS = 3;
        const int SMC_MAX_V3 = 5;
        const int SMC_H2 = 6;
        const int DEFAULT_PROBE_HEIGHT = 25;

        int machineType = 0;
        int stepsPerMM = 0;

        String[] xBedProbePoints;
        String[] yBedProbePoints;
        String[] zBedProbePoints;
        double bedRadius;

        public classDeltaCalEngine(double bedRadius) {
            xBedProbePoints = new String[numPoints];
            yBedProbePoints = new String[numPoints];
            zBedProbePoints = new String[numPoints];

            this.bedRadius = bedRadius;

        }

        void calcProbePoints(int numPoints) {
            if (numPoints == 4) {
                for (int i = 0; i < 3; ++i) {
                    xBedProbePoints[i] = (bedRadius * Math.Sin((2 * Math.PI * i) / 3)).ToString("N2");
                    yBedProbePoints[i] = (bedRadius * Math.Cos((2 * Math.PI * i) / 3)).ToString("N2");
                    zBedProbePoints[i] = "0.0"; // we default this to zero -gwb
                }
                xBedProbePoints[3] = "0.0";
                yBedProbePoints[3] = "0.0";
                zBedProbePoints[3] = "0.0";
            } else {
                if (numPoints >= 7) {
                    for (int i = 0; i < 6; ++i) {
                        xBedProbePoints[i] = (bedRadius * Math.Sin((2 * Math.PI * i) / 6)).ToString("N2");
                        yBedProbePoints[i] = (bedRadius * Math.Cos((2 * Math.PI * i) / 6)).ToString("N2");
                        zBedProbePoints[i] = "0.0"; // we default this to zero -gwb
                    }
                }
                if (numPoints >= 10) {
                    for (int i = 6; i < 9; ++i) {
                        xBedProbePoints[i] = (bedRadius / 2 * Math.Sin((2 * Math.PI * (i - 6)) / 3)).ToString("N2");
                        yBedProbePoints[i] = (bedRadius / 2 * Math.Cos((2 * Math.PI * (i - 6)) / 3)).ToString("N2");
                        zBedProbePoints[i] = "0.0"; // we default this to zero -gwb
                    }
                    xBedProbePoints[9] = "0.0";
                    yBedProbePoints[9] = "0.0";
                    zBedProbePoints[9] = "0.0";
                } else {
                    xBedProbePoints[6] = "0.0";
                    yBedProbePoints[6] = "0.0";
                    zBedProbePoints[6] = "0.0";
                }
            }
        }

        void setParameters() {

            stepsPerMM = 80;
            switch (this.machineType) {
                case SMC_ORION:
                    bedRadius = 90;
                    break;

                case SMC_MAX_V2:
                    bedRadius = 120;
                    break;

                case SMC_ERIS:
                    bedRadius = 60;
                    break;

                case SMC_MAX_V3:
                    bedRadius = 120;
                    break;

                case SMC_H2:
                    bedRadius = 90;
                    break;
            }

        }

        /*************************************************************************/
    }
}
