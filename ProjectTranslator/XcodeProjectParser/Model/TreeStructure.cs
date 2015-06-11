using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XcodeProjectParser
{
	public class TreeNode<T> : IEnumerable<TreeNode<T>>
	{
		public T Data { get; set; }

		public TreeNode<T> Parent { get; set; }

		public ICollection<TreeNode<T>> Children { get; set; }

		ICollection<TreeNode<T>> ElementsIndex { get; set; }

		public Boolean IsRoot {
			get {
				return Parent == null;
			}
		}

		public Boolean IsLeaf {
			get {
				return Children.Count == 0;
			}
		}

		public int Level {
			get {
				if (IsRoot)
					return 0;
				return Parent.Level + 1;
			}
		}

		public TreeNode (T data)
		{
			Data = data;
			Children = new LinkedList<TreeNode<T>> ();

			ElementsIndex = new LinkedList<TreeNode<T>> ();
			ElementsIndex.Add (this);
		}

		public TreeNode<T> AddChild (T child)
		{
			TreeNode<T> childNode = new TreeNode<T> (child) { 
				Parent = this
			};
			Children.Add (childNode);

			RegisterChildForSearch (childNode);

			return childNode;
		}

		public override string ToString ()
		{
			return Data != null ? Data.ToString () : "[data null]";
		}

		void RegisterChildForSearch (TreeNode<T> node)
		{
			ElementsIndex.Add (node);
			if (Parent != null)
				Parent.RegisterChildForSearch (node);
		}

		public TreeNode<T> FindTreeNode (Func<TreeNode<T>, bool> predicate)
		{
			return ElementsIndex.FirstOrDefault (predicate);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<TreeNode<T>> GetEnumerator ()
		{
			yield return this;
			foreach (var directChild in this.Children) {
				foreach (var anyChild in directChild)
					yield return anyChild;
			}
		}
	}
}

