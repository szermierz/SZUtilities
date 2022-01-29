using UnityEngine.UI;

namespace SZUtilities.UI
{
	public class NonDrawingGraphic : Graphic
	{
		public override void SetMaterialDirty()
		{ }

		public override void SetVerticesDirty()
		{ }

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();
			return;
		}
	}
}