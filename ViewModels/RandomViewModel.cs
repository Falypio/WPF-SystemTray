using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1.ViewModels
{
    public class RandomViewModel : BindableBase
    {

        private int redMax = 30;//33

        private int blueMax = 16;

        private Random random = new Random();

        private List<int> randoms = new List<int>();

        private bool isBegin  = true;
        /// <summary>
        /// 标识是否开始
        /// </summary>
        public bool IsBegin
        {
            get { return isBegin; }
            set { SetProperty(ref isBegin, value); }
        }

        private ObservableCollection<RandomDuotone> randomDuotones;

        /// <summary>
        /// 数组列
        /// </summary>
        public ObservableCollection<RandomDuotone> RandomDuotones
        {
            get { return randomDuotones; }
            set { SetProperty(ref randomDuotones, value); }
        }

        /// <summary>
        /// 数量
        /// </summary>
        private int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }


        private ICommand startCommand;
        public ICommand StartCommand => startCommand ??= new Prism.Commands.DelegateCommand(ExecuteStartCommand);
        public RandomViewModel()
        {
            RandomDuotones = new ObservableCollection<RandomDuotone>();
        }


        private async void ExecuteStartCommand()
        {
            IsBegin = false;
            RandomDuotones.Clear();
            for (int i = 0; i < Quantity; i++)
            {
                RandomDuotones.Add(await SetDuotone());
            }
            IsBegin = true;
        }

        private async Task<RandomDuotone> SetDuotone()
        {
            RandomHelper randomHelper = new RandomHelper();
            RandomDuotone duotone = new RandomDuotone();
            duotone.Red1 = await randomHelper.GetNum(1, 11);
            duotone.Red2 = await randomHelper.GetNum(duotone.Red1 + 1, 15);
            duotone.Red3 = await randomHelper.GetNum(duotone.Red2 + 1, 20);
            duotone.Red4 = await randomHelper.GetNum(duotone.Red3 + 1, 25);
            duotone.Red5 = await randomHelper.GetNum(duotone.Red4 + 1, 30);
            duotone.Red6 = await randomHelper.GetNum(duotone.Red5 + 1, 33);
            duotone.Blue1 = await randomHelper.GetNum(1, 16);
            return duotone;
        }

    }

    public class RandomDuotone : BindableBase
    {
        private int red1;
        public int Red1
        {
            get { return red1; }
            set { SetProperty(ref red1, value); }
        }

        private int red2;
        public int Red2
        {
            get { return red2; }
            set { SetProperty(ref red2, value); }
        }

        private int red3;
        public int Red3
        {
            get { return red3; }
            set { SetProperty(ref red3, value); }
        }

        private int red4;
        public int Red4
        {
            get { return red4; }
            set { SetProperty(ref red4, value); }
        }

        private int red5;
        public int Red5
        {
            get { return red5; }
            set { SetProperty(ref red5, value); }
        }

        private int red6;
        public int Red6
        {
            get { return red6; }
            set { SetProperty(ref red6, value); }
        }

        private int blue1;
        public int Blue1
        {
            get { return blue1; }
            set { SetProperty(ref blue1, value); }
        }

    }

    public class RandomHelper
    {
        
        public async Task<int> GetNum(int min, int max)
        {
            await Task.Delay(500);//随机休息0.5秒
            return GetIntNum(min, max);
        }

        /// <summary>
        /// 获取随机数，解决重复问题
        /// </summary>
        /// <param name="min">返回的随机数字包含下限</param>
        /// <param name="max">返回的随机数字不包含上限</param>
        /// <returns></returns>
        private int GetIntNum(int min, int max)
        {

            Guid Guidnum = Guid.NewGuid();
            string guid = Guidnum.ToString();
            int seed = DateTime.Now.Millisecond;
            for (int i = 0; i < guid.Length; i++)
            {
                switch (guid[i])
                {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                        seed = seed + 1;
                        break;
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                        seed = seed + 2;
                        break;
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                        seed = seed + seed;
                        break;
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        seed = seed + 3;
                        break;
                    default:
                        seed = seed + 4;
                        break;
                }
            }

            return new Random(seed).Next(min, max);
        }
    }
}
