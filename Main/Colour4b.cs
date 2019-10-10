/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    /// <summary>
    /// An object representing an RGBA colour
    /// </summary>
    public struct Colour4b
    {
        private byte mR;
        private byte mG;
        private byte mB;
        private byte mA;

        /// <summary>
        /// The red component of this colour
        /// </summary>
        public byte R
        {
            get { return mR; }
            set { mR = value; }
        }

        /// <summary>
        /// The green component of this colour
        /// </summary>
        public byte G
        {
            get { return mG; }
            set { mG = value; }
        }

        /// <summary>
        /// The blue component of this colour
        /// </summary>
        public byte B
        {
            get { return mB; }
            set { mB = value; }
        }

        /// <summary>
        /// The transparent component of this colour
        /// </summary>
        public byte A
        {
            get { return mA; }
            set { mA = value; }
        }

        public static Colour4b Black
        {
            get { return new Colour4b(0, 0, 0, 255); }
        }

        public static Colour4b White
        {
            get { return new Colour4b(255, 255, 255, 255); }
        }

        public static Colour4b Red
        {
            get { return new Colour4b(255, 0, 0, 255); }
        }

        public static Colour4b Green
        {
            get { return new Colour4b(0, 255, 0, 255); }
        }

        public static Colour4b Blue
        {
            get { return new Colour4b(0, 0, 255, 255); }
        }

        /// <summary>
        /// Initialises a new Colour4b object
        /// </summary>
        /// <param name="aR">The red component</param>
        /// <param name="aG">The green component</param>
        /// <param name="aB">The blue component</param>
        public Colour4b(byte aR = 0, byte aG = 0, byte aB = 0)
        {
            mR = aR;
            mG = aG;
            mB = aB;
            mA = 255;
        }

        /// <summary>
        /// Initialises a new Colour4b object
        /// </summary>
        /// <param name="aR">The red component</param>
        /// <param name="aG">The green component</param>
        /// <param name="aB">The blue component</param>
        /// <param name="aA">The transparent component</param>
        public Colour4b(byte aR, byte aG, byte aB, byte aA)
        {
            mR = aR;
            mG = aG;
            mB = aB;
            mA = aA;
        }

        /// <summary>
        /// Returns a Colour4b object as a string. Format: (R, G, B, A)
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "(" + mR + ", " + mG + ", " + mB + ", " + mA + ")";
        }
    }
}
