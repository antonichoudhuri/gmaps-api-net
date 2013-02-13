﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Google.Maps
{
	/// <summary>
	/// Provides an intuitive and simple HTTP client wrapper.
	/// </summary>
	internal static class Http
	{
		public class HttpGetResponse
		{
			private Uri requestUri;

			public HttpGetResponse(Uri uri)
			{
				requestUri = uri;
			}

			public string AsString()
			{
				var output = String.Empty;

				var response = WebRequest.Create(requestUri).GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					output = reader.ReadToEnd();
					reader.Close();
				}
				response.Close();

				return output;
			}

			public T As<T>() where T : class
			{
				T output = null;

				using (var stringReader = new StringReader(AsString()))
				{
					var jsonReader = new JsonTextReader(stringReader);
					var serializer = new JsonSerializer();
					serializer.Converters.Add(new JsonEnumTypeConverter());
					output = serializer.Deserialize<T>(jsonReader);
				}

				return output;
			}
		}

		public static HttpGetResponse Get(Uri uri)
		{
			return Factory.CreateResponse(uri);
		}

		/// <summary>
		/// Gets or sets the factory that provides HttpGetResponse instances. Crude depency injection for the time being.
		/// </summary>
		public static HttpGetResponseFactory Factory = new HttpGetResponseFactory();

		/// <summary>
		/// A factory class for building HttpGetResponse instances.
		/// </summary>
		public class HttpGetResponseFactory
		{
			/// <summary>
			/// Builds a standard HttpGetResponse instance.
			/// </summary>
			/// <param name="uri"></param>
			/// <returns></returns>
			public virtual HttpGetResponse CreateResponse(Uri uri)
			{
				return new HttpGetResponse(uri);
			}
		}

	}
}