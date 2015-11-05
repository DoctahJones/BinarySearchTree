using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trees;
using System.Collections.Generic;
using System.Linq;

namespace TreeTests
{
    [TestClass]
    public class TreeTests
    {
        
        [TestMethod]
        public void TestAdd()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            for(int i = 0; i < 50; i++)  {
                tree.Add(i);
            }
            for(int i = 0; i < 50; i++)  {
                Assert.IsTrue(tree.Contains(i));
            }
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void TestAddDuplicate()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            tree.Add(10);
            tree.Add(10);
        }

        [TestMethod]
        public void TestContains()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            Assert.IsFalse(tree.Contains('c'));
            tree.Add('c');
            Assert.IsTrue(tree.Contains('c'));
            Assert.IsFalse(tree.Contains('b'));
        }

        [TestMethod]
        public void TestCount()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            Assert.IsTrue(tree.Count == 0);
            tree.Add(10);
            Assert.IsTrue(tree.Count == 1);
            for (int i = 100; i < 200; i++)
            {
                tree.Add(i);
            }
            Assert.IsTrue(tree.Count == 101);
            tree.Remove(10);
            Assert.IsTrue(tree.Count == 100);
            for (int i = 100; i < 200; i++)
            {
                tree.Remove(i);
            }
            Assert.IsTrue(tree.Count == 0);
            //count isnt decremented if unsuccessfully try to remove an item.
            tree.Remove(10);
            Assert.IsTrue(tree.Count == 0);
        }

        [TestMethod]
        public void TestClear()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            //count is 0 after clear
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i);
            }
            tree.Clear();
            Assert.IsTrue(tree.Count == 0);
        }


        [TestMethod]
        public void TestRemove()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            //fails to remove item from empty list.
            Assert.IsFalse(tree.Remove(10));
            tree.Add(10);
            tree.Add(12);
            Assert.IsTrue(tree.Remove(10));
            Assert.IsFalse(tree.Contains(10));
            Assert.IsFalse(tree.Remove(10));
        }

        [TestMethod]
        public void TestIterator()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            //empty tree should give no iterations.
            foreach (int curr in tree)
            {
                Assert.Fail("Should be empty.");
            }
            for(int i = 0; i < 100; i++)  {
                tree.Add(i);
            }
            int element = 0;
            foreach(int curr in tree) { 
                Assert.AreEqual(element, curr);
                element++;
            }
            //remove root element, an item with 1 child in the middle and a leaf with no children at bottom. Recheck everything iterates in order.
            tree.Remove(0);
            tree.Remove(50);
            tree.Remove(100);
            element = 1;
            foreach (int curr in tree)
            {
                Assert.AreEqual(element, curr);
                if (element == 49)
                    element += 2;
                else
                    element++;
            }
            //re add 50 so that 51 now has child on left of 50 and child right of 52.
            tree.Add(50);
            element = 1;
            foreach (int curr in tree)
            {
                Assert.AreEqual(element, curr);
                element++;
            }
            //remove node with 2 children, check iterates correctly still.
            tree.Remove(51);
            element = 1;
            foreach (int curr in tree)
            {
                Assert.AreEqual(element, curr);
                if (element == 50)
                    element += 2;
                else
                    element++;
            }
        }

        [TestMethod]
        public void TestCopyTo()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            int[] array = new int[0];
            tree.CopyTo(array, 0);
            Assert.IsTrue(array.Length == 0);
            //basic copy
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i);
            }
            array = new int[10];
            int startIndex = 0;
            tree.CopyTo(array, startIndex);
            foreach (int curr in tree)
            {
                Assert.IsTrue(curr == array[startIndex]);
                startIndex++;
            }
            //copy at later position in middle
            array = new int[25];
            startIndex = 10;
            tree.CopyTo(array, startIndex);
            foreach (int curr in tree)
            {
                Assert.IsTrue(curr == array[startIndex]);
                startIndex++;
            }
            //copy to final positions of array.
            array = new int[25];
            startIndex = 15;
            tree.CopyTo(array, startIndex);
            foreach (int curr in tree)
            {
                Assert.IsTrue(curr == array[startIndex]);
                startIndex++;
            }
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestCopyToNullArray()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            int[] array  = null;
            tree.CopyTo(array, 0);
        }

        [TestMethod]
        public void TestCopyToInvalidIndex()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i);
            }
            int[] array = new int[10];
            //Try to set start index outside of max size of array.
            try
            {
                tree.CopyTo(array, 11);
                Assert.Fail("Should fail, index is larger than size of array.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }
            //try to copy to start index below 0
            try
            {
                tree.CopyTo(array, -2);
                Assert.Fail("Should fail, index is below 0");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }

        }

        [TestMethod]
        public void TestCopyToSmallArray()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            for (int i = 0; i < 10; i++)
            {
                tree.Add(i);
            }
            int[] array = new int[5];
            //try copying to array that is too small
            try
            {
                tree.CopyTo(array, 0);
                Assert.Fail("Should fail, array too small");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
            //try copying too close to the end of the array.
            array = new int[25];
            try
            {
                tree.CopyTo(array, 20);
                Assert.Fail("Should fail, Not enough room to copy");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        [TestMethod]
        public void testMassAddRemove()
        {
            BinarySearchTree<int> tree = new BinarySearchTree<int>();
            List<int> list = new List<int>();
            Random r = new Random();
            int numberToAdd = r.Next(1000000);
            for (int i = 0; i < 1000; i++)
            {
                while (list.Contains(numberToAdd))
                {
                    numberToAdd = r.Next(1000000);
                }
                list.Add(numberToAdd);
                tree.Add(numberToAdd);
            }
            Assert.IsTrue(list.Count == tree.Count && list.Count == 1000);
            list.Sort();
            Assert.IsTrue(Enumerable.SequenceEqual(list, tree));
            int curr;
            for (int i = 0; i < list.Count; i++)
            {
                curr = list.ElementAt(0);
                Assert.IsTrue(list.Remove(curr));
                Assert.IsTrue(tree.Remove(curr));
            }
            Assert.IsTrue(Enumerable.SequenceEqual(list, tree));

            numberToAdd = r.Next(1000000);
            for (int i = 0; i < 1000; i++)
            {
                while (list.Contains(numberToAdd))
                {
                    numberToAdd = r.Next(1000000);
                }
                list.Add(numberToAdd);
                tree.Add(numberToAdd);
                if (i % 2 == 0)
                {
                    curr = list.ElementAt(r.Next(list.Count));
                    Assert.IsTrue(list.Remove(curr));
                    Assert.IsTrue(tree.Remove(curr));
                }
            }
            list.Sort();
            Assert.IsTrue(Enumerable.SequenceEqual(list, tree));
        }



    }
}
