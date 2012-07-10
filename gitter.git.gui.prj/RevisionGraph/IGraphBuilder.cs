namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;

	/// <summary>Interface for graph builder.</summary>
	/// <typeparam name="T">Type of graph nodes.</typeparam>
	public interface IGraphBuilder<T>
		where T : class
	{
		GraphAtom[][] BuildGraph(IList<T> items, Func<T, IList<T>> getParents);

		GraphAtom[] AddGraphLineToTop(GraphAtom[] topLine);

		void CleanGraph(GraphAtom[] graph);

		void CleanGraph(GraphAtom[] prev, GraphAtom[] next);
	}

	/// <summary>Factory for <see cref="IGraphBuilder&lt;T&gt;"/></summary>
	public interface IGraphBuilderFactory
	{
		IGraphBuilder<T> CreateGraphBuilder<T>()
			where T : class;
	}
}
