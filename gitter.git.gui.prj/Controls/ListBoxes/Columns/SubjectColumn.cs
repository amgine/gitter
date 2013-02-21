namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Subject" column.</summary>
	public sealed class SubjectColumn : CustomListBoxColumn
	{
		#region Constants

		public const bool DefaultAlignToGraph = false;
		public const bool DefaultShowTags = true;
		public const bool DefaultShowLocalBranches = true;
		public const bool DefaultShowRemoteBranches = true;
		public const bool DefaultShowStash = true;

		private const int GraphCellWidth = 21;
		private const int TagSpacing = 2;

		#endregion

		#region Data

		private readonly bool _enableExtender;
		private bool _alignToGraph;
		private bool _showLocalBranches;
		private bool _showRemoteBranches;
		private bool _showTags;
		private bool _showStash;

		private SubjectColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler AlignToGraphChanged;
		public event EventHandler ShowLocalBranchesChanged;
		public event EventHandler ShowRemoteBranchesChanged;
		public event EventHandler ShowTagsChanged;
		public event EventHandler ShowStashChanged;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="SubjectColumn"/>.</summary>
		/// <param name="enableExtender">Enable column extender menu.</param>
		public SubjectColumn(bool enableExtender)
			: base((int)ColumnId.Subject, Resources.StrSubject, true)
		{
			SizeMode = ColumnSizeMode.Fill;

			_enableExtender = enableExtender;
			_alignToGraph = DefaultAlignToGraph;
			_showLocalBranches = DefaultShowLocalBranches;
			_showRemoteBranches = DefaultShowRemoteBranches;
			_showTags = DefaultShowTags;
			_showStash = DefaultShowStash;
		}

		/// <summary>Create <see cref="SubjectColumn"/>.</summary>
		public SubjectColumn()
			: this(true)
		{
		}

		#endregion

		#region Properties

		/// <summary>Gets the identification string.</summary>
		/// <value>The identification string.</value>
		public override string IdentificationString
		{
			get { return "Subject"; }
		}

		/// <summary>Align text and tags to graph column, if it is possible.</summary>
		public bool AlignToGraph
		{
			get { return _alignToGraph; }
			set
			{
				if(_alignToGraph != value)
				{
					_alignToGraph = value;
					var prev = PreviousVisibleColumn;
					if(!value && prev != null && prev.Id == (int)ColumnId.Graph)
					{
						prev.InvalidateContent();
					}
					InvalidateContent();
					AlignToGraphChanged.Raise(this);
				}
			}
		}

		/// <summary>Draw tags for all local branches, pointing to revision.</summary>
		public bool ShowLocalBranches
		{
			get { return _showLocalBranches; }
			set
			{
				if(_showLocalBranches != value)
				{
					_showLocalBranches = value;
					InvalidateContent();
					ShowLocalBranchesChanged.Raise(this);
				}
			}
		}

		/// <summary>Draw tags for all remote branches, pointing to revision.</summary>
		public bool ShowRemoteBranches
		{
			get { return _showRemoteBranches; }
			set
			{
				if(_showRemoteBranches != value)
				{
					_showRemoteBranches = value;
					InvalidateContent();
					ShowRemoteBranchesChanged.Raise(this);
				}
			}
		}

		/// <summary>Draw pointers for all tags, pointing to revision.</summary>
		public bool ShowTags
		{
			get { return _showTags; }
			set
			{
				if(_showTags != value)
				{
					_showTags = value;
					InvalidateContent();
					ShowTagsChanged.Raise(this);
				}
			}
		}

		/// <summary>Draw stash tag, if it is pointing to revision.</summary>
		public bool ShowStash
		{
			get { return _showStash; }
			set
			{
				if(_showStash != value)
				{
					_showStash = value;
					InvalidateContent();
					ShowStashChanged.Raise(this);
				}
			}
		}

		/// <summary>This column affects content of the column to the left.</summary>
		public override bool ExtendsToLeft
		{
			get
			{
				var pc = PreviousVisibleColumn;
				return pc != null && pc.Id == (int)ColumnId.Graph;
			}
		}

		#endregion

		#region Static Methods

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, Revision data, GraphAtom[] graph)
		{
			return measureEventArgs.MeasureText(data.Subject);
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string subject)
		{
			return measureEventArgs.MeasureText(subject);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, Revision data, GraphAtom[] graph)
		{
			OnPaintSubItem(paintEventArgs, data, graph, null, -1);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, Revision data, GraphAtom[] graph, IList<Tuple<Rectangle, IRevisionPointer>> drawnPointers, int hoveredPointer)
		{
			#region get painting options

			bool showLocalBranches;
			bool showRemoteBranches;
			bool showTags;
			bool showStash;
			bool alignToGraph;
			var rsc = paintEventArgs.Column as SubjectColumn;
			if(drawnPointers != null)
			{
				drawnPointers.Clear();
				if(rsc != null)
				{
					alignToGraph		= rsc.AlignToGraph;
					showLocalBranches	= rsc.ShowLocalBranches;
					showRemoteBranches	= rsc.ShowRemoteBranches;
					showTags			= rsc.ShowTags;
					showStash			= rsc.ShowStash;
				}
				else
				{
					alignToGraph		= SubjectColumn.DefaultAlignToGraph;
					showLocalBranches	= SubjectColumn.DefaultShowLocalBranches;
					showRemoteBranches	= SubjectColumn.DefaultShowRemoteBranches;
					showTags			= SubjectColumn.DefaultShowTags;
					showStash			= SubjectColumn.DefaultShowStash;
				}
			}
			else
			{
				alignToGraph = (rsc != null) ? rsc.AlignToGraph : SubjectColumn.DefaultAlignToGraph;
				showLocalBranches = false;
				showRemoteBranches = false;
				showTags = false;
				showStash = false;
			}
			var graphStyle = GlobalBehavior.GraphStyle;

			#endregion

			#region align to graph column

			var rect = paintEventArgs.Bounds;
			var graphColumn = paintEventArgs.Column.PreviousVisibleColumn;
			int graphColumnX = 0;
			if(graphColumn != null)
			{
				if(graphColumn.Id != (int)ColumnId.Graph)
				{
					graphColumn = null;
				}
				else
				{
					graphColumnX = rect.X - graphColumn.Width;
				}
			}
			if(graphColumn != null && alignToGraph)
			{
				int availWidth = graphColumn.Width - GraphCellWidth * graph.Length;
				for(int i = graph.Length - 1; i >= 0; --i)
				{
					if(graph[i].Elements != GraphElement.Space)
					{
						break;
					}
					availWidth += GraphCellWidth;
				}
				if(availWidth != 0)
				{
					rect.X -= availWidth;
					rect.Width += availWidth;
				}
			}

			#endregion

			#region prepare to draw references

			int drawnRefs = 0;
			int xoffset = 0;
			var font = paintEventArgs.Column.ContentFont;
			SubItemPaintEventArgs.PrepareContentRectangle(ref rect);

			#endregion

			#region paint tag & branch refs

			bool refsPresent;
			lock(data.References.SyncRoot)
			{
				refsPresent = data.References.Count != 0;
				if((showLocalBranches || showRemoteBranches || showTags) && refsPresent)
				{
					foreach(var reference in data.References)
					{
						int w = 0;
						switch(reference.Type)
						{
							case ReferenceType.LocalBranch:
								if(showLocalBranches)
								{
									w = graphStyle.DrawBranch(
										paintEventArgs.Graphics,
										font,
										GitterApplication.TextRenderer.LeftAlign,
										rect.Left + xoffset,
										rect.Top,
										rect.Right,
										rect.Height,
										drawnRefs == hoveredPointer,
										(Branch)reference);
								}
								break;
							case ReferenceType.RemoteBranch:
								if(showRemoteBranches)
								{
									w = graphStyle.DrawBranch(
										paintEventArgs.Graphics,
										font,
										GitterApplication.TextRenderer.LeftAlign,
										rect.Left + xoffset,
										rect.Top,
										rect.Right,
										rect.Height,
										drawnRefs == hoveredPointer,
										(RemoteBranch)reference);
								}
								break;
							case ReferenceType.Tag:
								if(showTags)
								{
									w = graphStyle.DrawTag(
										paintEventArgs.Graphics,
										font,
										GitterApplication.TextRenderer.LeftAlign,
										rect.Left + xoffset,
										rect.Top,
										rect.Right,
										rect.Height,
										drawnRefs == hoveredPointer,
										(Tag)reference);
								}
								break;
						}
						if(w != 0)
						{
							drawnPointers.Add(new Tuple<Rectangle, IRevisionPointer>(
								new Rectangle(rect.X + xoffset, rect.Y, w, rect.Height), reference));
							xoffset += w + TagSpacing;
							++drawnRefs;
						}
						if(xoffset >= rect.Width) break;
					}
				}
			}

			#endregion

			#region paint stash ref

			var stash = data.Repository.Stash.MostRecentState;
			bool stashPresent = (stash != null && data == stash.Revision);
			if(showStash && stashPresent)
			{
				if(xoffset < rect.Width)
				{
					int w = graphStyle.DrawStash(
						paintEventArgs.Graphics,
						font,
						GitterApplication.TextRenderer.LeftAlign,
						rect.Left + xoffset,
						rect.Top,
						rect.Right,
						rect.Height,
						drawnRefs == hoveredPointer,
						stash);

					drawnPointers.Add(new Tuple<Rectangle, IRevisionPointer>(
						new Rectangle(rect.X + xoffset, rect.Y, w, rect.Height), stash));
					xoffset += w + TagSpacing;
					++drawnRefs;
				}
			}

			#endregion

			#region paint reference presence indication

			if(drawnRefs != 0)
			{
				if(graph != null && graph.Length != 0)
				{
					if(graphColumn != null)
					{
						graphStyle.DrawReferenceConnector(paintEventArgs.Graphics, graph, graphColumnX, 21, rect.X, rect.Y, rect.Height);
					}
				}
				xoffset += 2;
			}
			else
			{
				if(refsPresent || stashPresent)
				{
					if(graph != null && graph.Length != 0 && graphColumn != null)
					{
						graphStyle.DrawReferencePresenceIndicator(paintEventArgs.Graphics, graph, graphColumnX, 21, rect.Y, rect.Height);
					}
				}
			}

			#endregion

			#region paint subject text

			rect.X += xoffset;
			rect.Width -= xoffset;
			if(rect.Width > 1)
			{
				paintEventArgs.PrepareTextRectangle(font, ref rect);
				GitterApplication.TextRenderer.DrawText(
					paintEventArgs.Graphics, data.Subject, font, paintEventArgs.Brush, rect);
			}

			#endregion
		}

		#endregion

		#region Methods

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			if(_enableExtender)
			{
				_extender = new SubjectColumnExtender(this);
				Extender = new Popup(_extender);
			}
		}

		protected override void OnListBoxDetached()
		{
			if(_enableExtender)
			{
				Extender.Dispose();
				Extender = null;
				_extender.Dispose();
				_extender = null;
			}
			base.OnListBoxDetached();
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("AlignToGraph", AlignToGraph);
			section.SetValue("ShowLocalBranches", ShowLocalBranches);
			section.SetValue("ShowRemoteBranches", ShowRemoteBranches);
			section.SetValue("ShowTags", ShowTags);
			section.SetValue("ShowStash", ShowStash);
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			AlignToGraph = section.GetValue("AlignToGraph", AlignToGraph);
			ShowLocalBranches = section.GetValue("ShowLocalBranches", ShowLocalBranches);
			ShowRemoteBranches = section.GetValue("ShowRemoteBranches", ShowRemoteBranches);
			ShowTags = section.GetValue("ShowTags", ShowTags);
			ShowStash = section.GetValue("ShowStash", ShowStash);
		}

		#endregion
	}
}
