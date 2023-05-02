// Brief comments: Add constant forcing field where vector + magnitude 


using ParticleCollisionSimulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class Particle
{
    public float mass;
    public float radius;
    public PointF position;
    public PointF velocity;

    public Particle(float mass, float radius, PointF position, PointF velocity)
    {
        this.mass = mass;
        this.radius = radius;
        this.position = position;
        this.velocity = velocity;
    }
}

public class ParticleSimulation : Form
{
    private const int Width = 1600;
    private const int Height = 1200;
    private const int ParticleCount = 200;
    private const float TimeStep = 0.01f;
    private const float CollisionThreshold = 0.95f;
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
            float mass = (float)random.NextDouble() * 10 + 1;
            float radius = (float)Math.Sqrt(mass);
            PointF position = new PointF(random.Next(Width), random.Next(Height));
            PointF velocity = new PointF((float)random.NextDouble() * 12 - 2, (float)random.NextDouble() * 12 - 2);

            particles.Add(new Particle(mass, radius, position, velocity));
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
        for (int i = 0; i < ParticleCount; i++)
        {
            Particle particle = particles[i];

            // Apply velocity
            particle.position.X += particle.velocity.X * TimeStep;
            particle.position.Y += particle.velocity.Y * TimeStep;

            // Apply periodic boundary conditions
            if (particle.position.X < 0) particle.position.X += Width;
            if (particle.position.X >= Width) particle.position.X -= Width;
            if (particle.position.Y < 0) particle.position.Y += Height;
            if (particle.position.Y >= Height) particle.position.Y -= Height;

            // Render particle
            graphics.FillEllipse(Brushes.White, particle.position.X - particle.radius, particle.position.Y - particle.radius, 2 * particle.radius, 2 * particle.radius);
        }

        // Update particle velocities
        for (int i = 0; i < ParticleCount; i++)
        {
            Particle particle1 = particles[i];

            for (int j = i + 1; j < ParticleCount; j++)
            {
                Particle particle2 = particles[j];

                // Calculate distance and direction
                float dx = particle2.position.X - particle1.position.X;
                float dy = particle2.position.Y - particle1.position.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float directionX = dx / distance;
                float directionY = dy / distance;

                // Calculate potential energy
                float mass = Math.Min(particle1.mass, particle2.mass);
                float potentialEnergy = (float)(PotentialStrength * Math.Pow(mass / distance, 12) - 2 * PotentialStrength * Math.Pow(mass / distance, 6));

                // Calculate force
                float force = (float)(-12 * PotentialStrength * Math.Pow(mass / distance, 13) + 6 * PotentialStrength * Math.Pow(mass / distance, 7));

                // Apply force to particles
                particle1.velocity.X += force * directionX / particle1.mass * TimeStep % 2;
                particle1.velocity.Y += force * directionY / particle1.mass * TimeStep % 2;
                particle2.velocity.X -= force * directionX / particle2.mass * TimeStep % 2;
                particle2.velocity.Y -= force * directionY / particle2.mass * TimeStep % 2;

                // Handle collisions
                if (distance < particle1.radius + particle2.radius)
                {
                    // Calculate overlap
                    float overlap = (particle1.radius + particle2.radius - distance) * CollisionThreshold;

                    // Move particles to avoid overlap
                    particle1.position.X -= overlap * directionX * particle2.mass / (particle1.mass + particle2.mass);
                    particle1.position.Y -= overlap * directionY * particle2.mass / (particle1.mass + particle2.mass);
                    particle2.position.X += overlap * directionX * particle1.mass / (particle1.mass + particle2.mass);
                    particle2.position.Y += overlap * directionY * particle1.mass / (particle1.mass + particle2.mass);

                    // Calculate new velocities after collision
                    float vx1 = particle1.velocity.X;
                    float vy1 = particle1.velocity.Y;
                    float vx2 = particle2.velocity.X;
                    float vy2 = particle2.velocity.Y;
                    float dotProduct = (vx2 - vx1) * directionX + (vy2 - vy1) * directionY;
                    float massSum = particle1.mass + particle2.mass;
                    float impulse = (1 + CollisionThreshold) * dotProduct / massSum;

                    particle1.velocity.X += impulse * directionX;
                    particle1.velocity.Y += impulse * directionY;
                    particle2.velocity.X -= impulse * directionX;
                    particle2.velocity.Y -= impulse * directionY;
                }
            }
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


