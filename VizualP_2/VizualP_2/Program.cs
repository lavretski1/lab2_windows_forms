using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VP_2
{
    public struct Point 
    {
        public float X;
        public float Y;
    }
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            Application.Run(form);
        }

        public static List<Point> FunctionPoints(int quantityOfPoints, float parameterStart, float parameterEnd, Func<float, float> funcX, Func<float,float> funcY) 
        {
            float step = (parameterEnd - parameterStart) / quantityOfPoints;
            
            var result = new List<Point>();

            for (float parameter = parameterStart; parameter < parameterEnd; parameter += step) 
            {
                result.Add(new Point { X = funcX.Invoke(parameter), Y = funcY.Invoke(parameter) });
            }

            return result;
        }

        public static float FuncOneY(float argument) 
        {
            return (float)Math.Sqrt(1 - Math.Exp(-argument * argument));
        }

        public static float FuncOneX(float argument)
        {
            return argument;
        }

        public static float FuncTwoY(float argument) 
        {
            return 1f / (4f * argument * argument + 1);
        }

        public static float FuncTwoX(float argument)
        {
            return 1f / (argument * argument);
        }
    }
}
