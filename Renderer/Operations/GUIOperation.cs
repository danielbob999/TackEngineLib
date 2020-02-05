using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using TackEngineLib.GUI;
using TackEngineLib.Main;

namespace TackEngineLib.Renderer.Operations {
	public class GUIOperation {
		public int DrawLevel { get; set; }			// The order that sets the order that all operations are drawn.
													//	- Drawn from smallest value to largest (Operation with a larger draw level value be be drawn over the top of a smaller draw level value operation)
		public int OperationType { get; }           // The operation type that this object should be drawn as
													//	- 0 if this object should be treated as a default box
													//	- 1 if this object should be treated as a text area

		public bool IsSpriteDrawMode { get; set; }  // Sets the draw mode for the background. 
													//	- (true) if background should be drawn as an image
													//	- (false) if background should be drawn as a colour
		public RectangleShape Bounds { get; set; }
		public GUIBorder Border { get; set; }
		public Sprite Sprite { get; set; }
		public Colour4b Colour { get; set; }
		public string Text { get; set; }
		public Font Font { get; set; }
		public Colour4b TextColour { get; set; }
		public HorizontalAlignment TextHAlignment { get; set; }
		public VerticalAlignment TextVAlignment { get; set; }
		public float FontSize { get; set; }

		public GUIOperation(int opType) {
			OperationType = opType;
			DrawLevel = 0;
		}
	}
}
