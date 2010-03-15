using System;
using System.Configuration;
using System.Net;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	/// Represents a configuration element that contains a Memcached node address. This class cannot be inherited. 
	/// </summary>
	public sealed class EndPointElement : ConfigurationElement
	{
		private System.Net.IPEndPoint endpoint;

		/// <summary>
		/// Gets or sets the ip address of the node.
		/// </summary>
		[ConfigurationProperty("address", IsRequired = true, IsKey = true)]
		public string Address
		{
			get { return (string)base["address"]; }
			set { base["address"] = value; }
		}

		/// <summary>
		/// Gets or sets the port of the node.
		/// </summary>
		[ConfigurationProperty("port", IsRequired = true, IsKey = true), IntegerValidator(MinValue = 0, MaxValue = 65535)]
		public int Port
		{
			get { return (int)base["port"]; }
			set { base["port"] = value; }
		}

		/// <summary>
		/// Gets the <see cref="T:IPEndPoint"/> representation of this instance.
		/// </summary>
		public System.Net.IPEndPoint EndPoint
		{
			get
			{
				if (this.endpoint == null)
				{
					IPHostEntry entry = System.Net.Dns.GetHostEntry(this.Address);
					IPAddress[] list = entry.AddressList;

					if (list.Length == 0)
						throw new ConfigurationErrorsException(String.Format("Could not resolve host '{0}'.", this.Address));

					// get the first IPv4 address from the list (not sure how memcached works against ipv6 addresses whihc are not localhost)
                    IPAddress address = null;
                    for (int i = 0; i < list.Length; i++) {
                        if (list[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                            address = list[i];
                            break;
                        }
                    }
					if (address == null)
						throw new ConfigurationErrorsException(String.Format("Host '{0}' does not have an IPv4 address.", this.Address));
						
					this.endpoint = new System.Net.IPEndPoint(address, this.Port);
				}

				return this.endpoint;
			}
		}

		#region [ T:IPAddressValidator         ]
		private class IPAddressValidator : ConfigurationValidatorBase
		{
			private IPAddressValidator() { }

			public override bool CanValidate(Type type)
			{
				return (type == typeof(string)) || base.CanValidate(type);
			}

			public override void Validate(object value)
			{
				string address = value as string;

				if (String.IsNullOrEmpty(address))
					return;

				System.Net.IPAddress tmp;

				if (!System.Net.IPAddress.TryParse(address, out tmp))
					throw new ConfigurationErrorsException("Invalid address specified: " + address);
			}
		}
		#endregion
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kisk�, enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion
