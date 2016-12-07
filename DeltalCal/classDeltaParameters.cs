using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltalCal {
    class classDeltaParameters {

        const double degreesToRadians = Math.PI / 180.0;

        double diagonal;
        double radius;
        double homedHeight;
        double xStop;
        double yStop;
        double zStop;
        double xAdj;
        double yAdj;
        double zAdj;
        List<double> towerX;
        List<double> towerY;

        double Xbc;
        double Xca;
        double Xab;
        double Ybc;
        double Yca;
        double Yab;

        double coreFa;
        double coreFb;
        double coreFc;
        double Q;
        double Q2;
        double D2;

        double tempHeight;
        double homedCarriageHeight;
        String firmware;

        public classDeltaParameters(String firmware, double diagonal, double radius, double height, double xStop, double yStop,
            double zStop, double xAdj, double yAdj, double zAdj) {

            if (firmware.Equals("")) {
                throw new Exception("Firmware type not passed to classDeltaParameters constructor!");
            }
            this.firmware = firmware;
            this.diagonal = diagonal;
            this.radius = radius;
            this.homedHeight = height;
            this.xStop = xStop;
            this.yStop = yStop;
            this.zStop = zStop;
            this.xAdj = xAdj;
            this.yAdj = yAdj;
            this.zAdj = zAdj;
            this.Recalc();
        }
        double fsquare(double x) {
            return x * x;
        }


        double Transform(double[] machinePos, int axis) {
            return machinePos[2] + Math.Sqrt(this.D2 - fsquare(machinePos[0] - this.towerX[axis]) - fsquare(machinePos[1] - this.towerY[axis]));
        }

        double InverseTransform(double Ha, double Hb, double Hc) {

            double Fa = this.coreFa + fsquare(Ha);
            double Fb = this.coreFb + fsquare(Hb);
            double Fc = this.coreFc + fsquare(Hc);

            // Setup PQRSU such that x = -(S - uz)/P, y = (P - Rz)/Q
            double P = (this.Xbc * Fa) + (this.Xca * Fb) + (this.Xab * Fc);
            double S = (this.Ybc * Fa) + (this.Yca * Fb) + (this.Yab * Fc);

            double R = 2 * ((this.Xbc * Ha) + (this.Xca * Hb) + (this.Xab * Hc));
            double U = 2 * ((this.Ybc * Ha) + (this.Yca * Hb) + (this.Yab * Hc));

            double R2 = fsquare(R), U2 = fsquare(U);

            double A = U2 + R2 + this.Q2;
            double minusHalfB = S * U + P * R + Ha * this.Q2 + this.towerX[0] * U * this.Q - this.towerY[0] * R * this.Q;
            double C = fsquare(S + this.towerX[0] * this.Q) + fsquare(P - this.towerY[0] * this.Q) + (fsquare(Ha) - this.D2) * this.Q2;

            double rslt = (minusHalfB - Math.Sqrt(fsquare(minusHalfB) - A * C)) / A;
            if (double.IsNaN(rslt)) {
                throw new Exception("At least one probe point is not reachable. Please correct your delta radius, diagonal rod length, or probe coordinates.");
            }
            return rslt;
        }

        void Recalc() {
            this.towerX = new List<double>();
            this.towerY = new List<double>();
            this.towerX.Add(-(this.radius * Math.Cos((30 + this.xAdj) * degreesToRadians)));
            this.towerY.Add(-(this.radius * Math.Sin((30 + this.xAdj) * degreesToRadians)));
            this.towerX.Add(+(this.radius * Math.Cos((30 - this.yAdj) * degreesToRadians)));
            this.towerY.Add(-(this.radius * Math.Sin((30 - this.yAdj) * degreesToRadians)));
            this.towerX.Add(-(this.radius * Math.Sin(this.zAdj * degreesToRadians)));
            this.towerY.Add(+(this.radius * Math.Cos(this.zAdj * degreesToRadians)));

            this.Xbc = this.towerX[2] - this.towerX[1];
            this.Xca = this.towerX[0] - this.towerX[2];
            this.Xab = this.towerX[1] - this.towerX[0];
            this.Ybc = this.towerY[2] - this.towerY[1];
            this.Yca = this.towerY[0] - this.towerY[2];
            this.Yab = this.towerY[1] - this.towerY[0];
            this.coreFa = fsquare(this.towerX[0]) + fsquare(this.towerY[0]);
            this.coreFb = fsquare(this.towerX[1]) + fsquare(this.towerY[1]);
            this.coreFc = fsquare(this.towerX[2]) + fsquare(this.towerY[2]);
            this.Q = 2 * (this.Xca * this.Yab - this.Xab * this.Yca);
            this.Q2 = fsquare(this.Q);
            this.D2 = fsquare(this.diagonal);

            // Calculate the base carriage height when the printer is homed.
            double tempHeight = this.diagonal;		// any sensible height will do here, probably even zero
            this.homedCarriageHeight = this.homedHeight + tempHeight - this.InverseTransform(tempHeight, tempHeight, tempHeight);
        }

        double ComputeDerivative(int deriv, double ha, double hb, double hc) {
            var perturb = 0.2;			// perturbation amount in mm or degrees
            var hiParams = new classDeltaParameters(this.firmware, this.diagonal, this.radius, this.homedHeight, this.xStop, this.yStop, this.zStop, this.xAdj, this.yAdj, this.zAdj);
            var loParams = new classDeltaParameters(this.firmware, this.diagonal, this.radius, this.homedHeight, this.xStop, this.yStop, this.zStop, this.xAdj, this.yAdj, this.zAdj);
            switch (deriv) {
                case 0:
                case 1:
                case 2:
                    break;

                case 3:
                    hiParams.radius += perturb;
                    loParams.radius -= perturb;
                    break;

                case 4:
                    hiParams.xAdj += perturb;
                    loParams.xAdj -= perturb;
                    break;

                case 5:
                    hiParams.yAdj += perturb;
                    loParams.yAdj -= perturb;
                    break;

                case 6:
                    hiParams.diagonal += perturb;
                    loParams.diagonal -= perturb;
                    break;
            }

            hiParams.Recalc();
            loParams.Recalc();

            var zHi = hiParams.InverseTransform((deriv == 0) ? ha + perturb : ha, (deriv == 1) ? hb + perturb : hb, (deriv == 2) ? hc + perturb : hc);
            var zLo = loParams.InverseTransform((deriv == 0) ? ha - perturb : ha, (deriv == 1) ? hb - perturb : hb, (deriv == 2) ? hc - perturb : hc);

            return (zHi - zLo) / (2 * perturb);

        }

        void NormaliseEndstopAdjustments(String firmware) {
            var eav = (firmware.Equals("Marlin") || firmware.Equals("MarlinRC") || firmware.Equals("Repetier")) ? Math.Min(this.xStop, Math.Min(this.yStop, this.zStop))
             : (this.xStop + this.yStop + this.zStop) / 3.0;
            this.xStop -= eav;
            this.yStop -= eav;
            this.zStop -= eav;
            this.homedHeight += eav;
            this.homedCarriageHeight += eav;				// no need for a full recalc, this is sufficient
        }

        // Perform 3, 4, 6 or 7-factor adjustment.
        // The input vector contains the following parameters in this order:
        //  X, Y and Z endstop adjustments
        //  If we are doing 4-factor adjustment, the next argument is the delta radius. Otherwise:
        //  X tower X position adjustment
        //  Y tower X position adjustment
        //  Z tower Y position adjustment
        //  Diagonal rod length adjustment
        void Adjust(int numFactors, List<double> v, bool norm) {
            var oldCarriageHeightA = this.homedCarriageHeight + this.xStop;	// save for later

            // Update endstop adjustments
            this.xStop += v[0];
            this.yStop += v[1];
            this.zStop += v[2];
            if (norm) {
                this.NormaliseEndstopAdjustments(this.firmware);
            }

            if (numFactors >= 4) {
                this.radius += v[3];

                if (numFactors >= 6) {
                    this.xAdj += v[4];
                    this.yAdj += v[5];

                    if (numFactors == 7) {
                        this.diagonal += v[6];
                    }
                }

                this.Recalc();
            }

            // Adjusting the diagonal and the tower positions affects the homed carriage height.
            // We need to adjust homedHeight to allow for this, to get the change that was requested in the endstop corrections.
            var heightError = this.homedCarriageHeight + this.xStop - oldCarriageHeightA - v[0];
            this.homedHeight -= heightError;
            this.homedCarriageHeight -= heightError;
        }

    }
}
