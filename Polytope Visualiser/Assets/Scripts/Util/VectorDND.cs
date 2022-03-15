using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public struct VectorDND
    {
        private const double Epsilon = 1e-10;

        public double[] x;

        public static double GetEpsilon()
        {
            return Epsilon;
        }

        public override string ToString()
        {
            string vectorString = "";
            foreach (double n in x)
            {
                vectorString += n + "; ";
            }

            return vectorString;
        }

        public static VectorDND operator +(VectorDND a, VectorDND b)
        {
            List<double> newVector = new List<double>();
            for (int i = 0; i < a.Count(); i++)
            {
                newVector.Add(a[i] + b[i]);
            }

            return new VectorDND(newVector);
        }
        
        public static VectorDND operator -(VectorDND a, VectorDND b)
        {
            List<double> newVector = new List<double>();
            for (int i = 0; i < a.Count(); i++)
            {
                newVector.Add(a[i] - b[i]);
            }

            return new VectorDND(newVector);
        }
        
        public static VectorDND operator *(VectorDND a, VectorDND b)
        {
            List<double> newVector = new List<double>();
            for (int i = 0; i < a.Count(); i++)
            {
                newVector.Add(a[i] * b[i]);
            }

            return new VectorDND(newVector);
        }
        
        public static VectorDND operator /(VectorDND a, VectorDND b)
        {
            List<double> newVector = new List<double>();
            for (int i = 0; i < a.Count(); i++)
            {
                newVector.Add(a[i] / b[i]);
            }

            return new VectorDND(newVector);
        }

        public static bool operator ==(VectorDND a, VectorDND b)
        {
            for (int i = 0; i < a.Count(); i++)
            {
                if (Math.Abs(a[i] - b[i]) > Epsilon) return false;
            }

            return true;
        }

        public static bool operator !=(VectorDND a, VectorDND b)
        {
            return !(a == b);
        }

        public static double Distance(VectorDND a, VectorDND b)
        {
            return (a - b).Magnitude();
        }

        public static double Dot(VectorDND a, VectorDND b)
        {
            double sum = 0;
            for (int i = 0; i < a.Count(); i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        public static VectorDND Cross(List<VectorDND> vectors)
        {
            double[,] matrix = new double[vectors.Count, vectors[0].Count()];
            for (int i = 0; i < vectors.Count; i++)
            {
                double[] row = vectors[i].GetCopy();
                for (int j = 0; j < vectors[i].Count(); j++)
                {
                    matrix[i, j] = row[j];
                }
            }

            matrix = UtilLib.ToReducedRowEchelon(matrix);

            List<double> newVector = new List<double>();

            for (int i = 0; i < vectors.Count; i++)
            {
                newVector.Add(-matrix[i,matrix.GetLength(1) - 1]);
            }
            newVector.Add(1);

            return new VectorDND(newVector);
        }

        public static bool AreNotCoplanar(List<VectorDND> vectors)
        {
            double[,] matrix = new double[vectors.Count, vectors[0].Count()];
            for (int i = 0; i < vectors.Count; i++)
            {
                double[] row = vectors[i].GetCopy();
                for (int j = 0; j < vectors.Count; j++)
                {
                    matrix[i, j] = row[j];
                }
            }

            for (int i = 0; i < vectors.Count; i++)
            {
                if (matrix[i, i] == 0) return false;
            }

            return true;
        }

        public static VectorDND Normalise(VectorDND a)
        {
            double magnitude = a.Magnitude();
            for (int i = 0; i < a.Count(); i++)
            {
                a[i] = a[i] / magnitude;
            }

            return a;
        }

        public VectorDND(List<double> _x)
        {
            x = new double[_x.Count];

            for (int i = 0; i < _x.Count; i++)
            {
                x[i] = _x[i];
            }
        }

        public double this[int i]
        {
            get
            {
                return x[i];
            }

            set
            {
                x[i] = value;
            }
        }

        public double Magnitude()
        {
            double sum = 0;
            foreach (double n in x)
            {
                sum += n * n;
            }

            sum /= x.Length;

            return Math.Sqrt(sum);
        }

        public double SquareMagnitude()
        {
            double sum = 0;
            foreach (double n in x)
            {
                sum += n * n;
            }

            return sum;
        }

        public int Count()
        {
            return x.Length;
        }

        public double[] GetCopy()
        {
            double[] toReturn = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                toReturn[i] = x[i];
            }

            return toReturn;
        }
    }
}