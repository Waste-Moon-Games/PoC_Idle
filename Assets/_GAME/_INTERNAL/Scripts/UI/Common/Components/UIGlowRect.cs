using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Common.Components
{
    [RequireComponent(typeof(Graphic))]
    public class UIGlowRect : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) 
                return;

            Rect r = graphic.rectTransform.rect;

            UIVertex v = UIVertex.simpleVert;
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                v.uv1 = new Vector4(r.width, r.height, 0f, 0f);
                vh.SetUIVertex(v, i);
            }
        }
    }
}