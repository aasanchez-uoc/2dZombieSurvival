
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Clase encargada de generar un array con los datos del mapa con los parámetros que le indiquemos
/// </summary>
public class MapGenerator
{
    public int[,] Level;
    public List<Tuple<int, int, int, int>> room_list;

    private int width;
    private int height;
    private int max_rooms;
    private int min_room_xy;
    private int max_room_xy;
    private bool rooms_overlap;
    private int random_connections;
    private int random_spurs;

    private List<List<Vector2Int>> corridor_list;
    private System.Random random;


    public MapGenerator (int width= 64, int height= 64, int max_rooms = 15, int min_room_xy= 5,
                int max_room_xy = 10, bool rooms_overlap= false, int random_connections= 1,
                 int random_spurs= 3, int seed = 0)
    {
        this.width = width;
        this.height = height;
        this.max_rooms = max_rooms;
        this.min_room_xy = min_room_xy;
        this.max_room_xy = max_room_xy;
        this.rooms_overlap = rooms_overlap;
        this.random_connections = random_connections;
        this.random_spurs = random_spurs;
        Level = new int[width, height];
        room_list = new List<Tuple<int, int, int, int>>();
        corridor_list = new List<List<Vector2Int>>();
        //this.tiles_level = []
        if (seed != 0) random = new System.Random(seed);
        else random = new System.Random();
 
    }



