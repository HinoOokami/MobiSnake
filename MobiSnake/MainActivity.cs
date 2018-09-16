using System.Collections.Generic;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Widget;
using Android.OS;
using Android.Views;
using Java.Lang;
using MobiSnake.Resources.layout;
using static Android.Views.GestureDetector;
using System;
using Math = System.Math;

namespace MobiSnake
{
    [Activity(Label = "MobiSnake", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, IOnGestureListener
    {
        GameArea gameArea;

        static Timer timer = new Timer();

        GestureDetector _gestureDetector;
        const int SWIPE_DISTANCE_THRESHOLD = 100;
        const int SWIPE_VELOCITY_THRESHOLD = 100;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            gameArea = new GameArea(this);
            //SetContentView(Resource.Layout.Main);
            //SetContentView(new GameArea(this));
            SetContentView(gameArea);

            _gestureDetector = new GestureDetector(this);

            timer.Interval = 200;
            timer.Elapsed += Draw;

            timer.Start();

            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetActionBar(toolbar);
            //ActionBar.Title = "MobiSnake";
        }

        public void Draw(object sender, ElapsedEventArgs e)
        {
            CheckState();
            RunOnUiThread(() => gameArea.Draw());
        }

        void CheckState()
        {
            if (gameArea.IsOffScreen(gameArea.snakeHead)) Lose();
            if (gameArea.snakeBody.Count == gameArea.snakeMaxLength)
            {
                gameArea.foodTimerCount = 0;
                Win();
            }

            if (gameArea.snakeBody.Count > 4)
            {
                for (int i = gameArea.snakeNeck - 4;;)
                {
                    if (i < 0) i = 100 - i;
                    if (gameArea.IsCrossed(gameArea.snakeHead, gameArea.snakeBody[i]))
                    {
                        Lose();
                        break;
                    }

                    if (i-- == gameArea.snakeTail) break;
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_about:
                    Toast.MakeText(this, ActionBar.TitleFormatted + " is a mobile snake game!", 
                                   ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            float distanceX = e2.GetX() - e1.GetX();
            float distanceY = e2.GetY() - e1.GetY();

            if (Math.Abs(distanceX) > SWIPE_DISTANCE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD
                || Math.Abs(distanceY) > SWIPE_DISTANCE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
            {
                if (Math.Abs(velocityX) > Math.Abs(velocityY) && Math.Abs(distanceX) > Math.Abs(distanceY))
                {
                    OnSwipeLeftRight(distanceX);
                    return true;
                }


                if (Math.Abs(velocityY) > Math.Abs(velocityX) && Math.Abs(distanceY) > Math.Abs(distanceX))
                {
                    OnSwipeUpDown(distanceY);
                    return true;
                }
            }

            return false;
        }

        internal void Lose()
        {
            gameArea.isWorking = true;
            timer.Stop();
            RunOnUiThread(() =>  Toast.MakeText(this, "You lose!",
                ToastLength.Long).Show());
        }

        internal void Win()
        {
            gameArea.isWorking = true;
            timer.Stop();
            RunOnUiThread(() => Toast.MakeText(this, "You win!",
                ToastLength.Long).Show());
        }

        void OnSwipeUpDown(float distanceY)
        {
            if (distanceY < 0) gameArea.MoveUp();
            else gameArea.MoveDown();
        }

        void OnSwipeLeftRight(float distanceX)
        {
            if (distanceX < 0) gameArea.MoveLeft();
            else gameArea.MoveRight();
        }

        public void OnLongPress(MotionEvent e) {}
        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (Math.Abs(distanceX) > SWIPE_DISTANCE_THRESHOLD || Math.Abs(distanceY) > SWIPE_DISTANCE_THRESHOLD)
            {
                if (Math.Abs(distanceX) > Math.Abs(distanceY))
                {
                    OnSwipeLeftRight(distanceX);
                    return true;
                }


                if (Math.Abs(distanceY) > Math.Abs(distanceX))
                {
                    OnSwipeUpDown(distanceY);
                    return true;
                }
            }

            return false;
        }
        public void OnShowPress(MotionEvent e) {}
        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            _gestureDetector.OnTouchEvent(e);
            return false;
        }
    }
}