using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Engine;
using TackEngineLib.Main;

namespace TackEngineLib.Objects.Components
{
    public class Camera : TackComponent
    {
        private int m_CameraScreenWidth;
        private int m_CameraScreenHeight;
        private Colour4b m_ColourOverlay;
        private float m_ZoomFactor = 1.0f;


        // Properties
        public int CameraScreenWidth
        {
            get
            {
                return m_CameraScreenWidth;
            }
            set
            {
                if (value > 0)
                {
                    int oldValue = m_CameraScreenWidth;
                    m_CameraScreenWidth = value;
                    TackConsole.EngineLog(EngineLogType.Message, string.Format("Updated Camera.CameraScreenWidth. Old ({0}) -> New ({1})", oldValue, m_CameraScreenWidth));
                }
                else
                    TackConsole.EngineLog(EngineLogType.Error, "Camera Screen Width cannot be set to less than 1");
            }
        }

        public int CameraScreenHeight
        {
            get
            {
                return m_CameraScreenHeight;
            }
            set
            {
                if (value > 0)
                {
                    int oldValue = m_CameraScreenHeight;
                    m_CameraScreenHeight = value;
                    TackConsole.EngineLog(EngineLogType.Message, string.Format("Updated Camera.CameraScreenHeight. Old ({0}) -> New ({1})", oldValue, m_CameraScreenHeight));
                }
                else
                    TackConsole.EngineLog(EngineLogType.Error, "Camera Screen Height cannot be set to less than 1");
            }
        }

        public Colour4b ColourOverlay
        {
            get { return m_ColourOverlay; }
            set { m_ColourOverlay = value; }
        }

        public float ZoomFactor { get { return m_ZoomFactor; } set { m_ZoomFactor = value; } }

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
