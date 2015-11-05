using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Created by David Bainbridge. 
 * https://github.com/DoctahJones
 */

namespace Trees
{
    /// <summary>
    /// A Binary Search Tree.
    /// </summary>
    /// <typeparam name="T">The type to be stored in the collection.</typeparam>
    public class BinarySearchTree<T> : ICollection<T>, IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        /// The root node of the tree, null if the tree is empty.
        /// </summary>
        private Node<T> root;
        //The count of the number of items currently in the tree.
        private int count;
        /// <summary>
        /// Property for accessing the count of the number of items in the tree.
        /// </summary>
        public int Count { get {return count;}}
        /// <summary>
        /// Property for determining whether the collection is read only, in this case this is always false.
        /// </summary>
        public bool IsReadOnly{ get {return false;}}
        
        /// <summary>
        /// Default constructor 
        /// </summary>
        public BinarySearchTree()
        {
            this.count = 0;
        }

        /// <summary>
        /// Add a node to the tree at the correct position.
        /// </summary>
        /// <param name="value">The value to add to the tree. Must not already be within the tree.</param>
        public void Add(T value)  {
            //If there are already values in the tree find the correct place to add it.
            if (root != null)
            {
                Node<T> currNode = root;
                bool cont = true;
                while (cont) {
                //.CompareTo() returns > 0 when argument is smaller, < 0 when it is larger and 0 when it is equal.
                int compare = currNode.getValue().CompareTo(value);
                if (compare > 0)
                {
                    Node<T> temp = currNode.getLeft();
                    if (temp == null)
                    {
                        currNode.addLeft(new Node<T>(value));
                        cont = false;
                    }
                    else
                        currNode = currNode.getLeft();
                }
                else if (compare < 0)
                {
                    Node<T> temp = currNode.getRight();
                    if (temp == null)
                    {
                        currNode.addRight(new Node<T>(value));
                        cont = false;
                    }
                    else
                        currNode = currNode.getRight();
                }
                else
                    throw new ArgumentException("Duplicate entry");
                }
            }
            else
                root = new Node<T>(value);
            this.count++;
        }

        /// <summary>
        /// Method to remove an item from the tree.
        /// </summary>
        /// <param name="item">The item to remove from the tree.</param>
        /// <returns>True if the item was successfully removed, false otherwise.</returns>
        public bool Remove(T item)
        {
            Node<T> removeNode = getNodeFromValue(item);
            if (removeNode == null)
                return false;
            bool output = Remove(removeNode);
            if (output)
                this.count--;
            return output;
 
        }

        private bool Remove(Node<T> item)
        {
            Node<T> nodeToBeReplacedWith;
            //Node to be removed has no children, we want to set parents pointer to null.
            if (item.getLeft() == null && item.getRight() == null)
                nodeToBeReplacedWith = null;
            //if the node we want to remove only has 1 child we want to set parents pointer to that child.
            else if (item.getLeft() == null)
                nodeToBeReplacedWith = item.getRight();
            else if (item.getRight() == null)
                nodeToBeReplacedWith = item.getLeft();
            //else node has 2 children so we want to find minimum value in the right sub tree of node to be removed, call this x.
            //we then replace the value of the node to be removed with x and remove the node which contained x originally.
            else
            {
                nodeToBeReplacedWith = item.getRight();
                while (nodeToBeReplacedWith.getLeft() != null)
                {
                    nodeToBeReplacedWith = nodeToBeReplacedWith.getLeft();
                }
                item.setValue(nodeToBeReplacedWith.getValue());
                return Remove(nodeToBeReplacedWith);
                //Remove(nodeToBeReplacedWith);
            }
            //if the item to be removed is the root item no need to search for parent, just replace it.
            if (item.Equals(root))
                root = nodeToBeReplacedWith;
            else
            {
                Node<T> itemParent = getParentNode(item, this.root, null);
                if (itemParent.getLeft() != null && itemParent.getLeft().Equals(item))
                    itemParent.addLeft(nodeToBeReplacedWith);
                if (itemParent.getRight() != null && itemParent.getRight().Equals(item))
                    itemParent.addRight(nodeToBeReplacedWith);
            }
            return true;
        }

        private Node<T> getNodeFromValue(T value)  {
            foreach(Node<T> curr in InOrderTraversal(root))  {
                if(curr.getValue().Equals(value))
                    return curr;
            }
            return null;
        }

        private Node<T> getParentNode(Node<T> nodeLookingFor, Node<T> nodeCurrent, Node<T> nodeCurrentParent)
        {
            if (nodeCurrent == null)
                return null;
            //.CompareTo() returns > 0 when argument is smaller, < 0 when it is larger and 0 when it is equal.
            int compare = nodeLookingFor.getValue().CompareTo(nodeCurrent.getValue());
            if (compare == 0)
                //the values could be the same because when removing an item from tree we change values of nodes. Therefore we have to check this is actually the node we want to remove or 
                //if it just has the same value at the moment. If it isnt the item then we look in the right sub tree for the item we actually want to find parent of.
                if(nodeLookingFor.Equals(nodeCurrent))
                    return nodeCurrentParent;
                else
                    return getParentNode(nodeLookingFor, nodeCurrent.getRight(), nodeCurrent);
            else if (compare > 0)
                return getParentNode(nodeLookingFor, nodeCurrent.getRight(), nodeCurrent);
            else if (compare < 0)
                return getParentNode(nodeLookingFor, nodeCurrent.getLeft(), nodeCurrent);
            return null;
        }

