using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Line : GameObject {
    public Vec2 point1;
    public Vec2 point2;
    public Item item;

    public Line(float x1, float y1, float x2, float y2, Item item = null) : base() { 
        point1 = new Vec2(x1, y1);
        point2 = new Vec2(x2, y2);

        this.item = item;
    }

    void Update() {

    }

    public bool CheckIntersection(Line line) {
        float q = (point1.y - line.point1.y) * (line.point2.x - line.point1.x) - (point1.x - line.point1.x) * (line.point2.y - line.point1.y);
        float d = (point2.x - point1.x) * (line.point2.y - line.point1.y) - (point2.y - point1.y) * (line.point2.x - line.point1.x);

        if (d == 0)
            return false;

        float r = q / d;

        q = (point1.y - line.point1.y) * (point2.x - point1.x) - (point1.x - line.point1.x) * (point2.y - point1.y);
        float s = q / d;

        if (r < 0 || r > 1 || s < 0 || s > 1)
            return false;

        return true;
    }
}