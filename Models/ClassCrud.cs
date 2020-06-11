using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using HondaGen.Models.Dto;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace HondaGen.Models
{
    public class ClassCrud
    {
        private static string strConn = Ut.GetMySQLConnect();

        public static readonly string getLangId = " SELECT DISTINCT plt.id FROM " +
                                                " pl_language_tbl plt " +
                                                " WHERE plt.lang = @lang ";

        public static List<CarTypeInfo> GetListCarTypeInfo(string vin)
        {
            List<CarTypeInfo> list = null;

            if (!(String.IsNullOrEmpty(vin)) && (vin.Length == 17))
            {
                try
                {
                    string nfrmpf = vin.Substring(0, 11);
                    string nfrmseqepc = vin.Substring(9, 8);

                    #region strCommand
                    string strCommand = "  SELECT  " +
                    "   pmotyt.hmodtyp vehicle_id, " +
                    "   pmotyt.cmodnamepc model_name,  " +
                    " 	pmotyt.xcardrs, " +
                    " 	pmotyt.dmodyr, " +
                    " 	pmotyt.xgradefulnam,  " +
                    " 	pmotyt.ctrsmtyp,  " +
                    " 	pmotyt.cmftrepc,  " +
                    "   pmotyt.carea " +

                    "  FROM pl_pmotyt pmotyt " +
                    "  WHERE " +
                    "    (pmotyt.nfrmpf = @nfrmpf) AND " +
                    "   ((pmotyt.nfrmseqepcstrt <= @nfrmseqepc) AND " +
                    "   (pmotyt.nfrmseqepcend >= @nfrmseqepc)); ";
                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        list = db.Query<CarTypeInfo>(strCommand, new { nfrmpf, nfrmseqepc }).ToList();
                    }
                }
                catch(Exception ex)
                {
                    string Error = ex.Message;
                    int h = 0;
                }
            }

            return list;
        }
        public static List<ModelCar> GetModelCars()
        {
            List<ModelCar> list = null;
            string strCommand = "SELECT CONCAT(cmodnamepc,'_', dmodyr, '_',xcardrs,'_',cmftrepc) AS Id, " +
                " CONCAT(cmodnamepc,' ', dmodyr, ' ',xcardrs,' ',cmftrepc) as cmodnamepc, " +
                " CONCAT(cmodnamepc,'-', dmodyr, '-',xcardrs,'-',cmftrepc) as seo_url FROM pl_pmodlt;";
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand).ToList();
                }
            }
            catch { }
            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string vehicle_id, string lang = "EN")
        {
            List<PartsGroup> list = null;
            string strCommand = " SELECT DISTINCT CONCAT(pgrout.clangjap, '_', " +
                                "  pgrout.nplgrp) Id,   " +
                                "  pgrout.xplgrp name " +
                                " FROM pl_pgrout pgrout " +
                                $" WHERE pgrout.clangjap IN ({getLangId}) " +
                                " AND EXISTS(SELECT 1 " +
                                " FROM pl_pblokt pblokt, pl_pblmtt pblmtt " +
                                " WHERE pblokt.nplgrp = pgrout.nplgrp " +
                                " AND pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                                " AND pblmtt.hmodtyp = @hmodtyp ); ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<PartsGroup>(strCommand, new { hmodtyp = vehicle_id, lang }).ToList();
                }

                for (int i = 0; i < list.Count; i++)
                {


                    List<Sgroups> listSgroups = GetSgroups(vehicle_id, list[i].Id, lang);
                    list[i].childs = listSgroups;
                }
            }
            catch(Exception ex)           
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<header> GetHeaders()
        {
            List<header> list = new List<header>();

            header header1 = new header { code = "model_name", title = "Модель" };
            list.Add(header1);
            header header2 = new header { code = "vehicle_id", title = "Код типа автомобиля" };
            list.Add(header2);
            header header3 = new header { code = "xcardrs", title = "Кол-во дверей" };
            list.Add(header3);
            header header4 = new header { code = "dmodyr", title = "Год выпуска" };
            list.Add(header4);
            header header5 = new header { code = "xgradefulnam", title = "Класс" };
            list.Add(header5);
            header header6 = new header { code = "ctrsmtyp", title = "Тип трансмиссии" };
            list.Add(header6);
            header header7 = new header { code = "cmftrepc", title = "Страна производитель" };
            list.Add(header7);
            header header8 = new header { code = "carea", title = "Страна рынок" };
            list.Add(header8);

            return list;
        }

        public static List<SpareParts4F> GetSpareParts(string nplblk, int hmodtyp, string npl)
        {
            List<SpareParts4F> list = null;

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                "      pblpat.hpartplblk, " +
                "      pblpat.xpartext, " +
                "      pblpat.nplblk, " +
                "      pblpat.npl " +
                "   FROM " +
                "  ( " +

                "    SELECT " +
                "      pl_pblpat.hpartplblk, " +
                "      pl_ppartt.xpartext, " +
                "      pl_pblpat.nplblk, " +
                "      pl_pblpat.npl " +
                "    FROM pl_pbpmtt " +
                "      JOIN pl_pblpat " +
                "        ON pl_pbpmtt.hpartplblk = pl_pblpat.hpartplblk " +
                "        AND pl_pbpmtt.disc_no = pl_pblpat.disc_no " +
                "      JOIN pl_ppartt " +
                "      ON pl_pblpat.npartgenu = pl_ppartt.npartgenu " +
                "    WHERE " +
                $"      pl_pbpmtt.hmodtyp = {hmodtyp} " +

                $"       AND pl_pblpat.nplblk = '{nplblk}' " +
                $"       AND pl_pblpat.npl = '{npl}' " +
                "    UNION " +
                "    SELECT " +
                "      pl_pblpat.hpartplblk, " +
                "      pl_ppartt.xpartext, " +
                "      pl_pblpat.nplblk, " +
                "      pl_pblpat.npl " +
                "    FROM pl_pblpat " +
                "      JOIN pl_ppartt " +
                "      ON pl_pblpat.npartgenu = pl_ppartt.npartgenu " +
                "    WHERE " +

                $"      pl_pblpat.npl = '{npl}' AND " +
                $"      pl_pblpat.nplblk = '{nplblk}' AND " +
                "      (NOT EXISTS( " +
                "        SELECT hmodtyp " +
                "        FROM pl_pbpmtt " +
                "        WHERE " +
                "          pl_pbpmtt.hpartplblk = pl_pblpat.hpartplblk " +
                "          AND pl_pbpmtt.disc_no = pl_pblpat.disc_no " +
                "      )) " +
                "  ) pblpat " +
                "  ORDER BY pblpat.hpartplblk; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<SpareParts4F>(strCommand).ToList();
                }

                int y;

            }
            catch(Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                  //  list[i].hotspots = GetHotspots(list[i].nplblk, list[i].npl);
                }
            }

            return list;
        }
        public static List<Filters> GetFilters(string modelId)
        {
            List<Filters> filters = new List<Filters>();

            try
            {
                string[] param = modelId.Split("_");

                if (param.Length == 4)
                {
                    string cmodnamepc = param[0];
                    string dmodyr = param[1];
                    string xcardrs = param[2];
                    string cmftrepc = param[3];

                    #region Тип трансмиссии
                    List<string> ctrsmtypList = new List<string>();
                    string ctrsmtypCom = "SELECT DISTINCT p.ctrsmtyp FROM " +
                                          " pl_pmotyt p " +
                                          " WHERE " +
                                          $" p.cmodnamepc = '{cmodnamepc}' AND " +
                                          $" p.xcardrs = '{xcardrs}' AND " +
                                          $" p.dmodyr = '{dmodyr}' AND " +
                                          $" p.cmftrepc = '{cmftrepc}'; ";

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        ctrsmtypList = db.Query<string>(ctrsmtypCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters ctrsmtypF = new Filters { Id = "1", name = "Тип трансмиссии" };
                    List<values> ctrsmtypVal = new List<values>();

                    for (int i = 0; i < ctrsmtypList.Count; i++)
                    {
                        values v1 = new values { Id = ctrsmtypList[i], name = ctrsmtypList[i] };
                        ctrsmtypVal.Add(v1);
                    }

                    ctrsmtypF.values = ctrsmtypVal;
                    filters.Add(ctrsmtypF);
                    #endregion

                    #region Страна рынок
                    List<string> careaList = new List<string>();
                    string careaCom = "SELECT DISTINCT carea FROM " +
                                          " pl_pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        careaList = db.Query<string>(careaCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters careaF = new Filters { Id = "2", name = "Страна рынок" };
                    List<values> careaVal = new List<values>();

                    for (int i = 0; i < careaList.Count; i++)
                    {
                        values v1 = new values { Id = careaList[i], name = careaList[i] };
                        careaVal.Add(v1);
                    }

                    careaF.values = careaVal;
                    filters.Add(careaF);
                    #endregion

                    #region  npl  'Каталог разборки',
                    List<string> nplList = new List<string>();
                    string nplCom = "SELECT DISTINCT npl FROM " +
                                          " pl_pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        nplList = db.Query<string>(nplCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters nplF = new Filters { Id = "3", name = "Каталог разборки" };
                    List<values> nplVal = new List<values>();

                    for (int i = 0; i < nplList.Count; i++)
                    {
                        values v1 = new values { Id = nplList[i], name = nplList[i] };
                        nplVal.Add(v1);
                    }

                    nplF.values = nplVal;
                    filters.Add(nplF);
                    #endregion

                    #region  cmftrepc Страна производитель
                    //List<string> cmftrepcList = new List<string>();

                    //string cmftrepcCom = " SELECT DISTINCT cmftrepc FROM " +
                    //                      " pmotyt p " +
                    //                      " WHERE " +
                    //                      " p.cmodnamepc = @cmodnamepc AND " +
                    //                      " p.xcardrs = @xcardrs AND " +
                    //                      " p.dmodyr = @dmodyr AND " +
                    //                      " p.cmftrepc = @cmftrepc; ";

                    //using (IDbConnection db = new MySqlConnection(strConn))
                    //{
                    //    cmftrepcList = db.Query<string>(cmftrepcCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    //}
                    //Filters cmftrepcF = new Filters { Id = "4", name = "Страна производитель" };
                    //List<values> cmftrepcVal = new List<values>();

                    //for (int i = 0; i < cmftrepcList.Count; i++)
                    //{
                    //    values v1 = new values { Id = cmftrepcList[i], name = cmftrepcList[i] };
                    //    cmftrepcVal.Add(v1);
                    //}

                    //cmftrepcF.values = cmftrepcVal;
                    //filters.Add(cmftrepcF);
                    #endregion

                    #region  nengnpf Код Двигателя
                    List<string> nengnpfList = new List<string>();
                    string nengnpfCom = "SELECT DISTINCT nengnpf FROM " +
                                          " pl_pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        nengnpfList = db.Query<string>(nengnpfCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters nengnpfF = new Filters { Id = "5", name = "Код Двигателя" };
                    List<values> nengnpfVal = new List<values>();

                    for (int i = 0; i < nengnpfList.Count; i++)
                    {
                        values v1 = new values { Id = nengnpfList[i], name = nengnpfList[i] };
                        nengnpfVal.Add(v1);
                    }

                    nengnpfF.values = nengnpfVal;
                    filters.Add(nengnpfF);
                    #endregion
                }
            }
            catch { }

            return filters;
        }
        public static List<Sgroups> GetSgroups(string vehicle_id, string group_id, string lang = "EN")
        {
            List<Sgroups> list = null;

            try
            {
                string[] arrTemp = group_id.Split("_");
                string nplgrp = arrTemp[1];


                #region strCommand
                string strNpl = " SELECT DISTINCT pl_pblmtt.npl FROM pl_pblmtt WHERE pl_pblmtt.hmodtyp = @vehicle_id ";

                string strCommand = "  SELECT DISTINCT " +
                                    "   CONCAT(pbldst.npl, '_', pblokt.nplblk) node_id, " +
                                    "           pbldst.xplblk AS  'name', " +
                                    " pblokt.nplblk image_id, " +
                                    " '.png' image_ext " +
                                    "      FROM pl_pblokt pblokt " +
                                    "           JOIN pl_pbldst pbldst " +
                                    "              ON pblokt.npl = pbldst.npl " +
                                    "             AND pblokt.nplblk = pbldst.nplblk " +
                                    $"             and pbldst.clangjap IN({getLangId}) " +     
                                    $" and pbldst.npl IN ({strNpl}) " +
                                    $"    WHERE pblokt.nplgrp = @nplgrp and " +
                                    "                  (pblokt.nplblk in (SELECT DISTINCT pblmtt.nplblk " +
                                    "                                FROM pl_pblmtt pblmtt " +
                                    "                               WHERE " +
                                    $" (pblmtt.npl  IN ({strNpl})  ) AND " +
                                    $"   (pblmtt.hmodtyp = @vehicle_id   )) ) " +
                                    "  ORDER BY pbldst.xplblk ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<Sgroups>(strCommand, new { vehicle_id, nplgrp, lang }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }
            return list;
        }
        public static List<CarTypeInfo> GetListCarTypeInfoFilterCars(string modelId, string ctrsmtyp, string carea, string nengnpf)
        {
            List<CarTypeInfo> list = null;

            string[] param = modelId.Split("_", StringSplitOptions.None);

            if (param.Length == 4)
            {
                string cmodnamepc = param[0];
                string dmodyr = param[1];
                string xcardrs = param[2];
                string cmftrepc = param[3];

                try
                {

                    #region strCommand
                    string strCommand = "   SELECT DISTINCT " +
                                        "   p.hmodtyp vehicle_id,  " +
                                        " 	p.cmodnamepc model_name, " +
                                        " 	p.xcardrs, " +
                                        " 	p.dmodyr, " +
                                        " 	p.xgradefulnam, " +
                                        " 	p.ctrsmtyp, " +
                                        " 	p.cmftrepc, " +
                                        "   p.carea " +
                                        "   FROM pl_pmotyt p " +
                                        "     WHERE " +
                                        "    p.cmodnamepc = @cmodnamepc " +
                                        "    AND p.dmodyr = @dmodyr " +
                                        "    AND p.xcardrs = @xcardrs " +
                                        "    AND p.cmftrepc = @cmftrepc " +
                                        "    AND p.ctrsmtyp = @ctrsmtyp " +
                                        "    AND p.carea = @carea " +
                                        "    AND p.nengnpf = @nengnpf; ";
                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        list = db.Query<CarTypeInfo>(strCommand, new { cmodnamepc, dmodyr, xcardrs, cmftrepc, ctrsmtyp, carea, nengnpf }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    string Errror = ex.Message;
                    int y = 0;
                }
            }
            return list;
        }
        public static List<lang> GetLang()
        {
            List<lang> list = new List<lang>();
            string strCommand = " SELECT DISTINCT " +
                                "   lang code, " +
                                "   name, " +
                                " is_default " +
                                " FROM pl_language_tbl; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<lang>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<string> GetWmi()
        {
            List<string> list = new List<string>();
            string strCommand = " SELECT value FROM wmi; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<string>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<attributes> GetAttributes()
        {
            List<attributes> list = new List<attributes>();

            attributes cmodnamepc = new attributes { code = "model_name", name = "Модель", value = "" };
            list.Add(cmodnamepc);

            attributes xcardrs = new attributes { code = "xcardrs", name = "Кол-во дверей", value = "" };
            list.Add(xcardrs);

            attributes dmodyr = new attributes { code = "dmodyr", name = "Год выпуска", value = "" };
            list.Add(dmodyr);

            attributes xgradefulnam = new attributes { code = "xgradefulnam", name = "Класс", value = "" };
            list.Add(xgradefulnam);

            attributes ctrsmtyp = new attributes { code = "ctrsmtyp", name = "Тип трансмиссии", value = "" };
            list.Add(ctrsmtyp);

            attributes cmftrepc = new attributes { code = "cmftrepc", name = "Страна производитель", value = "" };
            list.Add(cmftrepc);

            attributes engine = new attributes { code = "carea", name = "Страна рынок", value = "" };
            list.Add(engine);

            return list;
        }
        public static VehiclePropArr GetVehiclePropArr(string vehicle_id)
        {
            VehiclePropArr model = null;

            try
            {

                #region strCommand
                string strCommand = " SELECT  " +
                                    " pmotyt.hmodtyp vehicle_id, " +
                                    " pmotyt.cmodnamepc model_name,  " +
                                    " pmotyt.xcardrs, " +
                                    " pmotyt.dmodyr, " +
                                    " pmotyt.xgradefulnam,  " +
                                    " pmotyt.ctrsmtyp,  " +
                                    " pmotyt.cmftrepc,  " +
                                    " pmotyt.carea " +

                                    " FROM pl_pmotyt pmotyt " +
                                    " WHERE pmotyt.hmodtyp = @vehicle_id; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    CarTypeInfo carType = db.Query<CarTypeInfo>(strCommand, new { vehicle_id }).FirstOrDefault();

                    List<attributes> list = GetAttributes();

                    list[0].value = carType.model_name;
                    list[1].value = carType.xcardrs;
                    list[2].value = carType.dmodyr;
                    list[3].value = carType.xgradefulnam;
                    list[4].value = carType.ctrsmtyp;
                    list[5].value = carType.cmftrepc;
                    list[6].value = carType.carea;

                    model = new VehiclePropArr { model_name = carType.model_name };
                    model.attributes = list;
                }

            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return model;
        }
        public static List<node> GetNodes(string[] codesArr = null, string[] node_idsArr = null)
        {
            List<node> list = new List<node>();
            string codes = null;
            string node_ids = null;

            if (codesArr != null && codesArr.Length > 0)
            {
                codes = string.Empty;

                for (int i = 0; i < codesArr.Length; i++)
                {
                    if (i == 0)
                    {
                        codes += codesArr[i];
                    }
                    else
                    {
                        codes += "," + codesArr[i];
                    }

                }
            }
            if (node_idsArr != null && node_idsArr.Length > 0)
            {
                node_ids = string.Empty;

                for (int i = 0; i < node_idsArr.Length; i++)
                {
                    if (i == 0)
                    {
                        node_ids += node_idsArr[i];
                    }
                    else
                    {
                        node_ids += "," + node_idsArr[i];
                    }
                }
            }

            string strCommand = " SELECT nt.code, nt.group name, nt.node_ids FROM " +
                                " nodes_tb nt ";

            if (!String.IsNullOrEmpty(codes) || !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " WHERE";
            }
            if (!String.IsNullOrEmpty(codes))
            {
                strCommand += $"  nt.code IN  ({codes}) ";
            }
            if (!String.IsNullOrEmpty(codes) && !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " OR ";
            }
            if (!String.IsNullOrEmpty(node_ids))
            {
                strCommand += $"  nt.node_ids IN  ({node_ids}) ";
            }
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<node>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }

        public static DetailsInNode GetDetailsInNode(string node_id, string lang = "EN")
        {
            string npl = node_id.Substring(0, node_id.IndexOf("_"));
            string nplblk = node_id.Substring(node_id.IndexOf("_") + 1);

            DetailsInNode detailsInNode = new DetailsInNode { node_id = node_id };

            string strCommand = " SELECT " +
                                " pp.xplblk " +
                                " FROM " +
                                " pl_pbldst pp " +
                                " WHERE " +
                                " pp.npl = @npl " +
                                " AND pp.nplblk = @nplblk " +
                                " AND pp.clangjap = @lang; ";

         string strCommDeatil = " SELECT " +
                                " pp.hpartplblk number, " +
                                " p.xpartext name " +
                                " FROM " +
                                " pl_pblpat pp " +
                                " LEFT JOIN pl_ppartt p ON pp.npartgenu = p.npartgenu " +
                                " AND pp.disc_no = p.disc_no " +
                                " WHERE " +
                                " pp.npl = @npl " +
                                " AND " +
                                " pp.nplblk = @nplblk ; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    detailsInNode.name = db.Query<string>(strCommand, new { npl, nplblk, lang }).FirstOrDefault();
                    detailsInNode.parts = db.Query<Detail>(strCommDeatil, new { npl, nplblk }).ToList();
                }

                images images = new images { id = nplblk, ext = ".png" };
                List<images> listImages = new List<images>();
                listImages.Add(images);
                detailsInNode.images = listImages;

                for (int i = 0; i < detailsInNode.parts.Count; i++)
                {
                      detailsInNode.parts[i].hotspots = GetHotspots(node_id);
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int y = 0;
            }

            return detailsInNode;
        }
        public static List<hotspots> GetHotspots(string detail_id)
        {
            List<hotspots> list = null;
            try
            {
                #region strCommand
                string strCommand = " SELECT  " +
                                " h.recordid hotspot_id, " +
                                " h.illustrationnumber image_id, " +
                                " h.max_x x2, " +
                                " h.max_y y2, " +
                                " h.min_x x1, " +
                                " h.min_y y1 " +
                                " FROM pl_hotspots h " +
                                " WHERE h.illustrationnumber IN(SELECT DISTINCT pp.nplblk FROM pl_pblpat pp WHERE pp.hpartplblk = @detail_id) " +
                                " AND h.npl IN(SELECT DISTINCT pp.npl FROM pl_pblpat pp WHERE pp.hpartplblk = @detail_id) " +
                                " AND h.partreferencenumber IN(SELECT DISTINCT pp.nplpartref FROM pl_pblpat pp WHERE pp.hpartplblk = @detail_id); ";

                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<hotspots>(strCommand, new { detail_id }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
    }
}
