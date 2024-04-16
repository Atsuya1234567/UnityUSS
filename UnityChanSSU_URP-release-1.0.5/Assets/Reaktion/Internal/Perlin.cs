//
// Reaktion - An audio reactive animation toolkit for Unity.
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if UNITY_STANDALONE_OSX && UNITY_PRO_LICENSE
#define ENABLE_NATIVE_PLUGIN
#endif

using UnityEngine;
using System.Runtime.InteropServices;

namespace Reaktion {

public static class Perlin
{
#if !ENABLE_NATIVE_PLUGIN

    //
    // Based on the original implementation by Ken Perlin
    // http://mrl.nyu.edu/~perlin/noise/
    //

    #region Noise functions

    public static float Noise (float x)
    {
        var X = Mathf.FloorToInt (x) & 0xff;
        x -= Mathf.Floor (x);
        return Lerp (Fade (x), Grad(X, x), Grad (X + 1, x - 1));
    }

    public static float Noise (float x, float y)
    {
        var X = Mathf.FloorToInt (x) & 0xff;
        var Y = Mathf.FloorToInt (y) & 0xff;
        x -= Mathf.Floor (x);
        y -= Mathf.Floor (y);
        var u = Fade (x);
        var v = Fade (y);
        var A = (perm [X    ] + Y) & 0xff;
        var B = (perm [X + 1] + Y) & 0xff;
        return Lerp (v, Lerp (u, Grad (A    , x, y    ), Grad (B,     x - 1, y    )),
                        Lerp (u, Grad (A + 1, x, y - 1), Grad (B + 1, x - 1, y - 1)));
    }

    public static float Noise (Vector2 coord)
    {
        return Noise (coord.x, coord.y);
    }

    public static float Noise (float x, float y, float z)
    {
        var X = Mathf.FloorToInt (x) & 0xff;
        var Y = Mathf.FloorToInt (y) & 0xff;
        var Z = Mathf.FloorToInt (z) & 0xff;
        x -= Mathf.Floor (x);
        y -= Mathf.Floor (y);
        z -= Mathf.Floor (z);
        var u = Fade (x);
        var v = Fade (y);
        var w = Fade (z);
        var A  = (perm [X    ] + Y) & 0xff;
        var B  = (perm [X + 1] + Y) & 0xff;
        var AA = (perm [A    ] + Z) & 0xff;
        var BA = (perm [B    ] + Z) & 0xff;
        var AB = (perm [A + 1] + Z) & 0xff;
        var BB = (perm [B + 1] + Z) & 0xff;
        return Lerp (w, Lerp (v, Lerp (u, Grad (AA    , x    , y    , z    ), Grad (BA    , x - 1, y    , z    )),
                                 Lerp (u, Grad (AB    , x    , y - 1, z    ), Grad (BB    , x - 1, y - 1, z    ))),
                        Lerp (v, Lerp (u, Grad (AA + 1, x    , y    , z - 1), Grad (BA + 1, x - 1, y    , z - 1)),
                                 Lerp (u, Grad (AB + 1, x    , y - 1, z - 1), Grad (BB + 1, x - 1, y - 1, z - 1))));
    }

    public static float Noise (Vector3 coord)
    {
        return Noise (coord.x, coord.y, coord.z);
    }

    #endregion

    #region fBm functions

    public static float Fbm (float x, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise (x);
            x *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }
    
    public static float Fbm (Vector2 coord, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise (coord);
            coord *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }
    
    public static float Fbm (Vector3 coord, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise (coord);
            coord *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }

    #endregion

    #region Private functions

    static float Fade (float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    
    static float Lerp (float t, float a, float b)
    {
        return a + t * (b - a);
    }
    
    static float Grad (int i, float x)
    {
        return (perm [i] & 1) != 0 ? x : -x;
    }

    static float Grad (int i, float x, float y)
    {
        var h = perm [i];
        return ((h & 1) != 0 ? x : -x) + ((h & 2) != 0 ? y : -y);
    }
    
    static float Grad (int i, float x, float y, float z)
    {
        var h = perm [i] & 15;
        var u = h < 8 ? x : y;
        var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) != 0 ? u : -u) + ((h & 2) != 0 ? v : -v);
    }

    static int[] perm = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151
    };

    #endregion

#else // ENABLE_NATIVE_PLUGIN

    //
    // The native implementation.
    //

    #region Noise functions
    
    public static float Noise (float x)
    {
        return KvantNoise1D (x);
    }
    
    public static float Noise (float x, float y)
    {
        return KvantNoise2D (x, y);
    }
    
    public static float Noise (Vector2 coord)
    {
        return KvantNoise2D (coord.x, coord.y);
    }
    
    public static float Noise (float x, float y, float z)
    {
        return KvantNoise3D (x, y, z);
    }
    
    public static float Noise (Vector3 coord)
    {
        return KvantNoise3D (coord.x, coord.y, coord.z);
    }
    
    #endregion
    
    #region Fractal functions
    
    public static float Fbm (float x, int octave)
    {
        return KvantFBM1D (x, octave);
    }
    
    public static float Fbm (Vector2 coord, int octave)
    {
        return KvantFBM2D (coord.x, coord.y, octave);
    }
    
    public static float Fbm (Vector3 coord, int octave)
    {
        return KvantFBM3D (coord.x, coord.y, coord.z, octave);
    }
    
    #endregion

    #region Native plug-in interface
    
    [DllImport ("Kvant")]
    private static extern float KvantNoise1D (float x);
    
    [DllImport ("Kvant")]
    private static extern float KvantNoise2D (float x, float y);
    
    [DllImport ("Kvant")]
    private static extern float KvantNoise3D (float x, float y, float z);
    
    [DllImport ("Kvant")]
    private static extern float KvantFBM1D (float x, int octave);
    
    [DllImport ("Kvant")]
    private static extern float KvantFBM2D (float x, float y, int octave);
    
    [DllImport ("Kvant")]
    private static extern float KvantFBM3D (float x, float y, float z, int octave);
    
    #endregion

#endif // ENABLE_NATIVE_PLUGIN
}

} // namespace Reaktion
