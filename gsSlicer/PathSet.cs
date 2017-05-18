﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using g3;

namespace gs 
{
	public class PathSet : IPathSet
	{
		List<IPath> Paths;

		PathTypes eType;
		bool isPlanar;
		bool isLinear;

		public PathSet() 
		{
			Paths = new List<IPath>();

			eType = PathTypes.Custom;
			isPlanar = isLinear = false;
		}

		public PathTypes Type { 
			get { return eType; }
		}
		public bool IsPlanar { 
			get { return isPlanar; }
		}
		public bool IsLinear {
			get { return isLinear; }
		}


		public void Append(IPath path) {
			if ( Paths.Count == 0 ) {
				eType = path.Type;
				isPlanar = path.IsPlanar;
				isLinear = path.IsLinear;
			} else if ( eType != path.Type ) {
				eType = PathTypes.Composite;
				isPlanar = isPlanar && path.IsPlanar;
				isLinear = isLinear && path.IsLinear;
			}
			Paths.Add(path);
		}


		public void AppendChildren( IPathSet paths ) {
			foreach ( var p in paths )
				Append(p);
		}


		public IEnumerator<IPath> GetEnumerator() {
			return Paths.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Paths.GetEnumerator();
		}

		public virtual Vector3d StartPosition {
			get {
				return Paths[0].StartPosition;
			}
		}

		public virtual Vector3d EndPosition {
			get {
				return Paths[Paths.Count - 1].EndPosition;
			}
		}


		public AxisAlignedBox3d Bounds
		{
			get {
				AxisAlignedBox3d box = AxisAlignedBox3d.Empty;
				foreach ( var p in Paths )
					box.Contain(p.Bounds);
				return box;
			}
		}

		public AxisAlignedBox3d ExtrudeBounds
		{
			get {
				AxisAlignedBox3d box = AxisAlignedBox3d.Empty;
				foreach ( var p in Paths ) {
					if ( p.Type == PathTypes.Deposition )
						box.Contain(p.Bounds);
				}
				return box;				
			}
		}


		public bool HasFinitePositions {
			get { return false; }
		}
		public IEnumerable<Vector3d> AllPositionsItr()
		{
			return Enumerable.Empty<Vector3d>();
		}



		public List<double> GetZValues() {
			HashSet<double> Zs = new HashSet<double>();
			PathUtil.ApplyToLeafPaths(this, (ipath) => {
				if ( ipath is LinearPath3<IPathVertex> ) {
					foreach ( var v in (ipath as LinearPath3<IPathVertex>) ) {
						Zs.Add( v.Position.z );
					}
				}
			});
			return new List<double>(Zs);
		}

	}
}
