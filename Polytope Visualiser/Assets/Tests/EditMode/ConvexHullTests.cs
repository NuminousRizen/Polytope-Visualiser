using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Polytope2D.Util.Convex_Hull;
using Polytope3D.Util.Convex_Hull;
using UnityEngine;
using UnityEngine.TestTools;
using Util;

public class ConvexHullTests
{
    [Test]
    public void GrahamSquareTest()
    {
        List<VectorD2D> points = new List<VectorD2D>()
        {
            new VectorD2D(0,0),
            new VectorD2D(0,1),
            new VectorD2D(1,0),
            new VectorD2D(1,1),
            new VectorD2D(0.5,0.5)
        };
        
        List<VectorD2D> testPoints = new List<VectorD2D>()
        {
            new VectorD2D(0,0),
            new VectorD2D(0,1),
            new VectorD2D(1,0),
            new VectorD2D(1,1)
        };

        List<VectorD2D> hullPoints = GrahamScan.GetConvexHull(points);
        
        bool equals = testPoints.Count() == hullPoints.Count();

        foreach (VectorD2D point in testPoints)
        {
            if (!hullPoints.Contains(point)) equals = false;
        }

        Assert.AreEqual(true, equals);
    }
    
    [Test]
    public void GrahamTriangleTest()
    {
        List<VectorD2D> points = new List<VectorD2D>()
        {
            new VectorD2D(0,0),
            new VectorD2D(0,1),
            new VectorD2D(1,0),
            new VectorD2D(0.5,0.5)
        };
        
        List<VectorD2D> testPoints = new List<VectorD2D>()
        {
            new VectorD2D(0,0),
            new VectorD2D(0,1),
            new VectorD2D(1,0)
        };

        List<VectorD2D> hullPoints = GrahamScan.GetConvexHull(points);
        
        bool equals = testPoints.Count() == hullPoints.Count();

        foreach (VectorD2D point in testPoints)
        {
            if (!hullPoints.Contains(point)) equals = false;
        }

        Assert.AreEqual(true, equals);
    }
    
    [Test]
    public void GrahamHexagonTest()
    {
        List<VectorD2D> points = new List<VectorD2D>()
        {
            new VectorD2D (0.866025403784439, -0.5),
            new VectorD2D (0.866025403784439, 0.5),
            new VectorD2D (0, -1),
            new VectorD2D (0, 1),
            new VectorD2D (-0.866025403784439, -0.5),
            new VectorD2D (-0.866025403784439, 0.5),
            new VectorD2D(0.5,0.5)
        };
        
        List<VectorD2D> testPoints = new List<VectorD2D>()
        {
            new VectorD2D (0.866025403784439, -0.5),
            new VectorD2D (0.866025403784439, 0.5),
            new VectorD2D (0, -1),
            new VectorD2D (0, 1),
            new VectorD2D (-0.866025403784439, -0.5),
            new VectorD2D (-0.866025403784439, 0.5),
        };

        List<VectorD2D> hullPoints = GrahamScan.GetConvexHull(points);
        
        bool equals = testPoints.Count() == hullPoints.Count();

        foreach (VectorD2D point in testPoints)
        {
            if (!hullPoints.Contains(point)) equals = false;
        }

        Assert.AreEqual(true, equals);
    }
    
    [Test]
    public void GrahamOctagonTest()
    {
        List<VectorD2D> points = new List<VectorD2D>()
        {
            new VectorD2D (1, 0),
            new VectorD2D (0.7071067811865475, 0.7071067811865475),
            new VectorD2D (0.7071067811865475, -0.7071067811865475),
            new VectorD2D (0, -1),
            new VectorD2D (-0.7071067811865475, -0.7071067811865475),
            new VectorD2D (0, 1),
            new VectorD2D (-1, 0),
            new VectorD2D (-0.7071067811865475, 0.7071067811865475),
            new VectorD2D(0.5,0.5)
        };
        
        List<VectorD2D> testPoints = new List<VectorD2D>()
        {
            new VectorD2D (1, 0),
            new VectorD2D (0.7071067811865475, 0.7071067811865475),
            new VectorD2D (0.7071067811865475, -0.7071067811865475),
            new VectorD2D (0, -1),
            new VectorD2D (-0.7071067811865475, -0.7071067811865475),
            new VectorD2D (0, 1),
            new VectorD2D (-1, 0),
            new VectorD2D (-0.7071067811865475, 0.7071067811865475),
        };

        List<VectorD2D> hullPoints = GrahamScan.GetConvexHull(points);
        
        bool equals = testPoints.Count() == hullPoints.Count();

        foreach (VectorD2D point in testPoints)
        {
            if (!hullPoints.Contains(point)) equals = false;
        }

        Assert.AreEqual(true, equals);
    }
    
