using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS_ByteArray
{
    class ByteArray
    {
        public static byte[] CombomByteArray(byte[] srcArray1, byte[] srcArray2)
        {
            //根据要合并的两个数组元素总数新建一个数组
            byte[] newArray = new byte[srcArray1.Length + srcArray2.Length];

            //把第一个数组复制到新建数组
            Array.Copy(srcArray1, 0, newArray, 0, srcArray1.Length);

            //把第二个数组复制到新建数组
            Array.Copy(srcArray2, 0, newArray, srcArray1.Length, srcArray2.Length);

            return newArray;
        }

        public static byte[] CopyTo(byte[] src, int startIndex, int length)
        {
            byte[] newArray = new byte[length];
            Array.Copy(src, startIndex, newArray, 0, length);
            return newArray;
        }

        private byte[] bytes = null;
        private int rear;
        private int front;
        public ByteArray(int length)
        {
            bytes = new byte[length];
            front = 0;
            rear = 0;
        }

        public bool isFull()
        {
            return ((rear + 1) % bytes.Length == front);
        }

        public bool isEmpty()
        {
            return (rear == front);
        }

        public bool push(byte b)
        {
            if (isFull())
            {
                return false;
            }

            bytes[rear] = b;
            rear = (rear + 1) % bytes.Length;
            return true;
        }

        public bool pop(out byte b)
        {
            b = bytes[front];
            if (isEmpty())
            {
                return false;
            }           
            front = (front + 1) % bytes.Length;
            return true;
        }

        public void clear()
        {
            front = 0;
            rear = 0;
        }

        public int getDataLength()
        {
            return (rear - front);
        }
    }
}
