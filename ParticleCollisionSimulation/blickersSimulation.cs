// Add constant forcing field where vector + magnitude is decided by a text box with entered values. Additionally, need to add other forcing options like standard force fields. 
// Finally, use periodic boundary conditions (make them interchangable) to simulate different RP^2 connective edges (see: https://en.wikipedia.org/wiki/Real_projective_plane)


using ParticleCollisionSimulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Numerics;

public class Particle
{
    public float mass;
    public float radius;
    public PointF position;
    public PointF velocity;
    public PointF force;

    public Particle(float mass, float radius, PointF position, PointF velocity, PointF force)
    {
        this.mass = mass;
        this.radius = radius;
        this.position = position;
        this.velocity = velocity;
        this.force = force;
    }
}

public class ParticleSimulation : Form
{
    private const int Width = 1600;
    private const int Height = 1200;
    private const int ParticleCount = 200;
    private const float TimeStep = 0.0001f;
    //private const float CollisionThreshold = 0.95f;
    private const float PotentialStrength = 1.0f;

    private List<Particle> particles;
    private Random random;
    private Bitmap bitmap;
    private Graphics graphics;

    public ParticleSimulation()
    {
        Text = "Particle Simulation";
        Size = new Size(Width, Height);
        this.DoubleBuffered = true;

        particles = new List<Particle>();
        random = new Random();

        for (int i = 0; i < ParticleCount; i++)
        {
            float mass = (float)random.Next(8,14);
            float radius = ((float)mass);
            PointF position = new PointF((float)random.Next(1,Width-1), (float)random.Next(1,Height-1));
            PointF velocity = new PointF(0, 0);
            PointF force = new PointF(0, 0);
            // (float)random.Next(0, 1)
            particles.Add(new Particle(mass, radius, position, velocity, force));
        }

        bitmap = new Bitmap(Width, Height);
        graphics = Graphics.FromImage(bitmap);

        Timer timer = new Timer();
        timer.Interval = 1;
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // Clear the bitmap
        graphics.Clear(Color.Black);

        // Update particle positions
        float dispx = 0;
        float dispy = 0;

        for (int i = 0; i < ParticleCount; i++)
        {
            Particle particle = particles[i];

            // Apply velocity
            //particle.position.X += particle.velocity.X * TimeStep;
            //particle.position.Y += particle.velocity.Y * TimeStep;

            // Apply periodic boundary conditions
            if (particle.position.X <= 0)
            {
                particle.position.X += Width;
                dispx = Width;
            }
            if (particle.position.X > Width)
            {
                particle.position.X -= Width;
                dispx = -Width;
            }
            if (particle.position.Y <= 0)
            {
                particle.position.Y += Height;
                dispy = Height;
            }
            if (particle.position.Y > Height)
            {
                particle.position.Y -= Height;
                dispy = -Height;
            }

            // Render particle
            try
            {
                graphics.FillEllipse(Brushes.White, particle.position.X - particle.radius, particle.position.Y - particle.radius, particle.radius, particle.radius);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Update particle velocities
        for (int i = 0; i < ParticleCount; i++)
        {
            Particle particle1 = particles[i];
            particle1.force.X = 0;
            particle1.force.Y = 0;
        }
        for (int i = 0; i < ParticleCount; i++)
        {
            Particle particle1 = particles[i];

            for (int j = i + 1; j < ParticleCount; j++)
            {
                Particle particle2 = particles[j];

                // Calculate distance and direction
                float dx = Math.Abs(particle2.position.X - particle1.position.X + dispx);
                float dy = Math.Abs(particle2.position.Y - particle1.position.Y + dispy);
                float r2 = (float)Math.Sqrt(dx * dx + dy * dy);
                float r6 = r2 * r2 * r2;
                float r8 = r6 * r2;
                float r12 = r6 * r6;
                float r14 = r12 * r2;

                //float directionX = dx / distance;
                //float directionY = dy / distance;

                // Calculate potential energy
                float potentialEnergy1 = (float)(4*(PotentialStrength * Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 12) / r12 - PotentialStrength * Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 6) / r6));
                float potentialEnergy2 = (float)(4*(PotentialStrength * Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 12) / r12 - PotentialStrength * Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 6) / r6));

                // Calculate force
                float ftmp12x = (float)(4 * -(12 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 12) / r14) - 6 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 6) / r8)));
                float ftmp12y = (float)(4 * -(12 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 12) / r14) - 6 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle1.radius, 6) / r8)));
                float ftmp21x = (float)(4 * -(12 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 12) / r14) - 6 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 6) / r8)));
                float ftmp21y = (float)(4 * -(12 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 12) / r14) - 6 * PotentialStrength * (Math.Pow(Math.Pow(2, 1 / 6) * particle2.radius, 6) / r8)));
                float f12x = ftmp12x * dx;
                float f12y = ftmp12y * dy;
                float f21x = ftmp21x * dx;
                float f21y = ftmp21y * dy;
                particle1.force.X += f21x;
                particle1.force.Y += f21y;
                particle2.force.X -= f12x;
                particle2.force.Y -= f12y;
            }
                // Apply force to particles
            particle1.velocity.X += (particle1.force.X / particle1.mass) * TimeStep / 2;
            particle1.velocity.Y += (particle1.force.Y / particle1.mass) * TimeStep / 2;
                
                // Update position using velocity
            particle1.position.X += particle1.velocity.X * TimeStep;
                
            if (particle1.position.X <= 0) particle1.position.X += Width;
            else if (particle1.position.X > Width) particle1.position.X -= Width;
                
            particle1.position.Y += particle1.velocity.Y * TimeStep;
                
            if (particle1.position.Y <= 0) particle1.position.Y += Height;
            else if (particle1.position.Y > Height) particle1.position.X -= Height;

                // Handle collisions
                //if (distance < particle1.radius + particle2.radius)
                //{
                    // Calculate overlap
                    //float overlap = (particle1.radius + particle2.radius - distance) * CollisionThreshold;

                    // Move particles to avoid overlap
                    //particle1.position.X -= overlap * directionX * particle2.mass / (particle1.mass + particle2.mass);
                    //particle1.position.Y -= overlap * directionY * particle2.mass / (particle1.mass + particle2.mass);
                    //particle2.position.X += overlap * directionX * particle1.mass / (particle1.mass + particle2.mass);
                    //particle2.position.Y += overlap * directionY * particle1.mass / (particle1.mass + particle2.mass);

                    // Calculate new velocities after collision
                    //float vx1 = particle1.velocity.X;
                    //float vy1 = particle1.velocity.Y;
                    //float vx2 = particle2.velocity.X;
                    //float vy2 = particle2.velocity.Y;
                    //float xdiff = directionX - directionY;
                    //float dotProduct = ((vx1 - vx2) * xdiff) + ((vy1 - vy2) * xdiff);
                    //float xnorm = 2 * xdiff * xdiff;
                    //float massSum = particle1.mass + particle2.mass;

                    //particle1.velocity.X -= (2 * particle2.mass / massSum) * (dotProduct / xnorm) * xdiff; // These are almost right, need to get non-direct collisions fixed and vectors properly implemented
                    //particle1.velocity.Y -= (2 * particle2.mass / massSum) * (dotProduct / xnorm) * xdiff;
                    //particle2.velocity.X -= (2 * particle1.mass / massSum) * (dotProduct / xnorm) * (-1) * xdiff;
                    //particle2.velocity.Y -= (2 * particle1.mass / massSum) * (dotProduct / xnorm) * (-1) * xdiff; 
                //}
           
        }

        // Update the window
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.DrawImage(bitmap, 0, 0);
    }

    public static void Main()
    {
        settingsForm sf = new settingsForm();
        sf.Show();

        Application.Run(new ParticleSimulation());
    }
}


