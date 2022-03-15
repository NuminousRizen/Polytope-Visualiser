using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Polytope4D
{
    public class Incremental4D
    {
        private static bool IsSame(List<VectorD4D> a, List<VectorD4D> b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < b.Count; j++)
                {
                    if (a[i] == b[j])
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
        
        private static HashSet<List<VectorD4D>> GetHorizon(HashSet<HyperFacet> visible, HashSet<HyperFacet> nonVisible)
        {
            HashSet<List<VectorD4D>> horizon = new HashSet<List<VectorD4D>>();
            foreach (HyperFacet facetA in visible)
            {
                foreach (HyperFacet facetB in nonVisible)
                {
                    foreach (List<VectorD4D> subFacetA in facetA.subFacets)
                    {
                        foreach (List<VectorD4D> subFacetB in facetB.subFacets)
                        {
                            if (IsSame(subFacetA, subFacetB))
                            {
                                horizon.Add(subFacetA);
                            }
                        } 
                    }
                }
            }

            return horizon;
        }

        private static HashSet<VectorD4D> UpdateOutsidePoints(HashSet<VectorD4D> previousOutsidePoints,
            HashSet<HyperFacet> hyperFacets)
        {
            HashSet<VectorD4D> outsidePoints = new HashSet<VectorD4D>();

            foreach (VectorD4D point in previousOutsidePoints)
            {
                foreach (HyperFacet facet in hyperFacets)
                {
                    if (facet.IsVisible(point))
                    {
                        outsidePoints.Add(point);
                    }
                }
            }

            return outsidePoints;
        }

        public static HashSet<HyperFacet> GetConvexHull(List<VectorD4D> pointsIn)
        {
            if (pointsIn.Count < 5) throw new Exception("Cannot build a convex hull with less than 5 points.");

            HashSet<VectorD4D> outsidePoints = new HashSet<VectorD4D>(pointsIn);
            HashSet<HyperFacet> convexHullFacets = new HashSet<HyperFacet>();

            List<VectorD4D> initialPoints = new List<VectorD4D>();

            for (int i = 0; i < 5; i++)
            {
                initialPoints.Add(pointsIn[i]);
            }

            for (int i = 0; i < initialPoints.Count; i++)
            {
                HyperFacet newFacet = new HyperFacet(
                    initialPoints[i],
                    initialPoints[(i + 1) % initialPoints.Count],
                    initialPoints[(i + 2) % initialPoints.Count],
                    initialPoints[(i + 3) % initialPoints.Count]
                );
                newFacet.CorrectNormal(new List<VectorD4D>() {initialPoints[(i + 4) % initialPoints.Count]});
                convexHullFacets.Add(newFacet);
                outsidePoints.Remove(initialPoints[i]);
            }

            outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullFacets);

            while (outsidePoints.Count > 0)
            {
                List<VectorD4D> outsidePointsList = new List<VectorD4D>(outsidePoints);
                VectorD4D currentPoint = outsidePointsList[0];

                HashSet<HyperFacet> visibleFacets = new HashSet<HyperFacet>();
                HashSet<HyperFacet> nonVisibleFacets = new HashSet<HyperFacet>();

                foreach (HyperFacet facet in convexHullFacets)
                {
                    if (facet.IsVisible(currentPoint)) visibleFacets.Add(facet);
                    else nonVisibleFacets.Add(facet);
                }

                if (visibleFacets.Count > 0)
                {
                    HashSet<List<VectorD4D>> horizonFacets = GetHorizon(visibleFacets, nonVisibleFacets);

                    Debug.Log("Horizon: " + horizonFacets.Count);

                    foreach (HyperFacet facet in visibleFacets)
                    {
                        convexHullFacets.Remove(facet);
                    }

                    foreach (List<VectorD4D> subFacet in horizonFacets)
                    {
                        HyperFacet newHyperFacet = new HyperFacet(
                            subFacet[0],
                            subFacet[1],
                            subFacet[2],
                            currentPoint
                        );
                        
                        newHyperFacet.CorrectNormal(initialPoints);
                        convexHullFacets.Add(newHyperFacet);
                    }
                }

                outsidePoints.Remove(currentPoint);
                Debug.Log("--" + outsidePoints.Count);
                outsidePoints = UpdateOutsidePoints(outsidePoints, convexHullFacets);
                Debug.Log(outsidePoints.Count + "--");
            }

            return convexHullFacets;
        }
    }
}