    public void GenerateLevel()
    {
        //empezamos con un mapa vacío, y las listas de pasillos y habitaciones también vacíon
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                Level[j, i] = 0;
            }
        }
        room_list = new List<Tuple<int, int, int, int>>();
        corridor_list = new List<List<Vector2Int>>();

        int max_iters = max_rooms * 5;

        for (int i = 0; i < max_iters; i++)
        {
            Tuple<int, int, int, int>  tmp_room = generateRoom();
            if (rooms_overlap || room_list.Count == 0)
                room_list.Add(tmp_room);
            else
            {
                tmp_room = generateRoom();
                List<Tuple<int, int, int, int>>  tmp_room_list =  new List<Tuple<int, int, int, int>> (room_list);

                if (!isRoomOverlaping(tmp_room, tmp_room_list))
                    room_list.Add(tmp_room);

            }
            if ( room_list.Count >= max_rooms)
                break;
        }

        //conectamos las habitaciones
        for(int a = 0; a < room_list.Count - 1; a++)
        {
            joinRooms(room_list[a], room_list[a + 1]);
        }

        //hacemos conexiones aleatorias
        for(int a = 0; a < random_connections; a++)
        {
            Tuple<int, int, int, int> room1 = room_list[random.Next(0, room_list.Count - 1)];
            Tuple<int, int, int, int>  room2 = room_list[random.Next(0, room_list.Count - 1)];
            joinRooms(room1, room2);
        }


        //do the spurs
        for(int a = 0; a < random_spurs; a++)
        {
            Tuple<int, int, int, int> room1 = new Tuple<int, int, int, int>(random.Next(2, width - 2), random.Next(2, height - 2), 1, 1);
            Tuple<int, int, int, int> room2 = room_list[random.Next(0, room_list.Count - 1)];
            joinRooms(room1, room2);
        }

        //rellenamos el mapa
        //y pintamos las habitaciones
        foreach (Tuple<int, int, int, int> room in room_list)
        {
            for(int b = 0; b < room.Item3; b++)
            {
                for(int c = 0; c < room.Item4; c++)
                {
                    Level[room.Item2 + c, room.Item1 + b] = 1; // suelo
                }
            }
        }

        //pintamos los pasillos
        foreach(List<Vector2Int> corridor in corridor_list)
        {
            int x1 = corridor[0].x;
            int y1 = corridor[0].y;
            int x2 = corridor[1].x;
            int y2 = corridor[1].y;
            for(int w = 0; w < Mathf.Abs(x1 - x2) + 1; w++)
            {
                for (int h = 0; h < Mathf.Abs(y1 - y2) + 1; h++)
                {
                    Level[Mathf.Min(y1, y2) + h, Mathf.Min(x1, x2) + w] = 1; //suelo
                }
            }
            if (corridor.Count == 3)
            {
                int x3 = corridor[2].x;
                int y3 = corridor[2].y;
                for (int w = 0; w < Mathf.Abs(x2 - x3) + 1; w++)
                {
                    for (int h=0; h < Mathf.Abs(y2 - y3) + 1; h++)
                    {
                        Level[Mathf.Min(y2, y3) + h, Mathf.Min(x2, x3) + w] = 1; //suelo
                    }                    
                }
            }
        }

        //pintamos las paredes
        for (int row = 1; row < height - 1; row++)
        {
            for(int col = 1; col < width - 1; col++)
            {
                if(Level[row, col] == 1) //suelo
                {
                    if (Level[row - 1, col - 1] == 0) //piedra
                    {
                        Level[row - 1, col - 1] = 2; //pared
                    }
                    if (Level[row - 1, col] == 0) //piedra
                    {
                        Level[row - 1, col] = 2; //pared
                    }
                    if (Level[row - 1, col + 1] == 0) //piedra
                    {
                        Level[row - 1, col + 1] = 2; //pared
                    }
                    if (Level[row, col - 1] == 0) //piedra
                    {
                        Level[row, col - 1] = 2; //pared
                    }
                    if (Level[row, col + 1] == 0) //piedra
                    {
                        Level[row, col + 1] = 2; //pared
                    }
                    if (Level[row + 1, col - 1] == 0) //piedra
                    {
                        Level[row + 1, col - 1] = 2; //pared
                    }
                    if (Level[row + 1, col] == 0) //piedra
                    {
                        Level[row + 1, col] = 2; //pared
                    }
                    if (Level[row + 1, col + 1] == 0) //piedra
                    {
                        Level[row + 1, col + 1] = 2; //pared
                    }
                }
            }
        }

    }

    private void joinRooms(Tuple<int, int, int, int> room1, Tuple<int, int, int, int> room2)
    {
        //ordenamos las dos habitaciones por x
        Tuple<int, int, int, int> firstRoom = (room1.Item1 <= room2.Item1) ? room1 : room2;
        Tuple<int, int, int, int> secondRoom = (room1.Item1 <= room2.Item1) ? room2 : room1;
        int x1 = firstRoom.Item1;
        int y1 = firstRoom.Item2;
        int w1 = firstRoom.Item3;
        int h1 = firstRoom.Item4;
        int x1_2 = x1 + w1 - 1;
        int y1_2 = y1 + h1 - 1;

        int x2 = secondRoom.Item1;
        int y2 = secondRoom.Item2;
        int w2 = secondRoom.Item3;
        int h2 = secondRoom.Item4;
        int x2_2 = x2 + w2 - 1;
        int y2_2 = y2 + h2 - 1;

        //se superponen en x
        if (x1 < (x2 + w2) && x2 < (x1 + w1))
        {
            int jx1 = random.Next(x2, x1_2);
            int jx2 = jx1;
            int[] tmp_y = {y1, y2, y1_2, y2_2};
            Array.Sort(tmp_y);
            int jy1 = tmp_y[1] + 1;
            int jy2 = tmp_y[2] - 1;
            List<Vector2Int> corridors = corridor_between_points(jx1, jy1, jx2, jy2);
            corridor_list.Add(corridors);
        }
        //se superponen en y
        else if (y1 < (y2 + h2) && y2< (y1 +h1))
        {
            int jy1, jy2;
            if( y2 > y1)
            {
                jy1 = random.Next(y2, y1_2);
                jy2 = jy1;
            }
            else
            {
                jy1 = random.Next(y1, y2_2);
                jy2 = jy1;
            }
            int[] tmp_x = { x1, x2, x1_2, x2_2 };
            Array.Sort(tmp_x);
            int jx1 = tmp_x[1] + 1;
            int jx2 = tmp_x[2] - 1;

            List<Vector2Int> corridors = corridor_between_points(jx1, jy1, jx2, jy2);
            corridor_list.Add(corridors);
        }
        else //no se superponen
        {
            int joinDir = random.Next(0, 2);
            if( joinDir == 1) //unimos hacia arriba
            {
                if (y2 > y1)
                {
                    int jx1 = x1_2 + 1;
                    int jy1 = random.Next(y1, y1_2);
                    int jx2 = random.Next(x2, x2_2);
                    int jy2 = y2 - 1;
                    List<Vector2Int> corridors = corridor_between_points(jx1, jy1, jx2, jy2, 0);
                    corridor_list.Add(corridors);
                }
                else
                {
                    int jx1 = random.Next(x1, x1_2);
                    int jy1 = y1 - 1;
                    int jx2 = x2 - 1;
                    int jy2 = random.Next(y2, y2_2);
                    List<Vector2Int> corridors = corridor_between_points(jx1, jy1, jx2, jy2, 1);
                    corridor_list.Add(corridors);
                }

            }
            else // unimos hacia abajo
            {
                if (y2 > y1)
                {
                    int jx1 = random.Next(x1, x1_2);
                    int jy1 = y1_2 + 1;
                    int jx2 = x2 - 1;
                    int jy2 = random.Next(y2, y2_2);
                    List<Vector2Int> corridors = corridor_between_points(
                        jx1, jy1, jx2, jy2, 1);
                    corridor_list.Add(corridors);
                }

                else
                {
                    int jx1 = x1_2 + 1;
                    int jy1 = random.Next(y1, y1_2);
                    int jx2 = random.Next(x2, x2_2);
                    int jy2 = y2_2 + 1;
                    List<Vector2Int> corridors = corridor_between_points(
                        jx1, jy1, jx2, jy2, 0);
                    corridor_list.Add(corridors);
                }
            }
        }
    }

    private List<Vector2Int> corridor_between_points(int x1, int y1, int x2, int y2, int joinType = 3)
    {
        if (x1 == x2 && y1 == y2 || x1 == x2 || y1 == y2)
        {
            return new List<Vector2Int> { new Vector2Int(x1, y1), new Vector2Int(x2, y2) };
        }

        else
        {
            int join;
            if (joinType == 3 && (new int[] { 0, 1 }).Intersect(new int[] { x1, x2, y1, y2 }).Any())
            {
                join = 0;
            }
            else if (joinType == 3 && (new int[] { width - 1, width - 2 }).Intersect(new int[]{x1, x2}).Any() ||
                (new int[] { height - 1, height - 2 }).Intersect(new int[] { y1, y2 }).Any())
            {
                join = 1;
            }
            else if (joinType == 3)
            {
                join = random.Next(0, 2);
            }
            else
            {
                join = joinType;
            }
            if (join == 1)
                return new List<Vector2Int> { new Vector2Int(x1, y1),new Vector2Int(x1, y2) , new Vector2Int(x2, y2) };
            else
                return new List<Vector2Int> { new Vector2Int(x1, y1), new Vector2Int(x2, y1), new Vector2Int(x2, y2) };
        }

    }

    private bool isRoomOverlaping(Tuple<int, int, int, int> room, List<Tuple<int, int, int, int>> room_list)
    {
        int x = room.Item1;
        int y = room.Item2;
        int w = room.Item3;
        int h = room.Item4;

        foreach(Tuple<int, int, int, int> current_room in room_list)
        {
            if (x < (current_room.Item1 + current_room.Item3) &&
                current_room.Item1 < (x + w) &&
                y < (current_room.Item2 + current_room.Item4) &&
                current_room.Item2 < (y + h))
            {
                return true;
            } 

        }
        return false;
    }

    private Tuple<int, int, int, int> generateRoom()
    {
        int x, y, w, h;
        
        w = random.Next(min_room_xy, max_room_xy);
        h = random.Next(min_room_xy, max_room_xy);
        x = random.Next(1, (width - w - 1));
        y = random.Next(1, (height - h - 1));

        return new Tuple<int, int, int, int>(x, y, w, h);
    }
}
