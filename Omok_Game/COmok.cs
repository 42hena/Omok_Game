using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok_Game
{
    internal class CPoint
    {
        public CPoint(int x, int y, char color) 
        {
            X = x;
            Y = y;
            Color = color;
        }
    
        public int X, Y;
        public char Color;
    }

    internal class COmok
    {
        Graphics g;

        int boardMargin = 20;//60
        int boardSize = 15;
        int boardInterval = 10;//50
        int stoneSize = 48;
        int dotSize = 8;
        bool gameOver = false;
        char playerType = 'C';
        char firstPlayer = 'H';

        // 착수 정보 List
        //List<Point> mainList = new List<Point>();
        List<CPoint> mainList = new List<CPoint>();

        // 오목판 배열
        char[,] mainBoard;

        public COmok(Graphics graphics, int boardSize, char playerType, char firstPlayer)
        {
            this.g = graphics;
            this.boardSize = boardSize;
            this.playerType = playerType;
            this.firstPlayer = firstPlayer;

            mainBoard = new char[this.boardSize, this.boardSize];

            this.boardInterval = 30;
            this.stoneSize = 14;
        }

        public void DrawBoard(Graphics g)
        {
            System.Diagnostics.Debug.WriteLine("DrawBoardqwerqwer");


            Pen pen = new Pen(Color.Black);
            // Point point; ???
            char[] alphabet = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            };

            for (int i = 0; i  < boardSize - 1; ++i)
            {
                for (int j = 0; j < boardSize - 1; ++j)
                {
                    Rectangle rec = new Rectangle(
                        (i * boardInterval) + boardMargin,
                        (j * boardInterval) + boardMargin, 
                        boardInterval, boardInterval);
                    g.DrawRectangle(pen, rec);
                }
            }

            if (boardSize == 15)
            {
                DrawStone(g, 4, 4);
                DrawStone(g, 12, 4);
                DrawStone(g, 4, 12);
                DrawStone(g, 8, 8);
                DrawStone(g, 12, 12);
            }

            for (int i = 1; i <= boardSize; ++i)
            {
                g.DrawString(i.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black),
                    0, (boardInterval * i) - 18);// 3
                g.DrawString(alphabet[i - 1].ToString(), new Font("Arial", 10), new SolidBrush(Color.Black),
                    (boardInterval * i) - 17, 0);// 5
            }
        }

        private void DrawStone(Graphics g, int x, int y)
        {
            Point point = GetScreentPosition(new Point(x, y));

            point.X = (x - 1) * boardInterval + boardMargin - (dotSize / 2);
            point.Y = (y - 1) * boardInterval + boardMargin - (dotSize / 2);

            g.FillEllipse(Brushes.Black, new Rectangle(point, new Size(dotSize, dotSize)));
            g.DrawEllipse(Pens.Black, new Rectangle(point, new Size(dotSize, dotSize)));
        }


        public Point GetBoardPosition(int screenX, int screenY)
        {
            //System.Diagnostics.Debug.WriteLine($"sx:{screenX} sy:{screenY} boardMargin:{boardMargin} boardInterval:{boardInterval}");
            return new Point((int)((screenX - boardMargin + boardInterval / 2) / boardInterval),
              (int)((screenY - boardMargin + boardInterval / 2) / boardInterval));
        }

        private Point GetScreentPosition(Point point)
        {
            return new Point((point.X) * boardInterval + boardMargin - (stoneSize / 2),
                (point.Y) * boardInterval + boardMargin - (stoneSize / 2));
        }

        

        public void PutStone(int x, int y)
        {
            char nextColor = 'B';

            //
            if (mainList.Count > 0)
            {
                nextColor = mainList[mainList.Count - 1].Color == 'B' ? 'W' : 'B';
            }

            mainList.Add(new CPoint(x, y, nextColor));

            foreach (CPoint p in mainList)
            {
                mainBoard[p.Y, p.X] = p.Color;
            }    
        }

        enum DrawType
        {
            OmokStone = 1,
            BoardPoint = 2,
        };

        public void ExecuteCode()
        {
            
        }
        private void DrawStone(Graphics g, int x, int y, char color, int type)
        {
            System.Diagnostics.Debug.WriteLine("Stone_2\n");
            Point point = GetScreentPosition(new Point(x, y));

            //string result = ExecuteCode();
            

            if (type == (int)DrawType.OmokStone)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (color == 'B')
                {
                    g.FillEllipse(Brushes.Black, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    g.DrawEllipse(Pens.Black, new Rectangle(point, new Size(stoneSize, stoneSize)));

                }
                else
                {
                    g.FillEllipse(Brushes.White, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    g.DrawEllipse(Pens.DarkGray, new Rectangle(point, new Size(stoneSize, stoneSize)));
                }
            }
            else if (type == (int)DrawType.BoardPoint)
            {
                point.X = x * boardInterval + boardMargin - (dotSize / 2);
                point.Y = y * boardInterval + boardMargin - (dotSize / 2);
                g.FillEllipse(Brushes.Black, new Rectangle(point, new Size(dotSize, dotSize)));
                g.DrawEllipse(Pens.Black, new Rectangle(point, new Size(dotSize, dotSize)));
            }
        }

        private void DrawStone(Graphics g, int x, int y, char color, int order, bool orderFlag, int type)
        {
            Point point = GetScreentPosition(new Point(x, y));

            if (type == (int)DrawType.OmokStone)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (color == 'B')
                {
                    g.FillEllipse(Brushes.Black, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    g.DrawEllipse(Pens.Black, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    if (orderFlag)
                        g.DrawString(order.ToString(), new Font("Arial", (boardSize == 15 ? 15 : 12)),
                            new SolidBrush(Color.White), new Rectangle(point, new Size(stoneSize, stoneSize)), sf);
                }
                else
                {
                    g.FillEllipse(Brushes.White, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    g.DrawEllipse(Pens.DarkGray, new Rectangle(point, new Size(stoneSize, stoneSize)));
                    if (orderFlag)
                        g.DrawString(order.ToString(), new Font("Arial", (boardSize == 15 ? 15 : 12)),
                            new SolidBrush(Color.Black), new Rectangle(point, new Size(stoneSize, stoneSize)), sf);
                }

                if (order ==mainList.Count)
                {
                    Pen pen = new Pen(Color.Red);
                    Rectangle rec = new Rectangle((x * boardInterval) - (boardSize == 15 ? 15 : -2),
                        (y * boardInterval) - (boardSize == 15 ? 15 : -2), boardInterval, boardInterval);
                    g.DrawRectangle(pen, rec);
                }
            }
            else if (type == (int)DrawType.BoardPoint)
            {
                point.X = (x - 1) * boardInterval + boardMargin - (dotSize / 2);
                point.Y = (y - 1) * boardInterval + boardMargin - (dotSize / 2);
                g.FillEllipse(Brushes.Black, new Rectangle(point, new Size(dotSize, dotSize)));
                g.DrawEllipse(Pens.Black, new Rectangle(point, new Size(dotSize, dotSize)));
            }
        }


        public void DrawStoneAll(Graphics g)
        {
            foreach (var item in mainList.Select((value, index) => (value, index)))
            {
                DrawStone(g, item.value.X, item.value.Y, item.value.Color, (int)DrawType.OmokStone);
            }
        }

        public bool isGameOver()
        {
            return gameOver;
        }

        public int GetBoardSize()
        {
            return boardSize;
        }

        public bool IsOccupied(int x, int y)
        {
            foreach (var item in mainList)
            {
                if (item.X == x && item.Y == y)
                    return true;
            }
            return false;
        }

        public void UnDo()
        {
            Point point;

            if (mainList.Count == 1)
            {
                mainList.RemoveAt(mainList.Count - 1);

                for (int i = 0; i < boardSize; ++i)
                {
                    for (int j = 0; j < boardSize; ++j)
                    {
                        if (i == 0 || j == 0)
                            mainBoard[i, j] = 'X';
                        else
                            mainBoard[i, j] = 'E';
                    }
                }
            }
            else if (mainList.Count > 1)
            {
                mainList.RemoveAt(mainList.Count - 1);
                point = new Point(mainList[mainList.Count - 1].X, mainList[mainList.Count - 1].Y);
                mainList.RemoveAt(mainList.Count - 1);
                this.PutStone(this.GetScreentPosition(point).X, this.GetScreentPosition(point).Y);
            }
        }
    }
}
