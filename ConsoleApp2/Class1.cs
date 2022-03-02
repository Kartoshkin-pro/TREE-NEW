using Microsoft.Azure.Devices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class BinaryTreeNode<T> where T : IComparable
    {
        //элемент максимального значения
        public Maximum max = new Maximum();
        //конструктор узла
        public BinaryTreeNode(T data)
        {
            Data = data;
        }
        //пустой конструтор узла
        public BinaryTreeNode()
        {
        }

        //инфа
        public T Data { get; set; }
        //левая ветка
        public BinaryTreeNode<T> LeftNode { get; set; }
        //правая ветка
        public BinaryTreeNode<T> RightNode { get; set; }
        //родитель
        public BinaryTreeNode<T> ParentNode { get; set; }
        //искомый лист
        public BinaryTreeNode<T> target_leaf = null;
        //положение относительно родителя
        public Side? NodeSide =>
            ParentNode == null
            ? (Side?)null
            : ParentNode.LeftNode == this
            ? Side.Left
            : Side.Right;

        public override string ToString() => Data.ToString();

        public BinaryTreeNode<T> RootNode { get; set; }
        //создаём дерево
        //начиная сверху
        //если элемент юольше корня(родителя) идёт вправо ?? влево
        public BinaryTreeNode<T> Add(BinaryTreeNode<T> node, BinaryTreeNode<T> currentNode = null)
        {
            if (RootNode == null)
            {
                node.ParentNode = null;
                return RootNode = node;
            }

            currentNode = currentNode ?? RootNode;
            node.ParentNode = currentNode;
            int result;
            return (result = node.Data.CompareTo(currentNode.Data)) == 0
                ? currentNode
                : result < 0
                  ? currentNode.LeftNode == null
                   ? (currentNode.LeftNode = node)
                : Add(node, currentNode.LeftNode)
            : currentNode.RightNode == null
                ? (currentNode.RightNode = node)
                : Add(node, currentNode.RightNode);
        }
        //ищем лист начиная с корня(поиск в глубину если мне не изменяет память)
        //
        public BinaryTreeNode<T> FindNode(T data, BinaryTreeNode<T> startWithNode = null)
        {
            startWithNode = startWithNode ?? RootNode;
            int result;
            return (result = data.CompareTo(startWithNode.Data)) == 0
                ? startWithNode
                : result < 0
                    ? startWithNode.LeftNode == null
                        ? null
                        : FindNode(data, startWithNode.LeftNode)
                    : startWithNode.RightNode == null
                        ? null
                        : FindNode(data, startWithNode.RightNode);
        }

        public void Remove(BinaryTreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }

            var currentNodeSide = node.NodeSide;
            //если у узла нет подузлов, можно его удалить
            if (node.LeftNode == null && node.RightNode == null)
            {
                if (currentNodeSide == Side.Left)
                {
                    node.ParentNode.LeftNode = null;
                }
                else
                {
                    node.ParentNode.RightNode = null;
                }
            }
            //если нет левого, то правый ставим на место удаляемого 
            else if (node.LeftNode == null)
            {
                if (currentNodeSide == Side.Left)
                {
                    node.ParentNode.LeftNode = node.RightNode;
                }
                else
                {
                    node.ParentNode.RightNode = node.RightNode;
                }

                node.RightNode.ParentNode = node.ParentNode;
            }
            //если нет правого, то левый ставим на место удаляемого 
            else if (node.RightNode == null)
            {
                if (currentNodeSide == Side.Left)
                {
                    node.ParentNode.LeftNode = node.LeftNode;
                }
                else
                {
                    node.ParentNode.RightNode = node.LeftNode;
                }

                node.LeftNode.ParentNode = node.ParentNode;
            }
            //если оба дочерних присутствуют, 
            //то правый становится на место удаляемого,
            //а левый вставляется в правый
            else
            {
                switch (currentNodeSide)
                {
                    case Side.Left:
                        node.ParentNode.LeftNode = node.RightNode;
                        node.RightNode.ParentNode = node.ParentNode;
                        Add(node.LeftNode, node.RightNode);
                        break;
                    case Side.Right:
                        node.ParentNode.RightNode = node.RightNode;
                        node.RightNode.ParentNode = node.ParentNode;
                        Add(node.LeftNode, node.RightNode);
                        break;
                    default:
                        var bufLeft = node.LeftNode;
                        var bufRightLeft = node.RightNode.LeftNode;
                        var bufRightRight = node.RightNode.RightNode;
                        node.Data = node.RightNode.Data;
                        node.RightNode = bufRightRight;
                        node.LeftNode = bufRightLeft;
                        Add(bufLeft, node);
                        break;
                }
            }
        }
        //метод добавления
        public BinaryTreeNode<T> Add(T data)
        {
            return Add(new BinaryTreeNode<T>(data));
        }
        //метод удаления искомого листа
        public void Remove(T data)
        {
            var foundNode = FindNode(data);
            Remove(foundNode);
        }

        public class Maximum
        {
            //устанавливаем мин значение
            public int max_no = int.MinValue;
        }
       
        public virtual void getTargetLeaf(BinaryTreeNode<T> node, Maximum max_sum_ref, int curr_sum)
        {
            //проверка на ноль
            if (node == null)
            {
                return;
            }

            // Обновляем сумму на пути к нашему листу
            curr_sum = curr_sum + Convert.ToInt32(node.Data);

            // если мы дошли до конца и сумма больше суммы хранящейся, делаем свап
            if (node.LeftNode == null && node.RightNode == null)
            {
                if (curr_sum > max_sum_ref.max_no)
                {
                    max_sum_ref.max_no = curr_sum;
                    target_leaf = node;
                }
            }

            // если это не тот лист, спускаемся 
            // чтобы найти тот
            getTargetLeaf(node.LeftNode, max_sum_ref, curr_sum);
            getTargetLeaf(node.RightNode, max_sum_ref, curr_sum);
        }

        public virtual int maxSumPath()
        {
            // проверка на 0
            if (RootNode == null)
            {
                return 0;
            }

            // ищем лист и максимальную сумму
            getTargetLeaf(RootNode, max, 0);

            return max.max_no;
        }

    }


    //стороны
    public enum Side
    {
        Left,
        Right
    }
}
