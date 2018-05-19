using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment12
{
    class MathServer : IMathService
    {
        public double Add(double firstValue, double secondValue)
        {
            return firstValue + secondValue;
        }

        public double Div(double firstValue, double secondValue)
        {
            if(secondValue != 0)
                return firstValue / secondValue;
            throw new ArgumentException("Can not divide on 0");
        }

        public double Mult(double firstValue, double secondValue)
        {
            return firstValue*secondValue;
        }

        public double Sub(double firstValue, double secondValue)
        {
            return firstValue-secondValue;
        }
    }
}
