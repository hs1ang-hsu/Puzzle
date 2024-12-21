using System;

[Serializable]
public enum PuzzleType
{
    None = 0,
    Polyomino = 0x00010000,
    Jigsaw = 0x00020000,

    Dot1 = Polyomino | 1,
    H2 = Polyomino | 2,
    V2 = Polyomino | 3,
    O4 = Polyomino | 4,
    F5 = Polyomino | 5,
    I5 = Polyomino | 6,
    L5 = Polyomino | 7,
    N5 = Polyomino | 8,
    P5 = Polyomino | 9,
    T5 = Polyomino | 10,
    U5 = Polyomino | 11,
    V5 = Polyomino | 12,
    W5 = Polyomino | 13,
    X5 = Polyomino | 14,
    Y5 = Polyomino | 15,
    Z5 = Polyomino | 16,
    O6 = Polyomino | 17
}
