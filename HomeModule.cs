using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HondaGen.Models;
using HondaGen.Models.Dto;
//using Nancy.Serialization.JsonNet;
using Newtonsoft.Json;

namespace HondaGen
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get("/", args => {
				return View["home.html"];
			});

			Get("/models", args => {

				List<ModelCar> list = ClassCrud.GetModelCars();
				string json = JsonConvert.SerializeObject(list);

				return json;
			});

			Get("/image", args => {

			    string image_id = this.Request.Query["image_id"];

				string FilderImagePath = Ut.GetImagePath();  //"wwwroot/image/";
															 //image_id = "Yamato.jpg";
				string fullPath = FilderImagePath + image_id;

                if (System.IO.File.Exists(fullPath))
                {
                    byte[] file = System.IO.File.ReadAllBytes(fullPath);

                    string json = JsonConvert.SerializeObject(file);
					return json;
				}

				var response = new Response { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "File not found!",  };
				return response;

			});

			Get("/vehicle/vin", args => {
				//   1HGCE17600A300001
				string vin = Request.Query["vin"];

				List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin);  //  JHMED73600S205949
				List<header> headerList = ClassCrud.GetHeaders();

				var result = new
				{
					headers = headerList,
					items = list,
					cntitems = list.Count,
					page = 1
				};

				string json = JsonConvert.SerializeObject(result);

				return json;
			});

			Get("/mgroups", args => {

                string vehicle_id = Request.Query["vehicle_id"];
                List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id);

                string json = JsonConvert.SerializeObject(list);
                return json;
            });
			//=========================================================================================
			Get("/vehicle", args => {

				string nplblk = Request.Query["nplblk"];
				int hmodtyp = Request.Query["hmodtyp"];
				string npl = Request.Query["npl"];

				List<SpareParts4F> list = ClassCrud.GetSpareParts(nplblk, hmodtyp, npl);

                string json = JsonConvert.SerializeObject(list);
                return json;
            });
			//=========================================================================================
			Get("/ﬁlters", args => {
				string modelId = Request.Query["modelId"];
				List<Filters> list = ClassCrud.GetFilters(modelId);

				string json = JsonConvert.SerializeObject(list);
				return json;
			});

			Get("/ﬁlter-cars", args => {

				List<header> headerList = ClassCrud.GetHeaders();

				int page = Request.Query["page"];
				int page_size = Request.Query["page_size"];

				string paramStr = Request.Query["param"].Value;

                string[] param = paramStr.Split(",");

                string modelId = Request.Query["modelId"];

                string ctrsmtyp = param[0];
                string carea = param[1];
                string nengnpf = param[2];

                List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(modelId, ctrsmtyp, carea, nengnpf);

                list = list.Skip((page - 1) * page_size).Take(page_size).ToList();

                var result = new
                {
                    headers = headerList,
                    items = list,
                    cntitems = list.Count,
                    page = page
                };

                string json = JsonConvert.SerializeObject(result);
				return json;
			});

			Get("/vehicle/sgroups", args => {

				int hmodtyp = Request.Query["hmodtyp"];
				string nplgrp = Request.Query["nplgrp"];

				string npl = "";

				if(Request.Query["npl"] != null)
                {
					npl = Request.Query["npl"];
				}

				List<Sgroups> list = ClassCrud.GetSgroups(hmodtyp, nplgrp, npl);

				string json = JsonConvert.SerializeObject(list);
				return json;
			});

		}
	}
}

