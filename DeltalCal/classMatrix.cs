using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltalCal {
    class classMatrix {
        double[,] workingData;  // ugh.
              

        public classMatrix(int rows, int cols) {
            workingData = new double[rows, cols];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    workingData[i, j] = 0.0;
                }
            }
        }

        void SwapRows(int i, int j, int numCols) {
            double temp = 0;
            if (i != j) {
                for (int k = 0; k < numCols; k++) {
                    temp = workingData[i, k];
                    workingData[i, k] = workingData[j, k];
                    workingData[j, k] = temp;
                }
            }
        }

        List<double> GaussJordan(int numRows) {
            double vmax = 0.0;
            double rmax = 0.0;
            double factor = 0.0;
            List<double> solution = new List<double>();

            for (int i = 0; i < numRows; i++) {
                // swap the rows around for stable Gauss-Jordan elimination.
                vmax = Math.Abs(workingData[i, i]);
                for (int j = 0; j < numRows; j++) {
                    rmax = Math.Abs(workingData[j, i]);
                    if (rmax > vmax) {
                        SwapRows(i, j, numRows + 1);
                        vmax = rmax;
                    }
                }
                // Use row i to eliminate the ith element from previous and subsequent rows
                double v = workingData[i, i];
                for (int j = 0; j < i; ++j) {
                    factor = workingData[j, i] / v;
                    workingData[j, i] = 0.0;
                    for (int k = i + 1; k <= numRows; ++k) {
                        workingData[j, k] -= workingData[i, k] * factor;
                    }
                }
                for (var j = i + 1; j < numRows; ++j) {
                    factor = workingData[j, i] / v;
                    workingData[j, i] = 0.0;
                    for (int k = i + 1; k <= numRows; ++k) {
                        workingData[j, k] -= workingData[i, k] * factor;
                    }
                }

            }
            for (int i = 0; i < numRows; ++i) {
                solution.Add(workingData[i, numRows] / workingData[i, i]);
            }
            return solution;
        }

    }
}
