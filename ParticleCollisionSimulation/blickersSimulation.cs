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
//    public double BondingK { get; set; }
//    public double BondingRO {  get; set; }

    private List<Particle> particles;
    private Random random;
    private Bitmap bitmap;
    private Graphics graphics;

    
//    public void updateForces()
//    {
//        for (int i = 0; i < particles.Count; i++)
//        {
//            Particle p1 = particles[i];
//            for (int j = i + 1; j < particles.Count; j++)
//            {
//                Particle p2 = particles[j];
//                Vector2D r = p2.position;
//                double dist = r.Magnitude;
//            }
//        }
//    }

    public ParticleSimulation()
    {
        Text = "Particle Simulation";
        Size = new Size(Width, Height);
        this.DoubleBuffered = true;

        particles = new List<Particle>();
        random = new Random();

        for (int i = 0; i < ParticleCount; i++)
        {
            float mass = (float)random.Next(10,14);
            float radius = ((float)mass);
            PointF position = new PointF(random.Next(Width), random.Next(Height));
            PointF velocity = new PointF((float)random.Next(0,12), (float)random.Next(0,12));

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
            try
            {
                graphics.FillEllipse(Brushes.White, particle.position.X - particle.radius, particle.position.Y - particle.radius, 2 * particle.radius, 2 * particle.radius);
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
                float dx = particle2.position.X - particle1.position.X;
                float dy = particle2.position.Y - particle1.position.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                float directionX = dx / distance;
                float directionY = dy / distance;

                // Calculate potential energy
                float mass = Math.Min(particle1.mass, particle2.mass);
                float potentialEnergy = (float)(PotentialStrength * Math.Pow(mass / distance, 12) - PotentialStrength * Math.Pow(mass / distance, 6));

                // Calculate force
                float force = (float)(12 * PotentialStrength * Math.Pow(mass / distance, 13) - 6 * PotentialStrength * Math.Pow(mass / distance, 7));

                // Apply force to particles
                particle1.velocity.X += (force * directionX / particle1.mass * TimeStep) / 2;
                particle1.velocity.Y += (force * directionY / particle1.mass * TimeStep) / 2;
                particle2.velocity.X -= (force * directionX / particle2.mass * TimeStep) / 2;
                particle2.velocity.Y -= (force * directionY / particle2.mass * TimeStep) / 2;

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
                    float xdiff = directionX - directionY;
                    float dotProduct = ((vx1 - vx2) * xdiff) + ((vy1 - vy2) * xdiff);
                    float xnorm = 2 * xdiff * xdiff;
                    float massSum = particle1.mass + particle2.mass;

                    particle1.velocity.X -= (2 * particle2.mass / massSum) * (dotProduct / xnorm) * xdiff; // These are almost right, need to get non-direct collisions fixed and vectors properly implamented
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


