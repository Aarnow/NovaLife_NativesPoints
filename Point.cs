using UnityEngine;
using Newtonsoft.Json;

namespace NativePoints
{
    public class Point
    {
        public string Name {  get; set; }
        public bool IsActive { get; set; }
        public string Position { get; set; }
        [JsonIgnore]
        public Vector3 VPosition
        {
            get => Vector3Converter.ReadJson(Position);
            set => Position = Vector3Converter.WriteJson(value);
        }

        [JsonConstructor]
        public Point(string name, bool isActive, string position)
        {
            Name = name;
            IsActive = isActive;
            Position = position;
        }
        public Point(string name, bool isActive, Vector3 position)
        {
            Name = name;
            IsActive = isActive;
            VPosition = position;
        }
    }
}
