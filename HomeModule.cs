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
					cnt_items = list.Count,
					page = 1
				};

				return JsonConvert.SerializeObject(result);
			});

			Get("/mgroups", args => {

                string vehicle_id = Request.Query["vehicle_id"];
				string lang = Request.Query["lang"];

				List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id, lang);

				return JsonConvert.SerializeObject(list);
            });
			//=========================================================================================
			Get("/vehicle", args => {

				string node_id = Request.Query["node_id"];
				string lang = Request.Query["lang"];

				//List<SpareParts4F> list = ClassCrud.GetSpareParts(nplblk, hmodtyp, npl);
				DetailsInNode details = ClassCrud.GetDetailsInNode(node_id, lang );

				return JsonConvert.SerializeObject(details);
                
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

				string vehicle_id = Request.Query["vehicle_id"];
				string group_id = Request.Query["group_id"];
				string lang = Request.Query["lang"];

                List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, lang);
                return JsonConvert.SerializeObject(list);

            });

			Get("/locales", args => {

				List<lang> list = ClassCrud.GetLang();
				return JsonConvert.SerializeObject(list);
			});

			Get("/vehicle/wmi", args => {

				List<string> list = ClassCrud.GetWmi();
				return JsonConvert.SerializeObject(list);
			});

			Get("/vehicleAttr", args => {

				string vehicle_id = Request.Query["vehicle_id"];

				VehiclePropArr result = ClassCrud.GetVehiclePropArr(vehicle_id);
				return JsonConvert.SerializeObject(result);
			});

			Post("/vehicle/sgroups", args => {

				string strCodes = Request.Query["codes"];
				string strNodeIds = Request.Query["node_ids"];



				string[] codes = strCodes.Split(",");
				string[] node_ids = strNodeIds.Split(",");

				List<node> list = ClassCrud.GetNodes(codes, node_ids);
				return JsonConvert.SerializeObject(list);
			});
		}
	}
}

