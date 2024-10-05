using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public class StarDataLoader {
  public class Star {

    // Three variables used to define the star in the game.
    public long catalog_number;
    public Vector3 position;
    public Color colour;
    public float size;
    public string name;


    // Keep the original points so we can recalculate based on dates.
    private readonly double right_ascension;
    private readonly double declination;
    private readonly double distance;

    // Constructor
    public Star(long catalog_number, double distance, double right_ascension, double declination, float magnitude, string name, float spectral_type) {
      this.catalog_number = catalog_number;
      this.right_ascension = right_ascension;
      this.distance = distance;
      this.declination = declination;
      
      this.position = GetBasePosition();
      
      this.colour = SetColour((byte)(0), (byte)(0));
      
      this.size = magnitude;
      
      this.name = name;
    }


    // Get the starting position shown in the file.
    public Vector3 GetBasePosition() {
      // Place stars on a cylinder using 2D trigonometry.
      double x = System.Math.Cos(right_ascension);
      double y = System.Math.Sin(declination);
      double z = System.Math.Sin(right_ascension);

      // Pull in ends to make the sphere
      // Work out y-adjacent and use this to scale (as on unit sphere)
      double y_cos = System.Math.Cos(declination);
      x *= y_cos;
      z *= y_cos;

      // Return as float
      return new((float)x, (float)y, (float)z);
    }

    private Color SetColour(byte spectral_type, byte spectral_index) {
      Color IntColour(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
      }
      // OBAFGKM colours from: https://arxiv.org/pdf/2101.06254.pdf
      Color[] col = new Color[8];
      col[0] = IntColour(0x5c, 0x7c, 0xff); // O1
      col[1] = IntColour(0x5d, 0x7e, 0xff); // B0.5
      col[2] = IntColour(0x79, 0x96, 0xff); // A0
      col[3] = IntColour(0xb8, 0xc5, 0xff); // F0
      col[4] = IntColour(0xff, 0xef, 0xed); // G1
      col[5] = IntColour(0xff, 0xde, 0xc0); // K0
      col[6] = IntColour(0xff, 0xa2, 0x5a); // M0
      col[7] = IntColour(0xff, 0x7d, 0x24); // M9.5

      int col_idx = -1;
      if (spectral_type == 'O') {
        col_idx = 0;
      } else if (spectral_type == 'B') {
        col_idx = 1;
      } else if (spectral_type == 'A') {
        col_idx = 2;
      } else if (spectral_type == 'F') {
        col_idx = 3;
      } else if (spectral_type == 'G') {
        col_idx = 4;
      } else if (spectral_type == 'K') {
        col_idx = 5;
      } else if (spectral_type == 'M') {
        col_idx = 6;
      }

      // If unknown, make white.
      if (col_idx == -1) {
        return Color.white;
      }

      // Map second part 0 -> 0, 10 -> 100
      float percent = (spectral_index - 0x30) / 10.0f;
      return Color.Lerp(col[col_idx], col[col_idx + 1], percent);
    }
  }  
  

  private Dictionary<string, Star> starDictionary;



  public List<Star> LoadData() {
    List<Star> stars = new List<Star>();
    starDictionary = new Dictionary<string, Star>();

    using (StreamReader strReader = new StreamReader("csvrecieved.csv")) {
      strReader.ReadLine(); // Skip header

      string line;
      while ((line = strReader.ReadLine()) != null) {
          var points = line.Split(',');

          if (points.Length < 5 || string.IsNullOrEmpty(points[0]) || string.IsNullOrEmpty(points[1]) || 
            string.IsNullOrEmpty(points[2]) || string.IsNullOrEmpty(points[3]) || 
            System.Convert.ToDouble(points[3], CultureInfo.InvariantCulture) > 7) {
            continue;
          }

          long catalogNumber = long.Parse(points[4]);
          float ra = float.Parse(points[0], CultureInfo.InvariantCulture) * Mathf.PI / 180f;
          float dec = float.Parse(points[1], CultureInfo.InvariantCulture) * Mathf.PI / 180f;
          float magnitude = float.Parse(points[3], CultureInfo.InvariantCulture);
          string name = $"GDR3 {catalogNumber}";
          float distance = float.Parse(points[2], CultureInfo.InvariantCulture);
          float spectral_type = float.Parse(points[5], CultureInfo.InvariantCulture);

          Star star = new Star(catalogNumber, distance, ra, dec, magnitude, name, spectral_type);
          stars.Add(star);
          starDictionary[name] = star;
      }
    }

    return stars;
  }

  public Star FindStarByName(string name) {
    if (starDictionary.TryGetValue(name, out Star star)) {
      return star;
    }
    return null;
  }

}

