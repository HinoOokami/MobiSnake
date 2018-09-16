using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobiSnake.Resources.layout
{
    [Activity(Label = "MobiSnake")]
    public class GameArea : View
    {
        ShapeDrawable food;
        internal ShapeDrawable snakeHead;
        internal Dictionary<int, ShapeDrawable> snakeBody;
        int size;
        static Random rnd;
        int speed;
        int shiftX;
        int shiftY;
        public int foodTimerCount;
        internal int snakeMaxLength;
        int snakePartsCounter;
        int snakeTail;

        Canvas gameArea;

        bool isStarted;
        internal bool isWorking;

        public GameArea(Context context) : base(context)
        {
            Init();
        }

        void Init()
        {
            size = 50;
            rnd = new Random();
            //speed = 8;
            speed = 16;
            snakeMaxLength = 10;
            foodTimerCount = 99;
            var foodPaint = new Paint();
            foodPaint.SetARGB(255, 255, 0, 0);
            foodPaint.SetStyle(Paint.Style.Fill);
            
            food = new ShapeDrawable(new OvalShape());
            food.Paint.Set(foodPaint);

            var snakePaint = new Paint();
            snakePaint.SetARGB(255, 0, 0, 255);
            snakePaint.SetStyle(Paint.Style.Fill);
            
            snakeHead = new ShapeDrawable(new OvalShape());
            snakeHead.Paint.Set(snakePaint);

            snakeBody = new Dictionary<int, ShapeDrawable>();
        }

        protected override void OnDraw(Canvas canvas)
        {
            isWorking = true;
            gameArea = canvas;
            
            if (isStarted)
            {
                PlaceSnake();
                PlaceFood();
            }
            else InitialPlaceSnake();
            isWorking = false;
        }

        public void Draw()
        {
            if (isWorking) return;
            Invalidate();
        }

        void PlaceFood()
        {
            if (++foodTimerCount == 100)
            {
                int xLeft = rnd.Next(gameArea.Width - size);
                int xRight = xLeft + size;
                int yTop = rnd.Next(gameArea.Height - size);
                int yBottom = yTop + size;
                food.SetBounds(xLeft, yTop, xRight, yBottom);
                foodTimerCount = 0;
            }

            food.Draw(gameArea);
        }

        void PlaceSnake()
        {
            var bounds = snakeHead.Bounds;
            bounds.Left += shiftX;
            bounds.Right += shiftX;
            bounds.Top += shiftY;
            bounds.Bottom += shiftY;
            snakeHead.SetBounds(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
            //snakeBody.Add(snakePartsCounter++, snakeHead);
            var s = new ShapeDrawable(new OvalShape());
            s.Paint.Set(snakeHead.Paint);
            s.Bounds = snakeHead.Bounds;
            snakeBody.Add(snakePartsCounter++, s);

            if (snakePartsCounter == 10) snakePartsCounter = 0;
            
            if (IsCrossed(snakeHead, food))
            {
                if (snakeBody.Count == snakeMaxLength) foodTimerCount = 0;
                foodTimerCount = 99;
                PlaceFood();
            }
            else
            {
                snakeBody.Remove(snakeTail++);
                if (snakeTail == 10) snakeTail = 0;
            }

            DrawSnake(snakeBody);
        }

        void InitialPlaceSnake()
        {
            int xLeft = gameArea.Width / 2 - size / 2;
            int xRight = xLeft + size;
            int yTop = gameArea.Height / 2 - size / 2;
            int yBottom = yTop + size;
            snakeHead.SetBounds(xLeft, yTop, xRight, yBottom);
            snakeBody.Add(snakePartsCounter, snakeHead);
            snakeTail = snakePartsCounter++;
            DrawSnake(snakeBody);
            isStarted = true;
        }

        void DrawSnake(Dictionary<int, ShapeDrawable> snake)
        {
            foreach (var s in snake.Values)
                s.Draw(gameArea);
            //TestPlaceSnake();
        }

        void TestPlaceSnake()
        {
            int xLeft = gameArea.Width / 2 - size / 2;
            int xRight = xLeft + size;
            int yTop = gameArea.Height / 2 - size / 2;
            int yBottom = yTop + size;

            var sPaint = new Paint();
            sPaint.SetARGB(0, 255, 0, 255);
            sPaint.SetStyle(Paint.Style.Fill);
            var s = new ShapeDrawable(new OvalShape());
            s.Paint.Set(sPaint);
            s.SetBounds(xLeft, yTop, xRight, yBottom);
            s.Draw(gameArea);
        }

        public void MoveLeft()
        {
            shiftX = -speed;
            shiftY = 0;
        }

        public void MoveRight()
        {
            shiftX = speed;
            shiftY = 0;
        }

        public void MoveUp()
        {
            shiftY = -speed;
            shiftX = 0;
        }

        public void MoveDown()
        {
            shiftY = speed;
            shiftX = 0;
        }

        bool IsCrossed(ShapeDrawable s, ShapeDrawable f)
        {
            return Math.Abs(s.Bounds.Left - f.Bounds.Left) < size &&
                   Math.Abs(s.Bounds.Top - f.Bounds.Top) < size;
        }

        internal bool IsOffScreen(ShapeDrawable s)
        {
            return s.Bounds.Left < 0 || s.Bounds.Right > gameArea.Width ||
                   s.Bounds.Top < 0 || s.Bounds.Bottom > gameArea.Height;
        }
    }
}