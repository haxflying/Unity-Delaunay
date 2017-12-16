using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public Vector2 center;
    public float radiu;

    public Circle(Vector2 c, float r)
    {
        center = c;
        radiu = r;
    }

    public bool inside(Vector2 p)
    {
        return (p - center).magnitude < radiu;
    }

    public bool atRight(Vector2 p)
    {
        return center.x + radiu < p.x;
    }
}

public class Triangle
{

    public Vector2 p1, p2, p3;
    public Circle circumcircle;
    public List<Edge> edges = new List<Edge>();
    public List<Vector2> ps;
    public Triangle(Vector2 s1, Vector2 s2, Vector2 s3)
    {
        p1 = s1;
        p2 = s2;
        p3 = s3;

        circumcircle = GenerateCircle(p1, p2, p3);

        edges.Add(new Edge(p1, p2));
        edges.Add(new Edge(p2, p3));
        edges.Add(new Edge(p1, p3));
        ps = new List<Vector2> { p1, p2, p3 };
    }

    private Circle GenerateCircle(Vector2 A, Vector2 B, Vector2 C)
    {
        float a = (B - C).magnitude;
        float b = (A - C).magnitude;
        float c = (A - B).magnitude;
        float a2 = a * a;
        float b2 = b * b;
        float c2 = c * c;
        Vector2 U = a2 * (b2 + c2 - a2) * A + b2 * (c2 + a2 - b2) * B + c2 * (a2 + b2 - c2) * C;
        U /= a2 * (b2 + c2 - a2) + b2 * (c2 + a2 - b2) + c2 * (a2 + b2 - c2);
        float radiu = (A - U).magnitude;
        //Debug.Log("Got circle : " + U + " r : " + radiu);
        return new Circle(U, radiu);
    }

    public bool isRelated(Triangle t)
    {
        if (t.p1 == p1 || t.p1 == p2 || t.p1 == p3
           || t.p2 == p1 || t.p2 == p2 || t.p2 == p3
           || t.p3 == p1 || t.p3 == p2 || t.p3 == p3)
            return true;
        else
            return false;
    }

    public bool Contains(Edge e)
    {
        int count = 0;
        for (int i = 0; i < 3; i++)
        {
            if (ps[i] == e.p1 || ps[i] == e.p2)
                count++;
        }
        if (count > 1)
            return true;
        else
            return false;
    }
}

public class Edge
{
    public Vector2 p1, p2;

    public Edge(Vector2 v1, Vector2 v2)
    {
        p1 = v1;
        p2 = v2;
    }

    public bool Equals(Edge e)
    {
        if(p1 == e.p1)
        {
            if (p2 == e.p2)
                return true;
            else
                return false;
        }
        else if(p1 == e.p2)
        {
            if (p2 == e.p1)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }
}
