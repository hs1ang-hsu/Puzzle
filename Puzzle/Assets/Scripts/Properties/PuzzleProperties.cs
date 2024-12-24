using System.Collections.Generic;
using UnityEngine;


public class PuzzleProperty
{
    public int[,] shape;
    public int grid_num;
    public int size;
    public Vector2Int dim;
    public Color color;
}

public static class PuzzleProperties
{
    private static Dictionary<PuzzleType, PuzzleProperty> dictionary = new Dictionary<PuzzleType, PuzzleProperty>()
    {
        [PuzzleType.None] = new PuzzleProperty
        {
            shape = new int[1, 1] { { 1 } },
            size = 1,
            grid_num = 1,
            dim = new Vector2Int(1, 1),
            color = MyColorPalette.Ivory
        },
        [PuzzleType.Dot1] = new PuzzleProperty
        {
            shape = new int[1, 1] { { 1 } },
            size = 1,
            grid_num = 1,
            dim = new Vector2Int(1, 1),
            color = MyColorPalette.Ivory
        },
        [PuzzleType.V2] = new PuzzleProperty
        {
            shape = new int[2, 2] { { 1, 0 }, { 1, 0 } },
            size = 2,
            dim = new Vector2Int(2, 1),
            grid_num = 2,
            color = MyColorPalette.DarkOrange
        },
        [PuzzleType.H2] = new PuzzleProperty
        {
            shape = new int[2, 2] { { 1, 0 }, { 1, 0 } },
            size = 2,
            dim = new Vector2Int(1, 2),
            grid_num = 2,
            color = MyColorPalette.CornFlowerBlue
        },
        [PuzzleType.O4] = new PuzzleProperty
        {
            shape = new int[2, 2] { { 1, 1 }, { 1, 1 } },
            size = 2,
            dim = new Vector2Int(2, 2),
            grid_num = 4,
            color = MyColorPalette.Gold
        },
        [PuzzleType.F5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 0, 1, 1 }, { 1, 1, 0 }, { 0, 1, 0 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.Maroon
        },
        [PuzzleType.I5] = new PuzzleProperty
        {
            shape = new int[5, 5] { { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0 } },
            size = 5,
            dim = new Vector2Int(5, 1),
            grid_num = 5,
            color = MyColorPalette.OrangeRed
        },
        [PuzzleType.L5] = new PuzzleProperty
        {
            shape = new int[4, 4] { { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 1, 0, 0 } },
            size = 4,
            dim = new Vector2Int(4, 2),
            grid_num = 5,
            color = MyColorPalette.LightCoral
        },
        [PuzzleType.N5] = new PuzzleProperty
        {
            shape = new int[4, 4] { { 0, 0, 1, 1 }, { 1, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
            size = 4,
            dim = new Vector2Int(2, 4),
            grid_num = 5,
            color = MyColorPalette.GoldenRod
        },
        [PuzzleType.P5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 1, 0 }, { 1, 1, 0 }, { 1, 0, 0 } },
            size = 3,
            dim = new Vector2Int(3, 2),
            grid_num = 5,
            color = MyColorPalette.Orchid
        },
        [PuzzleType.T5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.YellowGreen
        },
        [PuzzleType.U5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 0, 1 }, { 1, 1, 1 }, { 0, 0, 0 } },
            size = 3,
            dim = new Vector2Int(2, 3),
            grid_num = 5,
            color = MyColorPalette.ForestGreen
        },
        [PuzzleType.V5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 0 }, { 1, 0, 0 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.Peru
        },
        [PuzzleType.W5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 0, 1, 1 }, { 1, 1, 0 }, { 1, 0, 0 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.Teal
        },
        [PuzzleType.X5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.DeepSkyBlue
        },
        [PuzzleType.Y5] = new PuzzleProperty
        {
            shape = new int[4, 4] { { 1, 0, 0, 0 }, { 1, 1, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 } },
            size = 4,
            dim = new Vector2Int(4, 2),
            grid_num = 5,
            color = MyColorPalette.Navy
        },
        [PuzzleType.Z5] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 1 } },
            size = 3,
            dim = new Vector2Int(3, 3),
            grid_num = 5,
            color = MyColorPalette.Indigo
        },
        [PuzzleType.O6] = new PuzzleProperty
        {
            shape = new int[3, 3] { { 1, 1, 0 }, { 1, 1, 0 }, { 1, 1, 0 } },
            size = 3,
            dim = new Vector2Int(3, 2),
            grid_num = 6,
            color = MyColorPalette.DarkViolet
        }
    };

    public static PuzzleProperty GetProperty(PuzzleType type)
    {
        return dictionary.GetValueOrDefault(type, dictionary[PuzzleType.None]);
    }
}