    [Test]
    public void IncrementalTetrahedronTest()
    {
        List<VectorD3D> pointsIn = new List<VectorD3D>()
        {
            new VectorD3D(-1,-1,-1),
            new VectorD3D(-1,1,1),
            new VectorD3D(1,-1,1),
            new VectorD3D(1,1,-1),
            new VectorD3D(0,0,0)
        };
        
        List<VectorD3D> tetrahedronPoints = new List<VectorD3D>()
        {
            new VectorD3D(-1,-1,-1),
            new VectorD3D(-1,1,1),
            new VectorD3D(1,-1,1),
            new VectorD3D(1,1,-1)
        };

        HashSet<Face> expected = new HashSet<Face>(new FaceEqualityComparer());
        foreach (VectorD3D point1 in tetrahedronPoints)
        {
            foreach (VectorD3D point2 in tetrahedronPoints)
            {
                if (point1 != point2)
                {
                    foreach (VectorD3D point3 in tetrahedronPoints)
                    {
                        if (point3 != point2 && point3 != point1)
                        {
                            expected.Add(new Face(point1, point2, point3));
                        }
                    }
                }
            }
        }

        HashSet<Face> returned = Incremental3D.GetConvexHull(pointsIn);

        Assert.AreEqual(true, expected.SetEquals(returned));
    }
    
    [Test]
    public void IncrementalIcosahedronTest()
    {
        List<VectorD3D> pointsIn = new List<VectorD3D>()
        {
            new VectorD3D (0, 0.5, 0.559017 + 0.25),
            new VectorD3D (0, -0.5, 0.559017 + 0.25),
            new VectorD3D (0.5, 0.559017 + 0.25, 0),
            new VectorD3D (0.5, -0.559017 - 0.25, 0),
            new VectorD3D (0.559017 + 0.25, 0, 0.5),
            new VectorD3D (0.559017 + 0.25, 0, -0.5),
            new VectorD3D (-0.5, 0.559017 + 0.25, 0),
            new VectorD3D (-0.5, -0.559017 - 0.25, 0),
            new VectorD3D (-0.559017 - 0.25, 0, 0.5),
            new VectorD3D (0, 0.5, -0.559017 - 0.25),
            new VectorD3D (0, -0.5, -0.559017 - 0.25),
            new VectorD3D (-0.559017 - 0.25, 0, -0.5),
            new VectorD3D (0,0,0)
        };

        HashSet<Face> expected = new HashSet<Face>(new FaceEqualityComparer())
        {
            new Face(new VectorD3D(0,0.5,0.809017),new VectorD3D(-0.5,0.809017,0),new VectorD3D(0.5,0.809017,0)),
            new Face(new VectorD3D(0.5,0.809017,0),new VectorD3D(0.809017,0,-0.5),new VectorD3D(0.809017,0,0.5)),
            new Face(new VectorD3D(0.5,0.809017,0),new VectorD3D(0.809017,0,0.5),new VectorD3D(0,0.5,0.809017)),
            new Face(new VectorD3D(0.809017,0,0.5),new VectorD3D(0,-0.5,0.809017),new VectorD3D(0,0.5,0.809017)),
            new Face(new VectorD3D(0.809017,0,-0.5),new VectorD3D(0,0.5,-0.809017),new VectorD3D(0.5,0.809017,0)),
            new Face(new VectorD3D(0.809017,0,0.5),new VectorD3D(0.5,-0.809017,0),new VectorD3D(0,-0.5,0.809017)),
            new Face(new VectorD3D(0.5,-0.809017,0),new VectorD3D(-0.5,-0.809017,0),new VectorD3D(0,-0.5,-0.809017)),
            new Face(new VectorD3D(0,-0.5,-0.809017),new VectorD3D(0,0.5,-0.809017),new VectorD3D(0.809017,0,-0.5)),
            new Face(new VectorD3D(0.809017,0,0.5),new VectorD3D(0.809017,0,-0.5),new VectorD3D(0.5,-0.809017,0)),
            new Face(new VectorD3D(0.5,-0.809017,0),new VectorD3D(0.809017,0,-0.5),new VectorD3D(0,-0.5,-0.809017)),
            new Face(new VectorD3D(0,-0.5,0.809017),new VectorD3D(-0.809017,0,0.5),new VectorD3D(-0.5,-0.809017,0)),
            new Face(new VectorD3D(0,-0.5,0.809017),new VectorD3D(-0.5,-0.809017,0),new VectorD3D(0.5,-0.809017,0)),
            new Face(new VectorD3D(0,0.5,0.809017),new VectorD3D(-0.809017,0,0.5),new VectorD3D(0,-0.5,0.809017)),
            new Face(new VectorD3D(0,0.5,0.809017),new VectorD3D(-0.809017,0,0.5),new VectorD3D(-0.5,0.809017,0)),
            new Face(new VectorD3D(-0.5,0.809017,0),new VectorD3D(-0.809017,0,-0.5),new VectorD3D(0,0.5,-0.809017)),
            new Face(new VectorD3D(-0.5,0.809017,0),new VectorD3D(0,0.5,-0.809017),new VectorD3D(0.5,0.809017,0)),
            new Face(new VectorD3D(-0.5,-0.809017,0),new VectorD3D(-0.809017,0,-0.5),new VectorD3D(0,-0.5,-0.809017)),
            new Face(new VectorD3D(-0.5,-0.809017,0),new VectorD3D(-0.809017,0,-0.5),new VectorD3D(-0.809017,0,0.5)),
            new Face(new VectorD3D(-0.5,0.809017,0),new VectorD3D(-0.809017,0,-0.5),new VectorD3D(-0.809017,0,0.5)),
            new Face(new VectorD3D(0,0.5,-0.809017),new VectorD3D(-0.809017,0,-0.5),new VectorD3D(0,-0.5,-0.809017))
        };

        HashSet<Face> returned = Incremental3D.GetConvexHull(pointsIn);

        Assert.AreEqual(true, expected.SetEquals(returned));
    }
}
