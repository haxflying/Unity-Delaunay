//http://paulbourke.net/papers/triangulate/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Circumscribed : MonoBehaviour
{
    public bool DisplayCircle;
    public int count = 10;
    public float stepWaitTime = 0.2f;
    public float range = 50f;
    public GameObject original;
    public GameObject point;
    public Material line;

    private const float eps = 0.5f;

    private List<Triangle> tmp_triangles = new List<Triangle>();
    public List<Vector2> vertices = new List<Vector2>();
    private List<Triangle> triangles = new List<Triangle>();  
    private List<Edge> edges = new List<Edge>();

    private List<Triangle> draw_triangles = new List<Triangle>();
    private List<Circle> draw_circle = new List<Circle>();
    private List<Vector2> draw_aabb = new List<Vector2>();
    private void Start()
    {

        Tick.instance.OnDraw += DrawTriangle;
        if(DisplayCircle)
            Tick.instance.OnDraw += DrawCircle;

        StartCoroutine(Delaunay());
    }


    private void DrawTriangle()
    {

        line.SetPass(0);
        line.SetColor("_Color", Color.green);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(new Color(0, 1, 1));
        for (int i = 0; i < draw_triangles.Count; i++)
        {
            GL.Vertex3(draw_triangles[i].p1.x, draw_triangles[i].p1.y, 0);
            GL.Vertex3(draw_triangles[i].p2.x, draw_triangles[i].p2.y, 0);
            GL.Vertex3(draw_triangles[i].p2.x, draw_triangles[i].p2.y, 0);
            GL.Vertex3(draw_triangles[i].p3.x, draw_triangles[i].p3.y, 0);
            GL.Vertex3(draw_triangles[i].p3.x, draw_triangles[i].p3.y, 0);
            GL.Vertex3(draw_triangles[i].p1.x, draw_triangles[i].p1.y, 0);
        }
        
        GL.End();
        GL.PopMatrix();
    }

    private void DrawAABB()
    {
        line.SetPass(0);
        line.SetColor("_Color", Color.cyan);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINE_STRIP);

        //GL.Color(new Color(0, 1, 1));
        for (int i = 0; i < draw_aabb.Count; i += 4)
        {
            GL.Vertex3(draw_aabb[i].x, draw_aabb[i].y, 0);
            GL.Vertex3(draw_aabb[i + 1].x, draw_aabb[i + 1].y, 0);
            GL.Vertex3(draw_aabb[i + 2].x, draw_aabb[i + 2].y, 0);
            GL.Vertex3(draw_aabb[i + 3].x, draw_aabb[i + 3].y, 0);
            GL.Vertex3(draw_aabb[i].x, draw_aabb[i].y, 0);
        }
        GL.End();

        GL.PopMatrix();
    }

    private void DrawCircle()
    {
        line.SetPass(0);
        for (int k = 0; k < draw_circle.Count; k++)
        {
            GL.PushMatrix();
            Matrix4x4 translate = Matrix4x4.Translate(new Vector3(draw_circle[k].center.x, draw_circle[k].center.y, 0));
            GL.MultMatrix(translate * transform.localToWorldMatrix);
            GL.Begin(GL.LINE_STRIP);
        
            for (int i = 0; i < 361; i++)
            {
                float angle = Mathf.Deg2Rad * (float)i;
                GL.Vertex3(Mathf.Cos(angle) * draw_circle[k].radiu, Mathf.Sin(angle) * draw_circle[k].radiu, 0);
            }
        
            GL.End();
            GL.PopMatrix();
            line.SetColor("_Color", Color.yellow);
        }
    }

    private IEnumerator Delaunay()
    {
        WaitForSeconds wat = new WaitForSeconds(stepWaitTime);
        GeneratePoints();
        Triangle superTriangle = GenerateSuperTriangle(vertices[0], vertices[1], vertices[2]);
        tmp_triangles.Add(superTriangle);
        for (int k = 0; k < vertices.Count; k++)
        {
            edges.Clear();
            for (int i = 0; i < tmp_triangles.Count; i++)
            {
                if(tmp_triangles[i].circumcircle.inside(vertices[k]))
                {
                    edges.InsertRange(edges.Count, tmp_triangles[i].edges);
                    tmp_triangles.RemoveAt(i--);
                }                
            }
            //delete all doubly specified edges from the edge buffer
            for (int i = 0; i < edges.Count; i++)
            {
                List<Edge> dedges = edges.FindAll(e => e.Equals(edges[i]));
                if (dedges.Count > 1)
                {
                    print("remove edges");
                    edges.Remove(dedges[0]);
                    edges.Remove(dedges[1]);
                    i--;
                }                    
            }
            yield return wat;
            for (int i = 0; i < edges.Count; i++)
            {
                Triangle newTri = new Triangle(vertices[k], edges[i].p1, edges[i].p2);
                tmp_triangles.Add(newTri);
                draw_circle.Add(newTri.circumcircle);
            }
            draw_triangles = new List<Triangle>(tmp_triangles);
            yield return wat;                      
        }
        yield return wat;
        for (int i = 0; i < tmp_triangles.Count; i++)
        {
            if (superTriangle.isRelated(tmp_triangles[i]))
                tmp_triangles.RemoveAt(i--);
        }
        draw_triangles = new List<Triangle>(tmp_triangles);
    }
    private void GeneratePoints()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject point = Instantiate(original, new Vector3(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range), 0f), Quaternion.identity);
            vertices.Add(point.transform.toVec2());
        }
        vertices.Sort((a,b) => a.x.CompareTo(b.x));
    }
    private Triangle GenerateSuperTriangle(Vector2 A, Vector2 B, Vector2 C)
    {
        float xmin = float.MaxValue, xmax = float.MinValue, ymin = float.MaxValue, ymax = float.MinValue;
        foreach(Vector2 p in vertices)
        {
            if (xmin > p.x)
                xmin = p.x;
            if (xmax < p.x)
                xmax = p.x;
            if (ymin > p.y)
                ymin = p.y;
            if (ymax < p.y)
                ymax = p.y;
        }

        //aabb.Add(new Vector2(xmin, ymin));
        //aabb.Add(new Vector2(xmin, ymax));
        //aabb.Add(new Vector2(xmax, ymax));
        //aabb.Add(new Vector2(xmax, ymin));
        //Tick.instance.OnDraw += DrawAABB;

        Vector2 _p1 = new Vector2(xmax + (xmax - xmin) * 0.5f + eps, ymin - eps);
        Vector2 _p2 = new Vector2(xmin - (xmax - xmin) * 0.5f - eps, ymin - eps);
        Vector2 _p3 = new Vector2((xmin + xmax) * 0.5f, ymax + (ymax - ymin) + eps);

        Instantiate(point, new Vector3(_p1.x, _p1.y, 0.0f), Quaternion.identity);
        Instantiate(point, new Vector3(_p2.x, _p2.y, 0.0f), Quaternion.identity);
        Instantiate(point, new Vector3(_p3.x, _p3.y, 0.0f), Quaternion.identity);

        Triangle superTriangle = new Triangle(_p1,_p2,_p3);
        draw_triangles.Add(superTriangle);
        draw_circle.Add(superTriangle.circumcircle);

        return superTriangle;
    }

    private Edge getSharedEdge(Triangle a, Triangle b)
    {
        List<Vector2> vs = new List<Vector2>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (a.ps[i] == b.ps[j])
                {
                    vs.Add(a.ps[i]);
                }
            }
        }

        if (vs.Count >= 2)
            return new Edge(vs[0], vs[1]);
        else
            return null;
    }

}
