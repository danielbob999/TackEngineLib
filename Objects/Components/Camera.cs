﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Engine;
using TackEngineLib.Main;

namespace TackEngineLib.Objects.Components
{
    /// <summary>
    /// The component that draws the world view to the screen
    /// </summary>
    public class Camera : TackComponent
    {
        private int mCameraScreenWidth;
        private int mCameraScreenHeight;
        private Colour4b mColourOverlay;
        private float mZoomFactor = 1.0f;

        // Properties
        public int CameraScreenWidth
        {
            get { return mCameraScreenWidth; }
            set
            {
                if (value > 0) {
                    int oldValue = mCameraScreenWidth;
                    mCameraScreenWidth = value;
                } else {
                    TackConsole.EngineLog(EngineLogType.Error, "Camera Screen Width cannot be set to less than 1");
                }
            }
        }

        public int CameraScreenHeight
        {
            get
            {
                return mCameraScreenHeight;
            }
            set
            {
                if (value > 0) {
                    int oldValue = mCameraScreenHeight;
                    mCameraScreenHeight = value;
                } else {
                    TackConsole.EngineLog(EngineLogType.Error, "Camera Screen Height cannot be set to less than 1");
                }
            }
        }

        public Colour4b ColourOverlay
        {
            get { return mColourOverlay; }
            set { mColourOverlay = value; }
        }

        public float ZoomFactor { get { return mZoomFactor; } set { mZoomFactor = value; } }

        public Camera()
        {
            CameraScreenWidth = TackEngine.ScreenWidth;
            CameraScreenHeight = TackEngine.ScreenHeight;
        }

        public void UpdateCameraDimensions(int _w, int _h)
        {
            CameraScreenWidth = _w;
            CameraScreenHeight = _h;
        }
    }
}
