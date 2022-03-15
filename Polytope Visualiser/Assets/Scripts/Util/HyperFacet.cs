using System.Collections.Generic;

namespace Util
{
    public struct HyperFacet
    {
        public List<List<VectorD4D>> subFacets;
        public List<VectorD4D> vertices;

        private HyperPlane hyperPlane;

        public static bool operator ==(HyperFacet a, HyperFacet b)
        {
            foreach (VectorD4D v1 in a.vertices)
            {
                bool found = false;
                foreach (VectorD4D v2 in b.vertices)
                {
                    if (v1 == v2) found = true;
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(HyperFacet a, HyperFacet b)
        {
            return !(a == b);
        }

        public HyperFacet(VectorD4D p0, VectorD4D p1, VectorD4D p2, VectorD4D p3)
        {
            subFacets = new List<List<VectorD4D>>();
            subFacets.Add(new List<VectorD4D>() {p0, p1, p3});
            subFacets.Add(new List<VectorD4D>() {p0, p2, p3});
            subFacets.Add(new List<VectorD4D>() {p1, p2, p3});
            subFacets.Add(new List<VectorD4D>() {p0, p1, p2});

            vertices = new List<VectorD4D>() {p0, p1, p2, p3};
            
            VectorD4D v1 = p1 - p0;
            VectorD4D v2 = p2 - p1;
            VectorD4D v3 = p3 - p2;
            VectorD4D normal = VectorD4D.Cross(v1,v2,v3);

            hyperPlane = new HyperPlane(normal, p0);
        }

        public bool IsVisible(VectorD4D eyePoint)
        {
            return hyperPlane.GetDistance(eyePoint) > VectorD4D.GetEpsilon();
        }

        public void CorrectNormal(List<VectorD4D> internalPoints)
        {
            foreach (VectorD4D point in internalPoints)
            {
                if (hyperPlane.GetDistance(point) > VectorD4D.GetEpsilon())
                {
                    hyperPlane.InvertNormal();
                    return;
                }
            }
        }
    }
}