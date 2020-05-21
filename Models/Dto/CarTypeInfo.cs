using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class CarTypeInfo
    {
		
		public int hmodtyp { get; set; }         //   hmodtyp 'Код типа автомобиля',
		public string cmodnamepc { get; set; }   //  cmodnamepc  'Модель автомобиля',
		public string xcardrs { get; set; }      //  xcardrs  'Кол-во дверей',
		public string dmodyr { get; set; }       // dmodyr  'Год выпуска',
		public string xgradefulnam { get; set; } //  xgradefulnam  Класс
		public string ctrsmtyp { get; set; }     //   ctrsmtyp VARCHAR(3) NOT NULL COMMENT 'Тип трансмиссии',
		public string cmftrepc { get; set; }     //  cmftrepc  'Страна производитель',
		public string carea { get; set; }        //  carea  'Страна рынок',

	}
}
