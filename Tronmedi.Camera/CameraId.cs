﻿using System;

namespace Tronmedi.Camera
{
	public sealed class CameraId : IEquatable<CameraId>
	{
		/// <summary>
		/// Initializes a new instance of the WebCameraId class.
		/// </summary>
		internal CameraId(DirectShowProxy.VideoInputDeviceInfo info)
		{
			Name = info.FriendlyName;
			DevicePath = info.DevicePath;
		}

		/// <summary>
		/// Gets the name of the web camera.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the device path of the web camera.
		/// </summary>
		public string DevicePath { get; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(CameraId)) return false;
			return Equals((CameraId)obj);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(CameraId other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, Name) && Equals(other.DevicePath, DevicePath);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return (Name.GetHashCode() * 397) ^ DevicePath.GetHashCode();
			}
		}

		public static bool operator ==(CameraId left, CameraId right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(CameraId left, CameraId right)
		{
			return !Equals(left, right);
		}
	}
}
