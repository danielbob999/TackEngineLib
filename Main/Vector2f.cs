/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Main
{
    /// <summary>
    /// 
    /// </summary>
    public struct Vector2f {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2f(float _x, float _y) {
            X = _x;
            Y = _y;
        }

        public static Vector2f operator+ (Vector2f _a, Vector2f _b) {
            return new Vector2f(_a.X + _b.X, _a.Y + _b.Y);
        }

        public static Vector2f operator- (Vector2f _a, Vector2f _b) {
            return new Vector2f(_b.X - _a.X, _b.Y - _a.Y);
        }

        public static Vector2f operator- (Vector2f _a, float _b) {
            return new Vector2f(_a.X - _b, _a.Y - _b);
        }

        public static Vector2f operator* (Vector2f _a, Vector2f _b) {
            return new Vector2f(_a.X * _b.X, _a.Y * _b.Y);
        }

        public static Vector2f operator* (Vector2f _a, float _b) {
            return new Vector2f(_a.X * _b, _a.Y * _b);
        }

        public static Vector2f operator/ (Vector2f _a, float _b) {
            return new Vector2f(_a.X / _b, _a.Y / _b);
        }

        public static bool operator== (Vector2f _a, Vector2f _b) {
            if (_a.X == _b.X) {
                if (_a.Y == _b.Y) {
                    return true;
                }
            }

            return false;
        }

        public static bool operator!= (Vector2f _a, Vector2f _b) {
            if (_a.X == _b.X) {
                if (_a.Y == _b.Y) {
                    return false;
                }
            }

            return true;
        }

        public static Vector2f operator- (Vector2f _a) {
            return new Vector2f(_a.X * -1.0f, _a.Y * -1.0f);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            string returnStr = "(" + X + ", " + Y + ")";
            return returnStr;
        }

        public static float Distance(Vector2f a, Vector2f b) {
            float xDiff = (float)System.Math.Pow((b.X - a.X), 2);
            float yDiff = (float)System.Math.Pow((b.Y - a.Y), 2);

            return (float)System.Math.Sqrt(xDiff + yDiff);
        }

        public static Vector2f Normalise(Vector2f vec) {
            float distance = (float)System.Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
            return new Vector2f(vec.X / distance, vec.Y / distance);
        }
    }
}
