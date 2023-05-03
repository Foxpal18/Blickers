using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetTopologySuite.Triangulate;
using NetTopologySuite.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class Particle1 : Form
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    public float radius;
    public float mass;

    public Particle1(Vector2 position, Vector2 velocity, Vector2 acceleration, float radius, float mass)
    {
        this.position = position;
        this.velocity = velocity;
        this.acceleration = acceleration;
        this.radius = radius;
        this.mass = mass;
    }

    public void Update(float dt, List<Particle1> particles)
    {
        // Apply Lennard-Jones potential to all particles in the system
        foreach (var other in particles)
        {
            if (other != this)
            {
                float r = Vector2.Distance(position, other.position);
                if (r < radius + other.radius)
                {
                    // Handle collision
                    HandleCollision(other);
                }
                else
                {
                    // Apply Lennard-Jones potential
                    float epsilon = 1.0f;
                    float sigma = radius + other.radius;
                    Vector2 direction = Vector2.Normalize(other.position - position);
                    acceleration += Vector2.Multiply(direction, (float)(4 * epsilon * (12 * Math.Pow(sigma, 12) / Math.Pow(r, 13) - 6 * Math.Pow(sigma, 6) / Math.Pow(r, 7)))) / mass;

                }
            }
        }

        foreach (var p in particles)
        {
            p.Update(dt, particles);
        }

        // Update position and velocity
        velocity += acceleration * dt;
        position += velocity * dt;

        // Reset acceleration
        acceleration = Vector2.Zero;
    }

    private void HandleCollision(Particle1 other)
    {
        // Calculate new velocities after collision
        Vector2 normal = Vector2.Normalize(other.position - position);
        Vector2 tangential = new Vector2(-normal.Y, normal.X);
        float vn1 = Vector2.Dot(normal, velocity);
        float vn2 = Vector2.Dot(normal, other.velocity);
        float vt1 = Vector2.Dot(tangential, velocity);
        float vt2 = Vector2.Dot(tangential, other.velocity);
        float m1 = mass;
        float m2 = other.mass;
        float v1f = ((m1 - m2) * vn1 + 2 * m2 * vn2) / (m1 + m2);
        float v2f = ((m2 - m1) * vn2 + 2 * m1 * vn1) / (m1 + m2);
        Vector2 vf1 = normal * v1f + tangential * vt1;
        Vector2 vf2 = normal * v2f + tangential * vt2;
        velocity = vf1;
        other.velocity = vf2;
    }
}

public class ParticleSystem
{
    public List<Particle> particles;

    public ParticleSystem()
    {
        particles = new List<Particle>();
    }

    public void AddParticle(Particle p)
    {
        particles.Add(p);
    }
}

public class Renderer
{
    static int segments = 30;
    float radius = 1.0f;
    Vector2[] vertices = new Vector2[segments];
    public void Render(List<Particle> particles)
    {
        // Clear the screen
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Draw all particles as circles
        GL.Begin(PrimitiveType.TriangleFan);
        foreach (var p in particles)
        {
            GL.Color3(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i * (2 * (float)Math.PI / segments);
                float x = radius * (float)Math.Cos(angle);
                float y = radius * (float)Math.Sin(angle);
                vertices[i] = new Vector2(x, y);
            }
        }
        GL.End();
    }
}

