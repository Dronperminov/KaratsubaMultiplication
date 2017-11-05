using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Karatsuba {
    class Program {
        const string fileInPath = "input_3_2.txt";
        const string fileOutPath = "out.txt";

        // обычное умножение в столбик для чисел небольшой длины
        static string MulSqr(string value, string value2) {
            if (value == "0" || value2 == "0" || value == "" || value2 == "")
                return "0"; // если один из множителей равен нулю, то результат равен нулю

            int len1 = value.Length; // запоминаем длину первого числа
            int len2 = value2.Length; // запоминаем длину второго числа
            int length = len1 + len2 + 1; // резульат влезет в сумму длин + 1 из-за возможного переноса

            // массивы аргументов и результата
            int[] a = new int[length];
            int[] b = new int[length]; 
            int[] res = new int[length + 1];

            // заполняем массивы инверсной записью чисел (с ведущими нулями)
            for (int i = 0; i < length; i++) {
                a[i] = (i < len1) ? (value[len1 - 1 - i] - '0') : 0;
                b[i] = (i < len2) ? (value2[len2 - 1 - i] - '0') : 0;
                res[i] = 0;
            }

            // выполняем умножение "в столбик""
            for (int i = 0; i < len1; i++) {
                for (int j = 0; j < len2; j++) {
                    res[length - 1 - (i + j)] += a[i] * b[j];
                    res[length - 1 - (i + j + 1)] += res[length - 1 - (i + j)] / 10;
                    res[length - 1 - (i + j)] %= 10;
                }
            }

            int index = 0;
            while (res[index] == 0 && index < length)
                index++;

            if (index == res.Length)
                return "0";

            string result = "";
            // переписываем результат в строку
            for (int i = index; i < length; i++)
                result += res[i].ToString();

            return result;
        }
    
        // операция сложения
        static string AddOp(string value, string value2) {
            string num2 = value2; // запоминаем значение второго числа

            int len1 = value.Length; // запоминаем длину первого числа
            int len2 = num2.Length; // запоминаем длину второго числа
            int length = 1 + Math.Max(len1, len2);  // длина суммы равна максимуму из двух длин + 1 из-за возможного переноса разряда
   
            int[] res = new int[length];

            res[length - 1] = 0;

            for (int i = 0; i < length - 1; i++) {
                int j = length - 1 - i;
                res[j] += ((i < len2) ? (num2[len2 - 1 - i] - '0') : 0) + ((i < len1) ? (value[len1 - 1 - i] - '0') : 0); // выполняем сложение разрядов
                res[j - 1] = res[j] / 10; // выполняем перенос в следущий разряд, если он был
                res[j] = res[j] % 10; // оставляем только единицы от возможного переноса и превращаем символ в цифру
            }
            
            string result = "";
            // переписываем результат в строку
            for (int i = res[0] == 0 ? 1 : 0; i < length; i++)
                result += res[i].ToString();

            return result;
        }

        // Операция вычитания
        static string SubOp(string value, string value2) {
            if (value == "")
                value = "0";

            if (value2 == "")
                value2 = "0";

            int len1 = value.Length; // запоминаем длину первого числа
            int len2 = value2.Length; // запоминаем длину второго числа
            int length = Math.Max(len1, len2); // длина результата не превысит максимума длин чисел

            // массивы аргументов и результата
            int[] a = new int[length];
            int[] b = new int[length];
            int[] res = new int[length + 1]; // массив для результата

            a[0] = b[0] = 0; // обнуляем нулевые элементы массивов            
            res[length - 1] = res[length] = 0;

            for (int i = 0; i < length - 1; i++) {
                a[i] += (i < len1) ? (value[len1 - 1 - i] - '0') : 0; // формируем разряды
                b[i] += (i < len2) ? (value2[len2 - 1 - i] - '0') : 0; // из строк аргументов

                b[i + 1] = 0; // занимаем 10 у
                a[i + 1] = -1; // следующего разряда

                res[length - 1 - i] += 10 - (b[i] - a[i]);
                res[length - 1 - i - 1] = res[length - 1 - i] / 10;
                res[length - 1 - i] = res[length - 1 - i] % 10;
            }

            // выполняем операцию с последним разрядом
            a[length - 1] += (length - 1 < len1) ? (value[0] - '0') : 0;
            b[length - 1] += (length - 1 < len2) ? (value2[0] - '0') : 0;
            res[0] += a[length - 1] - b[length - 1]; // записываем последний разряд

            // удаляем ведущие нули
            int index = 0;
            while (index < length && res[index] == 0)
                index++;

            if (index == length)
                return "0";

            string result = "";
            // переписываем результат в строку
            for (int i = index; i < length; i++)
                result += res[i].ToString();

            return result;
        }

        // умножение карацубы
        static string karatsuba(string value, string value2) {
            int len1 = value.Length;
            int len2 = value2.Length;

            // если длина чисел относительно мала, умножаем в столбик
            if (len1 + len2 < 100) 
                return MulSqr(value, value2);

            int len = Math.Max(len1, len2);
            len += len % 2;
            int n = len / 2;

            // разделяем оба числа на две равные части (X = Xr Xl, Y = Yr Yl)
            string Xr = len1 > n ? value.Substring(len1 - n, n) : value;
            string Xl = len1 > n ? value.Substring(0, value.Length - n) : "0";
            string Yr = len2 > n ? value2.Substring(len2 - n, n) : value2;
            string Yl = len2 > n ? value2.Substring(0, value2.Length - n) : "0";

            string P1 = karatsuba(Xl, Yl);
            string P2 = karatsuba(Xr, Yr);
            string P3 = karatsuba(AddOp(Xl, Xr), AddOp(Yl, Yr));

            string res = AddOp(AddOp(P1 + new String('0', len), SubOp(SubOp(P3, P2),  P1) + new String('0', n)), P2);

            // убираем ведущие нули
            int index = 0;
            while (index < res.Length && res[index] == '0')
                index++;

            if (index == res.Length)
                return "0";

            return res.Substring(index);
        }

        // Сумма цифр числа
        static int sumNumbers(string value) {
            int sum = 0;

            for (int i = 0; i < value.Length; i++)
                sum += (value[i] - '0');

            return sum;
        }

        static void Main(string[] args) {
            /* Пример для проверки корректного умножения обоими методами
            Console.Write("Enter a: ");
            string a = Console.ReadLine();
            Console.Write("Enter a: ");
            string b = Console.ReadLine();
            string ab = MulSqr(a, b);
            string ba = karatsuba(a, b);

            Console.WriteLine(a + " * " + b + " = " + ab);
            Console.WriteLine(a + " * " + b + " = " + ba);
            Console.ReadKey();*/

            string[] values = File.ReadAllLines(fileInPath);

            for (int i = 0; i < values.Length; i++) {
                int index = values[i].IndexOf(" ");
                if (index > -1) {
                    string a = values[i].Substring(0, index);
                    string b = values[i].Substring(index + 1);

                    Console.WriteLine(i + ") " + a.Length + ", " + b.Length);
                    string ab = karatsuba(a, b);

                    File.AppendAllText(fileOutPath, sumNumbers(ab) + "\n");
                }
            }
        }
    }
}
