using System;
using System.Security.Cryptography;
using GXPEngine;

public struct Vec2
{
    public float x;
    public float y;

    public Vec2(float pX = 0, float pY = 0)
    {
        x = pX;
        y = pY;
    }

    public void Normalize()
    {
        float length = Length();
        if (length != 0)
        {
            x /= length;
            y /= length;
        }
        else
        {
            x = 0;
            y = 0;
        }
    }

    public void SetXY(float num1, float num2)
    {
        x = num1;
        y = num2;
    }

    public void SetAngleDeg(float deg)
    {
        SetAngleRad(Deg2Rad(deg));
    }

    public void SetAngleRad(float rad)
    {
        float length = Length();
        Normalize();
        Vec2 newVec = GetUnitVectorRad(rad);
        this = newVec * length;
    }

    public void RotateDeg(float deg)
    {
        RotateRad(Deg2Rad(deg));
    }

    public void RotateRad(float rad)
    {
        rad += GetAngleRad();
        SetAngleRad(rad);
    }

    public Vec2 RotatedDeg(float deg)
    {
        return RotatedRad(Deg2Rad(deg));
    }

    public Vec2 RotatedRad(float rad)
    {
        Vec2 newVec = this;
        newVec.RotateRad(rad);
        return newVec;
    }

    public void RotateAroundDeg(float deg, Vec2 vec)
    {
        RotateAroundRad(Deg2Rad(deg), vec);
    }

    public void RotateAroundRad(float rad, Vec2 vec)
    {
        x -= vec.x;
        y -= vec.y;
        RotateRad(rad);
        x += vec.x;
        y += vec.y;
    }

    public Vec2 Normalized()
    {
        float length = Length();
        if (length != 0)
        {
            float roundFactor = 1000000f;
            return new Vec2(Mathf.Round(x / length * roundFactor) / roundFactor, Mathf.Round(y / length * roundFactor) / roundFactor);
        }
        else
        {
            return new Vec2(0, 0);
        }
    }

    public float Length()
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }

    public float GetAngleDeg()
    {
        return Rad2Deg(GetAngleRad());
    }

    public float GetAngleRad()
    {
        float angle = Mathf.Atan2(y, x);
        if (angle < 0)
        {
            angle += 2 * Mathf.PI;
        }
        angle %= 2 * Mathf.PI;
        return angle;
    }

    public static Vec2 operator +(Vec2 left, Vec2 right)
    {
        return new Vec2(left.x + right.x, left.y + right.y);
    }

    public static Vec2 operator -(Vec2 left, Vec2 right)
    {
        return new Vec2(left.x - right.x, left.y - right.y);
    }

    public static Vec2 operator *(Vec2 vec, float num)
    {
        return new Vec2(vec.x * num, vec.y * num);
    }

    public static Vec2 operator *(float num, Vec2 vec)
    {
        return new Vec2(vec.x * num, vec.y * num);
    }

    public static Vec2 operator /(Vec2 vec2, float num)
    {
        if (num != 0)
        {
            return new Vec2(vec2.x / num, vec2.y / num);
        }
        else
        {
            return vec2;
        }
    }

    public static Vec2 operator /(float num, Vec2 vec2)
    {
        if (num != 0)
        {
            return new Vec2(vec2.x / num, vec2.y / num);
        }
        else
        {
            return vec2;
        }
    }

    public static bool operator ==(Vec2 vec1, Vec2 vec2)
    {
        if (vec1.x == vec2.x && vec1.y == vec2.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator !=(Vec2 vec1, Vec2 vec2)
    {
        if (vec1.x == vec2.x && vec1.y == vec2.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static float Deg2Rad(float deg)
    {
        return deg * (Mathf.PI / 180);
    }

    public static float Rad2Deg(float rad)
    {
        return rad * (180 / Mathf.PI);
    }

    public static Vec2 GetUnitVectorDeg(float deg)
    {
        return GetUnitVectorRad(Deg2Rad(deg));
    }

    public static Vec2 GetUnitVectorRad(float rad)
    {
        return new Vec2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static Vec2 RandomUnitVector()
    {
        Random rnd = new Random();
        return GetUnitVectorDeg(rnd.Next(0, 360));
    }

    public override string ToString()
    {
        if (Mathf.Abs(x) % 1 == 0 && Mathf.Abs(y) % 1 == 0)
        {
            return String.Format("({0},{1})", x, y);
        }
        else
        {
            return String.Format("({0};{1})", x, y);
        }
    }

    public float Dot(Vec2 vec)
    {
        return x * vec.x + y * vec.y;
    }

    public void Reflect(Vec2 normal, float bounciness = 1)
    {
        this -= (1 + bounciness) * (Dot(normal.Normalized()) * normal.Normalized());
    }

    public Vec2 Normal()
    {
        return new Vec2(-y, x).Normalized();
    }
}