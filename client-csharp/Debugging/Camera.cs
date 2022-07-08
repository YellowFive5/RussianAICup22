#region Usings

using System.IO;
using AiCup22.Model;

#endregion

namespace AiCup22.Debugging
{
    /// <summary>
    /// Camera state
    /// </summary>
    public struct Camera
    {
        /// <summary>
        /// Center
        /// </summary>
        public Vec2 Center { get; set; }

        /// <summary>
        /// Rotation
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Attack angle
        /// </summary>
        public double Attack { get; set; }

        /// <summary>
        /// Vertical field of view
        /// </summary>
        public double Fov { get; set; }

        public Camera(Vec2 center, double rotation, double attack, double fov)
        {
            Center = center;
            Rotation = rotation;
            Attack = attack;
            Fov = fov;
        }

        /// <summary> Read Camera from reader </summary>
        public static Camera ReadFrom(BinaryReader reader)
        {
            var result = new Camera();
            result.Center = Vec2.ReadFrom(reader);
            result.Rotation = reader.ReadDouble();
            result.Attack = reader.ReadDouble();
            result.Fov = reader.ReadDouble();
            return result;
        }

        /// <summary> Write Camera to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            Center.WriteTo(writer);
            writer.Write(Rotation);
            writer.Write(Attack);
            writer.Write(Fov);
        }

        /// <summary> Get string representation of Camera </summary>
        public override string ToString()
        {
            string stringResult = "Camera { ";
            stringResult += "Center: ";
            stringResult += Center.ToString();
            stringResult += ", ";
            stringResult += "Rotation: ";
            stringResult += Rotation.ToString();
            stringResult += ", ";
            stringResult += "Attack: ";
            stringResult += Attack.ToString();
            stringResult += ", ";
            stringResult += "Fov: ";
            stringResult += Fov.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}