using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Polytope2D.Util.Other;
using UnityEngine;
using UnityEngine.TestTools;
using Util;

public class InequalityIntersectionTests
{
    [Test]
    public void Inequality2DIntersectionTest()
    {
        Inequality inequality1 = new Inequality(6, -1, 18);
        Inequality inequality2 = new Inequality(16, -1, 20);

        VectorD2D intersection = inequality1.GetIntersection(inequality2);
        Assert.AreEqual(new VectorD2D(-0.2, 16.8), intersection);
    }
    
    [Test]
    public void Inequality3DIntersectionTest()
    {
        PlaneInequality inequality1 = new PlaneInequality(2, 1, 6, -7);
        PlaneInequality inequality2 = new PlaneInequality(3, 4, 3, 8);
        PlaneInequality inequality3 = new PlaneInequality(1, -2, -4, -9);
        VectorD3D? intersection = PlaneInequality.GetIntersection(inequality1, inequality2, inequality3);
        Assert.AreEqual(new VectorD3D(3, -5, 1), intersection);
    }
}
