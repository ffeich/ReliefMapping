using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReliefMapViewer.Models
{
    public class ReliefMap
    {
        public int Cols { get; private set; } = 0;
        public int Rows { get; private set; } = 0;
        public double XLLCorner { get; set; } = 0d;
        public double YLLCorner { get; set; } = 0d;
        public double CellSize { get; set; } = 1d;
        public float[,] Data { get; private set; }
        public bool HasData
            => Data != null && Data.Length > 0;

        public (double X, double Y) GetAbsXY(int row, int col)
        {
            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (col < 0 || col >= Cols)
                throw new ArgumentOutOfRangeException(nameof(col));

            double x = XLLCorner + col * CellSize;
            double y = YLLCorner + (Rows - row) * CellSize;

            return (x, y);
        }

        public double GetRadius(double centerX, double centerY, double edgeX, double edgeY)
        {
            double r = CircleShape.CalculateRadius(centerX, centerY, edgeX, edgeY) * CellSize;
            return r;
        }

        public static ReliefMap LoadFromAsciiFile(string path)
        {
            const string _ncols = "ncols";
            const string _nrows = "nrows";
            const string _xllcorner = "xllcorner";
            const string _yllcorner = "yllcorner";
            const string _cellsize = "cellsize";

            var map = new ReliefMap();

            string line;
            string[] vals;

            using (var reader = new StreamReader(path))
            {
                #region Header
                //ncols
                line = reader.ReadLine();
                vals = line?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 1 && vals[0] == _ncols && int.TryParse(vals[1], out int ncols))
                    map.Cols = ncols;
                else
                    throw new InvalidDataException($"Invalid header {_ncols}");

                //nrows
                line = reader.ReadLine();
                vals = line?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 1 && vals[0] == _nrows && int.TryParse(vals[1], out int nrows))
                    map.Rows = nrows;
                else
                    throw new InvalidDataException($"Invalid header {_nrows}");

                //xllcorner
                line = reader.ReadLine();
                vals = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 1 && vals[0] == _xllcorner && double.TryParse(vals[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double xllCorner))
                    map.XLLCorner = xllCorner;
                else
                    throw new InvalidDataException($"Invalid header {_xllcorner}");

                //yllcorner
                line = reader.ReadLine();
                vals = line?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 1 && vals[0] == _yllcorner && double.TryParse(vals[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double yllCorner))
                    map.YLLCorner = yllCorner;
                else
                    throw new InvalidDataException($"Invalid header {_yllcorner}");

                //cellsize
                line = reader.ReadLine();
                vals = line?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals != null && vals.Length > 1 && vals[0] == _cellsize && double.TryParse(vals[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double cellsize))
                    map.CellSize = cellsize;
                else
                    throw new InvalidDataException($"Invalid header {_cellsize}");
                #endregion

                #region Data
                //data matrix
                map.Data = new float[map.Rows, map.Cols];

                for (int row = 0; row < map.Rows; row++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        throw new InvalidDataException($"Invalid data row {row + 1} expected {map.Rows} rows");

                    vals = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length != map.Cols)
                        throw new InvalidDataException($"Invalid data row {row + 1} expected {map.Cols} cols");

                    for (int col = 0; col < map.Cols; col++)
                    {
                        if (float.TryParse(vals[col], NumberStyles.Float, CultureInfo.InvariantCulture, out float height))
                            map.Data[row, col] = height;
                        else
                            throw new InvalidDataException($"Invalid data row {row + 1} column {col + 1}");
                    }
                }
                #endregion
            }

            return map;
        }
    }
}
