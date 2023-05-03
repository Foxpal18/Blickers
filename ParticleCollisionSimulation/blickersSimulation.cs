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
    public double mass;
    public double radius;
    public Point position;
    public Point velocity;

    public Particle(double mass, double radius, Point position, Point velocity)
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
    private const int TimeStep = 1;
    private const double CollisionThreshold = 0.95f;
    private const double PotentialStrength = 1.0f;

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
            double mass = (double)random.Next(10,14);
            double radius = ((double)mass);
            Point position = new Point(random.Next(Width), random.Next(Height));
            Point velocity = new Point((int)random.NextDouble(), (int)random.NextDouble());

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
            particle.position.X += particle.velocity.X * TimeStep / 200;
            particle.position.Y += particle.velocity.Y * TimeStep / 200;

            // Apply periodic boundary conditions
            if (particle.position.X < 0) particle.position.X += Width;
            if (particle.position.X >= Width) particle.position.X -= Width;
            if (particle.position.Y < 0) particle.position.Y += Height;
            if (particle.position.Y >= Height) particle.position.Y -= Height;

            // Render particle
            try
            {
                graphics.FillEllipse(Brushes.White, (float)(particle.position.X - particle.radius), (float)(particle.position.Y - particle.radius), (float)(2 * particle.radius), (float)(2 * particle.radius));
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

            for (int j = i + 1; j < ParticleCount; j++)
            {
                Particle particle2 = particles[j];

                // Calculate distance and direction
                double dx = particle2.position.X - particle1.position.X;
                double dy = particle2.position.Y - particle1.position.Y;
                double distance = (double)Math.Sqrt(dx * dx + dy * dy);
                double directionX = dx / distance;
                double directionY = dy / distance;

                // Calculate potential energy
                double mass = Math.Min(particle1.mass, particle2.mass);
                double potentialEnergy = (double)(PotentialStrength * Math.Pow(mass / distance, 12) - PotentialStrength * Math.Pow(mass / distance, 6));

                // Calculate force
                double force = (double)(12 * PotentialStrength * Math.Pow(mass / distance, 13) - 6 * PotentialStrength * Math.Pow(mass / distance, 7));

                // Apply force to particles
                particle1.velocity.X += (force * directionX / particle1.mass * TimeStep / 20) / 2;
                particle1.velocity.Y += (force * directionY / particle1.mass * TimeStep / 20) / 2;
                particle2.velocity.X -= (force * directionX / particle2.mass * TimeStep / 20) / 2;
                particle2.velocity.Y -= (force * directionY / particle2.mass * TimeStep / 20) / 2;

                // Handle collisions
                if (distance < particle1.radius + particle2.radius)
                {
                    // Calculate overlap
                    double overlap = (particle1.radius + particle2.radius - distance) * CollisionThreshold;

                    // Move particles to avoid overlap
                    particle1.position.X -= overlap * directionX * particle2.mass / (particle1.mass + particle2.mass);
                    particle1.position.Y -= overlap * directionY * particle2.mass / (particle1.mass + particle2.mass);
                    particle2.position.X += overlap * directionX * particle1.mass / (particle1.mass + particle2.mass);
                    particle2.position.Y += overlap * directionY * particle1.mass / (particle1.mass + particle2.mass);

                    // Calculate new velocities after collision
                    double vx1 = particle1.velocity.X;
                    double vy1 = particle1.velocity.Y;
                    double vx2 = particle2.velocity.X;
                    double vy2 = particle2.velocity.Y;
                    double xdiff = directionX - directionY;
                    double dotProduct = ((vx1 - vx2) * xdiff) + ((vy1 - vy2) * xdiff);
                    double xnorm = 2 * xdiff * xdiff;
                    double massSum = particle1.mass + particle2.mass;

                    particle1.velocity.X -= (2 * particle2.mass / massSum) * (dotProduct / xnorm) * xdiff; // These are almost right, need to get non-direct collisions fixed and vectors properly implemented
                    particle1.velocity.Y -= (2 * particle2.mass / massSum) * (dotProduct / xnorm) * xdiff;
                    particle2.velocity.X -= (2 * particle1.mass / massSum) * (dotProduct / xnorm) * (-1) * xdiff;
                    particle2.velocity.Y -= (2 * particle1.mass / massSum) * (dotProduct / xnorm) * (-1) * xdiff; 
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


