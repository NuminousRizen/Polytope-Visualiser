namespace Util
{
    public struct HyperPlane
    {
        public VectorD4D normal;
        public VectorD4D p0;
        
        public HyperPlane(VectorD4D _normal, VectorD4D _p0)
        {
            normal = VectorD4D.Normalise(_normal);
            p0 = _p0;
        }

        public double GetDistance(VectorD4D p)
        {
            return VectorD4D.Dot(normal, p - p0);
        }

        public void InvertNormal()
        {
            normal *= new VectorD4D(-1, -1, -1, -1);
        }
    }
}