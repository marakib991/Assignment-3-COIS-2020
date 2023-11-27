using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTree
{
    // Interfaces used for a Splay Tree

    public interface IContainer<T>
    {
        void MakeEmpty();
        bool Empty();
        int Size();
    }

    //-------------------------------------------------------------------------

    public interface ISearchable<T> : IContainer<T>
    {
        void Insert(T item);
        bool Remove(T item);
        bool Contains(T item);
    }

    //-------------------------------------------------------------------------

    // Common generic node class for a splay tree
    // Same data members as a binary search tree

    public class Node<T> where T : IComparable
    {
        // Read/write properties

        public T Item { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Node(T item)
        {
            Item = item;
            Left = Right = null;
        }
    }

    //-------------------------------------------------------------------------

    // Implementation:  Splay Tree

    // Note:
    // If item is not found in the tree then the last node (item) visited is splayed to the given root

    class SplayTree<T> : ISearchable<T> where T : IComparable
    {
        private Node<T> root;                // Reference to the root of a splay tree

        // Constructor
        // Initializes an empty splay tree
        // Time complexity:  O(1)

        public SplayTree()
        {
            root = null;                     // Empty splay tree
        }

        // RightRotate
        // Rotates the splay tree to the right around Node p
        // Returns the new root of the (sub)tree
        // Worst case time complexity:  O(1)

        private Node<T> RightRotate(Node<T> p)
        {
            Node<T> q = p.Left;

            p.Left = q.Right;
            q.Right = p;

            return q;
        }

        // RotateLeft
        // Rotates the splay tree to the left around Node p
        // Returns the new root of the (sub)tree
        // Worst case time complexity:  O(1)

        // New Code
        private Node<T> LeftRotate(Node<T> p)
        {
            if (p == null || p.Right == null)
                return p; // Return the original node if it's null or its right child is null.

            Node<T> q = p.Right;

            p.Right = q.Left;
            q.Left = p;

            return q;
        }


        // Splay
        // Splays (brings) item to the top of the subtree rooted at curr
        // If item is not found, the last item visited is splayed to the top the subtree rooted at curr 
        // Worst case time complexity:  O(n)

        private Node<T> Splay(T item, Node<T> curr)
        {

            // Terminating conditions (item not found or found)
            if (curr == null || item.CompareTo(curr.Item) == 0)
                return curr;

            // Determine where the path heads down the splay tree

            // Item is in left subtree
            if (item.CompareTo(curr.Item) < 0)
            {
                // Item not found
                if (curr.Left == null)
                    return curr;

                // Right-Right
                if (item.CompareTo(curr.Left.Item) < 0)
                {
                    // Splay item to the root of left-left
                    curr.Left.Left = Splay(item, curr.Left.Left);

                    // Rotate right at the root
                    curr = RightRotate(curr);
                }
                else
                // Left-Right
                if (item.CompareTo(curr.Left.Item) > 0)
                {
                    // Splay item to the root of left-right
                    curr.Left.Right = Splay(item, curr.Left.Right);

                    // Rotate left (if possible) at the root of the left subtree
                    if (curr.Left.Right != null)
                        curr.Left = LeftRotate(curr.Left);
                }
                // Rotate right (if possible) at the root
                return (curr.Left == null) ? curr : RightRotate(curr);
            }

            // Item is in right subtree (mirror image of the code)
            else
            {
                // Item not found
                if (curr.Right == null)
                    return curr;

                // Right-Left
                if (item.CompareTo(curr.Right.Item) < 0)
                {
                    // Splay item to the root of right-left
                    curr.Right.Left = Splay(item, curr.Right.Left);

                    // Rotate right (if possible) at the root of the right subtree
                    if (curr.Right.Left != null)
                        curr.Right = RightRotate(curr.Right);
                }
                else
                // Left-Left
                if (item.CompareTo(curr.Right.Item) > 0)
                {
                    // Splay item to the root of right-right
                    curr.Right.Right = Splay(item, curr.Right.Right);

                    // Rotate left at the root
                    curr = LeftRotate(curr);
                }
                // Rotate left (if possible) at the root 
                return (curr.Right == null) ? curr : LeftRotate(curr);
            }
        }

        // New Code
        //
        private Stack<Node<T>> Access(T item)
        {
            Stack<Node<T>> stack = new Stack<Node<T>>();
            Node<T> current = root;

            while (current != null)
            {
                stack.Push(current);

                int compareResult = item.CompareTo(current.Item);
                if (compareResult == 0)
                    break;
                else if (compareResult < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            return stack;
        }
        // New Code
        //
        private void Splay(Node<T> p, Stack<Node<T>> S)
        {
            while (S.Count > 1)
            {
                Node<T> parent = S.Pop();
                Node<T> grandParent = S.Peek();

                if (parent.Left == p)
                {
                    if (grandParent.Left == parent)
                    {
                        // Zig-Zig (Right-Right) rotation
                        RightRotate(grandParent);
                        RightRotate(parent);
                    }
                    else
                    {
                        // Zig-Zag (Right-Left) rotation
                        RightRotate(parent);
                        LeftRotate(grandParent);
                    }
                }
                else
                {
                    if (grandParent.Right == parent)
                    {
                        // Zag-Zag (Left-Left) rotation
                        LeftRotate(grandParent);
                        LeftRotate(parent);
                    }
                    else
                    {
                        // Zag-Zig (Left-Right) rotation
                        LeftRotate(parent);
                        RightRotate(grandParent);
                    }
                }
            }

            if (S.Count == 1)
            {
                // Single rotation
                if (S.Peek().Left == p)
                    RightRotate(S.Pop());
                else
                    LeftRotate(S.Pop());
            }

            root = p;
        }

        // Public Insert
        // adapted from https://www.geeksforgeeks.org/splay-tree-set-2-insert-delete/?ref=rp

        // Inserts an item into a splay tree
        // An exception is throw if the item is already in the tree
        // Amortized time complexity:  O(log n)

        // New Insert
        public void Insert(T item)
        {
            Stack<Node<T>> accessPath = Access(item);

            if (accessPath.Count > 0 && accessPath.Peek().Item.CompareTo(item) == 0)
                throw new InvalidOperationException("Duplicate item");

            Node<T> newNode = new Node<T>(item);
            if (accessPath.Count == 0)
            {
                root = newNode;
            }
            else
            {
                Node<T> lastNode = accessPath.Peek();
                if (item.CompareTo(lastNode.Item) < 0)
                    lastNode.Left = newNode;
                else
                    lastNode.Right = newNode;

                accessPath.Push(newNode);
                Splay(newNode, accessPath);
            }
        }


        // Public Remove
        // adapted from https://www.geeksforgeeks.org/splay-tree-set-3-delete/?ref=rp

        // Remove an item from a splay tree
        // Nothing is performed if the item is not found or the tree is empty
        // Amortized time complexity:  O(log n)

        // New Remove
        public bool Remove(T item)
        {
            Stack<Node<T>> accessPath = Access(item);
            if (accessPath.Count == 0 || accessPath.Peek().Item.CompareTo(item) != 0)
                return false;

            Node<T> nodeToRemove = accessPath.Peek();
            Splay(nodeToRemove, accessPath);

            if (nodeToRemove.Left != null)
            {
                Node<T> maxNodeLeftSubtree = nodeToRemove.Left;
                while (maxNodeLeftSubtree.Right != null)
                    maxNodeLeftSubtree = maxNodeLeftSubtree.Right;

                Stack<Node<T>> leftAccessPath = Access(maxNodeLeftSubtree.Item);
                Splay(maxNodeLeftSubtree, leftAccessPath);

                maxNodeLeftSubtree.Right = nodeToRemove.Right;
                root = maxNodeLeftSubtree;
            }
            else
            {
                root = nodeToRemove.Right;
            }

            return true;
        }


        // Public Contains
        // Returns true if the item is found in an AVL Tree; false otherwise
        // Amortized time complexity:  O(log n)

        // New Contains
        public bool Contains(T item)
        {
            Stack<Node<T>> accessPath = Access(item);
            if (accessPath.Count == 0)
                return false;

            Node<T> lastNode = accessPath.Peek();
            Splay(lastNode, accessPath);

            return lastNode.Item.CompareTo(item) == 0;
        }


        // MakeEmpty
        // Resets the splay tree to empty
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            root = null;
        }

        // Empty
        // Returns true if the splay tree is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return root == null;
        }

        // Public Size
        // Returns the number of items in an splay tree
        // Time complexity:  O(n)

        public int Size()
        {
            return Size(root);          // Calls the private, recursive Size
        }

        // Size
        // Calculates the size of the tree rooted at node
        // Time complexity:  O(n)

        private int Size(Node<T> node)
        {
            if (node == null)
                return 0;
            else
                return 1 + Size(node.Left) + Size(node.Right);
        }

        // Public Print
        // Outputs the items of an splay tree in sorted order
        // Time complexity:  O(n)

        public void Print()
        {
            int indent = 0;

            Print(root, indent);             // Calls private, recursive Print
            Console.WriteLine();
        }

        // Private Print
        // Outputs items using an inorder traversal
        // Time complexity:  O(n)

        private void Print(Node<T> node, int indent)
        {
            if (node != null)
            {
                Print(node.Right, indent + 3);
                Console.WriteLine(new String(' ', indent) + node.Item.ToString());
                Print(node.Left, indent + 3);
            }
        }

        // Clone method
        public object Clone()
        {
            SplayTree<T> clonedTree = new SplayTree<T>();
            CloneTree(root, clonedTree);
            return clonedTree;
        }

        private void CloneTree(Node<T> originalNode, SplayTree<T> clonedTree)
        {
            if (originalNode != null)
            {
                clonedTree.Insert(originalNode.Item);
                CloneTree(originalNode.Left, clonedTree);
                CloneTree(originalNode.Right, clonedTree);
            }
        }

        // Override Equals method
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SplayTree<T> otherTree = (SplayTree<T>)obj;
            return Equals(root, otherTree.root);
        }

        private bool Equals(Node<T> node1, Node<T> node2)
        {
            if (node1 == null && node2 == null)
            {
                return true;
            }

            if (node1 != null && node2 != null)
            {
                return node1.Item.Equals(node2.Item) &&
                       Equals(node1.Left, node2.Left) &&
                       Equals(node1.Right, node2.Right);
            }

            return false;
        }

        // Undo method
        public SplayTree<T> Undo()
        {
            if (root == null || (root.Left == null && root.Right == null))
            {
                // The tree is empty or has only one node, nothing to undo
                return Clone() as SplayTree<T>;
            }

            SplayTree<T> originalTree = Clone() as SplayTree<T>; // Create a copy of the original tree

            // Attempt to rotate the root node back to its original position
            UndoRotateRoot();

            // Now, the item is at the leaf, remove it
            if (root.Left != null)
            {
                root = root.Left;
            }
            else
            {
                root = null;
            }

            return originalTree;
        }

        private void UndoRotateRoot()
        {
            // Rotate the root node back to its original position using a series of rotations
            // Note: You may need to experiment with different rotations to find the right combination

            // Example: Rotate right if the root has a left child
            if (root.Left != null)
            {
                root = RightRotate(root);
            }
            // Example: Rotate left if the root has a right child
            else if (root.Right != null)
            {
                root = LeftRotate(root);
            }

            // You may need additional cases or rotations based on your specific implementation
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Part A: Test each of the re-implemented methods of the splay tree
            SplayTree<int> originalTree = new SplayTree<int>();

            for (int i = 1; i <= 6; i++)
                originalTree.Insert(i * 10);

            Console.WriteLine("Original Tree:");
            originalTree.Print();

            // Test Remove
            Console.WriteLine("\nRemoving item 40:");
            originalTree.Remove(40);
            originalTree.Print();

            // Test Contains
            Console.WriteLine("\nChecking if item 30 is in the tree:");
            Console.WriteLine(originalTree.Contains(30));

            // Part B: Verify using the Equals method that the Clone method produces an exact copy
            SplayTree<int> clonedTree = originalTree.Clone() as SplayTree<int>;
            Console.WriteLine("\nChecking if the cloned tree is equal to the original tree:");
            Console.WriteLine(originalTree.Equals(clonedTree));

            // Part C: Perform a successful Insert followed by the Undo method
            Console.WriteLine("\nInserting item 70:");
            originalTree.Insert(70);
            Console.WriteLine("Original Tree after insertion:");
            originalTree.Print();

            Console.WriteLine("\nUndoing the last insertion:");
            SplayTree<int> undoneTree = originalTree.Undo();
            Console.WriteLine("Tree after Undo:");
            originalTree.Print();

            // Perform the Insert again with the same item
            Console.WriteLine("\nInserting item 70 again:");
            originalTree.Insert(70);
            Console.WriteLine("Original Tree after second insertion:");
            originalTree.Print();

            // Verify using the Equals method that the original splay tree is reproduced
            Console.WriteLine("\nChecking if the original tree is reproduced after the second insertion:");
            Console.WriteLine(originalTree.Equals(undoneTree));

            Console.ReadKey();
        }
    }

}