        private IEnumerable<Node<T>> InOrderTraversal(Node<T> startNode)
        {
            if (startNode == null)
                yield break;
            if (startNode.getLeft() != null)
            {
                foreach (Node<T> curr in InOrderTraversal(startNode.getLeft()))
                {
                    yield return curr;
                }
            }
            yield return startNode;
            if (startNode.getRight() != null)
            {
                foreach (Node<T> curr in InOrderTraversal(startNode.getRight()))
                {
                    yield return curr;
                }
            }
        }

        /// <summary>
        /// Method to return an enmerator for this collection which returns the items in a sorted order.
        /// </summary>
        /// <returns>Returns an enumertor for the tree.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (Node<T> curr in InOrderTraversal(root))
            {
                yield return curr.getValue();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            //return generic version instead.
            return this.GetEnumerator();
        }

        private IEnumerable<Node<T>> PreOrderTraversal(Node<T> startNode)
        {
            if (startNode == null)
                yield break;
            yield return startNode;
            if (startNode.getLeft() != null)
            {
                foreach (Node<T> curr in PreOrderTraversal(startNode.getLeft()))
                {
                    yield return curr;
                }
            }
            if (startNode.getRight() != null)
            {
                foreach (Node<T> curr in PreOrderTraversal(startNode.getRight()))
                {
                    yield return curr;
                }
            }
        }

        /// <summary>
        /// Method which returns an enumerable list which traverses in a PreOrder fashion.
        /// </summary>
        /// <returns>Returns an Enumerable type to be iterated over.</returns>
        public IEnumerable<T> PreOrderTraversal()
        {
            foreach(Node<T> curr in PreOrderTraversal(root))  {
                yield return curr.getValue();
            }
        }

        private IEnumerable<Node<T>> PostOrderTraversal(Node<T> startNode)
        {
            if (startNode == null)
                yield break;
            if (startNode.getLeft() != null)
            {
                foreach (Node<T> curr in PostOrderTraversal(startNode.getLeft()))
                {
                    yield return curr;
                }
            }
            if (startNode.getRight() != null)
            {
                foreach (Node<T> curr in PostOrderTraversal(startNode.getRight()))
                {
                    yield return curr;
                }
            }
            yield return startNode;
        }

        /// <summary>
        /// Method which returns an enumerable list which traverses in a PostOrder fashion.
        /// </summary>
        /// <returns>Returns an Enumerable type to be iterated over.</returns>
        public IEnumerable<T> PostOrderTraversal()
        {
            foreach (Node<T> curr in PostOrderTraversal(root))
            {
                yield return curr.getValue();
            }
        }

        private IEnumerable<Node<T>> LevelOrderTraversal(Node<T> startNode)
        {
            if (startNode == null)
                yield break;
            Queue<Node<T>> q = new Queue<Node<T>>();
            q.Enqueue(startNode);
            Node<T> currNode;
            while (q.Count() > 0)
            {
                currNode = q.Dequeue();
                if(currNode.getLeft() != null)
                       q.Enqueue(currNode.getLeft());
                if (currNode.getRight() != null)
                    q.Enqueue(currNode.getRight());
                yield return currNode;
            }
        }

        /// <summary>
        /// Method which returns an enumerable list which traverses in a LevelOrder fashion.
        /// </summary>
        /// <returns>Returns an Enumerable type to be iterated over.</returns>
        public IEnumerable<T> LevelOrderTraversal()
        {
            foreach (Node<T> curr in LevelOrderTraversal(root))
            {
                yield return curr.getValue();
            }
        }

        /// <summary>
        /// Method to clear all items from the tree leaving an empty tree.
        /// </summary>
        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        /// <summary>
        /// Method to determine whether the tree contains an item.
        /// </summary>
        /// <param name="item">The item to check whether it is contained.</param>
        /// <returns>True if found, false if not.</returns>
        public bool Contains(T item)
        {
            if (root == null)
                return false;
            else
                return Contains(item, this.root);
        }

        private bool Contains(T item, Node<T> startNode)
        {
            if (startNode == null)
                return false;
            int compare = startNode.getValue().CompareTo(item);
            if (compare == 0)
                return true;
            else if (compare > 0)
                return Contains(item, startNode.getLeft());
            else if(compare < 0)
                return Contains(item, startNode.getRight());
            return false;
        }

        /// <summary>
        /// Method to copy the tree to a passed in array beginning at the given index.
        /// </summary>
        /// <param name="array">The array to copy the tree to.</param>
        /// <param name="arrayIndex">The index to begin copying at in the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array is null");
            if(arrayIndex < 0 || arrayIndex > array.Length) 
                throw new ArgumentOutOfRangeException("arrayIndex is not within the bounds of array");
            if ((array.Length - arrayIndex) < this.count)
                throw new ArgumentException("array is too small beginning at given arrayIndex");
            foreach (T curr in this)
            {
                array[arrayIndex] = curr;
                arrayIndex++;
            }
        }

    }

    class Node<T>
    {
        Node<T> left;
        Node<T> right;
        T value;

        public Node(T curr)
        {
            this.value = curr;
        }

        public void addLeft(Node<T> left)
        {
            this.left = left;
        }

        public void addRight(Node<T> right)
        {
            this.right = right;
        }

        public Node<T> getLeft()
        {
            return left;
        }

        public Node<T> getRight()
        {
            return right;
        }

        public T getValue()
        {
            return value;
        }

        public void setValue(T value)
        {
            this.value = value;
        }
    }

}
