using Microsoft.VisualBasic.ApplicationServices;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
   

    public struct Fraction
    {
        private int numerator;
        private int denominator;

        public Fraction(int numerator = 0, int denominator = 1)
        {
            this.numerator = numerator;
            this.denominator = denominator;
            Simplify();
        }
        
        public int Numerator
        {
            get {return numerator; }
            set {numerator = value; Simplify(); }
        }
        public int Denominator
        {
            get {return denominator; }
            set { denominator = value; Simplify(); }
        }
        
        public override string ToString()
        {
            return numerator + "/" + denominator;
        }

        public void Simplify()
        {
            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
        }
        public static int GCD(int n, int m)
        {
            return m == 0 ? n : GCD(m, n % m);
        }
        
        public static Fraction Add(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator * rhs.denominator) + (lhs.denominator * rhs.numerator), rhs.denominator * lhs.denominator);
        }
        public static Fraction operator +(Fraction lhs, Fraction rhs)
        {
            return Add(lhs, rhs);
        }
        public static Fraction Multiply(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.numerator, lhs.denominator * rhs.denominator);
        }
        public static Fraction operator *(Fraction lhs, Fraction rhs)
        {
            return Multiply(lhs, rhs);
        }
        public static Fraction Subtract(Fraction lhs, Fraction rhs)
        {
            return new Fraction((lhs.numerator * rhs.denominator) - (lhs.denominator * rhs.numerator), rhs.denominator * lhs.denominator);
        }
        public static Fraction operator -(Fraction lhs, Fraction rhs)
        {
            return Subtract(lhs, rhs);
        }
        public static Fraction Divide(Fraction lhs, Fraction rhs)
        {
            return new Fraction(lhs.numerator * rhs.denominator, lhs.denominator * rhs.numerator);
        }
        public static Fraction operator /(Fraction lhs, Fraction rhs)
        {
            return Divide(lhs, rhs);
        }

    }
}
